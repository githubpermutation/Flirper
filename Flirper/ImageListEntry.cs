using System;
using System.IO;

namespace Flirper
{
    public class ImageListEntry
    {
        public string uri;
        public string title;
        public string author;
        public string source;

        public ImageListEntry (string uri, string title, string author, string source)
        {
            this.uri = uri;
            this.title = title;
            this.author = author;
            this.source = source;
        }
        
        public bool isHTTP {
            get {
                return this.uri.ToLower ().StartsWith ("http:") || this.uri.ToLower ().StartsWith ("https:");
            }
        }

        public bool isDirectory {
            get {
                if (isHTTP) 
                    return false;

                FileAttributes attr = System.IO.File.GetAttributes (this.uri);
                return (attr & FileAttributes.Directory) == FileAttributes.Directory;
            }
        }
    }
}