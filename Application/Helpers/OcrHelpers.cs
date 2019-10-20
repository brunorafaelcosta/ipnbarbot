using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using Tesseract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ipnbarbot.Application.Helpers
{
    public static class OcrHelpers
    {
        private static async Task<byte[]> NormalizeImageAsync(byte[] imageData)
        {
            return await Task.Run<byte[]>(() =>
            {
                // https://github.com/tesseract-ocr/tesseract/wiki/ImproveQuality
                var image = new ImageMagick.MagickImage(imageData);

                image.AutoOrient();

                image.Density = new Density(300);
                
                image.Despeckle();

                image.Normalize();

                image.Grayscale();

                //image.Negate();

                //image.AdaptiveThreshold(25, 10, 5);

                //image.ContrastStretch(new Percentage(0), new Percentage(0));

                //image.Negate();

                return image.ToByteArray(ImageMagick.MagickFormat.Tiff);
            });
        }

        public static async Task<string> ReadTextFromImageAsync(string imagePath, IConfigurationSection configuration, ILogger logger)
        {
            if (System.IO.File.Exists(imagePath))
            {
                using (var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    var binaryReader = new BinaryReader(fs);

                    byte[] imageData = await Task.Run<byte[]>(() =>
                    {
                        return binaryReader.ReadBytes((int)fs.Length);
                    });

                    return await ReadTextFromImageAsync(imageData, configuration, logger);
                }
            }
            else
            {
                throw new FileNotFoundException(imagePath);
            }
        }

        public static async Task<string> ReadTextFromImageAsync(byte[] imageData, IConfigurationSection configuration, ILogger logger)
        {
            string dataPath = configuration.GetValue<string>("DataPath");
            string language = configuration.GetValue<string>("Language");
            float minimumMeanConfidence = configuration.GetValue<float>("MinimumMeanConfidence");

            byte[] normalizedImageData = await NormalizeImageAsync(imageData);

            string text = await Task.Run<string>(() =>
            {
                logger.LogDebug("Tesseract - Starting engine...");
                using (var engine = new TesseractEngine(dataPath, language, EngineMode.Default))
                {
                    using (var img = Pix.LoadTiffFromMemory(normalizedImageData))
                    {
                        logger.LogDebug("Tesseract - Going to process the image...");
                        using (var page = engine.Process(img, PageSegMode.Auto))
                        {
                            float meanConfidence = page.GetMeanConfidence();
                            string result = page.GetText();

                            logger.LogDebug($"Tesseract - Image sucessfully processed! Mean confidence: {meanConfidence} Text: {result}");
                            
                            if (meanConfidence <= minimumMeanConfidence)
                                throw new Exception($"Mean Confidence is {meanConfidence} but cannot be lower than {minimumMeanConfidence}");

                            return result;
                        }
                    }
                }
            });

            return text;
        }
    }
}