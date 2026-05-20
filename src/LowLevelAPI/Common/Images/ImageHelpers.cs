// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Runtime.InteropServices;

namespace VisualTests.LowLevel.Images
{
    public static class ImageHelpers
    {
        public static byte[] GetImageArray(Image<Rgba32> image, bool premultiplyAlpha, out int dataLength)
        {
            var bytesPerPixel = image.PixelType.BitsPerPixel / 8;
            dataLength = image.Width * image.Height * bytesPerPixel;
            var data = new byte[dataLength];

            image.ProcessPixelRows(accessor =>
            {
                var dataPixels = MemoryMarshal.Cast<byte, Rgba32>(new Span<byte>(data));
                for (int i = 0; i < image.Height; i++)
                {
                    var row = accessor.GetRowSpan(i);
                    if (premultiplyAlpha)
                    {
                        CopyToPremultiplied(row, dataPixels.Slice(i * image.Width, image.Width));
                    }
                    else
                    {
                        row.CopyTo(dataPixels.Slice(i * image.Width, image.Width));
                    }
                }
            });

            return data;
        }

        private static void CopyToPremultiplied(ReadOnlySpan<Rgba32> pixels, Span<Rgba32> destination)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                Rgba32 pixel = pixels[i];
                ref Rgba32 destinationPixel = ref destination[i];
                ref var a = ref pixel.A;
                if (a == 0)
                {
                    destinationPixel.PackedValue = 0;
                }
                else
                {
                    destinationPixel.R = (byte)((pixel.R * a) >> 8);
                    destinationPixel.G = (byte)((pixel.G * a) >> 8);
                    destinationPixel.B = (byte)((pixel.B * a) >> 8);
                    destinationPixel.A = pixel.A;
                }
            }
        }
    }
}
