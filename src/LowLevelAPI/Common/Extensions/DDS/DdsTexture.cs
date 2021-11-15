// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

/*
Copyright (c) 2012 Daniil Rodin

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

   1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.

   2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.

   3. This notice may not be removed or altered from any source
   distribution.
*/

using System;
using System.IO;
using System.Runtime.InteropServices;
using Evergine.Framework.Assets.Extensions;

namespace Evergine.Assets.Extensions.DDS
{
    internal class DdsTexture
    {
        public DxgiFormat DxgiFormat { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Depth { get; private set; }

        public D3DFormat D3DFormat { get; private set; }

        public ResourceDimension Dimension10 { get; private set; }

        public ResourceMiscFlags MiscFlags10 { get; private set; }

        public byte[][] Data { get; private set; }

        public ImageMipInfo[] MipInfos { get; private set; }

        public DdsTexture(
            DxgiFormat dxgiFormat,
            int width,
            int height = 1,
            int depth = 1,
            int arraySize = 1,
            int mipCount = 0,
            ResourceDimension dimension = ResourceDimension.Unknown,
            ResourceMiscFlags miscFlags = 0)
        {
            if (dxgiFormat == DxgiFormat.UNKNOWN)
            {
                throw new ArgumentException("Formats can not be 'unknown'");
            }

            if (width <= 0)
            {
                throw new ArgumentException("Width must be greater than zero");
            }

            if (height <= 0)
            {
                throw new ArgumentException("Height must be greater than zero");
            }

            if (depth <= 0)
            {
                throw new ArgumentException("Depth must be greater than zero");
            }

            if (arraySize <= 0)
            {
                throw new ArgumentException("Array size must be greater than zero");
            }

            if (mipCount < 0)
            {
                throw new ArgumentException("Mip count must can not be negative");
            }

            if (dimension != ResourceDimension.Unknown && dimension != ResourceDimension.Texture3D && depth != 1)
            {
                throw new ArgumentException(string.Format("Dimension10 is {0}, but Depth is not 1", dimension));
            }

            if ((dimension == ResourceDimension.Buffer || dimension == ResourceDimension.Texture1D) && height != 1)
            {
                throw new ArgumentException(string.Format("Dimension10 is {0}, but Height is not 1", dimension));
            }

            if (dimension == ResourceDimension.Buffer && mipCount > 1)
            {
                throw new ArgumentException("Dimension10 is Buffer, but MipCount is not 0 or 1");
            }

            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            /*
            if (depth != 1)
                Dimension10 = ResourceDimension.Texture3D;
            else if (height != 1)
                Dimension10 = ResourceDimension.Texture2D;
            else
                Dimension10 = ResourceDimension.Texture1D;*/

            this.MiscFlags10 = miscFlags;

            this.D3DFormat = Helper.D3DFormatFromDxgi(dxgiFormat);

            if (mipCount == 0)
            {
                int mipWidth = width;
                int mipHeight = height;
                int mipDepth = depth;

                mipCount = 1;
                while (mipWidth != 1 || mipHeight != 1 || mipDepth != 1)
                {
                    if (mipWidth != 1)
                    {
                        mipWidth /= 2;
                    }

                    if (mipHeight != 1)
                    {
                        mipHeight /= 2;
                    }

                    if (mipDepth != 1)
                    {
                        mipDepth /= 2;
                    }

                    mipCount++;
                }
            }

            this.Data = new byte[arraySize][];

            int chainSize;
            this.CalculateMipInfos(mipCount, -1, -1, out chainSize);

            for (int i = 0; i < this.Data.Length; i++)
            {
                this.Data[i] = new byte[chainSize];
            }
        }

        public unsafe DdsTexture(byte[] fileData, int byteOffset = 0)
        {
            fixed (byte* pFileData = fileData)
            {
                byte* p = pFileData + byteOffset;
                int remaining = fileData.Length - byteOffset;

                if (remaining < 128)
                {
                    throw new InvalidDataException("File data ends abruptly");
                }

                remaining -= 128;

                if (*(uint*)p != Helper.Magic)
                {
                    throw new InvalidDataException("'DDS ' magic number is missing or incorect");
                }

                p += 4;

                var pHeader = (Header*)p;
                p += Header.StructLength;

                if (pHeader->StructSize != Header.StructLength)
                {
                    throw new InvalidDataException("Header structure size must be 124 bytes");
                }

                if ((pHeader->Flags &
                    (HeaderFlags.Caps | HeaderFlags.Width | HeaderFlags.Height | HeaderFlags.PixelFormat)) !=
                    (HeaderFlags.Caps | HeaderFlags.Width | HeaderFlags.Height | HeaderFlags.PixelFormat))
                {
                    throw new InvalidDataException("One of the required header flags is missing");
                }

                if (!pHeader->Caps.HasFlag(Caps.Texture))
                {
                    throw new InvalidDataException("Required Caps.Texture flag is missing");
                }

                if (pHeader->Flags.HasFlag(HeaderFlags.MipMapCount) ^ pHeader->Caps.HasFlag(Caps.MipMap))
                {
                    throw new InvalidDataException("Flags 'HeaderFlags.MipMapCount' and 'Caps.MipMap' must be present or not at the same time");
                }

                this.Width = (int)pHeader->Width;
                this.Height = (int)pHeader->Height;
                this.Depth = pHeader->Flags.HasFlag(HeaderFlags.Depth) ? (int)pHeader->Depth : 1;

                if (pHeader->PixelFormat.StructSize != PixelFormat.StructLength)
                {
                    throw new InvalidDataException("PixelFormat structure size must be 32 bytes");
                }

                int chainSize;
                bool dataAlreadyCopied = false;

                if (pHeader->PixelFormat.FourCC != Helper.DX10)
                {
                    DxgiFormat dxgiFormat;
                    D3DFormat d3dFormat;
                    Helper.DetermineFormat(ref pHeader->PixelFormat, out dxgiFormat, out d3dFormat);
                    this.DxgiFormat = dxgiFormat;
                    this.D3DFormat = d3dFormat;

                    this.CalculateMipInfos(
                        pHeader->Flags.HasFlag(HeaderFlags.MipMapCount) ? (int)pHeader->MipMapCount : -1,
                        pHeader->Flags.HasFlag(HeaderFlags.Pitch) ? (int)pHeader->LinearSize : -1,
                        pHeader->Flags.HasFlag(HeaderFlags.LinearSize) ? (int)pHeader->LinearSize : -1,
                        out chainSize);

                    if (!(pHeader->Caps.HasFlag(Caps.Complex) && pHeader->Caps2.HasFlag(Caps2.CubeMap)))
                    {
                        this.Data = new byte[1][];
                    }
                    else
                    {
                        this.Data = new byte[6][];

                        for (int i = 0; i < 6; i++)
                        {
                            this.Data[i] = new byte[chainSize];

                            if (pHeader->Caps2.HasFlag(Helper.CubeMapFaces[i]))
                            {
                                if (remaining < chainSize)
                                {
                                    throw new InvalidDataException("File data ends abruptly");
                                }

                                remaining -= chainSize;
                                Marshal.Copy((IntPtr)p, this.Data[i], 0, chainSize);
                                p += chainSize;
                            }
                        }

                        dataAlreadyCopied = true;
                    }
                }
                else
                {
                    if (remaining < HeaderDx10.StructLength)
                    {
                        throw new InvalidDataException("File data ends abruptly");
                    }

                    remaining -= HeaderDx10.StructLength;

                    var pHeaderDx10 = (HeaderDx10*)p;
                    p += HeaderDx10.StructLength;

                    this.Dimension10 = pHeaderDx10->ResourceDimension;
                    this.MiscFlags10 = pHeaderDx10->MiscFlags;

                    this.DxgiFormat = pHeaderDx10->Format;
                    this.D3DFormat = Helper.D3DFormatFromDxgi(this.DxgiFormat);

                    this.CalculateMipInfos(
                        pHeader->Flags.HasFlag(HeaderFlags.MipMapCount) ? (int)pHeader->MipMapCount : -1,
                        pHeader->Flags.HasFlag(HeaderFlags.Pitch) ? (int)pHeader->LinearSize : -1,
                        pHeader->Flags.HasFlag(HeaderFlags.LinearSize) ? (int)pHeader->LinearSize : -1,
                        out chainSize);

                    if (this.MiscFlags10.HasFlag(ResourceMiscFlags.TextureCube))
                    {
                        this.Data = new byte[pHeaderDx10->ArraySize * 6][];
                    }
                    else
                    {
                        this.Data = new byte[pHeaderDx10->ArraySize][];
                    }
                }

                if (!dataAlreadyCopied)
                {
                    for (int i = 0; i < this.Data.Length; i++)
                    {
                        this.Data[i] = new byte[chainSize];

                        if (remaining < chainSize)
                        {
                            throw new InvalidDataException("File data ends abruptly");
                        }

                        remaining -= chainSize;

                        Marshal.Copy((IntPtr)p, this.Data[i], 0, chainSize);
                        p += chainSize;
                    }
                }
            }
        }

        public unsafe void SaveToStream(Stream stream)
        {
            if (this.DxgiFormat == DxgiFormat.UNKNOWN)
            {
                throw new NotSupportedException("Saving DDS with unknown DXGI format is not supported");
            }

            bool dx10 = false;

            var headersData = new byte[4 + Header.StructLength + HeaderDx10.StructLength];
            fixed (byte* pHeaders = headersData)
            {
                *(uint*)pHeaders = Helper.Magic;

                var pHeader = (Header*)(pHeaders + 4);
                pHeader->StructSize = Header.StructLength;
                pHeader->Flags = HeaderFlags.Caps | HeaderFlags.Height | HeaderFlags.Width | HeaderFlags.PixelFormat;
                pHeader->Height = (uint)this.Height;
                pHeader->Width = (uint)this.Width;

                if (this.Depth > 1 || this.Dimension10 == ResourceDimension.Texture3D)
                {
                    pHeader->Flags |= HeaderFlags.Depth;
                    pHeader->Depth = (uint)this.Depth;
                }

                if (this.MipInfos.Length > 1)
                {
                    pHeader->Flags |= HeaderFlags.MipMapCount;
                    pHeader->MipMapCount = (uint)this.MipInfos.Length;
                }

                pHeader->PixelFormat.StructSize = PixelFormat.StructLength;
                if (this.Dimension10 == ResourceDimension.Unknown && this.MiscFlags10 == ResourceMiscFlags.None)
                {
                    switch (this.DxgiFormat)
                    {
                        case DxgiFormat.R8G8B8A8_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.Rgb | PixelFormatFlags.AlphaPixels;
                            pHeader->PixelFormat.RgbBitCount = 32;
                            pHeader->PixelFormat.RBitMask = 0x000000ff;
                            pHeader->PixelFormat.GBitMask = 0x0000ff00;
                            pHeader->PixelFormat.BBitMask = 0x00ff0000;
                            pHeader->PixelFormat.ABitMask = 0xff000000;
                            break;
                        case DxgiFormat.R16G16_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.Rgb;
                            pHeader->PixelFormat.RgbBitCount = 32;
                            pHeader->PixelFormat.RBitMask = 0x0000ffff;
                            pHeader->PixelFormat.GBitMask = 0xffff0000;
                            break;
                        case DxgiFormat.B5G5R5A1_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.Rgb | PixelFormatFlags.AlphaPixels;
                            pHeader->PixelFormat.RgbBitCount = 16;
                            pHeader->PixelFormat.RBitMask = 0x7c00;
                            pHeader->PixelFormat.GBitMask = 0x03e0;
                            pHeader->PixelFormat.BBitMask = 0x001f;
                            pHeader->PixelFormat.ABitMask = 0x8000;
                            break;
                        case DxgiFormat.B5G6R5_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.Rgb;
                            pHeader->PixelFormat.RgbBitCount = 16;
                            pHeader->PixelFormat.RBitMask = 0xf800;
                            pHeader->PixelFormat.GBitMask = 0x07e0;
                            pHeader->PixelFormat.BBitMask = 0x001f;
                            break;
                        case DxgiFormat.A8_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.Alpha;
                            pHeader->PixelFormat.RgbBitCount = 8;
                            pHeader->PixelFormat.ABitMask = 0xff;
                            break;
                        case DxgiFormat.BC1_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.FourCC;
                            pHeader->PixelFormat.FourCC = Helper.DXT1;
                            break;
                        case DxgiFormat.BC2_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.FourCC;
                            pHeader->PixelFormat.FourCC = Helper.DXT3;
                            break;
                        case DxgiFormat.BC3_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.FourCC;
                            pHeader->PixelFormat.FourCC = Helper.DXT5;
                            break;
                        case DxgiFormat.R8G8_B8G8_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.FourCC;
                            pHeader->PixelFormat.FourCC = Helper.GRGB;
                            break;
                        case DxgiFormat.G8R8_G8B8_UNORM:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.FourCC;
                            pHeader->PixelFormat.FourCC = Helper.RGBG;
                            break;
                        default:
                            pHeader->PixelFormat.Flags = PixelFormatFlags.FourCC;
                            pHeader->PixelFormat.FourCC = Helper.DX10;
                            dx10 = true;
                            break;
                    }
                }
                else
                {
                    dx10 = true;
                }

                if (dx10)
                {
                    var pHeader10 = (HeaderDx10*)(pHeader + 4 + Header.StructLength);
                    pHeader10->Format = this.DxgiFormat;
                    pHeader10->ResourceDimension = this.Dimension10;
                    pHeader10->MiscFlags = this.MiscFlags10;
                    pHeader10->ArraySize = (uint)this.Data.Length;
                }
            }

            int headersLength = (int)(dx10 ? 4 + Header.StructLength + HeaderDx10.StructLength : 4 + Header.StructLength);
            stream.Write(headersData, 0, headersLength);
            foreach (byte[] t in this.Data)
            {
                stream.Write(t, 0, t.Length);
            }
        }

        public void SaveToFile(string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                this.SaveToStream(stream);
            }
        }

