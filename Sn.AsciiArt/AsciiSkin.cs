using SkiaSharp;

namespace Sn.AsciiArt
{
    public class AsciiSkin
    {
        private AsciiSkin(byte[,,] pixels, char c)
        {
            Charactor = c;
            Pixels = pixels;

            laziedLightness = new Lazy<int>(() =>
            {
                long sum = 0;
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                        sum += pixels[i, j, 0] + pixels[i, j, 1] + pixels[i, j, 2];
                sum /= Width * Height * 3;
                return (int)sum;
            });

            laziedComplexity = new Lazy<float>(() =>
            {
                HashSet<(byte r, byte g, byte b)> allColors = new HashSet<(byte r, byte g, byte b)>();
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                        allColors.Add((pixels[i, j, 0], pixels[i, j, 1], pixels[i, j, 2]));
                return (float)allColors.Count / (Width * Height);
            });
        }

        public byte[,,] Pixels { get; }

        public const int Width = 8;
        public const int Height = 14;
        public char Charactor { get; }

        private Lazy<int> laziedLightness;
        private Lazy<float> laziedComplexity;

        public int Lightness => laziedLightness.Value;
        public float Complexity => laziedComplexity.Value;

        private static AsciiSkin Create(SKPaint paint, char c)
        {
            using SKBitmap bmp = new SKBitmap(Width, Height, true);
            using SKCanvas canvas = new SKCanvas(bmp);
            
            canvas.DrawText(Convert.ToString(c), 0, 12, paint);
            byte[,,] pixels = new byte[Width,Height,3];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    SKColor color = bmp.GetPixel(i, j);
                    pixels[i, j, 0] = color.Red;
                    pixels[i, j, 1] = color.Green;
                    pixels[i, j, 2] = color.Blue;
                }
            }

            return new AsciiSkin(pixels, c);
        }

        public static AsciiSkin Create(char c)
        {
            SKFont simsun = new SKFont(SKTypeface.FromFamilyName("SimSun"), 16);
            return Create(new SKPaint(simsun), c);
        }

        public static AsciiSkin[] Create(char[] cs)
        {
            SKFont simsun = new SKFont(SKTypeface.FromFamilyName("SimSun"), 16);
            SKPaint paint = new SKPaint(simsun);

            paint.Color = new SKColor(255, 255, 255);

            AsciiSkin[] result = new AsciiSkin[cs.Length];

            for (int i = 0; i < cs.Length; i++)
                result[i] = Create(paint, cs[i]);

            return result;
        }

        public static float GetSimilarity(SKBitmap bmp, AsciiSkin skin, int x, int y)
        {
            float GetSimilarityOfColor()
            {
                float similarity = 0;
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        SKColor color = bmp.GetPixel(x + i, y + j);
                        similarity += 1 - MathF.Abs(color.Red - skin.Pixels[i, j, 0]) / 255f;
                        similarity += 1 - MathF.Abs(color.Green - skin.Pixels[i, j, 1]) / 255f;
                        similarity += 1 - MathF.Abs(color.Blue - skin.Pixels[i, j, 2]) / 255f;
                    }
                }

                similarity /= Width * Height * 3;
                return similarity;
            }

            float GetSimilarityOfLight()
            {
                long lightness = 0;
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        SKColor color = bmp.GetPixel(x + i, y + j);
                        lightness += color.Red;
                        lightness += color.Green;
                        lightness += color.Blue;
                    }
                }

                lightness /= Width * Height * 3;   // max 255
                return MathF.Abs(1 - (skin.Lightness - lightness) / 255f);
            }

            return GetSimilarityOfLight();
        }
    }
}