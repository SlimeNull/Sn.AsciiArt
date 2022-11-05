using SkiaSharp;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sn.AsciiArt
{
    public class AsciiArtGen
    {
        public static string Generate(SKBitmap src, char[] cs)
        {
            if (cs.Length == 0)
                throw new ArgumentException();

            AsciiSkin[] skins = AsciiSkin.Create(cs);
            int xend = src.Width / AsciiSkin.Width;
            int yend = src.Height / AsciiSkin.Height;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < yend; i++)
            {
                for (int j = 0; j < xend; j++)
                {
                    float[] similarities = new float[skins.Length];

                    for (int k = 0; k < similarities.Length; k++)
                        similarities[k] = AsciiSkin.GetSimilarity(src, skins[k], AsciiSkin.Width * j, AsciiSkin.Height * i);

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

            return sb.ToString();
        }
    }
}