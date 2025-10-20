// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

namespace OBJRuntime.DataTypes
{
    // https://en.wikipedia.org/wiki/Wavefront_.obj_file says ...
    //
    //  -blendu on | off                       # set horizontal texture blending
    //  (default on)
    //  -blendv on | off                       # set vertical texture blending
    //  (default on)
    //  -boost real_value                      # boost mip-map sharpness
    //  -mm base_value gain_value              # modify texture map values (default
    //  0 1)
    //                                         #     base_value = brightness,
    //                                         gain_value = contrast
    //  -o u [v [w]]                           # Origin offset             (default
    //  0 0 0)
    //  -s u [v [w]]                           # Scale                     (default
    //  1 1 1)
    //  -t u [v [w]]                           # Turbulence                (default
    //  0 0 0)
    //  -texres resolution                     # texture resolution to create
    //  -clamp on | off                        # only render texels in the clamped
    //  0-1 range (default off)
    //                                         #   When unclamped, textures are
    //                                         repeated across a surface,
    //                                         #   when clamped, only texels which
    //                                         fall within the 0-1
    //                                         #   range are rendered.
    //  -bm mult_value                         # bump multiplier (for bump maps
    //  only)
    //
    //  -imfchan r | g | b | m | l | z         # specifies which channel of the file
    //  is used to
    //                                         # create a scalar or bump texture.
    //                                         r:red, g:green,
    //                                         # b:blue, m:matte, l:luminance,
    //                                         z:z-depth..
    //                                         # (the default for bump is 'l' and
    //                                         for decal is 'm')
    //  bump -imfchan r bumpmap.tga            # says to use the red channel of
    //  bumpmap.tga as the bumpmap
    //
    // For reflection maps...
    //
    //   -type sphere                           # specifies a sphere for a "refl"
    //   reflection map
    //   -type cube_top    | cube_bottom |      # when using a cube map, the texture
    //   file for each
    //         cube_front  | cube_back   |      # side of the cube is specified
    //         separately
    //         cube_left   | cube_right

    /// <summary>
    /// Represents the options for configuring a texture in an OBJ file.
    /// </summary>
    public class OBJTextureOption
    {
        /// <summary>
        /// Specifies the type of texture (e.g., None, Sphere, Cube).
        /// </summary>
        public OBJTextureType Type = OBJTextureType.None;

        /// <summary>
        /// Controls the sharpness of the texture. Default is 1.0.
        /// </summary>
        public float Sharpness = 1.0f;

        /// <summary>
        /// Adjusts the brightness of the texture. Default is 0.0.
        /// </summary>
        public float Brightness = 0.0f;

        /// <summary>
        /// Adjusts the contrast of the texture. Default is 1.0.
        /// </summary>
        public float Contrast = 1.0f;

        /// <summary>
        /// Specifies the origin offset of the texture. Default is [0, 0, 0].
        /// </summary>
        public float[] OriginOffset = new float[3] { 0, 0, 0 };

        /// <summary>
        /// Specifies the scale of the texture. Default is [1, 1, 1].
        /// </summary>
        public float[] Scale = new float[3] { 1, 1, 1 };

        /// <summary>
        /// Specifies the turbulence of the texture. Default is [0, 0, 0].
        /// </summary>
        public float[] Turbulence = new float[3] { 0, 0, 0 };

        /// <summary>
        /// Specifies the texture resolution. Default is -1 (unspecified).
        /// </summary>
        public int TextureResolution = -1;

        /// <summary>
        /// Indicates whether the texture is clamped to the 0-1 range. Default is false.
        /// </summary>
        public bool Clamp = false;

        /// <summary>
        /// Specifies the channel of the file used to create a scalar or bump texture.
        /// Default is 'm' (matte).
        /// </summary>
        public char Imfchan = 'm';

        /// <summary>
        /// Indicates whether horizontal texture blending is enabled. Default is true.
        /// </summary>
        public bool Blendu = true;

        /// <summary>
        /// Indicates whether vertical texture blending is enabled. Default is true.
        /// </summary>
        public bool Blendv = true;

        /// <summary>
        /// Specifies the bump multiplier for bump maps. Default is 1.0.
        /// </summary>
        public float BumpMultiplier = 1.0f;

        /// <summary>
        /// Specifies the color space of the texture (e.g., "sRGB" or "linear").
        /// </summary>
        public string Colorspace = string.Empty; // e.g. "sRGB" or "linear"
    }
}
