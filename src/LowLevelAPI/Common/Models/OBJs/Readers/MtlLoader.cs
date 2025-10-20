// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Evergine.Mathematics;
using OBJRuntime.DataTypes;

namespace OBJRuntime.Readers
{
    /// <summary>
    /// Provides functionality to load material definitions from .mtl files into a list of <see cref="OBJMaterial"/> objects.
    /// </summary>
    public static class MtlLoader
    {
        /// <summary>
        /// Loads material definitions from a .mtl file stream into a list of <see cref="OBJMaterial"/> objects.
        /// </summary>
        /// <param name="sr">The <see cref="StreamReader"/> to read the .mtl file content.</param>
        /// <param name="materials">The list to store the loaded <see cref="OBJMaterial"/> objects.</param>
        /// <param name="materialMap">A dictionary mapping material names to their indices in the materials list.</param>
        /// <param name="warning">A reference to a string that will contain any warnings encountered during parsing.</param>
        /// <param name="error">A reference to a string that will contain any errors encountered during parsing.</param>
        public static void Load(
            StreamReader sr,
            List<OBJMaterial> materials,
            Dictionary<string, int> materialMap,
            ref string warning,
            ref string error)
        {
            // Use StringBuilders to accumulate warning/error messages.
            var warnSB = new StringBuilder();
            var errSB = new StringBuilder();

            OBJMaterial material = new OBJMaterial();
            bool firstMaterial = true;
            bool hasD = false;
            bool hasTr = false;
            bool hasKd = false;

            int lineNo = 0;
            string line;

            // Process each line.
            while (!sr.EndOfStream)
            {
                lineNo++;
                line = sr.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line) || line[0] == '#')
                {
                    continue;
                }

                // Tokenize the line. (If Helpers.Tokenize internally uses Split with
                // StringSplitOptions.RemoveEmptyEntries, that is already efficient.)
                var tokens = Helpers.Tokenize(line);
                int tokenCount = tokens.Count;
                if (tokenCount == 0)
                {
                    continue;
                }

                string key = tokens[0];
                switch (key)
                {
                    case "newmtl":
                        // When starting a new material, flush the previous one.
                        if (!firstMaterial || !string.IsNullOrEmpty(material.Name))
                        {
                            if (!materialMap.ContainsKey(material.Name))
                            {
                                materialMap.Add(material.Name, materials.Count);
                            }

                            materials.Add(material);
                        }

                        material = new OBJMaterial();
                        hasD = hasTr = hasKd = false;
                        firstMaterial = false;

                        // newmtl names might contain spaces so use the entire substring.
                        material.Name = line.Substring(6).Trim();
                        break;
                    case "Ka":
                    case "ka":
                        if (tokenCount >= 4)
                        {
                            Helpers.ParseVector3(tokens, 1, ref material.Ambient);
                        }

                        break;
                    case "Kd":
                    case "kd":
                        if (tokenCount >= 4)
                        {
                            Helpers.ParseVector3(tokens, 1, ref material.Diffuse);
                            hasKd = true;
                        }

                        break;
                    case "Ks":
                    case "ks":
                        if (tokenCount >= 4)
                        {
                            Helpers.ParseVector3(tokens, 1, ref material.Specular);
                        }

                        break;
                    case "Ke":
                        if (tokenCount >= 4)
                        {
                            Helpers.ParseVector3(tokens, 1, ref material.Emission);
                        }

                        break;
                    case "Tf":
                    case "Kt":
                        if (tokenCount >= 4)
                        {
                            Helpers.ParseVector3(tokens, 1, ref material.Transmittance);
                        }

                        break;
                    case "Ns":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float ns))
                        {
                            material.Shininess = ns;
                        }

