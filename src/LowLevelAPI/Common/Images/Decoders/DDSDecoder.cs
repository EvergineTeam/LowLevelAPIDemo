// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Evergine.Assets.Extensions.DDS;
using Evergine.Common.Graphics;
using Evergine.Framework.Assets.Extensions;

namespace VisualTests.LowLevel.Images
{
    public class DDSDecoder : IDecoder
    {
        // 1_5_5_5_ARGB.dds  -> not supported
        // 16_16_16_16F_ABGR.dds -> supported
        // 16_16_GB.dds.dds -> supported RG
        // 16_16_VU.dds -> not supported
        // 16_16F_GR.dds -> supported RG
        // 16F_R.dds -> not supported
        // 32_32_32_32F_ABGR.dds -> supported
        // 32F_R.dds -> supported
        // 3DcXY.dds -> not supported
        // 4_4_4_4_RGB.dds -> not supported
        // 5_5_5_RGB.dds -> not supported
        // 5_6_5_RGB.dds -> not supported
        // 8_8_8_8QWVU.dds -> not supported
        // 8_8_8_8_ARGB.dds -> supported
        // 8_8_8_RGB.dds -> not supported
        // 8_8_AL.dds -> not supported
        // 8_8_CxVU.dds -> not supported
        // 8_8_VU.dds -> not supported
        // 8A.dds -> not supported
        // 8L.dds -> not supported
        // DXT1ARGB.dds -> supported but no draw good
        // DXT1RGB.dds -> supported but not draw good
        // DXT3ARGB.dds -> supported
        // DXT5_NMXY.dds -> supported
        // DXT5ARGB.dds -> supported
        // paletteARGB4.dds -> not supported
        // paletteARGB8.dds -> not supported
        // paletteRGB4.dds -> not supported
        // paletteRGB8.dds -> not supported
        // X_8_8_8_XRGB.dds -> supported
        // Total 10/30

        /// <summary>
        /// PNG Header format bytes.
        /// </summary>
        private static readonly byte[] headerBytes = new byte[] { 0x44, 0x44, 0x53, 0x20 };

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
        /// Decode DDS Header.
        /// </summary>
        /// <param name="reader">Image stream.</param>
        /// <param name="description">Image description.</param>
        public void DecodeHeader(BinaryReader reader, out ImageDescription description)
        {
            Header header = ImageHelpers.ReadStruct<Header>(reader);

            DxgiFormat dxgiFormat;
            D3DFormat d3dFormat = D3DFormat.Unknown;
            HeaderDx10 headerDX10 = default;
            if (header.PixelFormat.FourCC != Helper.DX10)
            {
                Helper.DetermineFormat(ref header.PixelFormat, out dxgiFormat, out d3dFormat);
            }
            else
            {
                byte[] headerDx10Bytes = reader.ReadBytes(Marshal.SizeOf(typeof(HeaderDx10)));
                GCHandle headerDx10Handle = GCHandle.Alloc(headerDx10Bytes, GCHandleType.Pinned);
                headerDX10 = (HeaderDx10)Marshal.PtrToStructure(headerDx10Handle.AddrOfPinnedObject(), typeof(HeaderDx10));
                headerDx10Handle.Free();

                dxgiFormat = headerDX10.Format;
            }

            uint faces = 1;
            if (headerDX10.MiscFlags.HasFlag(ResourceMiscFlags.TextureCube) ||
                header.Caps2.HasFlag(Caps2.CubeMap))
            {
                faces = 6;
            }

            description = new ImageDescription()
            {
                imageFormat = ImageFormat.DDS,
                Width = header.Width,
                Height = header.Height,
                Depth = header.Flags.HasFlag(HeaderFlags.Depth) ? header.Depth : 1,
                MipLevels = header.MipMapCount == 0 ? 1 : header.MipMapCount,
                ArraySize = headerDX10.ArraySize == 0 ? 1 : headerDX10.ArraySize,
                Faces = faces,
                pixelFormat = (Evergine.Common.Graphics.PixelFormat)dxgiFormat,
            };
        }

        /// <summary>
        /// Decode BMP Header.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <param name="databoxes">Databoxes array.</param>
        /// <param name="description">Image Description.</param>
        public unsafe void DecodeData(BinaryReader reader, out DataBox[] databoxes, out ImageDescription description)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            DdsTexture dds = new DdsTexture(bytes);


            uint faces = 1;
            if (dds.MiscFlags10.HasFlag(ResourceMiscFlags.TextureCube))
            {
                faces = 6;
            }

            description = new ImageDescription()
            {
                imageFormat = ImageFormat.DDS,
                Width = (uint)dds.Width,
                Height = (uint)dds.Height,
                Depth = (uint)dds.Depth,
                MipLevels = (uint)dds.MipInfos.Length,
                ArraySize = (uint)dds.Data.Length / faces,
                Faces = faces,
                pixelFormat = (Evergine.Common.Graphics.PixelFormat)dds.DxgiFormat,
            };

            databoxes = new DataBox[description.ArraySize * description.Faces * description.MipLevels];

            for (uint i = 0; i < description.ArraySize; i++)
            {
                for (uint j = 0; j < description.Faces; j++)
                {
                    byte[] data = dds.Data[(i * description.Faces) + j];

                    GCHandle pinnedHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    IntPtr dataPointer = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);

                    for (uint z = 0; z < description.MipLevels; z++)
                    {
                        ImageMipInfo mipInfo = dds.MipInfos[z];
                        
                        uint slicePitch = (uint)mipInfo.SizeInBytes;                        
                        uint rowPitch = slicePitch / (uint)mipInfo.Height;                   
                        uint index = (i * description.Faces) + (j * description.MipLevels) + z;

                        databoxes[index] = new DataBox(IntPtr.Add(dataPointer, (int)mipInfo.OffsetInBytes), rowPitch, slicePitch);
                    }

                    pinnedHandle.Free();
                }
            }
        }
    }
}
