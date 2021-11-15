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
    internal enum HeaderFlags : uint
    {
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        Caps = 0x1,

        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        Height = 0x2,

        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        Width = 0x4,

        /// <summary>
        /// Required when pitch is provided for an uncompressed texture.
        /// </summary>
        Pitch = 0x8,

        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        PixelFormat = 0x1000,

        /// <summary>
        /// Required in a mipmapped texture.
        /// </summary>
        MipMapCount = 0x20000,

        /// <summary>
        /// Required when pitch is provided for a compressed texture.
        /// </summary>
        LinearSize = 0x80000,

        /// <summary>
        /// Required in a depth texture.
        /// </summary>
        Depth = 0x800000,
    }
}
