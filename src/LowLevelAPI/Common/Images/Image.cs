// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Assets.Extensions;

namespace VisualTests.LowLevel.Images
{
    public class Image : IDisposable
    {
        protected static List<IDecoder> decoders;

        protected ImageDescription description;

        protected DataBox[] dataBoxes;

        public ImageDescription Description
        {
            get
            {
                return this.description;
            }
        }

        public DataBox[] DataBoxes
        {
            get
            {
                return this.dataBoxes;
            }
        }

        public TextureDescription TextureDescription
        {
            get
            {
                TextureDescription textureDescription = new TextureDescription()
                {
                    Type = this.CalculateType(),
                    Width = this.description.Width,
                    Height = this.description.Height,
                    Format = this.description.pixelFormat,
                    MipLevels = this.description.MipLevels,
                    ArraySize = this.description.ArraySize,
                    Faces = this.description.Faces,
                    Depth = this.description.Depth,
                    CpuAccess = ResourceCpuAccess.None,
                    SampleCount = TextureSampleCount.None,
                    Flags = TextureFlags.ShaderResource,
                    Usage = ResourceUsage.Default,
                };

                textureDescription.Type = ImageHelpers.CalculateType(textureDescription);

                return textureDescription;
            }
        }

        static Image()
        {
            decoders = new List<IDecoder>();
            decoders.Add(new DDSDecoder());
            decoders.Add(new KTXDecoder());
        }

        public static Image Load(Stream stream)
        {
            if (stream == null || !stream.CanRead)
            {
                throw new ArgumentException("Invalid parameter. Stream must be readable", "imageStream");
            }

            Stream seekedStream = stream;
            MemoryStream memstream = null;
            if (!stream.CanSeek)
            {
                memstream = new MemoryStream();
                stream.CopyTo(memstream);

                memstream.Seek(0, SeekOrigin.Begin);
                seekedStream = memstream;
            }

            ImageDescription description = default(ImageDescription);
            DataBox[] dataBoxes = null;
            using (BinaryReader reader = new BinaryReader(seekedStream))
            {
                IDecoder decoder = FindDecoder(reader);
                decoder.DecodeData(reader, out dataBoxes, out description);
            }

            memstream?.Dispose();

            return new Image(description, dataBoxes);
        }

        /// <summary>
        /// Decode image file header (Only read file header).
        /// </summary>
        /// <param name="reader">File stream.</param>
        /// <returns>ImageDescription.</returns>
        public static ImageDescription DecodeHeader(BinaryReader reader)
        {
            IDecoder decoder = FindDecoder(reader);
            decoder.DecodeHeader(reader, out ImageDescription description);
            return description;
        }

        private Image(ImageDescription description, DataBox[] dataBoxes)
        {
            this.description = description;
            this.dataBoxes = dataBoxes;
        }

        private static IDecoder FindDecoder(BinaryReader reader)
        {
            int maxMagicBytesLength = decoders.OrderByDescending(x => x.HeaderSize).First().HeaderSize;
            byte[] magicBytes = new byte[maxMagicBytesLength];
            for (int i = 0; i < maxMagicBytesLength; i += 1)
            {
                magicBytes[i] = reader.ReadByte();
                foreach (var decoder in decoders)
                {
                    if (ImageHelpers.StartsWith(magicBytes, decoder.HeaderBytes))
                    {
                        return decoder;
                    }
                }
            }

            throw new Exception("Could not recognise image format");
        }

        private static IDecoder FindDecoder(MemoryStream reader)
        {
            int maxMagicBytesLength = decoders.OrderByDescending(x => x.HeaderSize).First().HeaderSize;
            byte[] magicBytes = new byte[maxMagicBytesLength];
            for (int i = 0; i < maxMagicBytesLength; i += 1)
            {
                magicBytes[i] = (byte)reader.ReadByte();
                foreach (var decoder in decoders)
                {
                    if (ImageHelpers.StartsWith(magicBytes, decoder.HeaderBytes))
                    {
                        return decoder;
                    }
                }
            }

            throw new Exception("Could not recognise image format");
        }

        private TextureType CalculateType()
        {
            if (this.description.ArraySize == 6)
            {
                // Texture Cube without miplevels
                return TextureType.TextureCube;
            }
            else if (this.description.Depth > 1)
            {
                return TextureType.Texture3D;
            }
            else if (this.description.Height == 1)
            {
                return TextureType.Texture1D;
            }
            else
            {
                return TextureType.Texture2D;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.DataBoxes != null)
            {
                foreach (DataBox databox in this.DataBoxes)
                {
                    databox.Dispose();
                }
            }
        }
    }
}
