// See https://aka.ms/new-console-template for more information
using SkiaSharp;
using Sn.AsciiArt;

Console.WriteLine("Hello, World!");

string path = @"C:\Users\slime\Downloads\test.png";
SKBitmap src = SKBitmap.Decode(path);

string ascii = AsciiArtGen.Generate(src,
    new char[] { ' ', '.', ',', '-', '~', ':', ';', '=', '!', '*', '#', '$', '@',
                 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' });

Console.WriteLine(ascii);

while (true)
    Console.ReadKey();
