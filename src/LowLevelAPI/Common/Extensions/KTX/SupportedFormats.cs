// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace Evergine.Assets.Extensions.KTX
{
    /// <summary>
    /// Helper class.
    /// </summary>
    internal class SupportedFormats
    {
        /// <summary>
        /// Convert from KTX format to Evergine format.
        /// </summary>
        /// <param name="header">The KTX header.</param>
        /// <returns>The Evergine format if this exists.</returns>
        public static Evergine.Common.Graphics.PixelFormat FromOpenGLFormat(ref KTXHeader header)
        {
            if ((PixelFormat)header.glFormat == PixelFormat.Rgba
                && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgba32f)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32B32A32_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgbaInteger
                && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgba32ui)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32B32A32_UInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgbaInteger
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgba32i)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32B32A32_SInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgbaInteger
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgb32f)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32B32_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgbaInteger
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgb32ui)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32B32_UInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgbInteger
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgb32i)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32B32_SInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Rgba
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgba16f)
            {
                return Evergine.Common.Graphics.PixelFormat.R16G16B16A16_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgbaInteger
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgba16ui)
            {
                return Evergine.Common.Graphics.PixelFormat.R16G16B16A16_UInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgbaInteger
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgba16i)
            {
                return Evergine.Common.Graphics.PixelFormat.R16G16B16A16_SInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Rg
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rg32f)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgInteger
               && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rg32ui)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32_UInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgInteger
                && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rg32i)
            {
                return Evergine.Common.Graphics.PixelFormat.R32G32_SInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Rgba
                && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgb10A2)
            {
                return Evergine.Common.Graphics.PixelFormat.R10G10B10A2_UNorm;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Rgb
                && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.R11fG11fB10f)
            {
                return Evergine.Common.Graphics.PixelFormat.R11G11B10_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Rgba
                && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgba8)
            {
                return Evergine.Common.Graphics.PixelFormat.R8G8B8A8_UNorm;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Rgba
                 && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Srgb8Alpha8)
            {
                return Evergine.Common.Graphics.PixelFormat.R8G8B8A8_UNorm_SRgb;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Rg
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rg16f)
            {
                return Evergine.Common.Graphics.PixelFormat.R16G16_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgInteger
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rg16ui)
            {
                return Evergine.Common.Graphics.PixelFormat.R16G16_UInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RgInteger
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rg16i)
            {
                return Evergine.Common.Graphics.PixelFormat.R16G16_SInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.DepthComponent
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.DepthComponent32f)
            {
                return Evergine.Common.Graphics.PixelFormat.D32_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Red
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.R32f)
            {
                return Evergine.Common.Graphics.PixelFormat.R32_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RedInteger
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.R32ui)
            {
                return Evergine.Common.Graphics.PixelFormat.R32_UInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RedInteger
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.R32i)
            {
                return Evergine.Common.Graphics.PixelFormat.R32_SInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.DepthStencil
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Depth24Stencil8)
            {
                return Evergine.Common.Graphics.PixelFormat.D24_UNorm_S8_UInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Red
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.R16f)
            {
                return Evergine.Common.Graphics.PixelFormat.R16_Float;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.DepthComponent
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.DepthComponent16)
            {
                return Evergine.Common.Graphics.PixelFormat.D16_UNorm;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RedInteger
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.R16ui)
            {
                return Evergine.Common.Graphics.PixelFormat.R16_UInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.RedInteger
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.R16i)
            {
                return Evergine.Common.Graphics.PixelFormat.R16_SInt;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Luminance
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Luminance)
            {
                return Evergine.Common.Graphics.PixelFormat.R8_UNorm;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.LuminanceAlpha
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.LuminanceAlpha)
            {
                return Evergine.Common.Graphics.PixelFormat.R8G8_UNorm;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Alpha
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Alpha)
            {
                return Evergine.Common.Graphics.PixelFormat.A8_UNorm;
            }
            else if (header.glFormat == 0
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.CompressedRgbS3tcDxt1Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.BC1_UNorm_SRgb;
            }
            else if (header.glFormat == 0
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.CompressedRgbaS3tcDxt1Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.BC1_UNorm;
            }
            else if (header.glFormat == 0
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.BC1_UNorm_SRgb;
            }
            else if (header.glFormat == 0
                  && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.CompressedRgbaS3tcDxt3Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.BC2_UNorm;
            }
            else if (header.glFormat == 0
                   && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.BC2_UNorm_SRgb;
            }
            else if (header.glFormat == 0
                    && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.CompressedRgbaS3tcDxt5Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.BC3_UNorm;
            }
            else if (header.glFormat == 0
                    && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.BC3_UNorm_SRgb;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Bgra
                    && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Rgba)
            {
                return Evergine.Common.Graphics.PixelFormat.B8G8R8A8_UNorm;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Bgra
                    && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.Srgb8Alpha8)
            {
                return Evergine.Common.Graphics.PixelFormat.B8G8R8A8_UNorm_SRgb;
            }
            else if ((PixelFormat)header.glFormat == PixelFormat.Red
                    && (PixelInternalFormat)header.glInternalFormat == PixelInternalFormat.R16ui)
            {
                return Evergine.Common.Graphics.PixelFormat.R16_UNorm;
            }
            else if (header.glFormat == 0
                    && header.glInternalFormat == 36196)
            {
                return Evergine.Common.Graphics.PixelFormat.ETC1_RGB8;
            }
            else if (header.glFormat == 0
                    && (CompressedInternalFormat)header.glInternalFormat == CompressedInternalFormat.Etc1Rgb8Oes)
            {
                return Evergine.Common.Graphics.PixelFormat.ETC2_RGBA;
            }
            else if (header.glFormat == 0
                    && (CompressedInternalFormat)header.glInternalFormat == CompressedInternalFormat.CompressedRgba8Etc2Eac)
            {
                return Evergine.Common.Graphics.PixelFormat.ETC2_RGBA;
            }
            else if (header.glFormat == 0
                    && (CompressedInternalFormat)header.glInternalFormat == CompressedInternalFormat.CompressedSrgb8Alpha8Etc2Eac)
            {
                return Evergine.Common.Graphics.PixelFormat.ETC2_RGBA_SRGB;
            }
            else if (header.glFormat == 0
                    && (ImgTextureCompressionPvrtc)header.glInternalFormat == ImgTextureCompressionPvrtc.CompressedRgbPvrtc2Bppv1Img)
            {
                return Evergine.Common.Graphics.PixelFormat.PVRTC_2BPP_RGB;
            }
            else if (header.glFormat == 0
                    && (ImgTextureCompressionPvrtc)header.glInternalFormat == ImgTextureCompressionPvrtc.CompressedRgbPvrtc4Bppv1Img)
            {
                return Evergine.Common.Graphics.PixelFormat.PVRTC_4BPP_RGB;
            }
            else if (header.glFormat == 0
                    && (ImgTextureCompressionPvrtc)header.glInternalFormat == ImgTextureCompressionPvrtc.CompressedRgbaPvrtc2Bppv1Img)
            {
                return Evergine.Common.Graphics.PixelFormat.PVRTC_2BPP_RGBA;
            }
            else if (header.glFormat == 0
                  && (ImgTextureCompressionPvrtc)header.glInternalFormat == ImgTextureCompressionPvrtc.CompressedRgbaPvrtc4Bppv1Img)
            {
                return Evergine.Common.Graphics.PixelFormat.PVRTC_4BPP_RGBA;
            }
            else if (header.glFormat == 0
                 && (ExtPvrtcSrgb)header.glInternalFormat == ExtPvrtcSrgb.CompressedSrgbPvrtc2Bppv1Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.PVRTC_2BPP_RGB_SRGB;
            }
            else if (header.glFormat == 0
                 && (ExtPvrtcSrgb)header.glInternalFormat == ExtPvrtcSrgb.CompressedSrgbPvrtc4Bppv1Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.PVRTC_4BPP_RGB_SRGB;
            }
            else if (header.glFormat == 0
                 && (ExtPvrtcSrgb)header.glInternalFormat == ExtPvrtcSrgb.CompressedSrgbAlphaPvrtc2Bppv1Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.PVRTC_2BPP_RGBA_SRGBA;
            }
            else if (header.glFormat == 0
                 && (ExtPvrtcSrgb)header.glInternalFormat == ExtPvrtcSrgb.CompressedSrgbAlphaPvrtc4Bppv1Ext)
            {
                return Evergine.Common.Graphics.PixelFormat.PVRTC_4BPP_RGBA_SRGBA;
            }
            else
            {
                throw new NotSupportedException("Texture format not supported");
            }
        }
    }

    // Summary:
    //     Used in GL.Arb.CompressedTexImage1D, GL.Arb.CompressedTexImage2D and 45 other
    //     functions
    internal enum PixelInternalFormat
    {
        // Summary:
        //     Original was GL_ONE = 1
        One = 1,

        // Summary:
        //     Original was GL_TWO = 2
        Two = 2,

        // Summary:
        //     Original was GL_THREE = 3
        Three = 3,

        // Summary:
        //     Original was GL_FOUR = 4
        Four = 4,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT = 0x1902
        DepthComponent = 6402,

        // Summary:
        //     Original was GL_ALPHA = 0x1906
        Alpha = 6406,

        // Summary:
        //     Original was GL_RGB = 0x1907
        Rgb = 6407,

        // Summary:
        //     Original was GL_RGBA = 0x1908
        Rgba = 6408,

        // Summary:
        //     Original was GL_LUMINANCE = 0x1909
        Luminance = 6409,

        // Summary:
        //     Original was GL_LUMINANCE_ALPHA = 0x190A
        LuminanceAlpha = 6410,

        // Summary:
        //     Original was GL_R3_G3_B2 = 0x2A10
        R3G3B2 = 10768,

        // Summary:
        //     Original was GL_ALPHA4 = 0x803B
        Alpha4 = 32827,

        // Summary:
        //     Original was GL_ALPHA8 = 0x803C
        Alpha8 = 32828,

        // Summary:
        //     Original was GL_ALPHA12 = 0x803D
        Alpha12 = 32829,

        // Summary:
        //     Original was GL_ALPHA16 = 0x803E
        Alpha16 = 32830,

        // Summary:
        //     Original was GL_LUMINANCE4 = 0x803F
        Luminance4 = 32831,

        // Summary:
        //     Original was GL_LUMINANCE8 = 0x8040
        Luminance8 = 32832,

        // Summary:
        //     Original was GL_LUMINANCE12 = 0x8041
        Luminance12 = 32833,

        // Summary:
        //     Original was GL_LUMINANCE16 = 0x8042
        Luminance16 = 32834,

        // Summary:
        //     Original was GL_LUMINANCE4_ALPHA4 = 0x8043
        Luminance4Alpha4 = 32835,

        // Summary:
        //     Original was GL_LUMINANCE6_ALPHA2 = 0x8044
        Luminance6Alpha2 = 32836,

        // Summary:
        //     Original was GL_LUMINANCE8_ALPHA8 = 0x8045
        Luminance8Alpha8 = 32837,

        // Summary:
        //     Original was GL_LUMINANCE12_ALPHA4 = 0x8046
        Luminance12Alpha4 = 32838,

        // Summary:
        //     Original was GL_LUMINANCE12_ALPHA12 = 0x8047
        Luminance12Alpha12 = 32839,

        // Summary:
        //     Original was GL_LUMINANCE16_ALPHA16 = 0x8048
        Luminance16Alpha16 = 32840,

        // Summary:
        //     Original was GL_INTENSITY = 0x8049
        Intensity = 32841,

        // Summary:
        //     Original was GL_INTENSITY4 = 0x804A
        Intensity4 = 32842,

        // Summary:
        //     Original was GL_INTENSITY8 = 0x804B
        Intensity8 = 32843,

        // Summary:
        //     Original was GL_INTENSITY12 = 0x804C
        Intensity12 = 32844,

        // Summary:
        //     Original was GL_INTENSITY16 = 0x804D
        Intensity16 = 32845,

        // Summary:
        //     Original was GL_RGB2_EXT = 0x804E
        Rgb2Ext = 32846,

        // Summary:
        //     Original was GL_RGB4 = 0x804F
        Rgb4 = 32847,

        // Summary:
        //     Original was GL_RGB5 = 0x8050
        Rgb5 = 32848,

        // Summary:
        //     Original was GL_RGB8 = 0x8051
        Rgb8 = 32849,

        // Summary:
        //     Original was GL_RGB10 = 0x8052
        Rgb10 = 32850,

        // Summary:
        //     Original was GL_RGB12 = 0x8053
        Rgb12 = 32851,

        // Summary:
        //     Original was GL_RGB16 = 0x8054
        Rgb16 = 32852,

        // Summary:
        //     Original was GL_RGBA2 = 0x8055
        Rgba2 = 32853,

        // Summary:
        //     Original was GL_RGBA4 = 0x8056
        Rgba4 = 32854,

        // Summary:
        //     Original was GL_RGB5_A1 = 0x8057
        Rgb5A1 = 32855,

        // Summary:
        //     Original was GL_RGBA8 = 0x8058
        Rgba8 = 32856,

        // Summary:
        //     Original was GL_RGB10_A2 = 0x8059
        Rgb10A2 = 32857,

        // Summary:
        //     Original was GL_RGBA12 = 0x805A
        Rgba12 = 32858,

        // Summary:
        //     Original was GL_RGBA16 = 0x805B
        Rgba16 = 32859,

        // Summary:
        //     Original was GL_DUAL_ALPHA4_SGIS = 0x8110
        DualAlpha4Sgis = 33040,

        // Summary:
        //     Original was GL_DUAL_ALPHA8_SGIS = 0x8111
        DualAlpha8Sgis = 33041,

        // Summary:
        //     Original was GL_DUAL_ALPHA12_SGIS = 0x8112
        DualAlpha12Sgis = 33042,

        // Summary:
        //     Original was GL_DUAL_ALPHA16_SGIS = 0x8113
        DualAlpha16Sgis = 33043,

        // Summary:
        //     Original was GL_DUAL_LUMINANCE4_SGIS = 0x8114
        DualLuminance4Sgis = 33044,

        // Summary:
        //     Original was GL_DUAL_LUMINANCE8_SGIS = 0x8115
        DualLuminance8Sgis = 33045,

        // Summary:
        //     Original was GL_DUAL_LUMINANCE12_SGIS = 0x8116
        DualLuminance12Sgis = 33046,

        // Summary:
        //     Original was GL_DUAL_LUMINANCE16_SGIS = 0x8117
        DualLuminance16Sgis = 33047,

        // Summary:
        //     Original was GL_DUAL_INTENSITY4_SGIS = 0x8118
        DualIntensity4Sgis = 33048,

        // Summary:
        //     Original was GL_DUAL_INTENSITY8_SGIS = 0x8119
        DualIntensity8Sgis = 33049,

        // Summary:
        //     Original was GL_DUAL_INTENSITY12_SGIS = 0x811A
        DualIntensity12Sgis = 33050,

        // Summary:
        //     Original was GL_DUAL_INTENSITY16_SGIS = 0x811B
        DualIntensity16Sgis = 33051,

        // Summary:
        //     Original was GL_DUAL_LUMINANCE_ALPHA4_SGIS = 0x811C
        DualLuminanceAlpha4Sgis = 33052,

        // Summary:
        //     Original was GL_DUAL_LUMINANCE_ALPHA8_SGIS = 0x811D
        DualLuminanceAlpha8Sgis = 33053,

        // Summary:
        //     Original was GL_QUAD_ALPHA4_SGIS = 0x811E
        QuadAlpha4Sgis = 33054,

        // Summary:
        //     Original was GL_QUAD_ALPHA8_SGIS = 0x811F
        QuadAlpha8Sgis = 33055,

        // Summary:
        //     Original was GL_QUAD_LUMINANCE4_SGIS = 0x8120
        QuadLuminance4Sgis = 33056,

        // Summary:
        //     Original was GL_QUAD_LUMINANCE8_SGIS = 0x8121
        QuadLuminance8Sgis = 33057,

        // Summary:
        //     Original was GL_QUAD_INTENSITY4_SGIS = 0x8122
        QuadIntensity4Sgis = 33058,

        // Summary:
        //     Original was GL_QUAD_INTENSITY8_SGIS = 0x8123
        QuadIntensity8Sgis = 33059,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT16 = 0x81a5
        DepthComponent16 = 33189,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT16_SGIX = 0x81A5
        DepthComponent16Sgix = 33189,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT24 = 0x81a6
        DepthComponent24 = 33190,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT24_SGIX = 0x81A6
        DepthComponent24Sgix = 33190,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT32 = 0x81a7
        DepthComponent32 = 33191,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT32_SGIX = 0x81A7
        DepthComponent32Sgix = 33191,

        // Summary:
        //     Original was GL_COMPRESSED_RED = 0x8225
        CompressedRed = 33317,

        // Summary:
        //     Original was GL_COMPRESSED_RG = 0x8226
        CompressedRg = 33318,

        // Summary:
        //     Original was GL_R8 = 0x8229
        R8 = 33321,

        // Summary:
        //     Original was GL_R16 = 0x822A
        R16 = 33322,

        // Summary:
        //     Original was GL_RG8 = 0x822B
        Rg8 = 33323,

        // Summary:
        //     Original was GL_RG16 = 0x822C
        Rg16 = 33324,

        // Summary:
        //     Original was GL_R16F = 0x822D
        R16f = 33325,

        // Summary:
        //     Original was GL_R32F = 0x822E
        R32f = 33326,

        // Summary:
        //     Original was GL_RG16F = 0x822F
        Rg16f = 33327,

        // Summary:
        //     Original was GL_RG32F = 0x8230
        Rg32f = 33328,

        // Summary:
        //     Original was GL_R8I = 0x8231
        R8i = 33329,

        // Summary:
        //     Original was GL_R8UI = 0x8232
        R8ui = 33330,

        // Summary:
        //     Original was GL_R16I = 0x8233
        R16i = 33331,

        // Summary:
        //     Original was GL_R16UI = 0x8234
        R16ui = 33332,

        // Summary:
        //     Original was GL_R32I = 0x8235
        R32i = 33333,

        // Summary:
        //     Original was GL_R32UI = 0x8236
        R32ui = 33334,

        // Summary:
        //     Original was GL_RG8I = 0x8237
        Rg8i = 33335,

        // Summary:
        //     Original was GL_RG8UI = 0x8238
        Rg8ui = 33336,

        // Summary:
        //     Original was GL_RG16I = 0x8239
        Rg16i = 33337,

        // Summary:
        //     Original was GL_RG16UI = 0x823A
        Rg16ui = 33338,

        // Summary:
        //     Original was GL_RG32I = 0x823B
        Rg32i = 33339,

        // Summary:
        //     Original was GL_RG32UI = 0x823C
        Rg32ui = 33340,

        // Summary:
        //     Original was GL_COMPRESSED_RGB_S3TC_DXT1_EXT = 0x83F0
        CompressedRgbS3tcDxt1Ext = 33776,

        // Summary:
        //     Original was GL_COMPRESSED_RGBA_S3TC_DXT1_EXT = 0x83F1
        CompressedRgbaS3tcDxt1Ext = 33777,

        // Summary:
        //     Original was GL_COMPRESSED_RGBA_S3TC_DXT3_EXT = 0x83F2
        CompressedRgbaS3tcDxt3Ext = 33778,

        // Summary:
        //     Original was GL_COMPRESSED_RGBA_S3TC_DXT5_EXT = 0x83F3
        CompressedRgbaS3tcDxt5Ext = 33779,

        // Summary:
        //     Original was GL_RGB_ICC_SGIX = 0x8460
        RgbIccSgix = 33888,

        // Summary:
        //     Original was GL_RGBA_ICC_SGIX = 0x8461
        RgbaIccSgix = 33889,

        // Summary:
        //     Original was GL_ALPHA_ICC_SGIX = 0x8462
        AlphaIccSgix = 33890,

        // Summary:
        //     Original was GL_LUMINANCE_ICC_SGIX = 0x8463
        LuminanceIccSgix = 33891,

        // Summary:
        //     Original was GL_INTENSITY_ICC_SGIX = 0x8464
        IntensityIccSgix = 33892,

        // Summary:
        //     Original was GL_LUMINANCE_ALPHA_ICC_SGIX = 0x8465
        LuminanceAlphaIccSgix = 33893,

        // Summary:
        //     Original was GL_R5_G6_B5_ICC_SGIX = 0x8466
        R5G6B5IccSgix = 33894,

        // Summary:
        //     Original was GL_R5_G6_B5_A8_ICC_SGIX = 0x8467
        R5G6B5A8IccSgix = 33895,

        // Summary:
        //     Original was GL_ALPHA16_ICC_SGIX = 0x8468
        Alpha16IccSgix = 33896,

        // Summary:
        //     Original was GL_LUMINANCE16_ICC_SGIX = 0x8469
        Luminance16IccSgix = 33897,

        // Summary:
        //     Original was GL_INTENSITY16_ICC_SGIX = 0x846A
        Intensity16IccSgix = 33898,

        // Summary:
        //     Original was GL_LUMINANCE16_ALPHA8_ICC_SGIX = 0x846B
        Luminance16Alpha8IccSgix = 33899,

        // Summary:
        //     Original was GL_COMPRESSED_ALPHA = 0x84E9
        CompressedAlpha = 34025,

        // Summary:
        //     Original was GL_COMPRESSED_LUMINANCE = 0x84EA
        CompressedLuminance = 34026,

        // Summary:
        //     Original was GL_COMPRESSED_LUMINANCE_ALPHA = 0x84EB
        CompressedLuminanceAlpha = 34027,

        // Summary:
        //     Original was GL_COMPRESSED_INTENSITY = 0x84EC
        CompressedIntensity = 34028,

        // Summary:
        //     Original was GL_COMPRESSED_RGB = 0x84ED
        CompressedRgb = 34029,

        // Summary:
        //     Original was GL_COMPRESSED_RGBA = 0x84EE
        CompressedRgba = 34030,

        // Summary:
        //     Original was GL_DEPTH_STENCIL = 0x84F9
        DepthStencil = 34041,

        // Summary:
        //     Original was GL_RGBA32F = 0x8814
        Rgba32f = 34836,

        // Summary:
        //     Original was GL_RGB32F = 0x8815
        Rgb32f = 34837,

        // Summary:
        //     Original was GL_RGBA16F = 0x881A
        Rgba16f = 34842,

        // Summary:
        //     Original was GL_RGB16F = 0x881B
        Rgb16f = 34843,

        // Summary:
        //     Original was GL_DEPTH24_STENCIL8 = 0x88F0
        Depth24Stencil8 = 35056,

        // Summary:
        //     Original was GL_R11F_G11F_B10F = 0x8C3A
        R11fG11fB10f = 35898,

        // Summary:
        //     Original was GL_RGB9_E5 = 0x8C3D
        Rgb9E5 = 35901,

        // Summary:
        //     Original was GL_SRGB = 0x8C40
        Srgb = 35904,

        // Summary:
        //     Original was GL_SRGB8 = 0x8C41
        Srgb8 = 35905,

        // Summary:
        //     Original was GL_SRGB_ALPHA = 0x8C42
        SrgbAlpha = 35906,

        // Summary:
        //     Original was GL_SRGB8_ALPHA8 = 0x8C43
        Srgb8Alpha8 = 35907,

        // Summary:
        //     Original was GL_SLUMINANCE_ALPHA = 0x8C44
        SluminanceAlpha = 35908,

        // Summary:
        //     Original was GL_SLUMINANCE8_ALPHA8 = 0x8C45
        Sluminance8Alpha8 = 35909,

        // Summary:
        //     Original was GL_SLUMINANCE = 0x8C46
        Sluminance = 35910,

        // Summary:
        //     Original was GL_SLUMINANCE8 = 0x8C47
        Sluminance8 = 35911,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB = 0x8C48
        CompressedSrgb = 35912,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA = 0x8C49
        CompressedSrgbAlpha = 35913,

        // Summary:
        //     Original was GL_COMPRESSED_SLUMINANCE = 0x8C4A
        CompressedSluminance = 35914,

        // Summary:
        //     Original was GL_COMPRESSED_SLUMINANCE_ALPHA = 0x8C4B
        CompressedSluminanceAlpha = 35915,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_S3TC_DXT1_EXT = 0x8C4C
        CompressedSrgbS3tcDxt1Ext = 35916,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT = 0x8C4D
        CompressedSrgbAlphaS3tcDxt1Ext = 35917,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT = 0x8C4E
        CompressedSrgbAlphaS3tcDxt3Ext = 35918,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT = 0x8C4F
        CompressedSrgbAlphaS3tcDxt5Ext = 35919,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT32F = 0x8CAC
        DepthComponent32f = 36012,

        // Summary:
        //     Original was GL_DEPTH32F_STENCIL8 = 0x8CAD
        Depth32fStencil8 = 36013,

        // Summary:
        //     Original was GL_RGBA32UI = 0x8D70
        Rgba32ui = 36208,

        // Summary:
        //     Original was GL_RGB32UI = 0x8D71
        Rgb32ui = 36209,

        // Summary:
        //     Original was GL_RGBA16UI = 0x8D76
        Rgba16ui = 36214,

        // Summary:
        //     Original was GL_RGB16UI = 0x8D77
        Rgb16ui = 36215,

        // Summary:
        //     Original was GL_RGBA8UI = 0x8D7C
        Rgba8ui = 36220,

        // Summary:
        //     Original was GL_RGB8UI = 0x8D7D
        Rgb8ui = 36221,

        // Summary:
        //     Original was GL_RGBA32I = 0x8D82
        Rgba32i = 36226,

        // Summary:
        //     Original was GL_RGB32I = 0x8D83
        Rgb32i = 36227,

        // Summary:
        //     Original was GL_RGBA16I = 0x8D88
        Rgba16i = 36232,

        // Summary:
        //     Original was GL_RGB16I = 0x8D89
        Rgb16i = 36233,

        // Summary:
        //     Original was GL_RGBA8I = 0x8D8E
        Rgba8i = 36238,

        // Summary:
        //     Original was GL_RGB8I = 0x8D8F
        Rgb8i = 36239,

        // Summary:
        //     Original was GL_FLOAT_32_UNSIGNED_INT_24_8_REV = 0x8DAD
        Float32UnsignedInt248Rev = 36269,

        // Summary:
        //     Original was GL_COMPRESSED_RED_RGTC1 = 0x8DBB
        CompressedRedRgtc1 = 36283,

        // Summary:
        //     Original was GL_COMPRESSED_SIGNED_RED_RGTC1 = 0x8DBC
        CompressedSignedRedRgtc1 = 36284,

        // Summary:
        //     Original was GL_COMPRESSED_RG_RGTC2 = 0x8DBD
        CompressedRgRgtc2 = 36285,

        // Summary:
        //     Original was GL_COMPRESSED_SIGNED_RG_RGTC2 = 0x8DBE
        CompressedSignedRgRgtc2 = 36286,

        // Summary:
        //     Original was GL_COMPRESSED_RGBA_BPTC_UNORM = 0x8E8C
        CompressedRgbaBptcUnorm = 36492,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM = 0x8E8D
        CompressedSrgbAlphaBptcUnorm = 36493,

        // Summary:
        //     Original was GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT = 0x8E8E
        CompressedRgbBptcSignedFloat = 36494,

        // Summary:
        //     Original was GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT = 0x8E8F
        CompressedRgbBptcUnsignedFloat = 36495,

        // Summary:
        //     Original was GL_R8_SNORM = 0x8F94
        R8Snorm = 36756,

        // Summary:
        //     Original was GL_RG8_SNORM = 0x8F95
        Rg8Snorm = 36757,

        // Summary:
        //     Original was GL_RGB8_SNORM = 0x8F96
        Rgb8Snorm = 36758,

        // Summary:
        //     Original was GL_RGBA8_SNORM = 0x8F97
        Rgba8Snorm = 36759,

        // Summary:
        //     Original was GL_R16_SNORM = 0x8F98
        R16Snorm = 36760,

        // Summary:
        //     Original was GL_RG16_SNORM = 0x8F99
        Rg16Snorm = 36761,

        // Summary:
        //     Original was GL_RGB16_SNORM = 0x8F9A
        Rgb16Snorm = 36762,

        // Summary:
        //     Original was GL_RGBA16_SNORM = 0x8F9B
        Rgba16Snorm = 36763,

        // Summary:
        //     Original was GL_RGB10_A2UI = 0x906F
        Rgb10A2ui = 36975,
    }

    // Summary:
    //     Used in GL.Arb.CompressedTexSubImage1D, GL.Arb.CompressedTexSubImage2D and 80
    //     other functions
    internal enum PixelFormat
    {
        // Summary:
        //     Original was GL_UNSIGNED_SHORT = 0x1403
        UnsignedShort = 5123,

        // Summary:
        //     Original was GL_UNSIGNED_INT = 0x1405
        UnsignedInt = 5125,

        // Summary:
        //     Original was GL_COLOR_INDEX = 0x1900
        ColorIndex = 6400,

        // Summary:
        //     Original was GL_STENCIL_INDEX = 0x1901
        StencilIndex = 6401,

        // Summary:
        //     Original was GL_DEPTH_COMPONENT = 0x1902
        DepthComponent = 6402,

        // Summary:
        //     Original was GL_RED = 0x1903
        Red = 6403,

        // Summary:
        //     Original was GL_RED_EXT = 0x1903
        RedExt = 6403,

        // Summary:
        //     Original was GL_GREEN = 0x1904
        Green = 6404,

        // Summary:
        //     Original was GL_BLUE = 0x1905
        Blue = 6405,

        // Summary:
        //     Original was GL_ALPHA = 0x1906
        Alpha = 6406,

        // Summary:
        //     Original was GL_RGB = 0x1907
        Rgb = 6407,

        // Summary:
        //     Original was GL_RGBA = 0x1908
        Rgba = 6408,

        // Summary:
        //     Original was GL_LUMINANCE = 0x1909
        Luminance = 6409,

        // Summary:
        //     Original was GL_LUMINANCE_ALPHA = 0x190A
        LuminanceAlpha = 6410,

        // Summary:
        //     Original was GL_ABGR_EXT = 0x8000
        AbgrExt = 32768,

        // Summary:
        //     Original was GL_CMYK_EXT = 0x800C
        CmykExt = 32780,

        // Summary:
        //     Original was GL_CMYKA_EXT = 0x800D
        CmykaExt = 32781,

        // Summary:
        //     Original was GL_BGR = 0x80E0
        Bgr = 32992,

        // Summary:
        //     Original was GL_BGRA = 0x80E1
        Bgra = 32993,

        // Summary:
        //     Original was GL_YCRCB_422_SGIX = 0x81BB
        Ycrcb422Sgix = 33211,

        // Summary:
        //     Original was GL_YCRCB_444_SGIX = 0x81BC
        Ycrcb444Sgix = 33212,

        // Summary:
        //     Original was GL_RG = 0x8227
        Rg = 33319,

        // Summary:
        //     Original was GL_RG_INTEGER = 0x8228
        RgInteger = 33320,

        // Summary:
        //     Original was GL_R5_G6_B5_ICC_SGIX = 0x8466
        R5G6B5IccSgix = 33894,

        // Summary:
        //     Original was GL_R5_G6_B5_A8_ICC_SGIX = 0x8467
        R5G6B5A8IccSgix = 33895,

        // Summary:
        //     Original was GL_ALPHA16_ICC_SGIX = 0x8468
        Alpha16IccSgix = 33896,

        // Summary:
        //     Original was GL_LUMINANCE16_ICC_SGIX = 0x8469
        Luminance16IccSgix = 33897,

        // Summary:
        //     Original was GL_LUMINANCE16_ALPHA8_ICC_SGIX = 0x846B
        Luminance16Alpha8IccSgix = 33899,

        // Summary:
        //     Original was GL_DEPTH_STENCIL = 0x84F9
        DepthStencil = 34041,

        // Summary:
        //     Original was GL_RED_INTEGER = 0x8D94
        RedInteger = 36244,

        // Summary:
        //     Original was GL_GREEN_INTEGER = 0x8D95
        GreenInteger = 36245,

        // Summary:
        //     Original was GL_BLUE_INTEGER = 0x8D96
        BlueInteger = 36246,

        // Summary:
        //     Original was GL_ALPHA_INTEGER = 0x8D97
        AlphaInteger = 36247,

        // Summary:
        //     Original was GL_RGB_INTEGER = 0x8D98
        RgbInteger = 36248,

        // Summary:
        //     Original was GL_RGBA_INTEGER = 0x8D99
        RgbaInteger = 36249,

        // Summary:
        //     Original was GL_BGR_INTEGER = 0x8D9A
        BgrInteger = 36250,

        // Summary:
        //     Original was GL_BGRA_INTEGER = 0x8D9B
        BgraInteger = 36251,
    }

    // Summary:
    //     Used in GL.CompressedTexImage2D, GL.CompressedTexImage3D and 1 other function
    internal enum CompressedInternalFormat
    {
        // Summary:
        //     Original was GL_ETC1_RGB8_OES = 0x8D64
        Etc1Rgb8Oes = 36196,

        // Summary:
        //     Original was GL_COMPRESSED_R11_EAC = 0x9270
        CompressedR11Eac = 37488,

        // Summary:
        //     Original was GL_COMPRESSED_SIGNED_R11_EAC = 0x9271
        CompressedSignedR11Eac = 37489,

        // Summary:
        //     Original was GL_COMPRESSED_RG11_EAC = 0x9272
        CompressedRg11Eac = 37490,

        // Summary:
        //     Original was GL_COMPRESSED_SIGNED_RG11_EAC = 0x9273
        CompressedSignedRg11Eac = 37491,

        // Summary:
        //     Original was GL_COMPRESSED_RGB8_ETC2 = 0x9274
        CompressedRgb8Etc2 = 37492,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB8_ETC2 = 0x9275
        CompressedSrgb8Etc2 = 37493,

        // Summary:
        //     Original was GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9276
        CompressedRgb8PunchthroughAlpha1Etc2 = 37494,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9277
        CompressedSrgb8PunchthroughAlpha1Etc2 = 37495,

        // Summary:
        //     Original was GL_COMPRESSED_RGBA8_ETC2_EAC = 0x9278
        CompressedRgba8Etc2Eac = 37496,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 0x9279
        CompressedSrgb8Alpha8Etc2Eac = 37497,
    }

    // Summary:
    //     Not used directly.
    internal enum ImgTextureCompressionPvrtc
    {
        // Summary:
        //     Original was GL_COMPRESSED_RGB_PVRTC_4BPPV1_IMG = 0x8C00
        CompressedRgbPvrtc4Bppv1Img = 35840,

        // Summary:
        //     Original was GL_COMPRESSED_RGB_PVRTC_2BPPV1_IMG = 0x8C01
        CompressedRgbPvrtc2Bppv1Img = 35841,

        // Summary:
        //     Original was GL_COMPRESSED_RGBA_PVRTC_4BPPV1_IMG = 0x8C02
        CompressedRgbaPvrtc4Bppv1Img = 35842,

        // Summary:
        //     Original was GL_COMPRESSED_RGBA_PVRTC_2BPPV1_IMG = 0x8C03
        CompressedRgbaPvrtc2Bppv1Img = 35843,
    }

    // Summary:
    //     Not used directly.
    internal enum ExtPvrtcSrgb
    {
        // Summary:
        //     Original was GL_COMPRESSED_SRGB_PVRTC_2BPPV1_EXT = 0x8A54
        CompressedSrgbPvrtc2Bppv1Ext = 35412,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_PVRTC_4BPPV1_EXT = 0x8A55
        CompressedSrgbPvrtc4Bppv1Ext = 35413,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA_PVRTC_2BPPV1_EXT = 0x8A56
        CompressedSrgbAlphaPvrtc2Bppv1Ext = 35414,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA_PVRTC_4BPPV1_EXT = 0x8A57
        CompressedSrgbAlphaPvrtc4Bppv1Ext = 35415,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA_PVRTC_2BPPV2_IMG = 0x93F0
        CompressedSrgbAlphaPvrtc2Bppv2Img = 37872,

        // Summary:
        //     Original was GL_COMPRESSED_SRGB_ALPHA_PVRTC_4BPPV2_IMG = 0x93F1
        CompressedSrgbAlphaPvrtc4Bppv2Img = 37873,
    }
}
