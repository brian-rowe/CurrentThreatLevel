using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageMagick;

namespace CurrentThreatLevel.Content.Controllers
{
    public class ExportController : ApiController
    {
        // POST: api/Export
        public HttpResponseMessage Get([FromUri]Models.CurrentThreatImage imageDefinition) 
        {
            HttpResponseMessage response = null;

            /* Conduit for getting the image into the response content */
            System.IO.Stream memStream = new System.IO.MemoryStream();

            int imageHeight = 315;
            int imageWidth = 828;

            using (MagickImage image = new MagickImage(new MagickColor("#" + imageDefinition.bgColor), imageWidth, imageHeight))
            {
                MagickColor mTextColor = new MagickColor("#" + imageDefinition.textColor);

                System.Drawing.Point textOffset = new System.Drawing.Point();

                textOffset.X = 64;
                textOffset.Y = imageHeight / 3;

                new Drawables()
                    .FillColor(mTextColor)
                    .Font("Times New Roman")
                    .StrokeWidth(0.25)
                    .StrokeColor(mTextColor)
                    .TextAlignment(TextAlignment.Left)

                    .FontPointSize(28)
                    .Text(textOffset.X, textOffset.Y, "current threat level is " + imageDefinition.threatLevel)

                    .FontPointSize(20)
                    .Text(textOffset.X, textOffset.Y * 2, imageDefinition.text)

                    .Draw(image);

                image.Format = MagickFormat.Png;
                image.Write(memStream);
            }

            /* The image.Write command will set memStream.Position to the size of the image, so we must reset it before passing it in the StreamContent constructor in order to successfully download the iamge */
            memStream.Position = 0;

            /* Required headers */
            response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(memStream);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = this.getFileName(imageDefinition.threatLevel, imageDefinition.text) + ".png";

            System.Web.HttpContext.Current.Response.SetCookie(new System.Web.HttpCookie("fileDownload", "true") { Path = "/" });

            return response;
        }

        private string getFileName(string threatLevel, string text)
        {
            string baseText = threatLevel + ' ' + text;

            // Strip punctuation
            string stripped_version = new string(baseText.Where(c => !char.IsPunctuation(c)).ToArray()).ToLower();

            // Replace all spaces with underscores;
            return string.Join("_", stripped_version.Split(' '));
        }
    }
}
