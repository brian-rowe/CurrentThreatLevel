﻿using System;
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
            HttpResponseMessage result = null;

            System.IO.Stream memStream = new System.IO.MemoryStream();

            using (MagickImage image = new MagickImage(new MagickColor("#" + imageDefinition.bgColor), 828, 315))
            {
                MagickColor mTextColor = new MagickColor("#" + imageDefinition.textColor);

                new Drawables()
                    .FillColor(mTextColor)
                    .Font("Times New Roman")
                    .StrokeWidth(0.25)
                    .StrokeColor(mTextColor)
                    .TextAlignment(TextAlignment.Left)

                    .FontPointSize(28)
                    .Text(64, 105, "current threat level is " + imageDefinition.threatLevel)

                    .FontPointSize(20)
                    .Text(64, 210, imageDefinition.text)

                    .Draw(image);

                image.Format = MagickFormat.Png;
                image.Write(memStream);
            }

            /* The image.Write command will set memStream.Position to the size of the image, so we must reset it before passing it in the StreamContent constructor in order to successfully download the iamge */
            memStream.Position = 0;

            /* Required headers */
            result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(memStream);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = this.getFileName(imageDefinition.threatLevel, imageDefinition.text) + ".png";

            System.Web.HttpContext.Current.Response.SetCookie(new System.Web.HttpCookie("fileDownload", "true") { Path = "/" });

            return result;
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
