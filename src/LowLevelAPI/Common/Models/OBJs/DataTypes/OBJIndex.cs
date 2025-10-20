// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents an index structure used in OBJ files to support different indices for vertices, normals, and texture coordinates.
    /// </summary>
    public struct OBJIndex
    {
        /// <summary>
        /// Gets or sets the index of the vertex. A value of -1 indicates that the vertex is not used.
        /// </summary>
        public int VertexIndex;

        /// <summary>
        /// Gets or sets the index of the normal. A value of -1 indicates that the normal is not used.
        /// </summary>
        public int NormalIndex;

        /// <summary>
        /// Gets or sets the index of the texture coordinate. A value of -1 indicates that the texture coordinate is not used.
        /// </summary>
        public int TexcoordIndex;
    }
}
