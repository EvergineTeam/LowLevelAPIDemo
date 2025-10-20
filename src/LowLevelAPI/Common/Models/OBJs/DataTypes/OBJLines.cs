// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System.Collections.Generic;

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents a collection of lines in an OBJ file.
    /// </summary>
    public class OBJLines
    {
        /// <summary>
        /// Gets or sets the list of indices for the vertices, normals, and texture coordinates of the lines.
        /// </summary>
        public List<OBJIndex> Indices = new List<OBJIndex>();

        /// <summary>
        /// Gets or sets the list of vertex counts for each line.
        /// </summary>
        public List<int> NumLineVertices = new List<int>();
    }
}
