// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System.IO;
using Evergine.Common.Graphics;

namespace VisualTests.LowLevel.Images
{
    public interface IDecoder
    {
        byte[] HeaderBytes { get; }

        int HeaderSize { get; }

        /// <summary>
        /// Decode PNG Header.
        /// </summary>
        /// <param name="reader">Image stream.</param>
        /// <param name="description">Image description.</param>
        void DecodeHeader(BinaryReader reader, out ImageDescription description);

        /// <summary>
        /// Decode data [ArraySize][MipLevels][Bytes].
        /// </summary>
        /// <param name="reader">Image stream.</param>
        /// <param name="databoxes">The databoxes of the image.</param>
        /// <param name="description">Image description.</param>
        void DecodeData(BinaryReader reader, out DataBox[] databoxes, out ImageDescription description);
    }
}
