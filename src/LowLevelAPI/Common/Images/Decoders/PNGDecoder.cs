// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using Evergine.Common.Graphics;
using CommonImageHelpers = Evergine.Common.Helpers.ImageHelpers;

namespace VisualTests.LowLevel.Images
{
    public class PNGDecoder : IDecoder
    {
        /// <summary>
        /// PNG Header format bytes.
        /// </summary>
        private static readonly byte[] headerBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        /// <summary>
        /// Gets header bytes.
        /// </summary>
        public byte[] HeaderBytes
        {
            get
            {
                return headerBytes;
            }
        }

        /// <summary>
        /// Gets header size.
        /// </summary>
        public int HeaderSize
        {
            get
            {
                return headerBytes.Length;
            }
        }

        public object PixelConversionModifiers { get; private set; }

        /// <summary>
        /// Decode PNG Header.
        /// </summary>
        /// <param name="reader">Image stream.</param>
        /// <param name="description">Image description.</param>
        public void DecodeHeader(BinaryReader reader, out ImageDescription description)
        {
#if IOS
            reader.ReadBytes(24);
#else
            reader.ReadBytes(8);
#endif
            uint width = (uint)CommonImageHelpers.ReadLittleEndianInt32(reader);
            uint height = (uint)CommonImageHelpers.ReadLittleEndianInt32(reader);

            description = new ImageDescription()
            {
                imageFormat = ImageFormat.PNG,
                Width = width,
                Height = height,
                Depth = 1,
                MipLevels = 1,
                ArraySize = 1,
                Faces = 1,
                pixelFormat = Evergine.Common.Graphics.PixelFormat.R8G8B8A8_UNorm,
            };
        }

        /// <summary>
        /// Decode PNG data.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <param name="databoxes">Databoxes array.</param>
        /// <param name="description">Image Description.</param>
        public void DecodeData(BinaryReader reader, out DataBox[] databoxes, out ImageDescription description)
        {
            this.DecodeHeader(reader, out description);
            if (reader.BaseStream.CanSeek)
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                byte[] data;
                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(reader.BaseStream))
                {
                    data = ImageHelpers.GetImageArray(image, true, out _);
                }

                uint pitch = description.pixelFormat.GetSizeInBytes(description.Width);
                uint sliceSize = description.pixelFormat.GetSizeInBytes(description.Width, description.Height);
                databoxes = new DataBox[] { new DataBox(data, pitch, sliceSize) };
            }
            else
            {
                throw new Exception("Is not possible decode file");
            }
        }
    }
}
