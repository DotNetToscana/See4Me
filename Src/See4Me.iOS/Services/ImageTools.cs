using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UIKit;

namespace See4Me.Services
{
    public static class ImageTools
    {
        public static UIImage MaxResizeImage(this UIImage sourceImage)
        {
            float h = 640.0f, w = 480.0f;

            //if(sourceImage.Orientation == UIImageOrientation.Up)
            if (sourceImage.Size.Height == 0)
                return sourceImage;

            if (sourceImage.Size.Height < sourceImage.Size.Width)
            {
                w = 640.0f;
                h = 480.0f;
            }

            return MaxResizeImage(sourceImage,w,h);
        }

        public static UIImage MaxResizeImage(this UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

            if (maxResizeFactor > 1)
                return sourceImage;

            var width =  maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;

            UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
            sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return resultImage;
        }

        // resize the image (without trying to maintain aspect ratio)
        public static UIImage ResizeImage(this UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return resultImage;
        }

        // crop the image, without resizing
        private static UIImage CropImage(this UIImage sourceImage, int cropX, int cropY, int width, int height)
        {
            var imgSize = sourceImage.Size;
            UIGraphics.BeginImageContext(new SizeF(width, height));
            var context = UIGraphics.GetCurrentContext();
            var clippedRect = new RectangleF(0, 0, width, height);
            context.ClipToRect(clippedRect);
            var drawRect = new RectangleF(-cropX, -cropY, (float)imgSize.Width, (float)imgSize.Height);
            sourceImage.Draw(drawRect);
            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return modifiedImage;
        }
    }
}
