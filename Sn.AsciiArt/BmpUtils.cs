using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sn.AsciiArt
{
    /// <summary>
    /// Unsafe bitmap use BGRA888 format
    /// </summary>
    internal unsafe static class BmpUtils
    {
        public static float GetSimilarityOfColor(byte* bmp1, byte* bmp2, int stride1, int stride2, int bmp1x, int bmp1y, int bmp2x, int bmp2y, int width, int height)
        {
            int totalOffset = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int
                        x1 = i + bmp1x,
                        y1 = j + bmp1y,
                        x2 = i + bmp2x,
                        y2 = j + bmp2y;
                    int
                        offset1 = y1 * stride1 + x1 * 4,
                        offset2 = y2 * stride2 + x2 * 4;
                    byte
                        r1 = bmp1[offset1 + 2],
                        g1 = bmp1[offset1 + 1],
                        b1 = bmp1[offset1 + 0],
                        r2 = bmp2[offset2 + 2],
                        g2 = bmp2[offset2 + 1],
                        b2 = bmp2[offset2 + 0];
                    totalOffset += Math.Abs(r1 - r2) + Math.Abs(g1 - g2) + Math.Abs(b1 - b2);
                }
            }

            int averageOffset = totalOffset / (width * height * 3);
            float similarity = 1 - averageOffset / 255f;
            return similarity;
        }

        public static float GetSimilarityOfLightness(byte* bmp1, byte* bmp2, int stride1, int stride2, int bmp1x, int bmp1y, int bmp2x, int bmp2y, int width, int height)
        {
            int totalLightness1r = 0;
            int totalLightness1g = 0;
            int totalLightness1b = 0;
            int totalLightness2r = 0;
            int totalLightness2g = 0;
            int totalLightness2b = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int
                        x1 = bmp1x + i,
                        y1 = bmp1y + j,
                        x2 = bmp2x + i,
                        y2 = bmp2y + j;
                    int
                        offset1 = y1 * stride1 + x1 * 4,
                        offset2 = y2 * stride2 + x2 * 4;
                    byte
                        r1 = bmp1[offset1 + 2],
                        g1 = bmp1[offset1 + 1],
                        b1 = bmp1[offset1 + 0],
                        r2 = bmp2[offset2 + 2],
                        g2 = bmp2[offset2 + 1],
                        b2 = bmp2[offset2 + 0];
                    totalLightness1r += r1;
                    totalLightness1g += g1;
                    totalLightness1b += b1;
                    totalLightness2r += r2;
                    totalLightness2g += g2;
                    totalLightness2b += b2;
                }
            }

            int averageLightnessOffsetR = Math.Abs(totalLightness1r - totalLightness2r) / (width * height);
            int averageLightnessOffsetG = Math.Abs(totalLightness1g - totalLightness2g) / (width * height);
            int averageLightnessOffsetB = Math.Abs(totalLightness1b - totalLightness2b) / (width * height);
            float similarity = 1 - (averageLightnessOffsetR * 0.3f + averageLightnessOffsetG * 0.59f + averageLightnessOffsetB * 0.11f) / 255f;
            return similarity;
        }

        public static float GetSimilarity(byte* bmp1, byte* bmp2, int stride1, int stride2, int bmp1x, int bmp1y, int bmp2x, int bmp2y, int width, int height)
        {
            return (GetSimilarityOfColor(bmp1, bmp2, stride1, stride2, bmp1x, bmp1y, bmp2x, bmp2y, width, height) +
                    GetSimilarityOfLightness(bmp1, bmp2, stride1, stride2, bmp1x, bmp1y, bmp2x, bmp2y, width, height)) / 2;
        }

        public static IntPtr GetNormalPixelBytes(this SKBitmap bmp, out bool unmanaged, out int length)
        {
            int
                width = bmp.Width,
                height = bmp.Height;

            SKBitmap source = bmp;
            unmanaged = false;
            if (source.ColorType != SKColorType.Rgba8888)
            {
                SKBitmap bgra = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
                if (!bmp.CanCopyTo(SKColorType.Bgra8888))
                    throw new Exception("QAQ");
                if (!bmp.CopyTo(bgra))
                    throw new Exception("QAQ QAQ");

                source = bgra;
                unmanaged = true;
            }

            IntPtr _len;

            int pixelByteCount = width * height * 4;
            byte* sp = (byte*)bmp.GetPixels(out _len);
            byte* p = (byte*)Marshal.AllocHGlobal(pixelByteCount);

            length = (int)_len;
            for (int i = 0; i < pixelByteCount; i++)
                p[i] = sp[i];

            if (source != bmp)
                source.Dispose();

            return (IntPtr)p;
        }
    }
}
