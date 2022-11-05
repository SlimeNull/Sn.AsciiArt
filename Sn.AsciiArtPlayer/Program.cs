using CommandLine;
using NAudio.Wave;
using Sn.AsciiArtPlayer;
using System.Collections.Concurrent;
using System.Diagnostics;

ParserResult<AppOptions> rst = Parser.Default.ParseArguments<AppOptions>(args);
rst.WithParsed(Run);
rst.WithNotParsed(err => err.Output());

void Run(AppOptions options)
{
    WaveOutEvent? audio = null;

    if (!Directory.Exists(options.AsciiFolder))
    {
        Console.WriteLine("Folder is not exist");
        return;
    }

    if (!string.IsNullOrWhiteSpace(options.Audio) && File.Exists(options.Audio))
    {
        AudioFileReader file = new AudioFileReader(options.Audio);
        audio = new WaveOutEvent();
        audio.Init(file);
        
        options.Duration = file.TotalTime.ToString();
    }

    if (!TimeSpan.TryParse(options.Duration, out TimeSpan span))
    {
        Console.WriteLine("Invalid duration");
        return;
    }

    int end = options.Start;
    while (File.Exists(Path.Combine(options.AsciiFolder, $"{end}.txt")))
        end++;

    int count = end - options.Start;
    string[] buffer = new string[count];
    for (int i = 0; i < count; i++)
        buffer[i] = File.ReadAllText(Path.Combine(options.AsciiFolder, $"{options.Start + i}.txt"));

    if (audio != null)
        audio.Play();

    DateTime start = DateTime.Now;
    for (int i = 0; i < count; i++)
    {
        while (DateTime.Now < start + span * ((float)i / count))
        { }

        Console.SetCursorPosition(0, 0);
        Console.WriteLine(buffer[i]);
    }
}