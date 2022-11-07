using CommandLine;
using NAudio.Wave;
using Sn.AsciiArtPlayer;
using Sn.AsciiArtPlayer.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

ParserResult<AppOptions> rst = Parser.Default.ParseArguments<AppOptions>(args);
rst.WithParsed(Run);
rst.WithNotParsed(err => err.Output());

void Run(AppOptions options)
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        uint err =PlatformUtils.EnableVirtualTerminalProcessingOnWindows();
        if (err != 0)
        {
            Console.WriteLine($"Failed to enable VT processing on Windows. Err code: {err}.");
            Console.ReadKey();
        }
    }

    WaveOutEvent? audio = null;

    if (!Directory.Exists(options.AsciiFolder))
    {
        Console.WriteLine("Folder is not exist");
        return;
    }
    
    Console.WriteLine($"Ascii: {options.AsciiFolder}");

    if (!string.IsNullOrWhiteSpace(options.Audio) && File.Exists(options.Audio))
    {
        Console.WriteLine("Loading audio...");
        AudioFileReader file = new AudioFileReader(options.Audio);
        audio = new WaveOutEvent();
        audio.Init(file);
        
        if (options.Duration == null)
            options.Duration = file.TotalTime;
    }

    if (options.Duration == null)
        options.Duration = TimeSpan.FromSeconds(10);

    Console.WriteLine("Checking files...");
    int end = options.Start;
    while (File.Exists(Path.Combine(options.AsciiFolder, $"{end}.txt")))
        end++;
    int count = end - options.Start;
    
    Console.WriteLine($"Reading {count} files...");

    StreamReader[] buffer = new StreamReader[count];
    for (int i = 0; i < count; i++)
        buffer[i] = new StreamReader(File.OpenRead(Path.Combine(options.AsciiFolder, $"{options.Start + i}.txt")));

    Console.Clear();

    if (audio != null)
        audio.Play();
    Stopwatch watch = new Stopwatch();
    watch.Start();

    for (int i = 0; i < count; i++)
    {
        while (watch.Elapsed < options.Duration * ((float)i / count))
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        return;
                    case ConsoleKey.Spacebar:
                        if (watch.IsRunning)
                            watch.Stop();
                        else
                            watch.Start();
                        if (audio != null)
                        {
                            if (audio.PlaybackState == PlaybackState.Playing)
                                audio.Pause();
                            else
                                audio.Play();
                        }
                        
                        break;
                }
            }
        }

        Console.SetCursorPosition(0, 0);
        Console.WriteLine(buffer[i].ReadToEnd());
    }
}