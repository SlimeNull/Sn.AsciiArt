using CommandLine;
using SkiaSharp;
using Sn.AsciiArt;
using Sn.AsciiArtApp;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

ParserResult<AppOptions> rst = Parser.Default.ParseArguments<AppOptions>(args);
rst.WithParsed(Run);

void Run(AppOptions options)
{
    Console.WriteLine("Processing...");

    if (options.MultiThreading)
        Console.WriteLine("Multithreading enabled.");
    if (options.AnsiSequence)
        Console.WriteLine("ANSI escape sequence enabled.");

    List<AsciiSkin> skinlist = new List<AsciiSkin>();
    List<ConsoleColor?> foregrounds = options.Foreground.Select(hex => Hex2ConsoleColor(hex)).ToList();
    List<ConsoleColor?> backgrounds = options.Background.Select(hex => Hex2ConsoleColor(hex)).ToList();
    
    if (foregrounds.Any(c => !c.HasValue))
    {
        Console.WriteLine("Invalid foreground colors.");
        return;
    }
    if (backgrounds.Any(c => !c.HasValue))
    {
        Console.WriteLine("Invalid background colors.");
        return;
    }

    char[] chars = options.Charactors.ToCharArray();
    foreach (ConsoleColor? background in backgrounds)
        foreach (ConsoleColor? foreground in foregrounds)
            skinlist.AddRange(AsciiSkin.Create(background!.Value, foreground!.Value, chars));

    AsciiArtGen.MultiThreading = options.MultiThreading;
    AsciiSkin[] skins = skinlist.ToArray();

    List<FileInfo> files = new List<FileInfo>();
    foreach (string path in options.Paths)
    {
        if (Directory.Exists(path))
        {
            files.AddRange(new DirectoryInfo(path).GetFiles().Where(file => IsImageFile(file)));
        }
        else if (File.Exists(path))
        {
            FileInfo info = new FileInfo(path);
            if (IsImageFile(info))
                files.Add(info);
        }
    }

    if (options.MultiThreading)
    {
        int finished = 0;
        Parallel.ForEach(files, (file, state, index) =>
        {
            try
            {
                Console.WriteLine($" Processing {file.FullName}; {finished}/{files.Count}");
                using SKBitmap src = SKBitmap.Decode(file.FullName);
                string result = options.AnsiSequence ?
                    AsciiArtGen.GenerateColored(src, skins) :
                    AsciiArtGen.GenerateGray(src, skins);

                string output = Path.ChangeExtension(file.FullName, ".txt");
                File.WriteAllText(output, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed {file.FullName}; {ex.GetType().Name}");
            }
            finally
            {
                finished++;
            }
        });
    }
    else
    {
        for (int i = 0; i < files.Count; i++)
        {
            FileInfo file = files[i];
            try
            {
                Console.WriteLine($" Processing {file.FullName}; {i}/{files.Count}");
                using SKBitmap src = SKBitmap.Decode(file.FullName);
                string result = options.AnsiSequence ?
                    AsciiArtGen.GenerateColored(src, skins) :
                    AsciiArtGen.GenerateGray(src, skins);

                string output = Path.ChangeExtension(file.FullName, ".txt");
                File.WriteAllText(output, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed {file.FullName}; {ex.GetType().Name}");
            }
        }
    }
}

ConsoleColor? Hex2ConsoleColor(char c)
{
    return char.ToUpper(c) switch
    {
        '0' => ConsoleColor.Black,
        '1' => ConsoleColor.DarkBlue,
        '2' => ConsoleColor.DarkGreen,
        '3' => ConsoleColor.DarkCyan,
        '4' => ConsoleColor.DarkRed,
        '5' => ConsoleColor.DarkMagenta,
        '6' => ConsoleColor.DarkYellow,
        '7' => ConsoleColor.Gray,
        '8' => ConsoleColor.DarkGray,
        '9' => ConsoleColor.Blue,
        'A' => ConsoleColor.Green,
        'B' => ConsoleColor.Cyan,
        'C' => ConsoleColor.Red,
        'D' => ConsoleColor.Magenta,
        'E' => ConsoleColor.Yellow,
        'F' => ConsoleColor.White,
        _ => null
    };
}

bool IsImageFile(FileInfo info)
{
    return
        info.Extension.Equals(".JPG", StringComparison.OrdinalIgnoreCase) ||
        info.Extension.Equals(".PNG", StringComparison.OrdinalIgnoreCase) ||
        info.Extension.Equals(".BMP", StringComparison.OrdinalIgnoreCase);
}