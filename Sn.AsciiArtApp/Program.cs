using SkiaSharp;
using Sn.AsciiArt;
using System.Diagnostics;

if (args.Length == 0)
{
    Console.WriteLine("Usage: TestConsole.exe <image files folder> ...");
    return;
}

Console.WriteLine("Processing...");

AsciiSkin[] skins = AsciiSkin.Create(AsciiSkin.DefaultCharactors);

foreach (string folder in args)
{
    DirectoryInfo info = new DirectoryInfo(folder);
    if (!info.Exists)
    {
        Console.WriteLine($"{folder} is not exist");
        continue;
    }

    Console.WriteLine($"Folder: {folder}");
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