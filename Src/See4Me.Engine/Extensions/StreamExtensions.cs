using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine.Extensions
{
    internal static class StreamExtensions
    {
        public static async Task<byte[]> ToArrayAsync(this Stream stream)
        {
            stream.Position = 0;

            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                stream.Position = 0;
                return ms.ToArray();
            }
        }
    }
}
