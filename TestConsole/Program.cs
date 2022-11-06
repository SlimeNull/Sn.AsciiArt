// See https://aka.ms/new-console-template for more information
using SkiaSharp;
using Sn.AsciiArt;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

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