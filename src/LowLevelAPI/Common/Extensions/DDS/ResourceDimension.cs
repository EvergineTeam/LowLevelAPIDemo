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

namespace Evergine.Assets.Extensions.DDS
{
    internal enum ResourceDimension : uint
    {
        /// <summary>
        /// Resource is of unknown type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Resource is a buffer.
        /// </summary>
        Buffer = 1,

        /// <summary>
        /// Resource is a 1D texture.
        /// </summary>
        Texture1D = 2,

        /// <summary>
        /// Resource is a 2D texture.
        /// </summary>
        Texture2D = 3,

        /// <summary>
        /// Resource is a 3D texture.
        /// </summary>
        Texture3D = 4,
    }
}
