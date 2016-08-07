using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;

namespace See4Me.Services
{
    /// <summary>
    /// Wrapper class around IMediaEncodingProperties to help how devices report supported resolutions
    /// </summary>
    public class StreamResolution
    {
        private readonly IMediaEncodingProperties properties;

        public StreamResolution(IMediaEncodingProperties properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            // Only handle ImageEncodingProperties and VideoEncodingProperties, which are the two types that GetAvailableMediaStreamProperties can return
            if (!(properties is ImageEncodingProperties) && !(properties is VideoEncodingProperties))
                throw new ArgumentException($"Argument is of the wrong type. Required: {typeof(ImageEncodingProperties).Name} or {typeof(VideoEncodingProperties).Name}.", nameof(properties));

            // Store the actual instance of the IMediaEncodingProperties for setting them later
            this.properties = properties;
        }

        public uint Width
        {
            get
            {
                if (properties is ImageEncodingProperties)
                    return (properties as ImageEncodingProperties).Width;

                else if (properties is VideoEncodingProperties)
                    return (properties as VideoEncodingProperties).Width;

                return 0;
            }
        }

        public uint Height
        {
            get
            {
                if (properties is ImageEncodingProperties)
                    return (properties as ImageEncodingProperties).Height;

                if (properties is VideoEncodingProperties)
                    return (properties as VideoEncodingProperties).Height;

                return 0;
            }
        }

        public uint FrameRate
        {
            get
            {
                if (properties is VideoEncodingProperties)
                {
                    if ((properties as VideoEncodingProperties).FrameRate.Denominator != 0)
                        return (properties as VideoEncodingProperties).FrameRate.Numerator / (properties as VideoEncodingProperties).FrameRate.Denominator;
                }

                return 0;
            }
        }

        public double AspectRatio => Math.Round((Height != 0) ? (Width / (double)Height) : double.NaN, 2);

        public IMediaEncodingProperties EncodingProperties => properties;

        /// <summary>
        /// Output properties to a readable format for UI purposes
        /// eg. 1920x1080 [1.78] 30fps MPEG
        /// </summary>
        /// <returns>Readable string</returns>
        public string GetFriendlyName(bool showFrameRate = true)
        {
            if (properties is ImageEncodingProperties || !showFrameRate)
                return $"{Width}x{Height} [{AspectRatio}] {properties.Subtype}";

            if (properties is VideoEncodingProperties)
                return $"{Width}x{Height} [{AspectRatio}] {FrameRate} FPS {properties.Subtype}";

            return string.Empty;
        }
    }
}
