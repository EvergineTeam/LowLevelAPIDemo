// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System.Collections.Generic;

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents the skin weight data for a vertex in an OBJ model.
    /// This class associates a vertex with a list of joint influences and their respective weights,
    /// which are used in skeletal animation.
    /// </summary>
    public class OBJSkinWeight
    {
        /// <summary>
        /// Gets or sets the index of the vertex in the "attrib_t.vertices" array.
        /// </summary>
        public int VertexId;

        /// <summary>
        /// Gets or sets the list of joint and weight pairs that influence this vertex.
        /// Each pair specifies a joint and its corresponding weight.
        /// </summary>
        public List<OBJJointAndWeight> WeightValues = new List<OBJJointAndWeight>();
    }
}
