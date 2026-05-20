// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace Evergine.Assets.Extensions.HDR
{
    /// <summary>
    /// Loads HDR image and converts it to a set of float32 RGB triplets.
    /// </summary>
    internal unsafe struct HDRLoaderResult
    {
        public int Width;
        public int Height;

        // each pixel takes 3 float32, each component can be of any value...
        public byte[] Data;
    }
}
