using System;
using Ilaro.Admin.Core.FileUpload;

namespace Ilaro.Admin.DataAnnotations
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ImageAttribute : Attribute
	{
		/// <summary>
		/// Default is OriginalFileName
		/// </summary>
		public NameCreation NameCreation { get; set; }

		/// <summary>
		/// Default is: 2048000 = 2MB
		/// </summary>
		public long MaxFileSize { get; set; }

		/// <summary>
		/// Default is: .jpg, .jpeg, .png, .gif, .bmp
		/// </summary>
		public string[] AllowedFileExtensions { get; set; }

		public bool IsMulti { get; set; }

		public ImageAttribute()
		{
			MaxFileSize = FileUploadDefault.MaxFileSize;
			AllowedFileExtensions = FileUploadDefault.ImageExtensions;
			NameCreation = NameCreation.OriginalFileName;
		}

		public ImageAttribute(params string[] allowedFileExtensions)
			: this()
		{
			AllowedFileExtensions = allowedFileExtensions;
		}
	}
}