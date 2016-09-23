using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Represents an image of the page currently loaded in the browser.
    /// </summary>
    [Serializable]
    public class Screenshot
    {
        private string base64Encoded = string.Empty;
        private byte[] byteArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="Screenshot"/> class.
        /// </summary>
        /// <param name="base64EncodedScreenshot">The image of the page as a Base64-encoded string.</param>
        public Screenshot(string base64EncodedScreenshot)
        {
            this.base64Encoded = base64EncodedScreenshot;
            this.byteArray = Convert.FromBase64String(this.base64Encoded);
        }

        /// <summary>
        /// Gets the value of the screenshot image as a Base64-encoded string.
        /// </summary>
        public string AsBase64EncodedString
        {
            get { return this.base64Encoded; }
        }

        /// <summary>
        /// Gets the value of the screenshot image as an array of bytes.
        /// </summary>
        public byte[] AsByteArray
        {
            get { return this.byteArray; }
        }

        /// <summary>
        /// Saves the screenshot to a file, overwriting the file if it already exists.
        /// </summary>
        /// <param name="fileName">The full path and file name to save the screenshot to.</param>
        /// <param name="format">A <see cref="System.Drawing.Imaging.ImageFormat"/> object indicating the format
        /// to save the image to.</param>
        public void SaveAsFile(string fileName, ImageFormat format)
        {
            using (MemoryStream imageStream = new MemoryStream(this.byteArray))
            {
                Image screenshotImage = Image.FromStream(imageStream);
                screenshotImage.Save(fileName, format);
            }
        }

        /// <summary>
        /// Returns a <see cref="string">String</see> that represents the current <see cref="object">Object</see>.
        /// </summary>
        /// <returns>A <see cref="string">String</see> that represents the current <see cref="object">Object</see>.</returns>
        public override string ToString()
        {
            return this.base64Encoded;
        }
    }
}