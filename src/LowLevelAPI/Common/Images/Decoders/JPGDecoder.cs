// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using Evergine.Common.Graphics;

namespace VisualTests.LowLevel.Images
{
    public class JPGDecoder : IDecoder
    {
        /// <summary>
        /// PNG Header format bytes.
        /// </summary>
        private static readonly byte[] headerBytes = new byte[] { 0xff, 0xd8 };

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

        /// <summary>
        /// Decode JPG Header.
        /// </summary>
        /// <param name="reader">Image stream.</param>
        /// <param name="description">Image description.</param>
        public void DecodeHeader(BinaryReader reader, out ImageDescription description)
        {
            var stream = reader.BaseStream;
            stream.Seek(0, SeekOrigin.Begin);
            var imageInfo = SixLabors.ImageSharp.Image.Identify(stream);

            description = new ImageDescription()
            {
                imageFormat = ImageFormat.JPG,
                Width = (uint)imageInfo.Width,
                Height = (uint)imageInfo.Height,
                Depth = 1,
                MipLevels = 1,
                ArraySize = 1,
                Faces = 1,
                pixelFormat = Evergine.Common.Graphics.PixelFormat.R8G8B8A8_UNorm,
            };

            /*while (reader.ReadByte() == 0xff)
            {
                byte marker = reader.ReadByte();
                short chunkLength = ImageHelpers.ReadLittleEndianInt16(reader);
                if (marker == 0xc0)
                {
                    reader.ReadByte();
                    uint height = (uint)ImageHelpers.ReadLittleEndianInt16(reader);
                    uint width = (uint)ImageHelpers.ReadLittleEndianInt16(reader);

                    description = new ImageDescription()
                    {
                        imageFormat = ImageFormat.JPG,
                        Width = width,
                        Height = height,
                        Depth = 1,
                        MipLevels = 1,
                        ArraySize = 1,
                        Faces = 1,
                        pixelFormat = Evergine.Common.Graphics.PixelFormat.R8G8B8A8_UNorm,
                    };

                    return;
                }
                else
                {
                    if (chunkLength < 0)
                    {
                        ushort uchunkLength = (ushort)chunkLength;
                        reader.ReadBytes(uchunkLength - 2);
                    }
                    else
                    {
                        reader.ReadBytes(chunkLength - 2);
                    }
                }
            }

            throw new ArgumentException("Could not recognize JPG Header.");*/
        }

        /// <summary>
        /// Decode JPG data.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <param name="databoxes">Databoxes array.</param>
        /// <param name="description">Image Description.</param>
        public void DecodeData(BinaryReader reader, out DataBox[] databoxes, out ImageDescription description)
        {
            this.DecodeHeader(reader, out description);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            byte[] data;
            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(reader.BaseStream))
            {
                data = ImageHelpers.GetImageArray(image, false, out _);
            }

            uint rowPitch = description.pixelFormat.GetSizeInBytes(description.Width);
            uint slicePitch = description.pixelFormat.GetSizeInBytes(description.Width, description.Height);
            databoxes = new DataBox[] { new DataBox(data, rowPitch, slicePitch) };
        }
    }
}
