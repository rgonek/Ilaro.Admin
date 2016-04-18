#addin "Cake.Powershell"

// Arguments
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

// helpers
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var branch = AppVeyor.Environment.Repository.Branch;
var isBuildingMaster = branch != null && branch == "master";
var isLocalBuild = BuildSystem.IsLocalBuild;
var author = "Robert Gonek";

// Version
var version = "0.5.0";
var buildSuffix = isLocalBuild ? "" : "." + AppVeyor.Environment.Build.Number;
var fullVersion = version + buildSuffix;

// Directories
var solution = "./Ilaro.Admin.sln";
var sourceDir = "./src";
var testsDir = "./tests";
var samplesDir = "./samples";
var buildDir = "./build";
var nuspecsDir = buildDir + "/nuspecs";
var artifactsDir = "./artifacts";
var outputDir = artifactsDir + "/v" + fullVersion;
var nugetsDir = artifactsDir + "/nugets";
var ilaroAdminDir = Directory(sourceDir + "/Ilaro.Admin/");
var ilaroAdminAutofacDir = Directory(sourceDir + "/Ilaro.Admin.Autofac/");
var ilaroAdminNinjectDir = Directory(sourceDir + "/Ilaro.Admin.Ninject/");
var ilaroAdminUnityDir = Directory(sourceDir + "/Ilaro.Admin.Unity/");
var ilaroAdminTestsDir = Directory(testsDir + "/Ilaro.Admin.Tests/");
var ilaroAdminSampleDir = Directory(samplesDir + "/Ilaro.Admin.Sample/");
var directories = new DirectoryPath[] { 
    ilaroAdminDir, 
    ilaroAdminAutofacDir,
    ilaroAdminNinjectDir,
    ilaroAdminUnityDir,
    ilaroAdminTestsDir,
    ilaroAdminSampleDir };
var nugets = new DirectoryPath[] {
    ilaroAdminDir, 
    ilaroAdminAutofacDir,
    ilaroAdminNinjectDir,
    ilaroAdminUnityDir,
};

// Tasks
Task("Clean")
    .Does(() =>
{
    CleanDirectory(outputDir);
    CleanDirectory(nugetsDir);
    foreach(var dir in directories)
    {
        CleanDirectory(dir + Directory("/bin/" + configuration));
    }
});

Task("Update-Versions")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var copyright = "Copyright © " + author + " 2014-" + DateTime.Now.Year;
    foreach(var dir in directories)
    {
        var file = dir + Directory("/Properties/AssemblyInfo.cs");
        var projectName = GetProjectName(dir.ToString());
        var assemblyInfo = ParseAssemblyInfo(file);
        CreateAssemblyInfo(file, new AssemblyInfoSettings {
            Title = projectName,
            Product = projectName,
            Version = version,
            FileVersion = version,
            InformationalVersion = fullVersion,
            Copyright = copyright
        });
    }
}); 

Task("Restore-NuGet-Packages")
    .IsDependentOn("Update-Versions")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      MSBuild(solution, settings =>
        settings.SetConfiguration(configuration));
});

Task("Before-Tests")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    StartPowershellFile(buildDir + "/before-tests.ps1", args =>
        {
            args.Append("Configuration", configuration);
        });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .IsDependentOn("Before-Tests")
    .Does(() =>
{
    XUnit2(testsDir + "/**/bin/" + configuration + "/*.Tests.dll", new XUnit2Settings{
        MaxThreads = 1
    });
});

Task("Update-AppVeyor-Build-Number")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(fullVersion);
});

Task("Copy-Files")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Create-Output-Directories")
    .Does(() =>
{
    foreach(var nugetDir in nugets)
    {
        var projectName = GetProjectName(nugetDir.ToString());
        var projectOutputDir = outputDir + "/" + projectName;
        CreateDirectory(projectOutputDir);
        
        var files = nugetDir + "/bin/" + configuration + "/*";
        Information(files);
        CopyFiles(
            files,
            projectOutputDir
        );
    }
});

Task("Create-Output-Directories")
    .Does(() =>
{
    CreateDirectory(nugetsDir);
    CreateDirectory(outputDir);
});

Task("Create-NuGet-Packages")
    .IsDependentOn("Copy-Files")
    .Does(() =>
{
    var nuspecs = GetFiles(nuspecsDir + "/*.nuspec");
    foreach(var nuspec in nuspecs)
    {
        var projectName = nuspec.GetFilenameWithoutExtension();
        // Create package.
        NuGetPack(nuspec, new NuGetPackSettings {
            Version = fullVersion,
            BasePath = outputDir + "/" + projectName,
            OutputDirectory = nugetsDir,
            Symbols = false,
            NoPackageAnalysis = true
        });
    }
});

// Utils
string GetProjectName(string directory)
{
    var projectFile = new DirectoryInfo(directory).GetFiles("*csproj").FirstOrDefault(); 
    var projectName = System.IO.Path.GetFileNameWithoutExtension(projectFile.Name);
    
    return projectName;
}

// Targets
Task("Default")
    .IsDependentOn("Run-Unit-Tests");
    
Task("AppVeyor")
    .IsDependentOn("Update-AppVeyor-Build-Number")
    .IsDependentOn("Create-NuGet-Packages");

// Execution
RunTarget(target);