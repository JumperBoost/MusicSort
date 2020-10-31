using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagLib.Mpeg;
using File = System.IO.File;

namespace MusicSort
{
    class Program
    {

        private static String pathInput = @"";
        private static String pathOutput = @"";
        
        private static Dictionary<AudioFile, String> audioFiles = new Dictionary<AudioFile, String>();
        private static String sortType;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Starting MusicSort...");
            Console.WriteLine("(1/3) Initializing...");
            ProcessFiles(pathInput);
            ProcessDirectory(pathInput);
            
            Console.WriteLine("(2/3) Start of sorting...");
            foreach (var audioFile in audioFiles)
            {
                if(audioFile.Key.Tag.Album == null) audioFile.Key.Tag.Album = "Unknown Album";
                if(audioFile.Key.Tag.Artists.Length == 0) audioFile.Key.Tag.Artists = new []{"Unknown Artist"};
                if(audioFile.Key.Tag.AlbumArtists.Length == 0) audioFile.Key.Tag.AlbumArtists = new []{"Unknown Artist"};
                
                SortByAlbum(audioFile);
                //SortByArtist(audioFile);
            }
            Console.WriteLine("(3/3) Verification...");
            List<String> artist = new List<string>();
            switch (sortType)
            {
                case "album":
                    foreach (var directory in Directory.GetDirectories(pathOutput))
                    {
                        artist.Clear();
                        foreach (var file in Directory.GetFiles(directory, "*.mp3"))
                        {
                            try
                            {
                                AudioFile audioFile = new AudioFile(file);
                                if (audioFile.Tag.AlbumArtists.Length == 0)
                                {
                                    if (audioFile.Tag.Artists.Length != 0 && (artist.Contains(audioFile.Tag.Artists[0]) || artist.Count == 0))
                                    {
                                        artist.Add(audioFile.Tag.Artists[0]);
                                    }
                                }
                                else if (audioFile.Tag.AlbumArtists.Length != 0 && (artist.Contains(audioFile.Tag.AlbumArtists[0]) || artist.Count == 0))
                                {
                                    artist.Add(audioFile.Tag.AlbumArtists[0]);
                                }
                                else break;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("An occured error with file (" + file + ")!");
                            }
                        }

                        if (artist.Count != 0)
                        {
                            if (artist[0].ToLower() != directory.Split(@"\").Last().ToLower())
                            {
                                String newDirectory = directory.Replace(directory.Split(@"\").Last(), artist[0] + " - " + directory.Split(@"\").Last());
                                Directory.Move(directory, newDirectory);
                            }
                        }
                    }
                    artist.Clear();
                    break;
            }
            Console.WriteLine("Done!");
        }

        static void SortByAlbum(KeyValuePair<AudioFile, String> audioFile)
        {
            try
            {
                sortType = "album";
                if (audioFile.Key.Tag.Album == "Unknown Album")
                {
                    Directory.CreateDirectory(pathOutput + @"\" + audioFile.Key.Tag.Album.Replace(":", " -") + @"\" + audioFile.Key.Tag.AlbumArtists[0]);
                    File.Copy(audioFile.Key.Name,
                        pathOutput + @"\" + audioFile.Key.Tag.Album.Replace(":", " -") + @"\" + audioFile.Key.Tag.AlbumArtists[0] + @"\" + audioFile.Value);
                }
                else
                {
                    Directory.CreateDirectory(pathOutput + @"\" + audioFile.Key.Tag.Album.Replace(":", " -"));
                    File.Copy(audioFile.Key.Name,
                        pathOutput + @"\" + audioFile.Key.Tag.Album.Replace(":", " -") + @"\" + audioFile.Value);
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to find file (" + audioFile.Key.Name + ")!");
            }
            catch (Exception)
            {
                Console.WriteLine("An occured error with file (" + audioFile.Key.Name + ")!");
            }
        }

        static void SortByArtist(KeyValuePair<AudioFile, String> audioFile)
        {
            try
            {
                sortType = "artist";
                if (audioFile.Key.Tag.Artists[0] != "Unknown Artist")
                {
                    Directory.CreateDirectory(pathOutput + @"\" + audioFile.Key.Tag.Artists[0] + @"\" + audioFile.Key.Tag.Album.Replace(":", " -"));
                    File.Copy(audioFile.Key.Name,
                        pathOutput + @"\" + audioFile.Key.Tag.Artists[0] + @"\" + audioFile.Key.Tag.Album.Replace(":", " -") + @"\" + audioFile.Value);
                }
                else
                {
                    if (audioFile.Key.Tag.AlbumArtists[0].Length != 0)
                    {
                        Directory.CreateDirectory(pathOutput + @"\" + audioFile.Key.Tag.AlbumArtists[0] + @"\" + audioFile.Key.Tag.Album.Replace(":", " -"));
                        File.Copy(audioFile.Key.Name,
                            pathOutput + @"\" + audioFile.Key.Tag.AlbumArtists[0] + @"\" + audioFile.Key.Tag.Album.Replace(":", " -") + @"\" + audioFile.Value);
                    }
                    else
                    {
                        Directory.CreateDirectory(pathOutput + @"\" + audioFile.Key.Tag.Artists[0] + @"\" + audioFile.Key.Tag.Album.Replace(":", " -"));
                        File.Copy(audioFile.Key.Name,
                            pathOutput + @"\" + audioFile.Key.Tag.Artists[0] + @"\" + audioFile.Key.Tag.Album.Replace(":", " -") + @"\" + audioFile.Value);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Unable to find file (" + audioFile.Key.Name + ")!");
            }
            catch (Exception)
            {
                Console.WriteLine("An occured error with file (" + audioFile.Key.Name + ")!");
            }
        }

        static void ProcessDirectory(String path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                foreach (var subDirectory in Directory.GetDirectories(directory))
                {
                    ProcessDirectory(subDirectory);
                    ProcessFiles(subDirectory);
                }
                
                foreach (var file in Directory.GetFiles(directory, "*.mp3"))
                {
                    try
                    {
                        audioFiles.Add(new AudioFile(file), file.Split(@"\").Last());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("An occured error with file (" + file + ")!");
                    }
                }
            }
        }

        static void ProcessFiles(String path)
        {
            foreach (var file in Directory.GetFiles(path, "*.mp3"))
            {
                try
                {
                    audioFiles.Add(new AudioFile(file), file.Split(@"\").Last());
                }
                catch (Exception)
                {
                    Console.WriteLine("An occured error with file (" + file + ")!");
                }
            }
        }
    }
}