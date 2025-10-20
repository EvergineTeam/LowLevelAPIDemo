// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using Evergine.Mathematics;
using OBJRuntime.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBJRuntime.Readers
{
    /// <summary>
    /// Provides helper methods for parsing and processing data in the OBJ runtime.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Splits a string into tokens based on whitespace and trims each token.
        /// </summary>
        /// <param name="line">The input string to tokenize.</param>
        /// <returns>A list of tokens extracted from the input string.</returns>
        public static List<string> Tokenize(string line)
        {
            var tokens = new List<string>();
            var parts = line.Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                tokens.Add(p.Trim());
            }

            return tokens;
        }

        /// <summary>
        /// Attempts to parse a string into a floating-point number using invariant culture.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="val">When this method returns, contains the parsed float value if the conversion succeeded, or zero if it failed.</param>
        /// <returns><c>true</c> if the string was successfully parsed; otherwise, <c>false</c>.</returns>
        public static bool TryParseFloat(string s, out float val)
        {
            return float.TryParse(
                s,
                NumberStyles.Float | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out val);
        }

        /// <summary>
        /// Parses a subset of tokens into a <see cref="Vector3"/> starting from a specified index.
        /// </summary>
        /// <param name="tokens">The list of tokens to parse.</param>
        /// <param name="startIndex">The index in the token list to start parsing from.</param>
        /// <param name="arr">The <see cref="Vector3"/> to populate with parsed values.</param>
        /// <remarks>
        /// This method attempts to parse up to three consecutive tokens starting from <paramref name="startIndex"/>.
        /// If a token cannot be parsed, the corresponding component of <paramref name="arr"/> remains unchanged.
        /// </remarks>
        public static void ParseVector3(List<string> tokens, int startIndex, ref Vector3 arr)
        {
            // tokens: e.g. ["Kd", "0.1", "0.2", "0.3"]
            // parse from tokens[startIndex] up to 3.
            int count = Math.Min(3, tokens.Count - startIndex);
            for (int i = 0; i < count; i++)
            {
                if (Helpers.TryParseFloat(tokens[startIndex + i], out float val))
                {
                    arr[i] = val;
                }
            }
        }
    }
}
