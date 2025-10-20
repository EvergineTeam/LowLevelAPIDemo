// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;
using OBJRuntime.DataTypes;
using Evergine.Mathematics;
using Evergine.Framework;
using Evergine.Common.IO;

namespace OBJRuntime.Readers
{
    /// <summary>
    /// Main OBJ loading logic in a static class.
    /// </summary>
    public static class OBJLoader
    {
        /// <summary>
        /// The index used to indicate an undefined or invalid index in the OBJ file.
        /// </summary>
        public const int UndefinedIndex = int.MaxValue;

        /// <summary>
        /// Loads an OBJ file from the provided stream and parses its contents into attributes, shapes, and materials.
        /// </summary>
        /// <param name="inStream">The input stream containing the OBJ file data.</param>
        /// <param name="attrib">The parsed attributes of the OBJ file, including vertices, normals, and texture coordinates.</param>
        /// <param name="shapes">The list of shapes parsed from the OBJ file, including meshes, lines, and points.</param>
        /// <param name="materials">The list of materials parsed from the OBJ file.</param>
        /// <param name="warning">A string to capture any warnings encountered during parsing.</param>
        /// <param name="error">A string to capture any errors encountered during parsing.</param>
        /// <param name="assetsDirectory">The directory containing additional assets, such as material files.</param>
        /// <param name="workingDirectory">The working directory for resolving relative paths in the OBJ file.</param>
        /// <param name="triangulate">A flag indicating whether to triangulate faces with more than three vertices.</param>
        /// <param name="defaultVcolsFallback">A flag indicating whether to use default vertex colors if none are found.</param>
        /// <returns>Returns <c>true</c> if the OBJ file was successfully loaded; otherwise, <c>false</c>.</returns>
        public static bool Load(
            StreamReader inStream,
            ref OBJAttrib attrib,
            List<OBJShape> shapes,
            List<OBJMaterial> materials,
            ref string warning,
            ref string error,
            AssetsDirectory assetsDirectory,
            string workingDirectory,
            bool triangulate,
            bool defaultVcolsFallback)
        {
            // Clear the output lists.
            attrib.Vertices.Clear();
            attrib.VertexWeights.Clear();
            attrib.Normals.Clear();
            attrib.Texcoords.Clear();
            attrib.TexcoordWs.Clear();
            attrib.Colors.Clear();
            attrib.SkinWeights.Clear();
            shapes.Clear();

            // Temporary lists for geometry data.
            var v = new List<Vector3>();
            var vertexWeights = new List<float>();
            var vn = new List<Vector3>();
            var vt = new List<Vector2>();
            var vc = new List<Vector3>();
            var vw = new List<OBJSkinWeight>();

            int materialId = -1;
            uint currentSmoothingId = 0;
            bool foundAllColors = true;

            var primGroup = new PrimGroup();
            string currentGroupName = string.Empty;

            var materialMap = new Dictionary<string, int>();
            var materialFilenames = new HashSet<string>();

            // Use StringBuilders for warnings and errors.
            var warnSB = new StringBuilder();
            var errSB = new StringBuilder();

            int lineNo = 0;
            while (!inStream.EndOfStream)
            {
                lineNo++;
                string line = inStream.ReadLine()?.TrimEnd();
                if (string.IsNullOrEmpty(line) || line[0] == '#')
                {
                    continue;
                }

                var tokens = Helpers.Tokenize(line);
                int tokenCount = tokens.Count;
                if (tokenCount < 1)
                {
                    continue;
                }

                var cmd = tokens[0];
                switch (cmd)
                {
                    case "v":
                        ParseVertex(tokens, tokenCount, v, vertexWeights, vc, ref foundAllColors);
                        break;
                    case "vn":
                        ParseNormal(tokens, tokenCount, vn);
                        break;
                    case "vt":
                        ParseTexcoord(tokens, tokenCount, vt);
                        break;
                    case "vw":
                        ParseVertexWeight(tokens, tokenCount, vw);
                        break;
                    case "f":
                        ParseFace(tokens, tokenCount, primGroup, currentSmoothingId, v.Count, vt.Count, vn.Count);
                        break;
                    case "l":
                        ParseLine(tokens, tokenCount, primGroup, v.Count, vt.Count, vn.Count);
                        break;
                    case "p":
                        ParsePoints(tokens, tokenCount, primGroup, v.Count, vt.Count, vn.Count);
                        break;
                    case "usemtl":
                        // Flush accumulated data.
                        ExportGroupsToShape(shapes, ref primGroup, currentGroupName, materialId, v, triangulate, warnSB);
                        primGroup.Clear();
                        {
                            string matName = tokenCount > 1 ? tokens[1] : string.Empty;
                            if (!materialMap.TryGetValue(matName, out materialId))
                            {
                                materialId = -1;
                                warnSB.Append($"material [{matName}] not found.\n");
                            }
                        }

                        break;
                    case "mtllib":
                        ParseMaterialLib(tokens, tokenCount, assetsDirectory, workingDirectory, materials, materialMap, materialFilenames, warnSB, errSB);
                        break;
                    case "g":
                        ExportGroupsToShape(shapes, ref primGroup, currentGroupName, materialId, v, triangulate, warnSB);

                        // Combine all tokens after "g" into a single group name.
                        currentGroupName = tokenCount > 1 ? string.Join(" ", tokens.GetRange(1, tokenCount - 1)) : string.Empty;
                        break;
                    case "o":
                        ExportGroupsToShape(shapes, ref primGroup, currentGroupName, materialId, v, triangulate, warnSB);
                        currentGroupName = tokenCount > 1 ? line.Substring(1).Trim() : string.Empty;
                        break;
                    case "s":
                        currentSmoothingId = (tokenCount > 1 && tokens[1] != "off" && uint.TryParse(tokens[1], out var smoothingId))
                            ? smoothingId : 0;
                        break;
                }
            }

            // Flush remaining data.
            ExportGroupsToShape(shapes, ref primGroup, currentGroupName, materialId, v, triangulate, warnSB);

            if (!foundAllColors && !defaultVcolsFallback)
            {
                vc.Clear();
            }

            // Populate final OBJAttrib.
            attrib.Vertices.AddRange(v);
            attrib.VertexWeights.AddRange(vertexWeights);
            attrib.Normals.AddRange(vn);
            attrib.Texcoords.AddRange(vt);
            attrib.TexcoordWs.AddRange(new float[vt.Count]); // Currently not used.
            attrib.Colors.AddRange(vc);
            attrib.SkinWeights.AddRange(vw);

            // Copy messages from StringBuilders.
            warning = warnSB.ToString();
            error = errSB.ToString();

            return true;
        }

