using System.Collections.Specialized;
using System.IO;
using ImageResizer;

namespace Ilaro.Admin.Core.File
{
    public class ImageResizer : IResizingImages
    {
        public Stream Resize(
            Stream sourceStream,
            int? width = null,
            int? height = null)
        {
            using (var memory = new MemoryStream())
            {
                var imgJob = new ImageJob(sourceStream, memory, new Instructions());

                if (width.HasValue && height.HasValue)
                {
                    imgJob.Instructions = new Instructions(
                        new NameValueCollection
                    {
                        {"width", width.Value.ToString()},
                        {"height", height.Value.ToString()},
                        {"format", "jpg"},
                        {"mode", "crop"}
                    });

                    imgJob.Build();
                    return memory;
                }

                if (width.HasValue)
                {
                    imgJob.Instructions = new Instructions(
                        new NameValueCollection
                    {
                        {"width", width.Value.ToString()},
                        {"format", "jpg"},
                        {"mode", "crop"}
                    });

                    imgJob.Build();
                    return memory;
                }

                if (height.HasValue)
                {
                    imgJob.Instructions = new Instructions(
                        new NameValueCollection
                    {
                        {"height", height.Value.ToString()},
                        {"format", "jpg"},
                        {"mode", "crop"}
                    });

                    imgJob.Build();
                    return memory;
                }

                imgJob.Instructions = new Instructions(
                    new NameValueCollection
                {
                    {"format", "jpg"},
                    {"mode", "crop"}
                });

                imgJob.Build();
                return memory;
            }
        }
    }
}