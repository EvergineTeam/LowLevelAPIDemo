// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Evergine.Framework.Assets.Extensions;

namespace Evergine.Assets.Extensions.KTX
{
    /// <summary>
    /// https://www.khronos.org/opengles/sdk/tools/KTX/file_format_spec.
    /// </summary>
    public class KTXTexture
    {
        /// <summary>
        /// Gets the KTX Header.
        /// </summary>
        public KTXHeader Header { get; }

        /// <summary>
        /// Gets the KTX keyvalue pairs.
        /// </summary>
        public KTXKeyValuePair[] KeyValuePairs { get; }

        /// <summary>
        /// Gets the KTX mipmaps.
        /// </summary>
        public KTXMipmapLevel[] Mipmaps { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KTXTexture"/> class.
        /// </summary>
        /// <param name="header">The ktx header.</param>
        /// <param name="keyValuePairs">The key value pairs.</param>
        /// <param name="mipmaps">The mipmaps.</param>
        public KTXTexture(KTXHeader header, KTXKeyValuePair[] keyValuePairs, KTXMipmapLevel[] mipmaps)
        {
            this.Header = header;
            this.KeyValuePairs = keyValuePairs;
            this.Mipmaps = mipmaps;
        }

        /// <summary>
        /// Loads the texture.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <param name="readKeyValuePairs">The key value pairs.</param>
        /// <returns>The KTX texture.</returns>
        public static KTXTexture Load(BinaryReader reader, bool readKeyValuePairs)
        {
            KTXHeader header = ImageHelpers.ReadStruct<KTXHeader>(reader);

            KTXKeyValuePair[] kvps = null;

            if (readKeyValuePairs)
            {
                int keyValuePairBytesRead = 0;
                List<KTXKeyValuePair> keyValuePairList = new List<KTXKeyValuePair>();

                while (keyValuePairBytesRead < header.bytesOfKeyValueData)
                {
                    int bytesRemaining = (int)(header.bytesOfKeyValueData - keyValuePairBytesRead);

                    // Read KeyValue
                    uint keyValueSize = reader.ReadUInt32();

                    byte[] keyValueArray = reader.ReadBytes((int)keyValueSize);
                    int paddingByteCount = (int)(3 - ((keyValueSize + 3) % 4));
                    reader.BaseStream.Seek(paddingByteCount, SeekOrigin.Current);

                    // Find the string terminator
                    int keySize;
                    for (keySize = 0; keySize < keyValueSize; keySize++)
                    {
                        if (keyValueArray[keySize] == 0)
                        {
                            break;
                        }
                    }

                    string key = Encoding.UTF8.GetString(keyValueArray, 0, keySize);
                    int valueStart = keySize + 1;
                    byte[] value = new byte[keyValueSize - valueStart];
                    Array.Copy(keyValueArray, valueStart, value, 0, value.Length);

                    KTXKeyValuePair kvp = new KTXKeyValuePair(key, value);

                    keyValuePairBytesRead += (int)keyValueSize + paddingByteCount + sizeof(uint);
                    keyValuePairList.Add(kvp);
                }

                kvps = keyValuePairList.ToArray();
            }
            else
            {
                reader.BaseStream.Seek(header.bytesOfKeyValueData, SeekOrigin.Current);
            }

            uint numberOfMipmapLevels = Math.Max(1, header.numberOfMipmapLevels);
            uint numberOfArrayElements = Math.Max(1, header.numberOfArrayElements);
            uint numberOfFaces = Math.Max(1, header.numberOfFaces);

            uint baseWidth = Math.Max(1, header.pixelWidth);
            uint baseHeight = Math.Max(1, header.pixelHeight);
            uint baseDepth = Math.Max(1, header.pixelDepth);

            KTXMipmapLevel[] images = new KTXMipmapLevel[numberOfMipmapLevels];

            for (int mip = 0; mip < numberOfMipmapLevels; mip++)
            {
                uint mipPow = (uint)Math.Pow(2, mip);
                uint mipWidth = Math.Max(1, baseWidth / mipPow);
                uint mipHeight = Math.Max(1, baseHeight / mipPow);
                uint mipDepth = Math.Max(1, baseDepth / mipPow);

                uint imageSize = reader.ReadUInt32();
                uint arrayElementSize = imageSize / numberOfArrayElements;
                uint faceSize = (numberOfFaces > 1 && numberOfArrayElements == 1) ? arrayElementSize : arrayElementSize / numberOfFaces;

                KTXArrayElement[] arrayElements = new KTXArrayElement[numberOfArrayElements];
                for (int arrayIndex = 0; arrayIndex < numberOfArrayElements; arrayIndex++)
                {
                    KTXFace[] faces = new KTXFace[numberOfFaces];

                    for (int face = 0; face < numberOfFaces; face++)
                    {
                        faces[face] = new KTXFace(reader.ReadBytes((int)faceSize));
                    }

                    arrayElements[arrayIndex] = new KTXArrayElement(faces);
                }

                images[mip] = new KTXMipmapLevel(mipWidth, mipHeight, mipDepth, imageSize, arrayElementSize, arrayElements);

                uint mipPaddingBytes = 3 - ((imageSize + 3) % 4);
                reader.BaseStream.Seek(mipPaddingBytes, SeekOrigin.Current);
            }

            return new KTXTexture(header, kvps, images);
        }
    }
}
