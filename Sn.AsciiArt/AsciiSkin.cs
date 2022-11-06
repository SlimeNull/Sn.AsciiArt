using SkiaSharp;
using Sn.AsciiArt.Properties;
using System.Runtime.InteropServices;

namespace Sn.AsciiArt
{

    public class AsciiSkin : IDisposable
    {
        private AsciiSkin(IntPtr pixels, ConsoleColor back, ConsoleColor fore, char c)
        {
            Charactor = c;
            Background = back;
            Foreground = fore;

            Pixels = pixels;
        }

        public IntPtr Pixels { get; }

        public const int Width = 8;
        public const int Height = 16;
        public const int Stride = Width * 4;
        public char Charactor { get; }

        public ConsoleColor Background { get; }
        public ConsoleColor Foreground { get; }

        public static readonly SKFont FontSimSun =
            new SKFont(SKTypeface.FromStream(new MemoryStream(Resources.NSimSun)), 16);
        public static readonly SKPaint PaintSimSun =
            new SKPaint(FontSimSun)
            {
                Color = new SKColor(255, 255, 255)
            };

        public static readonly char[] DefaultCharactors = new []
        {
            ' ', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+',
            '{', '}', '|', '[', ']', '\\', ':', '"', ';', '\'', '<', '>', '?', ',', '.', '/', '`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        private static unsafe AsciiSkin Create(SKPaint paint, ConsoleColor back, ConsoleColor fore, char c)
        {
            int pixelByteCount = Width * Height * 4;
            using SKBitmap bmp = new SKBitmap(Width, Height, SKColorType.Bgra8888, SKAlphaType.Opaque);
            using SKCanvas canvas = new SKCanvas(bmp);

            canvas.Clear(AnsiSequence.GetColor(back));
            canvas.DrawText(Convert.ToString(c), 0, 14, paint);

#if DEBUG
            string GetFileName(string src)
            {
                foreach (char i in new[] { ':', '/', '\\', '*', '?', '"', '<', '>', '|' })
                    src = src.Replace(i.ToString(), $"_{Path.GetRandomFileName()}");
                return src;
            }

            using FileStream fs = File.Create($"{GetFileName(c.ToString())}_{back}_{fore}.png");
            bmp.Encode(fs, SKEncodedImageFormat.Png, 100);
#endif

            byte* sp = (byte*)bmp.GetPixels();
            byte* p = (byte*)Marshal.AllocHGlobal(pixelByteCount);

            for (int i = 0; i < pixelByteCount; i++)
                p[i] = sp[i];

            return new AsciiSkin((IntPtr)p, back, fore, c);
        }

        public static AsciiSkin Create(ConsoleColor back, ConsoleColor fore, char c)
        {
            SKPaint paintCopy = PaintSimSun.Clone();
            paintCopy.Color = AnsiSequence.GetColor(fore);

            return Create(paintCopy, back, fore, c);
        }

        public static AsciiSkin[] Create(ConsoleColor back, ConsoleColor fore, char[] cs)
        {
            SKPaint paintCopy = PaintSimSun.Clone();
            paintCopy.Color = AnsiSequence.GetColor(fore);

            AsciiSkin[] result = new AsciiSkin[cs.Length];

            for (int i = 0; i < cs.Length; i++)
                result[i] = Create(paintCopy, back, fore, cs[i]);

            return result;
        }

        public static AsciiSkin Create(char c) => Create(ConsoleColor.Black, ConsoleColor.White, c);

        public static AsciiSkin[] Create(char[] cs) => Create(ConsoleColor.Black, ConsoleColor.White, cs);


        private bool disposed = false;
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            Marshal.FreeHGlobal(Pixels);
        }
    }
}