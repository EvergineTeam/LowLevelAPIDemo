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

namespace Evergine.Assets.Extensions.DDS
{
    [Flags]
    internal enum PixelFormatFlags : uint
    {
        /// <summary>
        /// Texture contains alpha data;
        /// RGBAlphaBitMask contains valid data.
        /// </summary>
        AlphaPixels = 0x1,

        /// <summary>
        /// Used in some older DDS files for alpha channel only uncompressed data
        /// (RGBBitCount contains the alpha channel bitcount; ABitMask contains valid data)
        /// </summary>
        Alpha = 0x2,

        /// <summary>
        /// Texture contains compressed RGB data; FourCC contains valid data.
        /// </summary>
        FourCC = 0x4,

        /// <summary>
        /// Texture contains uncompressed RGB data;
        /// RGBBitCount and the RGB masks (RBitMask, RBitMask, RBitMask) contain valid data.
        /// </summary>
        Rgb = 0x40,

        /// <summary>
        /// Used in some older DDS files for YUV uncompressed data
        /// (RGBBitCount contains the YUV bit count;
        /// RBitMask contains the Y mask, GBitMask contains the U mask, BBitMask contains the V mask)
        /// </summary>
        Yuv = 0x200,

        /// <summary>
        /// Used in some older DDS files for single channel color uncompressed data
        /// (RGBBitCount contains the luminance channel bit count; RBitMask contains the channel mask).
        /// Can be combined with AlphaPixels for a two channel DDS file.
        /// </summary>
        Luminance = 0x20000,
    }
}
