// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents a shape in an OBJ file, which can include meshes, lines, and points.
    /// </summary>
    public class OBJShape
    {
        /// <summary>
        /// Gets or sets the name of the shape.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets the mesh data associated with the shape.
        /// </summary>
        public OBJMesh Mesh = new OBJMesh();

        /// <summary>
        /// Gets or sets the line data associated with the shape.
        /// </summary>
        public OBJLines Lines = new OBJLines();

        /// <summary>
        /// Gets or sets the point data associated with the shape.
        /// </summary>
        public OBJPoints Points = new OBJPoints();
    }
}
