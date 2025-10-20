// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System.Collections.Generic;

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents a mesh structure in an OBJ file, containing indices, face vertex counts, material IDs, and smoothing group IDs.
    /// </summary>
    public class OBJMesh
    {
        /// <summary>
        /// Gets or sets the list of indices used in the mesh. Each index represents a combination of vertex, normal, and texture coordinate indices.
        /// </summary>
        public List<OBJIndex> Indices = new List<OBJIndex>();

        /// <summary>
        /// Gets or sets the number of vertices per face in the mesh.
        /// </summary>
        public List<uint> NumFaceVertices = new List<uint>();

        /// <summary>
        /// Gets or sets the material IDs for each face in the mesh.
        /// </summary>
        public List<int> MaterialIds = new List<int>();

        /// <summary>
        /// Gets or sets the smoothing group IDs for each face in the mesh.
        /// </summary>
        public List<uint> SmoothingGroupIds = new List<uint>();
    }
}
