// See https://aka.ms/new-console-template for more information
using SkiaSharp;
using Sn.AsciiArt;
using Sn.AsciiArtPlayer.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    uint err =PlatformUtils.EnableVirtualTerminalProcessingOnWindows();
    if (err != 0)
    {
        Console.WriteLine($"Failed to enable VT processing on Windows. Err code: {err}.");
        Console.ReadKey();
    }
}

string map = @"C:\Users\slime\Downloads\badapple_colored\1016.txt";
string text = File.ReadAllText(map);
string text2 = "\x1b[36;48mqwq";

for (int i = 0; i < 10; i++)
{
    Console.Write((int)text[i]);
    Console.Write(' ');
}

Console.WriteLine();

for (int i = 0; i < 10; i++)
{
    Console.Write((int)text2[i]);
    Console.Write(' ');
}

Console.WriteLine(text2);

Console.WriteLine(text);

while (true)
{
    string? qwq = Console.ReadLine();
    if (qwq == null)
        return;
    
    for (int i = 0; i < qwq.Length; i++)
    {
        Console.Write((int)qwq[i]);
        Console.Write(' ');
    }
}
return;


string path = @"C:\Users\slime\Downloads\test.png";
SKBitmap src = SKBitmap.Decode(path);

List<AsciiSkin> skinList = new List<AsciiSkin>();
foreach (ConsoleColor fore in Enum.GetValues<ConsoleColor>())
    skinList.AddRange(AsciiSkin.Create(ConsoleColor.Black, fore, AsciiSkin.DefaultCharactors));

AsciiSkin[] skins = skinList.ToArray();

Stopwatch watch = new Stopwatch();

AsciiArtGen.MultiThreading = true;
for (int i = 0; i < 100; i++)
{
    watch.Restart();
    string ascii = AsciiArtGen.GenerateColored(src, skins);
    watch.Stop();

    string awa = string.Intern(ascii);

    Console.WriteLine(awa);
    Console.WriteLine($"{watch.Elapsed.TotalMilliseconds}ms");
}

while (true)
    Console.ReadKey();