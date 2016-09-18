using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace See4Me.Extensions
{
    public static class ImageExtensions
    {
        public static async Task<SoftwareBitmap> AsSoftwareBitmapAsync(this Stream stream)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
            var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            var softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            return softwareBitmapBGR8;
        }

        public static async Task<ImageSource> AsImageSourceAsync(this SoftwareBitmap softwareBitmap)
        {
            var bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmap);

            return bitmapSource;
        }

        public static async Task<ImageSource> AsImageSourceAsync(this Stream stream)
        {
            var softwareBitmap = await stream.AsSoftwareBitmapAsync();
            var bitmapSource = await softwareBitmap.AsImageSourceAsync();

            return bitmapSource;
        }

        public static async Task SetSourceAsync(this BitmapImage image, byte[] buffer)
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(buffer.AsBuffer());
                stream.Seek(0);

                await image.SetSourceAsync(stream);
            }
        }

        public static async Task SetSourceAsync(this Image image, byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
                image.Source = await ms.AsImageSourceAsync();
        }
    }
}
