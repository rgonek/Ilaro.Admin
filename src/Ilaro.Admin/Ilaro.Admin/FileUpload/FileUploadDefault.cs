using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.FileUpload
{
	public class FileUploadDefault
	{

		public const long MaxFileSize = 2048000;

		public readonly static string[] ImageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
	}
}