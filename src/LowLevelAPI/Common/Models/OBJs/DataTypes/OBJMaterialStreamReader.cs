// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using OBJRuntime.Readers;
using System.Collections.Generic;
using System.IO;

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Provides functionality to read material definitions (MTL) from a stream, such as a MemoryStream.
    /// </summary>
    public class OBJMaterialStreamReader
    {
        /// <summary>
        /// The input stream containing the material data.
        /// </summary>
        private readonly Stream inStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="OBJMaterialStreamReader"/> class.
        /// </summary>
        /// <param name="inStream">The input stream containing the material data.</param>
        public OBJMaterialStreamReader(Stream inStream)
        {
            this.inStream = inStream;
        }

        /// <summary>
        /// Reads material data from the stream and populates the provided materials and material map.
        /// </summary>
        /// <param name="matId">The material identifier to read.</param>
        /// <param name="materials">The list to populate with <see cref="OBJMaterial"/> objects.</param>
        /// <param name="matMap">The dictionary to populate with material names and their indices.</param>
        /// <param name="warning">Outputs any warnings encountered during the read operation.</param>
        /// <param name="error">Outputs any errors encountered during the read operation.</param>
        /// <returns>
        /// <c>true</c> if the material data was successfully read; otherwise, <c>false</c>.
        /// </returns>
        public bool Read(
            string matId,
            List<OBJMaterial> materials,
            Dictionary<string, int> matMap,
            out string warning,
            out string error)
        {
            warning = string.Empty;
            error = string.Empty;

            if (this.inStream == null)
            {
                warning += "Material stream in error state.\n";
                return false;
            }

            using (var sr = new StreamReader(this.inStream, leaveOpen: true))
            {
                MtlLoader.Load(sr, materials, matMap, ref warning, ref error);
            }

            return true;
        }
    }
}