        private static void ParseVertex(List<string> tokens, int tokenCount, List<Vector3> v, List<float> vertexWeights, List<Vector3> vc, ref bool foundAllColors)
        {
            // Require at least x, y, and z.
            if (tokenCount >= 4)
            {
                Helpers.TryParseFloat(tokens[1], out float x);
                Helpers.TryParseFloat(tokens[2], out float y);
                Helpers.TryParseFloat(tokens[3], out float z);

                // When there is one extra token, treat it as the weight ("w").
                if (tokenCount == 5)
                {
                    Helpers.TryParseFloat(tokens[4], out float w);
                    v.Add(new Vector3(x, y, z));
                    vertexWeights.Add(w);
                    foundAllColors = false;
                }

                // When there are three extra tokens, treat them as vertex colors.
                else if (tokenCount == 7)
                {
                    Helpers.TryParseFloat(tokens[4], out float r);
                    Helpers.TryParseFloat(tokens[5], out float g);
                    Helpers.TryParseFloat(tokens[6], out float b);
                    v.Add(new Vector3(x, y, z));
                    vertexWeights.Add(1.0f);
                    vc.Add(new Vector3(r, g, b));
                }
                else
                {
                    // Standard vertex with no extra values.
                    v.Add(new Vector3(x, y, z));
                    vertexWeights.Add(1.0f);
                    foundAllColors = false;
                }
            }
        }

        private static void ParseNormal(List<string> tokens, int tokenCount, List<Vector3> vn)
        {
            if (tokenCount >= 4)
            {
                Helpers.TryParseFloat(tokens[1], out float x);
                Helpers.TryParseFloat(tokens[2], out float y);
                Helpers.TryParseFloat(tokens[3], out float z);
                vn.Add(new Vector3(x, y, z));
            }
        }

        private static void ParseTexcoord(List<string> tokens, int tokenCount, List<Vector2> vt)
        {
            if (tokenCount >= 2)
            {
                Helpers.TryParseFloat(tokens[1], out float u);
                float vVal = 0;
                if (tokenCount > 2)
                {
                    Helpers.TryParseFloat(tokens[2], out vVal);
                }

                vt.Add(new Vector2(u, vVal));
            }
        }