        private void CalculateMipInfos(int mipCount, int pitch, int linearSize, out int chainSize)
        {
            if (Helper.IsFormatCompressed(this.DxgiFormat, this.D3DFormat))
            {
                this.CalculateMipInfosCompressed(mipCount, linearSize, out chainSize);
            }
            else
            {
                this.CalculateMipInfosNoncompressed(mipCount, pitch, out chainSize);
            }
        }

        private unsafe void CalculateMipInfosNoncompressed(int mipCount, int pitch, out int chainSize)
        {
            if (mipCount == -1)
            {
                mipCount = 1;
            }

            int bytesPerPixel = Helper.FormatBits(this.DxgiFormat, this.D3DFormat) / 8;

            if (pitch == -1)
            {
                pitch = this.Width * bytesPerPixel;
                if ((pitch & 0x3) != 0)
                {
                    pitch += 4 - (pitch & 0x3);
                }
            }

            this.MipInfos = new ImageMipInfo[mipCount];
            fixed (ImageMipInfo* infos = this.MipInfos)
            {
                infos[0] = new ImageMipInfo
                {
                    Width = this.Width,
                    Height = this.Height,
                    Depth = this.Depth,
                    OffsetInBytes = 0,
                    SizeInBytes = pitch * this.Height * this.Depth,
                };

                for (int i = 1; i < mipCount; i++)
                {
                    infos[i] = new ImageMipInfo
                    {
                        Width = Math.Max(1, infos[i - 1].Width / 2),
                        Height = Math.Max(1, infos[i - 1].Height / 2),
                        Depth = Math.Max(1, infos[i - 1].Depth / 2),
                        OffsetInBytes = infos[i - 1].OffsetInBytes + infos[i - 1].SizeInBytes,
                    };

                    int mipPitch = infos[i].Width * bytesPerPixel;
                    if ((mipPitch & 0x3) != 0)
                    {
                        mipPitch += 4 - (mipPitch & 0x3);
                    }

                    infos[i].SizeInBytes = mipPitch * infos[i].Height * infos[i].Depth;
                }

                chainSize = infos[mipCount - 1].OffsetInBytes + infos[mipCount - 1].SizeInBytes;
            }
        }

