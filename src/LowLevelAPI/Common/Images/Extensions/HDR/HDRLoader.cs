// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Evergine.Mathematics;

namespace Evergine.Assets.Extensions.HDR
{
    /// <summary>
    /// Loads an HDR image and converts it to a set of float32 RGB triplets.
    /// </summary>
    internal static class HDRLoader
    {
        [StructLayout(LayoutKind.Explicit, Size = 4)]
        private unsafe struct RGBE
        {
            [FieldOffset(0)]
            public fixed byte Data[4];

            [FieldOffset(0)]
            public byte R;

            [FieldOffset(1)]
            public byte G;

            [FieldOffset(2)]
            public byte B;

            [FieldOffset(3)]
            public byte E;
        }

        private const int R = 0;
        private const int G = 1;
        private const int B = 2;
        private const int E = 3;

        private const int MINELEN = 8;      // minimum scanline length for encoding
        private const int MAXELEN = 0x7fff; // maximum scanline length for encoding

        private const int BufferLenght = 200; // maximum scanline length for encoding

        private static float ConvertComponent(int expo, int val)
        {
            float v = val / 256.0f;
            float d = (float)Math.Pow(2, expo);
            return v * d;
        }

        private unsafe static void WorkOnRGBE(RGBE* scan, int len, Vector4* cols)
        {
            Vector4 pixel;
            while (len-- > 0)
            {
                int expo = scan->E - 128;
                pixel.X = ConvertComponent(expo, scan->R);
                pixel.Y = ConvertComponent(expo, scan->G);
                pixel.Z = ConvertComponent(expo, scan->B);
                pixel.W = 1; // Alpha :)

                *cols = pixel;
                cols++;
                scan++;
            }
        }

        private unsafe static bool Decrunch(RGBE* scanline, int len, Stream file)
        {
            int i, j;

            if (len < MINELEN || len > MAXELEN)
            {
                return OldDecrunch(scanline, len, file);
            }

            i = file.ReadByte();
            if (i != 2)
            {
                file.Seek(-1, SeekOrigin.Current);
                return OldDecrunch(scanline, len, file);
            }

            scanline->G = (byte)file.ReadByte();
            scanline->B = (byte)file.ReadByte();
            i = file.ReadByte();

            if (scanline->G != 2 || ((scanline->B & 128) != 0))
            {
                scanline->R = 2;
                scanline->E = (byte)i;
                return OldDecrunch(scanline + 1, len - 1, file);
            }

            // read each component
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < len;)
                {
                    byte code = (byte)file.ReadByte();
                    if (code > 128)
                    {
                        // run
                        code &= 127;
                        byte val = (byte)file.ReadByte();
                        while (code-- != 0)
                        {
                            scanline[j++].Data[i] = val;
                        }
                    }
                    else
                    {
                        // non-run
                        while (code-- != 0)
                        {
                            scanline[j++].Data[i] = (byte)file.ReadByte();
                        }
                    }
                }
            }

            return file.Position < file.Length;
        }

        private unsafe static bool OldDecrunch(RGBE* scanline, int len, Stream file)
        {
            int i;
            int rshift = 0;

            while (len > 0)
            {
                scanline->R = (byte)file.ReadByte();
                scanline->G = (byte)file.ReadByte();
                scanline->B = (byte)file.ReadByte();
                scanline->E = (byte)file.ReadByte();
                if (file.Position >= file.Length)
                {
                    return false;
                }

                if (scanline->R == 1 &&
                    scanline->G == 1 &&
                    scanline->B == 1)
                {
                    for (i = scanline->E << rshift; i > 0; i--)
                    {
                        scanline[0] = scanline[-1];
                        scanline++;
                        len--;
                    }

                    rshift += 8;
                }
                else
                {
                    scanline++;
                    len--;
                    rshift = 0;
                }
            }

            return true;
        }

        public static unsafe bool Load(Stream file, out HDRLoaderResult res, bool onlyHeader = false)
        {
            int i;
            byte[] str = new byte[BufferLenght];
            res = default;

            file.ReadExactly(str, 0, 10);

            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            string s = enc.GetString(str, 0, 10);
            if (s != "#?RADIANCE")
            {
                return false;
            }

            file.Seek(1, SeekOrigin.Current);

            byte[] cmd = new byte[BufferLenght];
            i = 0;
            byte c = 0, oldc;
            while (true)
            {
                oldc = c;
                c = (byte)file.ReadByte();
                if (c == 0xa && oldc == 0xa)
                {
                    break;
                }

                cmd[i++] = c;
            }

            byte[] reso = new byte[BufferLenght];
            i = 0;
            while (true)
            {
                c = (byte)file.ReadByte();
                reso[i++] = c;
                if (c == 0xa)
                {
                    break;
                }
            }

            var resolutionStr = enc.GetString(reso, 0, 200);
            var match = Regex.Matches(resolutionStr, @"\d+");
            int w, h;
            if (match.Count == 0
                || !int.TryParse(match[1].Value, out w)
                || !int.TryParse(match[0].Value, out h))
            {
                return false;
            }

            res.Width = w;
            res.Height = h;

            if (onlyHeader)
            {
                return true;
            }

            int nfloats = w * h * 4;
            byte[] data = new byte[nfloats * 4];
            fixed (void* dataPtr = data)
            {
                res.Data = data;

                Vector4* cols = (Vector4*)dataPtr;
                var scanlinePtr = Marshal.AllocHGlobal(w * sizeof(RGBE));
                RGBE* scanline = (RGBE*)scanlinePtr.ToPointer();

                // convert image.
                for (int y = h - 1; y >= 0; y--)
                {
                    if (!Decrunch(scanline, w, file))
                    {
                        break;
                    }

                    WorkOnRGBE(scanline, w, cols);
                    cols += w;
                }
            }

            return true;
        }
    }
}