        private static void ParseVertexWeight(List<string> tokens, int tokenCount, List<OBJSkinWeight> vw)
        {
            if (tokenCount > 1 && int.TryParse(tokens[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var vid))
            {
                var sw = new OBJSkinWeight { VertexId = vid };
                for (int i = 2; i + 1 < tokenCount; i += 2)
                {
                    if (int.TryParse(tokens[i], out var j) &&
                        Helpers.TryParseFloat(tokens[i + 1], out var w))
                    {
                        sw.WeightValues.Add(new OBJJointAndWeight { JointId = j, Weight = w });
                    }
                }

                vw.Add(sw);
            }
        }

        private static void ParseFace(List<string> tokens, int tokenCount, PrimGroup primGroup, uint currentSmoothingId, int vOffset, int vtOffset, int vnOffset)
        {
            if (tokenCount < 2)
            {
                return;
            }

            var face = new Face { SmoothingGroupId = currentSmoothingId };
            for (int i = 1; i < tokenCount; i++)
            {
                face.VertexIndices.Add(ParseRawTriple(tokens[i], vOffset, vtOffset, vnOffset));
            }

            primGroup.FaceGroup.Add(face);
        }

        private static void ParseLine(List<string> tokens, int tokenCount, PrimGroup primGroup, int vOffset, int vtOffset, int vnOffset)
        {
            if (tokenCount < 2)
            {
                return;
            }

            var lineGroup = new LineElm();
            for (int i = 1; i < tokenCount; i++)
            {
                lineGroup.VertexIndices.Add(ParseRawTriple(tokens[i], vOffset, vtOffset, vnOffset));
            }

            primGroup.LineGroup.Add(lineGroup);
        }

        private static void ParsePoints(List<string> tokens, int tokenCount, PrimGroup primGroup, int vOffset, int vtOffset, int vnOffset)
        {
            if (tokenCount < 2)
            {
                return;
            }

            var pointsGroup = new PointsElm();
            for (int i = 1; i < tokenCount; i++)
            {
                pointsGroup.VertexIndices.Add(ParseRawTriple(tokens[i], vOffset, vtOffset, vnOffset));
            }

            primGroup.PointsGroup.Add(pointsGroup);
        }

        private static void ParseMaterialLib(
            List<string> tokens,
            int tokenCount,
            AssetsDirectory assetsDirectory,
            string workingDirectory,
            List<OBJMaterial> materials,
            Dictionary<string, int> materialMap,
            HashSet<string> materialFilenames,
            StringBuilder warnSB,
            StringBuilder errSB)
        {
            if (tokenCount > 1)
            {
                for (int i = 1; i < tokenCount; i++)
                {
                    string filename = tokens[i];
                    if (materialFilenames.Contains(filename))
                    {
                        continue;
                    }

                    string filePath = Path.Combine(workingDirectory, filename);
                    if (assetsDirectory != null && assetsDirectory.Exists(filePath))
                    {
                        using (var streamMtl = assetsDirectory.Open(filePath))
                        {
                            var mtlReader = new OBJMaterialStreamReader(streamMtl);
                            if (!mtlReader.Read(filename, materials, materialMap, out var warnMtl, out var errMtl))
                            {
                                warnSB.Append(warnMtl);
                                errSB.Append(errMtl);
                            }
                            else
                            {
                                warnSB.Append(warnMtl);
                            }

                            materialFilenames.Add(filename);
                        }
                    }
                }
            }
        }

        private static void ExportGroupsToShape(
            List<OBJShape> shapes,
            ref PrimGroup primGroup,
            string groupName,
            int materialId,
            List<Vector3> v,
            bool triangulate,
            StringBuilder warnSB)
        {
            if (!primGroup.HasData())
            {
                return;
            }

            var shape = new OBJShape { Name = groupName };

            foreach (var face in primGroup.FaceGroup)
            {
                int nVerts = face.VertexIndices.Count;
                if (nVerts < 3)
                {
                    warnSB.Append("Degenerate face found.\n");
                    continue;
                }

                if (triangulate && nVerts > 3)
                {
                    for (int i = 1; i < nVerts - 1; i++)
                    {
                        // Flip the winding order
                        shape.Mesh.Indices.Add(face.VertexIndices[0]);
                        shape.Mesh.Indices.Add(face.VertexIndices[i + 1]);
                        shape.Mesh.Indices.Add(face.VertexIndices[i]);

                        shape.Mesh.NumFaceVertices.Add(3);
                        shape.Mesh.MaterialIds.Add(materialId);
                        shape.Mesh.SmoothingGroupIds.Add(face.SmoothingGroupId);
                    }
                }
                else
                {
                    // Flip the winding order
                    shape.Mesh.Indices.Add(face.VertexIndices[0]);
                    shape.Mesh.Indices.Add(face.VertexIndices[2]);
                    shape.Mesh.Indices.Add(face.VertexIndices[1]);

                    shape.Mesh.NumFaceVertices.Add((uint)nVerts);
                    shape.Mesh.MaterialIds.Add(materialId);
                    shape.Mesh.SmoothingGroupIds.Add(face.SmoothingGroupId);
                }
            }

            foreach (var line in primGroup.LineGroup)
            {
                shape.Lines.Indices.AddRange(line.VertexIndices);
                shape.Lines.NumLineVertices.Add(line.VertexIndices.Count);
            }

            foreach (var pts in primGroup.PointsGroup)
            {
                shape.Points.Indices.AddRange(pts.VertexIndices);
            }

            shapes.Add(shape);
            primGroup.Clear();
        }

        /// <summary>
        /// Parses a raw triple from an OBJ file, which can represent vertex, texture coordinate, and normal indices.
        /// // Raw triple parse: i, i/j, i/j/k, i//k.
        /// </summary>
        /// <param name="token">The raw string token from the OBJ file, typically in the format "v", "v/vt", "v/vt/vn", or "v//vn".</param>
        /// <param name="vOffset">The offset for vertex indices, used to adjust the index to a 0-based system.</param>
        /// <param name="vtoffset">The offset for texture coordinate indices, used to adjust the index to a 0-based system.</param>
        /// <param name="vnoffset">The offset for normal indices, used to adjust the index to a 0-based system.</param>
        /// <returns>
        /// An <see cref="OBJIndex"/> structure containing the parsed vertex, texture coordinate, and normal indices.
        /// If an index is not specified in the token, it will be set to <see cref="UndefinedIndex"/>.
        /// </returns>
        public static OBJIndex ParseRawTriple(string token, int vOffset, int vtoffset, int vnoffset)
        {
            OBJIndex idx = new OBJIndex() { VertexIndex = 0, TexcoordIndex = 0, NormalIndex = 0 };

            // We just do naive splitting by '/'
            // If there's no '/', it's just the v index
            string[] parts = token.Split('/');
            int vIdx = 0, vtIdx = 0, vnIdx = 0;

            if (!string.IsNullOrEmpty(parts[0]))
            {
                int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out vIdx);
            }

            if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
            {
                int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out vtIdx);
            }

