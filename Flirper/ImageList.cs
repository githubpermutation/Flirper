using System;
using System.IO;
using System.Collections.Generic;
using Random = System.Random;
using ColossalFramework.IO;

namespace Flirper
{
    public class ImageList
    {
        private static readonly string filename = "FlirperImageList.txt";

        private static string pathToImageList {
            get {
                return Path.Combine (Path.Combine (DataLocation.localApplicationData, "ModConfig"), filename);
            }
        }

        public static ImageListEntry getRandomEntry ()
        {
            if (!System.IO.File.Exists (pathToImageList)) {
                createDefaultImageList ();
            }
            List<ImageListEntry> entries = new List<ImageListEntry> ();
            string[] fileEntries = System.IO.File.ReadAllLines (pathToImageList);

            foreach (string entry in fileEntries) {
                ImageListEntry imagelistentry = parse (entry);
                if (imagelistentry == null)
                    continue;

                if (imagelistentry.isDirectory) {
                    entries.AddRange (getDirectoryEntries (imagelistentry.uri));
                } else {
                    entries.Add (imagelistentry);
                }
            }

            if (entries.Count == 0)
                return null;

            Random random = new Random ();
            return entries [random.Next (entries.Count)];
        }

        static void createDefaultImageList ()
        {
            String path = Path.Combine (DataLocation.localApplicationData, "ModConfig");
            if (!Directory.Exists (path))
                Directory.CreateDirectory (path);
            
            using (System.IO.Stream inputStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Flirper.DefaultFlirperImageList.txt")) {
                using (System.IO.FileStream outputStream = new System.IO.FileStream(pathToImageList, System.IO.FileMode.Create)) {
                    for (int i = 0; i < inputStream.Length; i++) {
                        outputStream.WriteByte ((byte)inputStream.ReadByte ());
                    }
                    outputStream.Close ();
                }
            }
        }

        private static ImageListEntry parse (string entry)
        {
            string[] items = entry.Split (';');
            if (items.Length == 0 || items [0] == null || String.IsNullOrEmpty (items [0])) {
                return null;
            }
            string uri = items [0];

            string title = "";
            if (items.Length > 1) {
                title = items [1];
            }

            string author = "";
            if (items.Length > 2) {
                author = items [2];
            }

            string source = "";
            if (items.Length > 3) {
                source = items [3];
            }
            
            return new ImageListEntry (uri, title, author, source);
        }

        private static List<ImageListEntry> getDirectoryEntries (string directoryPath)
        {
            List<ImageListEntry> list = new List<ImageListEntry> ();
            DirectoryInfo dir = new DirectoryInfo (directoryPath);
            
            List<String> extensions = new List<string> ();
            extensions.Add ("*.jpg");
            extensions.Add ("*.jpeg");
            extensions.Add ("*.png");
            
            foreach (String ext in extensions) {
                foreach (FileInfo fileinfo in dir.GetFiles(ext)) {
                    String title = Path.GetFileNameWithoutExtension (@fileinfo.FullName);
                    ImageListEntry imagelistentry = new ImageListEntry (@fileinfo.FullName, title, "", "");
                    list.Add (imagelistentry);
                }
            }
            return list;
        }
    }
}