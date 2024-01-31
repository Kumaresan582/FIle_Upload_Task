using FIle_Upload_Task.Helper;
using FIle_Upload_Task.Model;
using FIle_Upload_Task.Service;
using System.Drawing;
using System.Drawing.Imaging;

namespace File_Upload_Task.Service
{
    public class ManageImage : IManageImage
    {
        private readonly FileDbContext dbContext;

        public ManageImage(FileDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<DetailsModel> UploadFile(IFormFile Files)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(Files.FileName);
                string fileName = Files.FileName + fileInfo.Extension;  //DateTime.UtcNow.Ticks.ToString() to be added to handle the same file repeatedly
                var filePath = Common.GetFilePath(fileName);

                if (File.Exists(filePath))
                {
                    throw new Exception("File with the same name already exists.");
                }

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await Files.CopyToAsync(fileStream);
                    }
                }
                catch (IOException)
                {
                    throw new Exception("File with the same name already exists.");
                }

                fileInfo.Refresh();

                int width = 0;
                int height = 0;
                if (fileInfo.Extension == ".jpg" || fileInfo.Extension == ".jpeg" || fileInfo.Extension == ".png")
                {
                    using (var image = System.Drawing.Image.FromFile(filePath))
                    {
                        width = image.Width;
                        height = image.Height;
                    }
                }
                FileInfo info = new FileInfo(filePath);
                long fileSizeInBytes = info.Length;
                double fileSizeInMegabytes = (double)fileSizeInBytes / 1048576; // 1024 bytes * 1024 bytes = 1048576 bytes
                DetailsModel fileDetails = new DetailsModel
                {
                    FileName = fileInfo.Name,
                    FullPath = filePath,
                    DirectoryName = fileInfo.DirectoryName,
                    Extension = fileInfo.Extension,
                    CreationTime = File.GetCreationTime(filePath).ToString("dd MMM yyyy, HH:mm:ss"),
                    LastAccessTime = File.GetLastAccessTime(filePath).ToString("dd MMM yyyy, HH:mm:ss"),
                    LastWriteTime = File.GetLastWriteTime(filePath).ToString("dd MMM yyyy, HH:mm:ss"),
                    Width = width,
                    Height = height,
                    FileSize = fileSizeInMegabytes.ToString("0.00") + "MB"
                };

                await dbContext.File_Details.AddAsync(fileDetails);
                await dbContext.SaveChangesAsync();

                return fileDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddWatermarkToImage(string imagePath, string watermarkText)
        {
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(imagePath));
            Image bitmap = Bitmap.FromStream(stream);
            Graphics graphicsimages = Graphics.FromImage(bitmap);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Far;
            stringFormat.LineAlignment = StringAlignment.Far;

            Color watermarkColor = ColorTranslator.FromHtml("#fff");

            Font watermarkFont = new Font("Arial Rounded MT Bold", 50, FontStyle.Regular); // Edwardian Script ITC
            SolidBrush watermarkBrush = new SolidBrush(watermarkColor);

            SizeF textSize = graphicsimages.MeasureString(watermarkText, watermarkFont);
            int x = (int)(bitmap.Width - textSize.Width) - 10;
            int y = (int)(bitmap.Height - textSize.Height) - 10;

            graphicsimages.DrawString(watermarkText, watermarkFont, watermarkBrush, new PointF(x, y), stringFormat);

            bitmap.Save(imagePath, ImageFormat.Jpeg);
        }

        public async Task<DetailsModel> UploadFileWithOptionalWatermark(IFormFile Files, bool addWatermark)
        {
            DetailsModel fileDetails = await UploadFile(Files);

            if (addWatermark)
            {
                string watermarkText = "Watermark";
                AddWatermarkToImage(fileDetails.FullPath, watermarkText);
            }

            return fileDetails;
        }
    }
}