            if (parts.Length > 2 && !string.IsNullOrEmpty(parts[2]))
            {
                int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out vnIdx);
            }

            vIdx = TranslateIndex(vIdx, vOffset);
            vtIdx = TranslateIndex(vtIdx, vtoffset);
            vnIdx = TranslateIndex(vnIdx, vnoffset);

            idx.VertexIndex = vIdx;
            idx.TexcoordIndex = vtIdx;
            idx.NormalIndex = vnIdx;

            return idx;
        }

        // Helper function to translate OBJ indices to 0-based indices.
        // Positive indices (starting at 1) are converted by subtracting 1.
        // Negative indices refer to elements relative to the end, e.g. -1 is the last element.
        private static int TranslateIndex(int index, int count)
        {
            if (count > 0)
            {
                if (index > 0)
                {
                    return index - 1;
                }
                else if (index < 0)
                {
                    return count + index;
                }
            }

            return UndefinedIndex;
        }

        // Private helper classes.
        private class Face
        {
            public uint SmoothingGroupId = 0;
            public List<OBJIndex> VertexIndices = new List<OBJIndex>();
        }

        private class LineElm
        {
            public List<OBJIndex> VertexIndices = new List<OBJIndex>();
        }

        private class PointsElm
        {
            public List<OBJIndex> VertexIndices = new List<OBJIndex>();
        }

        private class PrimGroup
        {
            public List<Face> FaceGroup = new List<Face>();
            public List<LineElm> LineGroup = new List<LineElm>();
            public List<PointsElm> PointsGroup = new List<PointsElm>();

            public void Clear()
            {
                this.FaceGroup.Clear();
                this.LineGroup.Clear();
                this.PointsGroup.Clear();
            }

            public bool HasData()
            {
                return this.FaceGroup.Count > 0 || this.LineGroup.Count > 0 || this.PointsGroup.Count > 0;
            }
        }
    }
}
