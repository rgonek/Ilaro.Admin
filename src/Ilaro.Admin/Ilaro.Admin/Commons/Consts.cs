using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilaro.Admin.Commons
{
	public class Consts
	{
		public const int ItemsQuantityPerPage = 10;

		public const long MaxFileSize = 2048000;

		public readonly static string[] AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
	}
}
