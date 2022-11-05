using SkiaSharp;
using Sn.AsciiArt;
using System.Diagnostics;

if (args.Length == 0)
{
    Console.WriteLine("Usage: Sn.AsciiArtApp <path> ...");
    return;
}

Console.WriteLine("Processing...");

AsciiSkin[] skins = AsciiSkin.Create(AsciiSkin.DefaultCharactors);

foreach (string path in args)
{
    if (Directory.Exists(path))
    {
        DirectoryInfo info = new DirectoryInfo(path);
        Console.WriteLine($"Folder: {path}");
        foreach (FileInfo file in info.GetFiles())
        {
            if (!string.Equals(file.Extension, ".JPG", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(file.Extension, ".PNG", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(file.Extension, ".BMP", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            try
            {
                Console.WriteLine($"  Image {file.Name} ...");
                using SKBitmap bmp = SKBitmap.Decode(file.FullName);
                string result = AsciiArtGen.Generate(bmp, skins);
                string output = Path.ChangeExtension(file.FullName, ".txt");
                File.WriteAllText(output, result);
            }
            catch
            {
                Console.WriteLine($"  Failed {file.Name}.");
            }
        }
    }
    else if (File.Exists(path))
    {
        FileInfo info = new FileInfo(path);
        if (!string.Equals(info.Extension, ".JPG", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(info.Extension, ".PNG", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(info.Extension, ".BMP", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"  File {info.Name} is not an image file.");
            continue;
        }

        try
        {
            Console.WriteLine($"  Image {info.Name} ...");
            using SKBitmap bmp = SKBitmap.Decode(info.FullName);
            string result = AsciiArtGen.Generate(bmp, skins);
            string output = Path.ChangeExtension(info.FullName, ".txt");
            File.WriteAllText(output, result);
        }
        catch
        {
            Console.WriteLine($"  Failed {info.Name}.");
        }
    }
}