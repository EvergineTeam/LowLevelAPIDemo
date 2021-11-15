// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace Evergine.Assets.Extensions.KTX
{
    /// <summary>
    /// For each array_element in numberOfArrayElements.
    /// </summary>
    public class KTXArrayElement
    {
        /// <summary>
        /// Gets the KTX faces.
        /// </summary>
        public KTXFace[] Faces { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KTXArrayElement"/> class.
        /// </summary>
        /// <param name="faces">The ktx faces.</param>
        public KTXArrayElement(KTXFace[] faces)
        {
            this.Faces = faces;
        }
    }
}
