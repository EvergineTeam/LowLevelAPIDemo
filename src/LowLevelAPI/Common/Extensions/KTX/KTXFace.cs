// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace Evergine.Assets.Extensions.KTX
{
    /// <summary>
    /// For each face in numberOfFaces.
    /// </summary>
    public class KTXFace
    {
        /// <summary>
        /// Gets the ktx face data.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KTXFace"/> class.
        /// </summary>
        /// <param name="data">The ktx face data.</param>
        public KTXFace(byte[] data)
        {
            this.Data = data;
        }
    }
}
