using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public static float GetSimilarityForColored(byte* bmp1, byte* bmp2, int stride1, int stride2, int bmp1x, int bmp1y, int bmp2x, int bmp2y, int width, int height)
        {
            int total1r = 0;
            int total1g = 0;
            int total1b = 0;
            int total2r = 0;
            int total2g = 0;
            int total2b = 0;
            int totalShapeOffset = 0;

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
                    
                    total1r += r1;
                    total1g += g1;
                    total1b += b1;
                    total2r += r2;
                    total2g += g2;
                    total2b += b2;

                    totalShapeOffset += Math.Abs(r1 - r2) + Math.Abs(g1 - g2) + Math.Abs(b1 - b2);
                }
            }

            byte average1r = (byte)(total1r / (width * height));
            byte average1g = (byte)(total1g / (width * height));
            byte average1b = (byte)(total1b / (width * height));
            byte average2r = (byte)(total2r / (width * height));
            byte average2g = (byte)(total2g / (width * height));
            byte average2b = (byte)(total2b / (width * height));

            SKColor color1 = new SKColor(average1r, average1g, average1b);
            SKColor color2 = new SKColor(average2r, average2g, average2b);

            color1.ToHsv(out float h1, out float s1, out float v1);
            color2.ToHsv(out float h2, out float s2, out float v2);

            Vector3 vec1 = HsvToVector(h1, s1, v1);
            Vector3 vec2 = HsvToVector(h1, s1, v1);

            float colordiff = (vec1 - vec2).Length();
            float colorSimilarity = 1 - colordiff;
            float averageShapeOffset = totalShapeOffset / (width * height * 3) / 255f;
            float shapeSimilarity = 1 - averageShapeOffset;
            
            return (colorSimilarity + shapeSimilarity) / 2;
        }
        
        public static float GetSimilarityForGray(byte* bmp1, byte* bmp2, int stride1, int stride2, int bmp1x, int bmp1y, int bmp2x, int bmp2y, int width, int height)
        {
            int totalOffset = 0;
            int total1r = 0;
            int total1g = 0;
            int total1b = 0;
            int total2r = 0;
            int total2g = 0;
            int total2b = 0;
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
                    
                    total1r += r1;
                    total1g += g1;
                    total1b += b1;
                    total2r += r2;
                    total2g += g2;
                    total2b += b2;
                    totalOffset += Math.Abs(r1 - r2) + Math.Abs(g1 - g2) + Math.Abs(b1 - b2);
                }
            }

            int averageDiffR = Math.Abs(total1r - total2r) / (width * height);
            int averageDiffG = Math.Abs(total1g - total2g) / (width * height);
            int averageDiffB = Math.Abs(total1b - total2b) / (width * height);
            float lightnessSimilarity = 1 - (averageDiffR * 0.3f + averageDiffG * 0.59f + averageDiffB * 0.11f) / 255f;

            int averageOffset = totalOffset / (width * height * 3);
            float shapeSimilarity = 1 - averageOffset / 255f;
            return (lightnessSimilarity + shapeSimilarity) / 2;
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

        /// <summary>
        /// HSV vector in color space
        /// </summary>
        /// <param name="h">0~360</param>
        /// <param name="s">0~100</param>
        /// <param name="v">0~100</param>
        public static Vector3 HsvToVector(float h, float s, float v)
        {
            float angle = h / 180 * MathF.PI;  // normalized angle
            float _s = s / 100;
            float _v = v / 100;
            
            float x = MathF.Cos(angle) * .5f;
            float y = MathF.Sin(angle) * .5f;
            float z = _v;
            x *= _s;                         // 饱和度, 当饱和度为 0 时, 白色
            y *= _s;

            x *= _v;
            y *= _v;                         // 明度, 当明度为 0 时, 黑色

            return new Vector3(x, y, z);
        }
    }
}
