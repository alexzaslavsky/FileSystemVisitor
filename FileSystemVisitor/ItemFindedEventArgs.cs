using System;

namespace FileSystemVisitor
{
    public class ItemFindedEventArgs : EventArgs
    {
        public string Path { get; }
        public FileType ItemType { get; }

        public ItemFindedEventArgs(string path, FileType itemType)
        {
            Path = path;
            ItemType = itemType;
        }
    }
}
