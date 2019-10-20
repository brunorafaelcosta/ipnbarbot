using System;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace ipnbarbot.Application.Helpers
{
    public static class FacebookHelpers
    {
        private const string FacebookCookieUrl = ".facebook.com";
        private const string FacebookBaseUrl = "https://mobile.facebook.com/";
        private const string FacebookPhotosGalleryUrl = "photos";

        private static HtmlWeb GetHtmlWeb()
        {
            return new HtmlWeb
                {
                    AutoDetectEncoding = true,
                    PreRequest = request =>
                    {
                        request.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 5.1; rv:10.0.2) Gecko/20100101 Firefox/10.0.2";
                        request.Headers.Add("Accept-Language", "en-US,en;q=0.5");

                        CookieContainer facebookCookies = new CookieContainer();
                        facebookCookies.Add(new Cookie("lh", "en_US") { Domain = FacebookCookieUrl, HttpOnly = false });
                        facebookCookies.Add(new Cookie("locale", "en_US") { Domain = FacebookCookieUrl, HttpOnly = false });
                        request.CookieContainer = facebookCookies;

                        return true;
                    }
                };
        }

        public static async Task<byte[]> GetLastPostImage(string facebookPage, ILogger logger)
        {
            try
            {
                var photoGalleryUrl = new Uri(new Uri(FacebookBaseUrl), $"{facebookPage}/{FacebookPhotosGalleryUrl}").ToString();
                var photoGalleryWeb = GetHtmlWeb();

                logger.LogDebug($"Facebook - Goind to load photo gallery page. Url: {photoGalleryUrl}");

                var photoGalleryDoc = photoGalleryWeb.Load(photoGalleryUrl);

                var lastphotoGalleryPostElement = photoGalleryDoc
                    .DocumentNode
                    .SelectSingleNode("//h3[text()=\"Uploads\"]/following-sibling::div/table/tbody/tr/td[1]/a");

                if (lastphotoGalleryPostElement is null)
                {
                    throw new Exception("Facebook last photo on gallery not found.");
                }
                else
                {
                    string lastphotoGalleryPostUrl = lastphotoGalleryPostElement.Attributes["href"].Value;
                    if (lastphotoGalleryPostUrl.StartsWith(facebookPage) || lastphotoGalleryPostUrl.StartsWith($"/{facebookPage}"))
                    {
                        lastphotoGalleryPostUrl = new Uri(new Uri(FacebookBaseUrl), lastphotoGalleryPostUrl).ToString();
                    }

                    var lastPhotoWeb = GetHtmlWeb();
                    
                    logger.LogDebug($"Facebook - Goind to load the last photo from the gallery page. Url: {lastphotoGalleryPostUrl}");

                    var lastPhotoDoc = photoGalleryWeb.Load(lastphotoGalleryPostUrl);

                    var openLastPhotoFullImageElement = lastPhotoDoc
                        .DocumentNode
                        .SelectSingleNode("//a[text()=\"View Full Size\"]");
                    
                    if (openLastPhotoFullImageElement is null)
                    {
                        throw new Exception("Facebook 'View Full Size' button not found.");
                    }
                    else
                    {
                        string lastPhotoFullImageUrl = openLastPhotoFullImageElement.Attributes["href"].Value;
                        lastPhotoFullImageUrl = lastPhotoFullImageUrl.Replace("&amp;", "&");

                        logger.LogDebug($"Facebook - Goind to download the last photo from the gallery page. Url: {lastPhotoFullImageUrl}");

                        byte[] imageData = await Task.Run<byte[]>(() =>
                        {
                            using (var facebookWebClient = new WebClient())
                            {
                                return facebookWebClient.DownloadData(lastPhotoFullImageUrl);
                            }
                        });

                        if (imageData is null)
                        {
                            throw new Exception("Facebook image download failed.");
                        }
                        else
                        {
                            logger.LogDebug($"Facebook - Sucessfully download the photo");
                            return imageData;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
