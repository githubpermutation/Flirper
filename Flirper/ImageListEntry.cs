using System;
using System.IO;

namespace Flirper
{
    public class ImageListEntry
    {
        public string uri;
        public string title;
        public string author;
        public string extraInfo;

        public static string fieldSeparator {
            get {
                return ";";
            }
        }

        public ImageListEntry (string uri, string title, string author, string extraInfo)
        {
            this.uri = uri;
            this.title = title;
            this.author = author;
            this.extraInfo = extraInfo;
        }
        
        public bool isHTTP {
            get {
                return this.uri.ToLower ().StartsWith ("http:") || this.uri.ToLower ().StartsWith ("https:");
            }
        }

        public bool isFile {
            get {
                return isLocal && !System.IO.Directory.Exists(@uri) && System.IO.File.Exists(@uri);
            }
        }

        public bool isDirectory {
            get {
                if (!isLocal || isFile) 
                    return false;

                FileAttributes attr = System.IO.File.GetAttributes (@uri);
                return (attr & FileAttributes.Directory) == FileAttributes.Directory;
            }
        }

        public bool isLatestSaveGame {
            get {
                return this.uri.ToLower ().StartsWith ("latestsavegame");
            }
        }

        private bool isLocal {
            get {
                return !(isHTTP || isLatestSaveGame);
            }
        }

        public bool isValidPath {
            get {
                if(!isLocal)
                    return true;

                try {
                    FileInfo fi = new System.IO.FileInfo(@uri);
                    FileAttributes attr = System.IO.File.GetAttributes (@uri);
                } catch (Exception ex) {
                    ex.ToString();
                    return false;
                }
                return true;
            }
        }

        public string asFileEntry {
            get {
                return string.Format ("{0}{1}{2}{1}{3}{1}{4}{1}{5}", uri, fieldSeparator, title, author, extraInfo, Environment.NewLine);
            }
        }

        public override bool Equals (object other)
        {
            if(!(other is ImageListEntry))
                return false;

            return this.asFileEntry.Equals(((ImageListEntry)other).asFileEntry);
        }
    }
}