// See https://aka.ms/new-console-template for more information
using SkiaSharp;
using Sn.AsciiArt;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

string path = @"C:\Users\slime\Downloads\test.png";
SKBitmap src = SKBitmap.Decode(path);

AsciiSkin[] skins = AsciiSkin.Create(
    new char[] { ' ', '.', ',', '-', '~', ':', ';', '=', '!', '*', '#', '$', '@',
                 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' });

Stopwatch watch = new Stopwatch();

for (int i = 0; i < 100; i++)
{
    watch.Restart();
    string ascii = AsciiArtGen.Generate(src, skins);
    watch.Stop();

    string awa = string.Intern(ascii);

    Console.WriteLine(awa);
    Console.WriteLine($"{watch.Elapsed.TotalMilliseconds}ms");
}

while (true)
    Console.ReadKey();