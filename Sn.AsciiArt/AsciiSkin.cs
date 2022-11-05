using SkiaSharp;
using System.Runtime.InteropServices;

namespace Sn.AsciiArt
{
    public static class AnsiSequence
    {
        public static string GetAnsiSequenceStart(ConsoleColor foreground, ConsoleColor background)
        {
            int
                fg = (int)foreground + 30,
                bg = (int)background + 40;
            return $"\x1b[{fg};{bg}m";
        }

        public static string GetAnsiSequenceEnd()
        {
            return "\x1b[0m";
        }
    }
    
    public class AsciiSkin : IDisposable
    {
        private AsciiSkin(IntPtr pixels, char c)
        {
            Charactor = c;
            Pixels = pixels;
        }

        public IntPtr Pixels { get; }

        public const int Width = 8;
        public const int Height = 14;
        public const int Stride = Width * 4;
        public char Charactor { get; }

        public static readonly char[] DefaultCharactors = new []
        {
            ' ', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+',
            '{', '}', '|', '[', ']', '\\', ':', '"', ';', '\'', '<', '>', '?', ',', '.', '/', '`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        private static unsafe AsciiSkin Create(SKPaint paint, char c)
        {
            int pixelByteCount = Width * Height * 4;
            using SKBitmap bmp = new SKBitmap(Width, Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
            using SKCanvas canvas = new SKCanvas(bmp);
            canvas.DrawText(Convert.ToString(c), 0, 12, paint);

            byte* sp = (byte*)bmp.GetPixels();
            byte* p = (byte*)Marshal.AllocHGlobal(pixelByteCount);

            for (int i = 0; i < pixelByteCount; i++)
                p[i] = sp[i];

            return new AsciiSkin((IntPtr)p, c);
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