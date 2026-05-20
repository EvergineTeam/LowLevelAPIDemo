// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System;
using System.IO;
using Evergine.Common.Graphics;
using Evergine.Common.Helpers;
using Evergine.Framework.Assets.Extensions;

namespace Evergine.Assets.Extensions.TexDmp
{
    /// <summary>
    /// Texture dump loader/writer.
    /// </summary>
    public class TexDmpTexture
    {
        /// <summary>
        /// Texture ID.
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Describes the texture.
        /// </summary>
        public TextureDescription TextureDescription;

        /// <summary>
        /// The texture data for each subresource.
        /// </summary>
        public TexDmpDataBox[] SubResources;

        /// <summary>
        /// The sampler state ID.
        /// </summary>
        public Guid? SamplerId;

        /// <summary>
        /// Loads the texture.
        /// </summary>
        /// <param name="graphicsContext">The graphics context.</param>
        /// <param name="writer">The binary writer.</param>
        /// <param name="texture">The KTX texture.</param>
        public static unsafe void Write(GraphicsContext graphicsContext, BinaryWriter writer, Texture texture)
        {
            // Write Id
            writer.Write(texture.Id.ToByteArray());

            var description = texture.Description;

            // Write texture description
            ImageHelpers.WriteTextureDescription(writer, description);

            // Write Sampler Id
            bool hasSampler = texture.Sampler != null;
            writer.Write(hasSampler);
            if (hasSampler)
            {
                writer.Write(texture.Sampler.Id.ToByteArray());
            }

            var stagingDescription = description;
            stagingDescription.Usage = ResourceUsage.Staging;
            stagingDescription.CpuAccess = ResourceCpuAccess.Read;
            stagingDescription.Flags = TextureFlags.None;

            var stagingTexture = graphicsContext.Factory.CreateTexture(ref stagingDescription);

            var queue = graphicsContext.Factory.CreateCommandQueue();
            var command = queue.CommandBuffer();

            command.Begin();
            command.CopyTextureDataTo(texture, stagingTexture);
            command.End();
            command.Commit();
            queue.Submit();
            queue.WaitIdle();

            uint subResource = 0;
            for (int array = 0; array < description.ArraySize; array++)
            {
                for (int mip = 0; mip < description.MipLevels; mip++)
                {
                    var map = graphicsContext.MapMemory(stagingTexture, MapMode.Read, subResource);

                    GetBytes(ref map, mip, ref description, out var bytes, out var rowPitch, out var slicePitch);

                    writer.Write(rowPitch);
                    writer.Write(slicePitch);
                    writer.Write(bytes.Length);
                    writer.Write(bytes);

                    graphicsContext.UnmapMemory(stagingTexture);
                    subResource++;
                }
            }
        }

        private static unsafe void GetBytes(ref MappedResource map, int mip, ref TextureDescription textureDescription, out byte[] bytes, out uint rowPitch, out uint slicePitch)
        {
            uint width = textureDescription.Width >> mip;
            uint height = textureDescription.Height >> mip;
            rowPitch = textureDescription.Format.GetSizeInBytes(width);
            slicePitch = textureDescription.Format.GetSizeInBytes(width, height);

            var bufferSize = slicePitch;
            bytes = new byte[bufferSize];

            Span<byte> targetSpan = new Span<byte>(bytes);
            Span<byte> sourceSpan = new Span<byte>(map.Data.ToPointer(), (int)map.SizeInBytes);

            if (map.RowPitch == rowPitch)
            {
                sourceSpan.CopyTo(targetSpan);
            }
            else
            {
                for (int row = 0; row < height; row++)
                {
                    var rowSourceSpan = sourceSpan.Slice((int)(row * map.RowPitch), (int)rowPitch);
                    var rowTargetSpan = targetSpan.Slice((int)(row * rowPitch), (int)rowPitch);
                    rowSourceSpan.CopyTo(rowTargetSpan);
                }
            }
        }

        /// <summary>
        /// Loads the texture.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <param name="onlyHeader">Indicates if only the header should be read.</param>
        /// <returns>The KTX texture.</returns>
        public static TexDmpTexture Load(BinaryReader reader, bool onlyHeader)
        {
            TexDmpTexture texture = new TexDmpTexture();

            texture.Id = new Guid(reader.ReadBytes(16));

            var description = ImageHelpers.ReadTextureDescription(reader);
            texture.TextureDescription = description;

            // Write Sampler Id
            bool hasSampler = reader.ReadBoolean();
            if (hasSampler)
            {
                texture.SamplerId = new Guid(reader.ReadBytes(16));
            }

            if (!onlyHeader)
            {
                uint nSubresources = description.MipLevels * description.ArraySize;
                texture.SubResources = new TexDmpDataBox[nSubresources];

                for (int subResource = 0; subResource < nSubresources; subResource++)
                {
                    ref TexDmpDataBox dataBox = ref texture.SubResources[subResource];

                    // Read databox
                    dataBox.RowPitch = reader.ReadUInt32();
                    dataBox.SlicePitch = reader.ReadUInt32();
                    int size = reader.ReadInt32();
                    dataBox.Data = reader.ReadBytes(size);
                }
            }

            return texture;
        }
    }
}