                        break;
                    case "Ni":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float ni))
                        {
                            material.Ior = ni;
                        }

                        break;
                    case "illum":
                        if (tokenCount >= 2 && int.TryParse(tokens[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int illum))
                        {
                            material.Illum = illum;
                        }

                        break;
                    case "d":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float d))
                        {
                            material.Dissolve = d;
                            if (hasTr)
                            {
                                warnSB.Append($"Both 'd' and 'Tr' found for material '{material.Name}'. Using 'd' (line {lineNo}).\n");
                            }

                            hasD = true;
                        }

                        break;
                    case "Tr":
                        if (tokenCount >= 2 && !hasD && Helpers.TryParseFloat(tokens[1], out float tr))
                        {
                            material.Dissolve = 1.0f - tr;
                            hasTr = true;
                        }
                        else if (hasD)
                        {
                            warnSB.Append($"Both 'd' and 'Tr' found for material '{material.Name}'. Using 'd' (line {lineNo}).\n");
                        }

                        break;
                    case "map_Ka":
                        ParseTextureAndOption(line.Substring(6).Trim(), ref material.AmbientTexname, material.AmbientTexopt);
                        break;
                    case "map_Kd":
                        ParseTextureAndOption(line.Substring(6).Trim(), ref material.DiffuseTexname, material.DiffuseTexopt);
                        if (!hasKd)
                        {
                            material.Diffuse = new Vector3(0.6f, 0.6f, 0.6f);
                        }

                        break;
                    case "map_Ks":
                        ParseTextureAndOption(line.Substring(6).Trim(), ref material.SpecularTexname, material.SpecularTexopt);
                        break;
                    case "map_Ns":
                        ParseTextureAndOption(line.Substring(6).Trim(), ref material.SpecularHighlightTexname, material.SpecularHighlightTexopt);
                        break;
                    case "map_d":
                        ParseTextureAndOption(line.Substring(5).Trim(), ref material.AlphaTexname, material.AlphaTexopt);
                        break;
                    case "map_bump":
                    case "map_Bump":
                    case "bump":
                        ParseTextureAndOption(line.Substring(key.Length).Trim(), ref material.BumpTexname, material.BumpTexopt);
                        break;
                    case "map_disp":
                    case "map_Disp":
                    case "disp":
                        ParseTextureAndOption(line.Substring(4).Trim(), ref material.DisplacementTexname, material.DisplacementTexopt);
                        break;
                    case "refl":
                        ParseTextureAndOption(line.Substring(4).Trim(), ref material.ReflectionTexname, material.ReflectionTexopt);
                        break;
                    case "map_Pr":
                        ParseTextureAndOption(line.Substring(6).Trim(), ref material.RoughnessTexname, material.Roughness_texopt);
                        break;
                    case "map_Pm":
                        ParseTextureAndOption(line.Substring(6).Trim(), ref material.MetallicTexname, material.Metallic_texopt);
                        break;
                    case "map_Ps":
                        ParseTextureAndOption(line.Substring(6).Trim(), ref material.SheenTexname, material.Sheen_texopt);
                        break;
                    case "map_Ke":
                        ParseTextureAndOption(line.Substring(6).Trim(), ref material.EmissiveTexname, material.Emissive_texopt);
                        break;
                    case "norm":
                        ParseTextureAndOption(line.Substring(4).Trim(), ref material.NormalTexname, material.Normal_texopt);
                        break;
                    case "Pr":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float pr))
                        {
                            material.Roughness = pr;
                        }

                        break;
                    case "Pm":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float pm))
                        {
                            material.Metallic = pm;
                        }

                        break;
                    case "Ps":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float ps))
                        {
                            material.Sheen = ps;
                        }

                        break;
                    case "Pc":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float pc))
                        {
                            material.ClearcoatThickness = pc;
                        }

                        break;
                    case "Pcr":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float pcr))
                        {
                            material.ClearcoatRoughness = pcr;
                        }

                        break;
                    case "aniso":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float aniso))
                        {
                            material.Anisotropy = aniso;
                        }

                        break;
                    case "anisor":
                        if (tokenCount >= 2 && Helpers.TryParseFloat(tokens[1], out float anisor))
                        {
                            material.AnisotropyRotation = anisor;
                        }

                        break;
                    default:
                        if (tokenCount >= 2)
                        {
                            // For unknown parameters, capture the rest of the line (minus the key).
                            string paramV = line.Substring(key.Length).Trim();
                            material.UnknownParameter[key] = paramV;
                        }

                        break;
                }
            }

            // Flush the last material.
            if (!materialMap.ContainsKey(material.Name))
            {
                materialMap.Add(material.Name, materials.Count);
            }

            materials.Add(material);

            // Copy the accumulated messages.
            warning = warnSB.ToString();
            error = errSB.ToString();
        }

        /// <summary>
        /// Parses a texture definition and its options from a line in the .mtl file.
        /// </summary>
        /// <param name="line">The line containing the texture definition and options.</param>
        /// <param name="texName">A reference to the texture name to be updated.</param>
        /// <param name="texOpt">The <see cref="OBJTextureOption"/> object to store the parsed options.</param>
        private static void ParseTextureAndOption(string line, ref string texName, OBJTextureOption texOpt)
        {
            // Tokenize using the Helpers.Tokenize routine.
            var tokens = Helpers.Tokenize(line);
            int tokenCount = tokens.Count;
            bool foundTexName = false;

            for (int idx = 0; idx < tokenCount; idx++)
            {
                string t = tokens[idx];
                switch (t)
                {
                    case "-blendu":
                        if (++idx < tokenCount)
                        {
                            texOpt.Blendu = tokens[idx] == "on";
                        }

                        break;
                    case "-blendv":
                        if (++idx < tokenCount)
                        {
                            texOpt.Blendv = tokens[idx] == "on";
                        }

                        break;
                    case "-clamp":
                        if (++idx < tokenCount)
                        {
                            texOpt.Clamp = tokens[idx] == "on";
                        }

                        break;
                    case "-boost":
                        if (++idx < tokenCount && Helpers.TryParseFloat(tokens[idx], out float boost))
                        {
                            texOpt.Sharpness = boost;
                        }

                        break;
                    case "-bm":
                        if (++idx < tokenCount && Helpers.TryParseFloat(tokens[idx], out float bm))
                        {
                            texOpt.BumpMultiplier = bm;
                        }

                        break;
                    case "-o":
                        ParseTextureOptionCoords(tokens, ref idx, texOpt.OriginOffset);
                        break;
                    case "-s":
                        ParseTextureOptionCoords(tokens, ref idx, texOpt.Scale);
                        break;
                    case "-t":
                        ParseTextureOptionCoords(tokens, ref idx, texOpt.Turbulence);
                        break;
                    case "-texres":
                        if (++idx < tokenCount && int.TryParse(tokens[idx], out int texres))
                        {
                            texOpt.TextureResolution = texres;
                        }

                        break;
                    case "-imfchan":
                        if (++idx < tokenCount && tokens[idx].Length >= 1)
                        {
                            texOpt.Imfchan = tokens[idx][0];
                        }

                        break;
                    case "-mm":
                        if (++idx < tokenCount && Helpers.TryParseFloat(tokens[idx], out float mmBase))
                        {
                            texOpt.Brightness = mmBase;
                            if (++idx < tokenCount && Helpers.TryParseFloat(tokens[idx], out float mmGain))
                            {
                                texOpt.Contrast = mmGain;
                            }
                        }

                        break;
                    case "-colorspace":
                        if (++idx < tokenCount)
                        {
                            texOpt.Colorspace = tokens[idx];
                        }

                        break;
                    case "-type":
                        if (++idx < tokenCount)
                        {
                            texOpt.Type = tokens[idx] switch
                            {
                                "sphere" => OBJTextureType.Sphere,
                                "cube_top" => OBJTextureType.CubeTop,
                                "cube_bottom" => OBJTextureType.CubeBottom,
                                "cube_front" => OBJTextureType.CubeFront,
                                "cube_back" => OBJTextureType.CubeBack,
                                "cube_left" => OBJTextureType.CubeLeft,
                                "cube_right" => OBJTextureType.CubeRight,
                                _ => OBJTextureType.None,
                            };
                        }

                        break;
                    default:
                        if (!foundTexName)
                        {
                            texName = t;
                            foundTexName = true;
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Parses texture option coordinates (e.g., origin offset, scale, turbulence) from tokens in the .mtl file.
        /// </summary>
        /// <param name="tokens">The list of tokens from the line being parsed.</param>
        /// <param name="idx">The current index in the tokens list, which will be updated as coordinates are parsed.</param>
        /// <param name="coords">The array to store the parsed coordinates.</param>
        private static void ParseTextureOptionCoords(List<string> tokens, ref int idx, float[] coords)
        {
            int maxCoords = 3;
            int coordCount = 0;

            // Advance while there are valid float tokens and room for up to three coordinates.
            while (coordCount < maxCoords && idx + 1 < tokens.Count && Helpers.TryParseFloat(tokens[++idx], out float val))
            {
                coords[coordCount++] = val;
            }
        }
    }
}
