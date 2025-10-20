// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using Evergine.Mathematics;
using System.Collections.Generic;

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents the attributes of an OBJ file, including vertices, normals, texture coordinates, and other optional data.
    /// </summary>
    public class OBJAttrib
    {
        /// <summary>
        /// Gets or sets the list of vertex positions ('v' xyz).
        /// </summary>
        public List<Vector3> Vertices = new List<Vector3>();

        /// <summary>
        /// Gets or sets the list of vertex weights ('v' w).
        /// </summary>
        public List<float> VertexWeights = new List<float>();

        /// <summary>
        /// Gets or sets the list of vertex normals ('vn').
        /// </summary>
        public List<Vector3> Normals = new List<Vector3>();

        /// <summary>
        /// Gets or sets the list of texture coordinates ('vt' (u, v)).
        /// </summary>
        public List<Vector2> Texcoords = new List<Vector2>();

        /// <summary>
        /// Gets or sets the list of texture coordinate weights ('vt' w, if present; otherwise unused).
        /// </summary>
        public List<float> TexcoordWs = new List<float>();

        /// <summary>
        /// Gets or sets the list of vertex colors, if present.
        /// </summary>
        public List<Vector3> Colors = new List<Vector3>();

        /// <summary>
        /// Gets or sets the list of skin weights for vertices, used for skeletal animation.
        /// </summary>
        public List<OBJSkinWeight> SkinWeights = new List<OBJSkinWeight>();
    }
}