        private unsafe void CalculateMipInfosCompressed(int mipCount, int linearSize, out int chainSize)
        {
            if (mipCount == -1)
            {
                mipCount = 1;
            }

            int multiplyer =
                        this.DxgiFormat == DxgiFormat.BC1_TYPELESS || this.DxgiFormat == DxgiFormat.BC1_UNORM ||
                        this.DxgiFormat == DxgiFormat.BC1_UNORM_SRGB || this.D3DFormat == D3DFormat.Dxt1
                            ? 8
                            : 16;

            ////if (linearSize == -1)
            ////{
            linearSize = Math.Max(1, this.Width / 4) * Math.Max(1, this.Height / 4) * this.Depth * multiplyer;
            ////}

            this.MipInfos = new ImageMipInfo[mipCount];
            fixed (ImageMipInfo* infos = this.MipInfos)
            {
                infos[0] = new ImageMipInfo
                {
                    Width = this.Width,
                    Height = this.Height,
                    Depth = this.Depth,
                    OffsetInBytes = 0,
                    SizeInBytes = linearSize,
                };

                for (int i = 1; i < mipCount; i++)
                {
                    infos[i] = new ImageMipInfo
                    {
                        Width = Math.Max(1, infos[i - 1].Width / 2),
                        Height = Math.Max(1, infos[i - 1].Height / 2),
                        Depth = Math.Max(1, infos[i - 1].Depth / 2),
                        OffsetInBytes = infos[i - 1].OffsetInBytes + infos[i - 1].SizeInBytes,
                    };

                    infos[i].SizeInBytes =
                        Math.Max(1, infos[i].Width / 4) * Math.Max(1, infos[i].Height / 4)
                        * infos[i].Depth * multiplyer;
                }

                chainSize = infos[mipCount - 1].OffsetInBytes + infos[mipCount - 1].SizeInBytes;
            }
        }
    }
}
