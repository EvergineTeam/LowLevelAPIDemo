// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using Evergine.Mathematics;
using System.Collections.Generic;

namespace OBJRuntime.DataTypes
{
    /// <summary>
    /// Represents a material definition in the OBJ format, including properties for
    /// ambient, diffuse, and specular components, as well as texture and PBR extensions.
    /// </summary>
    public class OBJMaterial
    {
        /// <summary>
        /// Gets or sets the name of the material.
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// Gets or sets the ambient color of the material.
        /// </summary>
        public Vector3 Ambient = Vector3.Zero;

        /// <summary>
        /// Gets or sets the diffuse color of the material.
        /// </summary>
        public Vector3 Diffuse = Vector3.One;

        /// <summary>
        /// Gets or sets the specular color of the material.
        /// </summary>
        public Vector3 Specular = Vector3.Zero;

        /// <summary>
        /// Gets or sets the transmittance color of the material.
        /// </summary>
        public Vector3 Transmittance = Vector3.Zero;

        /// <summary>
        /// Gets or sets the emission color of the material.
        /// </summary>
        public Vector3 Emission = Vector3.Zero;

        /// <summary>
        /// Gets or sets the shininess of the material.
        /// </summary>
        public float Shininess = 1.0f;

        /// <summary>
        /// Gets or sets the index of refraction of the material.
        /// </summary>
        public float Ior = 1.0f;

        /// <summary>
        /// Gets or sets the dissolve factor of the material (1 = opaque, 0 = fully transparent).
        /// </summary>
        public float Dissolve = 1.0f;

        /// <summary>
        /// Gets or sets the illumination model of the material.
        /// </summary>
        public int Illum = 0;

        // Texture information

        /// <summary>
        /// Gets or sets the texture name for the ambient map.
        /// </summary>
        public string AmbientTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the diffuse map.
        /// </summary>
        public string DiffuseTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the specular map.
        /// </summary>
        public string SpecularTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the specular highlight map.
        /// </summary>
        public string SpecularHighlightTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the bump map.
        /// </summary>
        public string BumpTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the displacement map.
        /// </summary>
        public string DisplacementTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the alpha map.
        /// </summary>
        public string AlphaTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the reflection map.
        /// </summary>
        public string ReflectionTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture options for the ambient map.
        /// </summary>
        public OBJTextureOption AmbientTexopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the diffuse map.
        /// </summary>
        public OBJTextureOption DiffuseTexopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the specular map.
        /// </summary>
        public OBJTextureOption SpecularTexopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the specular highlight map.
        /// </summary>
        public OBJTextureOption SpecularHighlightTexopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the bump map.
        /// </summary>
        public OBJTextureOption BumpTexopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the displacement map.
        /// </summary>
        public OBJTextureOption DisplacementTexopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the alpha map.
        /// </summary>
        public OBJTextureOption AlphaTexopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the reflection map.
        /// </summary>
        public OBJTextureOption ReflectionTexopt = new OBJTextureOption();

        // PBR extension

        /// <summary>
        /// Gets or sets the roughness value for PBR rendering.
        /// </summary>
        public float Roughness = 0.8f;

        /// <summary>
        /// Gets or sets the metallic value for PBR rendering.
        /// </summary>
        public float Metallic = 0.0f;

        /// <summary>
        /// Gets or sets the sheen value for PBR rendering.
        /// </summary>
        public float Sheen = 0.0f;

        /// <summary>
        /// Gets or sets the clearcoat thickness for PBR rendering.
        /// </summary>
        public float ClearcoatThickness = 0.0f;

        /// <summary>
        /// Gets or sets the clearcoat roughness for PBR rendering.
        /// </summary>
        public float ClearcoatRoughness = 0.0f;

        /// <summary>
        /// Gets or sets the anisotropy value for PBR rendering.
        /// </summary>
        public float Anisotropy = 0.0f;

        /// <summary>
        /// Gets or sets the anisotropy rotation value for PBR rendering.
        /// </summary>
        public float AnisotropyRotation = 0.0f;

        /// <summary>
        /// Gets or sets the texture name for the roughness map.
        /// </summary>
        public string RoughnessTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the metallic map.
        /// </summary>
        public string MetallicTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the sheen map.
        /// </summary>
        public string SheenTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the emissive map.
        /// </summary>
        public string EmissiveTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture name for the normal map.
        /// </summary>
        public string NormalTexname = string.Empty;

        /// <summary>
        /// Gets or sets the texture options for the roughness map.
        /// </summary>
        public OBJTextureOption Roughness_texopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the metallic map.
        /// </summary>
        public OBJTextureOption Metallic_texopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the sheen map.
        /// </summary>
        public OBJTextureOption Sheen_texopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the emissive map.
        /// </summary>
        public OBJTextureOption Emissive_texopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the texture options for the normal map.
        /// </summary>
        public OBJTextureOption Normal_texopt = new OBJTextureOption();

        /// <summary>
        /// Gets or sets the padding value.
        /// </summary>
        public int Pad2;

        /// <summary>
        /// Gets or sets the key-value pairs for unknown parameters.
        /// </summary>
        public Dictionary<string, string> UnknownParameter = new Dictionary<string, string>();
    }
}
