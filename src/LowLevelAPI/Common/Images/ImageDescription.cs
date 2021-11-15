// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using Evergine.Common.Graphics;

namespace VisualTests.LowLevel.Images
{
    /// <summary>
    /// Describe a file image.
    /// </summary>
    public struct ImageDescription : IEquatable<ImageDescription>
    {
        /// <summary>
        /// Image file format (JPG, PNG, TGA, BMP, DDS, RTX ...).
        /// </summary>
        public ImageFormat imageFormat;

        /// <summary>
        /// Image width (in texels).
        /// </summary>
        public uint Width;

        /// <summary>
        /// Image height (in texels).
        /// </summary>
        public uint Height;

        /// <summary>
        /// Image Depth (in texels).
        /// </summary>
        public uint Depth;

        /// <summary>
        /// Image Faces.
        /// </summary>
        public uint Faces;

        /// <summary>
        /// Number of images in the image array.
        /// </summary>
        public uint ArraySize;

        /// <summary>
        /// The maximum number of mipmap levels in the image.
        /// </summary>
        public uint MipLevels;

        /// <summary>
        /// The DataBox PixelFormat.
        /// </summary>
        public PixelFormat pixelFormat;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="other">Other used to compare.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public bool Equals(ImageDescription other)
        {
            if (this.imageFormat != other.imageFormat
                || this.Width != other.Width
                || this.Height != other.Height
                || this.Depth != other.Depth
                || this.ArraySize != other.ArraySize
                || this.MipLevels != other.MipLevels
                || this.pixelFormat != other.pixelFormat)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TextureDescription && this.Equals((TextureDescription)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.imageFormat.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.Width;
                hashCode = (hashCode * 397) ^ (int)this.Height;
                hashCode = (hashCode * 397) ^ (int)this.Depth;
                hashCode = (hashCode * 397) ^ (int)this.ArraySize;
                hashCode = (hashCode * 397) ^ (int)this.MipLevels;
                hashCode = (hashCode * 397) ^ this.pixelFormat.GetHashCode();

                return hashCode;
            }
        }
    }
}
