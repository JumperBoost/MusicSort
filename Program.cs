using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TagLib.Mpeg;
using File = System.IO.File;

namespace MusicSort
{
    class Program
    {

        private static String pathInput = @"";
        private static String pathOutput = @"";
        
        private static Dictionary<AudioFile, String> audioFiles = new Dictionary<AudioFile, String>();
        
        static void Main(string[] args)
        {
            ProcessFiles(pathInput);
            ProcessDirectory(pathInput);
            
            foreach (var audioFile in audioFiles)
            {
                /*String authors = null;
                for (int i = 1; i <= audioFile.Key.Tag.AlbumArtists.Length; i++)
                {
                    authors += audioFile.Key.Tag.AlbumArtists[i - 1];
                    if (i != audioFile.Key.Tag.AlbumArtists.Length)
                    {
                        authors += ", ";
                    }
                }*/

                if(audioFile.Key.Tag.Album == null) audioFile.Key.Tag.Album = "Unknown Album";
                Directory.CreateDirectory(pathOutput + @"\" + audioFile.Key.Tag.Album.Replace(":", " -"));
                try
                {
                    File.Copy(audioFile.Key.Name,
                        pathOutput + @"\" + audioFile.Key.Tag.Album.Replace(":", " -") + @"\" + audioFile.Value);
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
            Console.WriteLine("Done!");
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