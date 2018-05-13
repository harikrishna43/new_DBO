using System;
using System.IO;

namespace DBO.Data.Utilities
{
    public static class FileUploader
    {
        public static void UploadFile(Stream image, string filePath)
        {
            try
            {
                using (var file = File.Create(filePath))
                {
                    image.Seek(0, SeekOrigin.Begin);
                    image.CopyTo(file);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
