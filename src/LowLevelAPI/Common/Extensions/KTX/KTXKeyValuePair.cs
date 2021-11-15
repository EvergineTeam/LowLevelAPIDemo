// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace Evergine.Assets.Extensions.KTX
{
    /// <summary>
    /// The KTX key value pair class.
    /// </summary>
    public class KTXKeyValuePair
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the data value.
        /// </summary>
        public byte[] Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KTXKeyValuePair"/> class.
        /// </summary>
        /// <param name="key">The pair key.</param>
        /// <param name="value">The pair value.</param>
        public KTXKeyValuePair(string key, byte[] value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
