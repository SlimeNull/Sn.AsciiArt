using SkiaSharp;

namespace Sn.AsciiArt
{
    public static class AnsiSequence
    {
        public static int GetAnsiForegroundColor(ConsoleColor foreground)
        {
            return foreground switch
            {
                ConsoleColor.Black => 30,
                ConsoleColor.DarkBlue => 34,
                ConsoleColor.DarkGreen => 32,
                ConsoleColor.DarkCyan => 36,
                ConsoleColor.DarkRed => 31,
                ConsoleColor.DarkMagenta => 35,
                ConsoleColor.DarkYellow => 33,
                ConsoleColor.Gray => 37,
                ConsoleColor.DarkGray => 90,
                ConsoleColor.Blue => 94,
                ConsoleColor.Green => 92,
                ConsoleColor.Cyan => 96,
                ConsoleColor.Red => 91,
                ConsoleColor.Magenta => 95,
                ConsoleColor.Yellow => 93,
                ConsoleColor.White => 97,
                _ => 97,
            };
        }

        public static int GetAnsiBackgroundColor(ConsoleColor background)
        {
            return background switch
            {
                ConsoleColor.Black => 40,
                ConsoleColor.DarkBlue => 44,
                ConsoleColor.DarkGreen => 42,
                ConsoleColor.DarkCyan => 46,
                ConsoleColor.DarkRed => 41,
                ConsoleColor.DarkMagenta => 45,
                ConsoleColor.DarkYellow => 43,
                ConsoleColor.Gray => 47,
                ConsoleColor.DarkGray => 100,
                ConsoleColor.Blue => 104,
                ConsoleColor.Green => 102,
                ConsoleColor.Cyan => 106,
                ConsoleColor.Red => 101,
                ConsoleColor.Magenta => 105,
                ConsoleColor.Yellow => 103,
                ConsoleColor.White => 107,
                _ => 40,
            };
        }
        
        public static string GetAnsiSequenceStart(ConsoleColor background, ConsoleColor foreground)
        {
            int fg = GetAnsiForegroundColor(foreground);
            int bg = GetAnsiBackgroundColor(background);
            return $"\x1b[{fg};{bg}m";
        }

        public static string GetAnsiSequenceEnd()
        {
            return "\x1b[0m";
        }

        public static SKColor GetColor(ConsoleColor consoleColor)
        {
            return consoleColor switch
            {
                ConsoleColor.Black => new SKColor(12, 12, 12),
                ConsoleColor.DarkBlue => new SKColor(0, 55, 218),
                ConsoleColor.DarkGreen => new SKColor(19, 161, 14),
                ConsoleColor.DarkCyan => new SKColor(58, 150, 221),
                ConsoleColor.DarkRed => new SKColor(197, 15, 31),
                ConsoleColor.DarkMagenta => new SKColor(136, 23, 152),
                ConsoleColor.DarkYellow => new SKColor(193, 156, 0),
                ConsoleColor.Gray => new SKColor(204, 204, 204),
                ConsoleColor.DarkGray => new SKColor(118, 118, 118),
                ConsoleColor.Blue => new SKColor(59, 120, 255),
                ConsoleColor.Green => new SKColor(22, 198, 12),
                ConsoleColor.Cyan => new SKColor(97, 214, 214),
                ConsoleColor.Red => new SKColor(231, 72, 86),
                ConsoleColor.Magenta => new SKColor(180, 0, 158),
                ConsoleColor.Yellow => new SKColor(249, 241, 165),
                ConsoleColor.White => new SKColor(242, 242, 242),
                _ => throw new ArgumentException(),
            };
        }
    }
}