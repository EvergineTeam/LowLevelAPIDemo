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

using System.Runtime.InteropServices;

namespace Evergine.Assets.Extensions.DDS
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 32)]
    internal struct PixelFormat
    {
        public const int StructLength = 32;

        /// <summary>
        /// Structure size; set to 32 (bytes).
        /// </summary>
        public uint StructSize;

        /// <summary>
        /// Values which indicate what type of data is in the surface.
        /// </summary>
        public PixelFormatFlags Flags;

        /// <summary>
        /// Four-character codes for specifying compressed or custom formats.
        /// Possible values include: DXT1, DXT2, DXT3, DXT4, or DXT5.
        /// A FourCC of DX10 indicates the prescense of the DDS_HEADER_DXT10 extended header,
        /// and the DxgiFormat member of that structure indicates the true format.
        /// When using a four-character code, Flags must include FourCC.
        /// </summary>
        public uint FourCC;

        /// <summary>
        /// Number of bits in an RGB (possibly including alpha) format.
        /// Valid when Flags includes Rgb, Luminance, or Yuv.
        /// </summary>
        public uint RgbBitCount;

        /// <summary>
        /// Red (or lumiannce or Y) mask for reading color data.
        /// For instance, given the A8R8G8B8 format, the red mask would be 0x00ff0000.
        /// </summary>
        public uint RBitMask;

        /// <summary>
        /// Green (or U) mask for reading color data.
        /// For instance, given the A8R8G8B8 format, the green mask would be 0x0000ff00.
        /// </summary>
        public uint GBitMask;

        /// <summary>
        /// Blue (or V) mask for reading color data.
        /// For instance, given the A8R8G8B8 format, the blue mask would be 0x000000ff.
        /// </summary>
        public uint BBitMask;

        /// <summary>
        /// Alpha mask for reading alpha data.
        /// Flags must include AlphaPixels or Alpha.
        /// For instance, given the A8R8G8B8 format, the alpha mask would be 0xff000000.
        /// </summary>
        public uint ABitMask;
    }
}
