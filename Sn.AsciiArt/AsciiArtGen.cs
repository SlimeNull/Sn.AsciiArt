using SkiaSharp;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Sn.AsciiArt
{
    public class AsciiArtGen
    {
        public static bool MultiThreading { get; set; } = false;
        
        public unsafe static string GenerateGray(SKBitmap src, AsciiSkin[] skins)
        {
            if (skins.Length == 0)
                throw new ArgumentException();
            
            int xend = src.Width / AsciiSkin.Width;
            int yend = src.Height / AsciiSkin.Height;

            IntPtr srcBytes = src.GetNormalPixelBytes(out bool unmanaged, out _);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < yend; i++)
            {
                for (int j = 0; j < xend; j++)
                {
                    float[] similarities = new float[skins.Length];

                    if (MultiThreading)
                    {
                        var rst = Parallel.ForEach(skins, (skin, state, index) =>
                        {
                            similarities[index] = BmpUtils.GetSimilarityForGray(
                                (byte*)srcBytes, (byte*)skins[index].Pixels,
                                src.Width * 4, AsciiSkin.Stride,
                                AsciiSkin.Width * j, AsciiSkin.Height * i,
                                0, 0,
                                AsciiSkin.Width, AsciiSkin.Height);
                        });
                    }
                    else
                    {
                        for (int k = 0; k < similarities.Length; k++)
                            similarities[k] = BmpUtils.GetSimilarityForGray(
                                (byte*)srcBytes, (byte*)skins[k].Pixels,
                                src.Width * 4, AsciiSkin.Stride,
                                AsciiSkin.Width * j, AsciiSkin.Height * i,
                                0, 0,
                                AsciiSkin.Width, AsciiSkin.Height);
                    }

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

        public unsafe static string GenerateColored(SKBitmap src, AsciiSkin[] skins)
        {
            if (skins.Length == 0)
                throw new ArgumentException();

            int xend = src.Width / AsciiSkin.Width;
            int yend = src.Height / AsciiSkin.Height;

            IntPtr srcBytes = src.GetNormalPixelBytes(out bool unmanaged, out _);

            int[,] fitIndices = new int[xend, yend];

            if (MultiThreading)
            {
                Parallel.For(0, yend, y =>
                {
                    Parallel.For(0, xend, x =>
                    {
                        float[] similarities = new float[skins.Length];

                        if (MultiThreading)
                        {
                            var rst = Parallel.ForEach(skins, (skin, state, index) =>
                            {
                                similarities[index] = BmpUtils.GetSimilarityForColored(
                                (byte*)srcBytes, (byte*)skins[index].Pixels,
                                src.Width * 4, AsciiSkin.Stride,
                                AsciiSkin.Width * x, AsciiSkin.Height * y,
                                0, 0,
                                AsciiSkin.Width, AsciiSkin.Height);
                            });
                        }
                        else
                        {
                            for (int k = 0; k < similarities.Length; k++)
                                similarities[k] = BmpUtils.GetSimilarityForColored(
                                    (byte*)srcBytes, (byte*)skins[k].Pixels,
                                    src.Width * 4, AsciiSkin.Stride,
                                    AsciiSkin.Width * x, AsciiSkin.Height * y,
                                    0, 0,
                                    AsciiSkin.Width, AsciiSkin.Height);
                        }

                        int fitIndex = 0;
                        float similarity = similarities[0];

                        for (int k = 1; k < similarities.Length; k++)
                            if (similarities[k] > similarity)
                            {
                                fitIndex = k;
                                similarity = similarities[k];
                            }

                        fitIndices[x, y] = fitIndex;
                    });
                });
            }
            else
            {
                for (int y = 0; y < yend; y++)
                {
                    for (int x = 0; x < xend; x++)
                    {
                        float[] similarities = new float[skins.Length];

                        if (MultiThreading)
                        {
                            var rst = Parallel.ForEach(skins, (skin, state, index) =>
                            {
                                similarities[index] = BmpUtils.GetSimilarityForColored(
                                (byte*)srcBytes, (byte*)skins[index].Pixels,
                                src.Width * 4, AsciiSkin.Stride,
                                AsciiSkin.Width * x, AsciiSkin.Height * y,
                                0, 0,
                                AsciiSkin.Width, AsciiSkin.Height);
                            });
                        }
                        else
                        {
                            for (int k = 0; k < similarities.Length; k++)
                                similarities[k] = BmpUtils.GetSimilarityForColored(
                                    (byte*)srcBytes, (byte*)skins[k].Pixels,
                                    src.Width * 4, AsciiSkin.Stride,
                                    AsciiSkin.Width * x, AsciiSkin.Height * y,
                                    0, 0,
                                    AsciiSkin.Width, AsciiSkin.Height);
                        }

                        int fitIndex = 0;
                        float similarity = similarities[0];

                        for (int k = 1; k < similarities.Length; k++)
                            if (similarities[k] > similarity)
                            {
                                fitIndex = k;
                                similarity = similarities[k];
                            }

                        fitIndices[x, y] = fitIndex;
                    }
                }
            }

            ConsoleColor? currentBackground = null;
            ConsoleColor? currentForeground = null;
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < yend; y++)
            {
                for (int x = 0; x < xend; x++)
                {
                    int fitIndex = fitIndices[x, y];

                    AsciiSkin fit = skins[fitIndex];
                    if (fit.Background != currentBackground || fit.Foreground != currentForeground)
                    {
                        if (currentBackground != null || currentForeground != null)
                            sb.Append(AnsiSequence.GetAnsiSequenceEnd());

                        currentBackground = fit.Background;
                        currentForeground = fit.Foreground;
                        sb.Append(AnsiSequence.GetAnsiSequenceStart(currentBackground.Value, currentForeground.Value));
                    }
                    
                    sb.Append(skins[fitIndex].Charactor);
                }

                sb.AppendLine();
            }

            if (currentBackground != null || currentForeground != null)
                sb.Append(AnsiSequence.GetAnsiSequenceEnd());

            if (unmanaged)
                Marshal.FreeHGlobal(srcBytes);

            return sb.ToString();
        }
    }
}