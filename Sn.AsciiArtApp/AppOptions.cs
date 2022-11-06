using CommandLine;
using Sn.AsciiArt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sn.AsciiArtApp
{
    enum GenerationType
    {
        Gray = 0,
        Colored = 1,
        gray = 0,
        colored = 1
    }

    internal class AppOptions
    {
        [Option('m', "multi-threading", HelpText = "Enable multithreading")]
        public bool MultiThreading { get; set; } = false;

        [Option('a', "ansi-sequence", HelpText = "Enable ANSI escape sequence")]
        public bool AnsiSequence { get; set; } = false;

        [Option('b', "background", HelpText = "Background colors (16 bit hex). default: 0")]
        public string Background { get; set; } = "0";

        [Option('f', "foreground", HelpText = "Foreground colors (16 bit hex). default: F")]
        public string Foreground { get; set; } = "F";

        [Option('c', "charactors", HelpText = "Charactors for building ascii art")]
        public string Charactors { get; set; } = new string(AsciiSkin.DefaultCharactors);

        [Option('t', "type", HelpText = "Generation type (colored or gray)")]
        public GenerationType Type { get; set; } = GenerationType.Gray;

        [Value(0, MetaName = "paths", Min = 1, HelpText = "Images to process (file or folder)")]
        public IEnumerable<string> Paths { get; set; } = Array.Empty<string>();
    }
}
