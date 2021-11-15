// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.IO;
using Evergine.Assets.Extensions.KTX;
using Evergine.Common.Graphics;
using Evergine.Framework.Assets.Extensions;

namespace VisualTests.LowLevel.Images
{
    /// <summary>
    /// KTX File Format decoder
    /// https://www.khronos.org/opengles/sdk/tools/KTX/file_format_spec/#2.
    /// </summary>
    public class KTXDecoder : IDecoder
    {
        /// <summary>
        /// TGA Header format bytes.
        /// </summary>
        private static readonly byte[] headerBytes = new byte[] { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x31, 0x31, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A };

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
        /// Decode TGA Header.
        /// </summary>
        /// <param name="reader">Image stream.</param>
        /// <param name="description">Image description.</param>
        public unsafe void DecodeHeader(BinaryReader reader, out ImageDescription description)
        {
            KTXHeader header = ImageHelpers.ReadStruct<KTXHeader>(reader);

            description = new ImageDescription()
            {
                imageFormat = ImageFormat.KTX,
                Width = Math.Max(1, header.pixelWidth),
                Height = Math.Max(1, header.pixelHeight),
                Depth = Math.Max(1, header.pixelDepth),
                MipLevels = Math.Max(1, header.numberOfMipmapLevels),
                ArraySize = Math.Max(1, header.numberOfArrayElements),
                Faces = Math.Max(1, header.numberOfFaces),
                pixelFormat = SupportedFormats.FromOpenGLFormat(ref header),
            };
        }

        /// <summary>
        /// Decode TGA data.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <param name="databoxes">Databoxes array.</param>
        /// <param name="description">Image Description.</param>
        public void DecodeData(BinaryReader reader, out DataBox[] databoxes, out ImageDescription description)
        {
            KTXTexture texture = KTXTexture.Load(reader, true);
            var header = texture.Header;

            description = new ImageDescription()
            {
                imageFormat = ImageFormat.KTX,
                Width = Math.Max(1, texture.Header.pixelWidth),
                Height = Math.Max(1, texture.Header.pixelHeight),
                Depth = Math.Max(1, texture.Header.pixelDepth),
                MipLevels = Math.Max(1, texture.Header.numberOfMipmapLevels),
                ArraySize = Math.Max(1, texture.Header.numberOfArrayElements),
                Faces = Math.Max(1, header.numberOfFaces),
                pixelFormat = SupportedFormats.FromOpenGLFormat(ref header),
            };

            databoxes = new DataBox[description.ArraySize * description.Faces * description.MipLevels];

            for (int mipmapIndex = 0; mipmapIndex < texture.Mipmaps.Length; mipmapIndex++)
            {
                var level = texture.Mipmaps[mipmapIndex];
                for (int sliceIndex = 0; sliceIndex < level.ArrayElements.Length; sliceIndex++)
                {
                    var slice = level.ArrayElements[sliceIndex];

                    for (int faceIndex = 0; faceIndex < slice.Faces.Length; faceIndex++)
                    {
                        var face = slice.Faces[faceIndex];

                        uint formatSize = description.pixelFormat.GetSizeInBits() / 8;
                        uint rowPitch = (uint)level.Width * formatSize;
                        uint slicePitch = (uint)face.Data.Length;                        
                        int sliceIndexCalculated = (sliceIndex * slice.Faces.Length) + faceIndex;
                        int index = (sliceIndexCalculated * (int)description.MipLevels) + mipmapIndex;


                        databoxes[index] = new DataBox(face.Data, rowPitch, slicePitch);
                    }
                }
            }
        }
    }
}
