using Ilaro.Admin.FileUpload;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Ilaro.Admin.FileUpload
{
	public class FileUpload
	{
		private static string basePath = HttpContext.Current.Server.MapPath("~/");

		public static FileUploadValidationResult Validate(HttpPostedFile file, long maxFileSize, string[] allowedFileExtensions, bool allowEmptyFile = true)
		{
			if (file == null || file.ContentLength <= 0)
			{
				return allowEmptyFile ? FileUploadValidationResult.Valid : FileUploadValidationResult.EmptyFile;
			}

			if (file.ContentLength > maxFileSize)
			{
				return FileUploadValidationResult.TooBigFile;
			}

			var ext = Path.GetExtension(file.FileName).ToLower();

			if (allowedFileExtensions.Contains(ext))
			{
				return FileUploadValidationResult.Valid;
			}

			if (!IsImage(file))
			{
				return FileUploadValidationResult.NotImage;
			}

			return FileUploadValidationResult.WrongExtension;
		}

		public static FileUploadValidationResult SaveImage(HttpPostedFile file, long maxFileSize, string[] allowedFileExtensions, out string fileName, NameCreation nameCreation, params ImageSettings[] settings)
		{
			fileName = String.Empty;
			var validationResult = Validate(file, maxFileSize, allowedFileExtensions, false);
			if (validationResult != FileUploadValidationResult.Valid)
			{
				return validationResult;
			}
			Image img;
			try
			{
				img = Image.FromStream(file.InputStream);
			}
			catch
			{
				return FileUploadValidationResult.NotImage;
			}


			switch (nameCreation)
			{
				default:
				case NameCreation.OriginalFileName:
					fileName = file.FileName;
					break;
				case NameCreation.Guid:
					fileName = string.Format("{0}.jpg", Guid.NewGuid().ToString());
					break;
				case NameCreation.Timestamp:
					fileName = string.Format("{0}.jpg", DateTime.Now.ToString("ddMMyyhhmmss"));
					break;
				case NameCreation.UserInput:
					//because of "out"
					var tempFileName = fileName;
					fileName = tempFileName;
					break;
			}

			foreach (var setting in settings)
			{
				SaveImage(setting, (Image)img.Clone(), fileName);
			}

			file.InputStream.Dispose();
			img.Dispose();

			return validationResult;
		}

		public static FileUploadValidationResult SaveImage(HttpPostedFile file, long maxFileSize, string[] allowedFileExtensions, string fileName, params ImageSettings[] settings)
		{
			var validationResult = Validate(file, maxFileSize, allowedFileExtensions, false);
			if (validationResult != FileUploadValidationResult.Valid)
			{
				return validationResult;
			}
			Image img;
			try
			{
				img = Image.FromStream(file.InputStream);
			}
			catch
			{
				return FileUploadValidationResult.NotImage;
			}

			foreach (var setting in settings)
			{
				SaveImage(setting, (Image)img.Clone(), fileName);
			}

			file.InputStream.Dispose();
			img.Dispose();

			return validationResult;
		}

		public static FileUploadValidationResult SaveImage(HttpPostedFile file, long maxFileSize, string[] allowedFileExtensions, params ImageSettings[] settings)
		{
			return SaveImage(file, maxFileSize, allowedFileExtensions, file.FileName, settings);
		}

		public static long GetFileSize(string subPath, string fileName)
		{
			var path = Path.Combine(basePath, subPath, fileName);

			var fileInfo = new FileInfo(path);

			return fileInfo.Length;
		}

		private static void SaveImage(ImageSettings settings, Image img, string fileName)
		{
			var path = Path.Combine(basePath, settings.SubPath, fileName);

			if (settings.Width.HasValue || settings.Height.HasValue)
			{
				if (settings.Width.HasValue && settings.Height.HasValue)
				{
					var imgJob = new ImageJob(img, path, new Instructions(new NameValueCollection
					{
						{ "width", settings.Width.Value.ToString() },
						{ "height", settings.Height.Value.ToString() },
						{ "format", "jpg" },
						{ "mode", "crop" }
					}));

					imgJob.CreateParentDirectory = true;
					imgJob.Build();
				}
				else if (settings.Width.HasValue)
				{
					var imgJob = new ImageJob(img, path, new Instructions(new NameValueCollection
					{
						{ "width", settings.Width.Value.ToString() },
						{ "format", "jpg" },
						{ "mode", "crop" }
					}));

					imgJob.CreateParentDirectory = true;
					imgJob.Build();
				}
				else if (settings.Height.HasValue)
				{
					var imgJob = new ImageJob(img, path, new Instructions(new NameValueCollection
					{
						{ "height", settings.Height.Value.ToString() },
						{ "format", "jpg" },
						{ "mode", "crop" }
					}));

					imgJob.CreateParentDirectory = true;
					imgJob.Build();
				}
			}
			else
			{
				var imgJob = new ImageJob(img, path, new Instructions());

				imgJob.CreateParentDirectory = true;
				imgJob.Build();
			}

			img.Dispose();
		}

		private static bool IsImage(HttpPostedFile file)
		{
			return System.Text.RegularExpressions.Regex.IsMatch(file.ContentType, "image/\\S+");
		}

		public static string GetErrorMessage(FileUploadValidationResult validationResult, string[] allowedFileExtensions)
		{
			switch (validationResult)
			{
				case FileUploadValidationResult.EmptyFile:
					return "Musisz wybrać plik do wysłania";
				case FileUploadValidationResult.TooBigFile:
					return "Wysyłany plik jest zbyt duży, maksymalny dozwolony rozmiar to 2MB";
				case FileUploadValidationResult.WrongExtension:
					return "Wysyłany plik posiada niedozwolone rozszerzenie, dozwolone to: " + string.Join(", ", allowedFileExtensions);
				case FileUploadValidationResult.NotImage:
					return "Wysyłany plik nie jest zdjęciem";
			}

			return String.Empty;
		}
	}
}
