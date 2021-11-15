// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System.Runtime.InteropServices;

namespace Evergine.Assets.Extensions.KTX
{
    /// <summary>
    /// KTX header description.
    /// https://www.khronos.org/registry/OpenGL/specs/gl/glspec44.core.pdf.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KTXHeader
    {
        /// <summary>
        /// endianness contains the number 0x04030201 written as a 32 bit integer. If the file is little endian then this is represented as the bytes 0x01 0x02 0x03 0x04.
        /// If the file is big endian then this is represented as the bytes 0x04 0x03 0x02 0x01. When reading endianness as a 32 bit integer produces the value 0x04030201
        /// then the endianness of the file matches the the endianness of the program that is reading the file and no conversion is necessary. When reading endianness as
        /// a 32 bit integer produces the value 0x01020304 then the endianness of the file is opposite the endianness of the program that is reading the file, and in that
        /// case the program reading the file must endian convert all header bytes to the endianness of the program (i.e. a little endian program must convert from big endian,
        /// and a big endian program must convert to little endian).
        /// </summary>
        public uint endianness;

        /// <summary>
        /// For compressed textures, glType must equal 0. For uncompressed textures, glType specifies the type parameter passed to glTex{,Sub}Image*D, usually one of the values
        /// from table 8.2 of the OpenGL 4.4 specification [OPENGL44] (UNSIGNED_BYTE, UNSIGNED_SHORT_5_6_5, etc.)
        /// </summary>
        public uint glType;

        /// <summary>
        /// glTypeSize specifies the data type size that should be used when endianness conversion is required for the texture data stored in the file. If glType is not 0,
        /// this should be the size in bytes corresponding to glType. For texture data which does not depend on platform endianness, including compressed texture data, glTypeSize must equal 1.
        /// </summary>
        public uint glTypeSize;

        /// <summary>
        /// For compressed textures, glFormat must equal 0. For uncompressed textures, glFormat specifies the format parameter passed to glTex{,Sub}Image*D, usually one of
        /// the values from table 8.3 of the OpenGL 4.4 specification [OPENGL44] (RGB, RGBA, BGRA, etc.)
        /// </summary>
        public uint glFormat;

        /// <summary>
        /// For compressed textures, glInternalFormat must equal the compressed internal format, usually one of the values from table 8.14 of the OpenGL 4.4 specification [OPENGL44].
        /// For uncompressed textures, glInternalFormat specifies the internalformat parameter passed to glTexStorage*D or glTexImage*D, usually one of the sized internal formats from
        /// tables 8.12 &amp; 8.13 of the OpenGL 4.4 specification [OPENGL44]. The sized format should be chosen to match the bit depth of the data provided. glInternalFormat is used when
        /// loading both compressed and uncompressed textures, except when loading into a context that does not support sized formats, such as an unextended OpenGL ES 2.0 context where
        /// the internalformat parameter is required to have the same value as the format parameter.
        /// </summary>
        public uint glInternalFormat;

        /// <summary>
        /// For both compressed and uncompressed textures, glBaseInternalFormat specifies the base internal format of the texture, usually one of the values from table 8.11 of the
        /// OpenGL 4.4 specification [OPENGL44] (RGB, RGBA, ALPHA, etc.). For uncompressed textures, this value will be the same as glFormat and is used as the internalformat parameter
        /// when loading into a context that does not support sized formats, such as an unextended OpenGL ES 2.0 context.
        /// </summary>
        public uint glBaseInternalformat;

        /// <summary>
        /// The size of the texture image for level 0, in pixels. No rounding to block sizes should be applied for block compressed textures.
        /// For 1D textures pixelHeight and pixelDepth must be 0. For 2D and cube textures pixelDepth must be 0.
        /// </summary>
        public uint pixelWidth;

        /// <summary>
        /// The size of the texture image for level 0, in pixels. No rounding to block sizes should be applied for block compressed textures.
        /// For 1D textures pixelHeight and pixelDepth must be 0. For 2D and cube textures pixelDepth must be 0.
        /// </summary>
        public uint pixelHeight;

        /// <summary>
        /// The size of the texture image for level 0, in pixels. No rounding to block sizes should be applied for block compressed textures.
        /// For 1D textures pixelHeight and pixelDepth must be 0. For 2D and cube textures pixelDepth must be 0.
        /// </summary>
        public uint pixelDepth;

        /// <summary>
        /// numberOfArrayElements specifies the number of array elements. If the texture is not an array texture, numberOfArrayElements must equal 0.
        /// </summary>
        public uint numberOfArrayElements;

        /// <summary>
        /// numberOfFaces specifies the number of cubemap faces. For cubemaps and cubemap arrays this should be 6. For non cubemaps this should be 1.
        /// Cube map faces are stored in the order: +X, -X, +Y, -Y, +Z, -Z.
        /// </summary>
        public uint numberOfFaces;

        /// <summary>
        /// numberOfMipmapLevels must equal 1 for non-mipmapped textures. For mipmapped textures, it equals the number of mipmaps. Mipmaps are stored in order from largest size to smallest
        /// size. The first mipmap level is always level 0. A KTX file does not need to contain a complete mipmap pyramid. If numberOfMipmapLevels equals 0, it indicates that a full mipmap
        /// pyramid should be generated from level 0 at load time (this is usually not allowed for compressed formats).
        /// </summary>
        public uint numberOfMipmapLevels;

        /// <summary>
        /// An arbitrary number of key/value pairs may follow the header. This can be used to encode any arbitrary data. The bytesOfKeyValueData field indicates the total number of bytes
        /// of key/value data including all keyAndValueByteSize fields, all keyAndValue fields, and all valuePadding fields. The file offset of the first imageSize field is located at the
        /// file offset of the bytesOfKeyValueData field plus the value of the bytesOfKeyValueData field plus 4.
        /// </summary>
        public uint bytesOfKeyValueData;
    }
}
