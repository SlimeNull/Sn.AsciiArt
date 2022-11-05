using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sn.AsciiArtPlayer
{
    internal class AppOptions
    {
        [Option('s', "start", HelpText = "Start file number.")]
        public int Start { get; set; } = 1;
        
        [Option('a', "audio", HelpText = "Audio file to be played with.")]
        public string Audio { get; set; } = string.Empty;
        
        [Option('d', "duration", HelpText = "Audio file to be played with.")]
        public string Duration { get; set; } = TimeSpan.FromSeconds(10).ToString();


        [Value(0, MetaName = "ascii-folder", Required = true, HelpText = "Folder contains ascii art files.")]
        public string AsciiFolder { get; set; } = string.Empty;
    }
}
