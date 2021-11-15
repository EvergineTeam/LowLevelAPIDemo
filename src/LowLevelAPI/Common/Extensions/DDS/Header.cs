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
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 124)]
    internal struct Header
    {
        public const uint StructLength = 124;

        /// <summary>
        /// Size of structure. This member must be set to 124.
        /// </summary>
        public uint StructSize;

        /// <summary>
        /// Flags to indicate which members contain valid data.
        /// </summary>
        public HeaderFlags Flags;

        /// <summary>
        /// Surface height (in pixels).
        /// </summary>
        public uint Height;

        /// <summary>
        /// Surface width (in pixels).
        /// </summary>
        public uint Width;

        /// <summary>
        /// The number of bytes per scan line in an uncompressed texture;
        /// the total number of bytes in the top level texture for a compressed texture.
        /// The pitch must be DWORD aligned.
        /// </summary>
        public uint LinearSize;

        /// <summary>
        /// Depth of a volume texture (in pixels), otherwise unused.
        /// </summary>
        public uint Depth;

        /// <summary>
        /// Number of mipmap levels, otherwise unused.
        /// </summary>
        public uint MipMapCount;

        private int Reserved1_0;
        private int Reserved1_1;
        private int Reserved1_2;
        private int Reserved1_3;
        private int Reserved1_4;
        private int Reserved1_5;
        private int Reserved1_6;
        private int Reserved1_7;
        private int Reserved1_8;
        private int Reserved1_9;
        private int Reserved1_10;

        /// <summary>
        /// The pixel format.
        /// </summary>
        public PixelFormat PixelFormat;

        /// <summary>
        /// Specifies the complexity of the surfaces stored.
        /// </summary>
        public Caps Caps;

        /// <summary>
        /// Additional detail about the surfaces stored.
        /// </summary>
        public Caps2 Caps2;

        private uint Caps3;
        private uint Caps4;
        private uint Reserved2;
    }
}
