// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using SkiaSharp;
using System;
using System.IO;

namespace VisualTests.LowLevel.Images
{
    public static class ImageHelpers
    {
        public static byte[] GetImageArray(Stream stream, bool premultiplyAlpha, out int dataLength)
        {
            // SKManagedStream does not dispose the stream, so the caller keeps owning it.
            using (var codec = SKCodec.Create(new SKManagedStream(stream)) ?? throw new InvalidOperationException("Unsupported image format."))
            {
                var info = new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, premultiplyAlpha ? SKAlphaType.Premul : SKAlphaType.Unpremul);
                var result = codec.GetPixels(info, out byte[] data);
                if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput)
                {
                    throw new InvalidOperationException($"Could not decode image: {result}");
                }

                dataLength = data.Length;
                return data;
            }
        }
    }
}
