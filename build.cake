var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solution = "./Ilaro.Admin.sln";
var sourceDir = "./src";
var testsDir = "./tests";
var samplesDir = "./samples";
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

var version = "0.5.0";
var buildNumber = AppVeyor.Environment.Build.Number;
var buildSuffix = BuildSystem.IsLocalBuild ? "" : "." + buildNumber;
var buildVersion = version + buildSuffix;

Task("Clean")
    .Does(() =>
{
    foreach(var dir in directories)
    {
        CleanDirectory(dir + Directory("/bin/" + configuration));
    }
});

Task("Update-Versions")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var copyright = "Copyright © Robert Gonek 2014-" + DateTime.Now.Year;
    foreach(var dir in directories)
    {
        var file = dir + Directory("/Properties/AssemblyInfo.cs");
        var projectFile = new DirectoryInfo(dir.ToString()).GetFiles("*csproj").FirstOrDefault(); 
        var projectName = System.IO.Path.GetFileNameWithoutExtension(projectFile.Name);
        CreateAssemblyInfo(file, new AssemblyInfoSettings {
            Title = projectName,
            Product = projectName,
            Version = version,
            FileVersion = version,
            InformationalVersion = buildVersion,
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
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild(solution, settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild(solution, settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2(testsDir + "/**/bin/" + configuration + "/*.Tests.dll", new XUnit2Settings{
        MaxThreads = 1
    });
});

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

RunTarget(target);