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
    internal enum Caps : uint
    {
        /// <summary>
        /// Optional; must be used on any file that contains more than one surface
        /// (a mipmap, a cubic environment map, or volume texture).
        /// </summary>
        Complex = 0x8,

        /// <summary>
        /// Optional; should be used for a mipmap.
        /// </summary>
        MipMap = 0x400000,

        /// <summary>
        /// Required
        /// </summary>
        Texture = 0x1000,
    }
}
