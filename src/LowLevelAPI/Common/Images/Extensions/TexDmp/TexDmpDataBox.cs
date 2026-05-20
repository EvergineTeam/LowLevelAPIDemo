// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace Evergine.Assets.Extensions.TexDmp
{
    /// <summary>
    /// Text Dump data box.
    /// </summary>
    public struct TexDmpDataBox
    {
        /// <summary>
        /// The data array.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Gets the number of bytes per row.
        /// </summary>
        public uint RowPitch;

        /// <summary>
        /// Gets the number of bytes per slice (for a 3D texture, where a slice is a 2D image).
        /// </summary>
        public uint SlicePitch;
    }
}
