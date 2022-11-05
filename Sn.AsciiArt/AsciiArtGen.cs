using SkiaSharp;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Sn.AsciiArt
{
    public class AsciiArtGen
    {
        public unsafe static string Generate(SKBitmap src, AsciiSkin[] skins)
        {
            if (skins.Length == 0)
                throw new ArgumentException();
            
            int xend = src.Width / AsciiSkin.Width;
            int yend = src.Height / AsciiSkin.Height;

            IntPtr srcBytes = src.GetNormalPixelBytes(out bool unmanaged, out int srcPixelByteCount);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < yend; i++)
            {
                for (int j = 0; j < xend; j++)
                {
                    float[] similarities = new float[skins.Length];

                    for (int k = 0; k < similarities.Length; k++)
                        similarities[k] = BmpUtils.GetSimilarity(
                            (byte*)srcBytes, (byte*)skins[k].Pixels,
                            src.Width * 4, AsciiSkin.Stride,
                            AsciiSkin.Width * j, AsciiSkin.Height * i,
                            0, 0,
                            AsciiSkin.Width, AsciiSkin.Height);

                    int fitIndex = 0;
                    float similarity = similarities[0];

                    for (int k = 1; k < similarities.Length; k++)
                        if (similarities[k] > similarity)
                        {
                            fitIndex = k;
                            similarity = similarities[k];
                        }

                    sb.Append(skins[fitIndex].Charactor);
                }

                sb.AppendLine();
            }

            if (unmanaged)
                Marshal.FreeHGlobal(srcBytes);

            return sb.ToString();
        }
    }
}