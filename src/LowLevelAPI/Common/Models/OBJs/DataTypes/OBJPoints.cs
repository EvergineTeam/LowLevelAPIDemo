// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System.Collections.Generic;

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents a collection of indices used in an OBJ file.
    /// </summary>
    public class OBJPoints
    {
        /// <summary>
        /// Gets or sets the list of indices that define the points in the OBJ file.
        /// Each index corresponds to a vertex, normal, or texture coordinate.
        /// </summary>
        public List<OBJIndex> Indices = new List<OBJIndex>();
    }
}
