// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace Evergine.Assets.Extensions.KTX
{
    /// <summary>
    /// For each mipmap level in numberOfMipmapLevels.
    /// </summary>
    public class KTXMipmapLevel
    {
        /// <summary>
        /// Gets the mip map width.
        /// </summary>
        public uint Width { get; }

        /// <summary>
        /// Gets the mipmap height.
        /// </summary>
        public uint Height { get; }

        /// <summary>
        /// Gets the mipmap depth.
        /// </summary>
        public uint Depth { get; }

        /// <summary>
        /// Gets the mipmap total size.
        /// </summary>
        public uint TotalSize { get; }

        /// <summary>
        /// Gets the array element size.
        /// </summary>
        public uint ArrayElementSize { get; }

        /// <summary>
        /// Getsthe array elements.
        /// </summary>
        public KTXArrayElement[] ArrayElements { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KTXMipmapLevel"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="totalSize">The total size.</param>
        /// <param name="arraySliceSize">The array slize size.</param>
        /// <param name="slices">The slices.</param>
        public KTXMipmapLevel(uint width, uint height, uint depth, uint totalSize, uint arraySliceSize, KTXArrayElement[] slices)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.TotalSize = totalSize;
            this.ArrayElementSize = arraySliceSize;
            this.ArrayElements = slices;
        }
    }
}
