using System;
using System.IO;

namespace FileSystemVisitor
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var fileSystemVisitor = new FileSystemVisitor(@"d:\ideas", file =>
            {
                return file.FullName.Contains(".mp3");
            });
            

            fileSystemVisitor.FileFinded += (object sender, ItemFindedEventArgs args) =>
            {
                var fileSystemVisitor = sender as FileSystemVisitor;

                if (fileSystemVisitor != null && args.Path.Contains("Samplestar"))
                {
                    fileSystemVisitor.IsItemExcluded = true;
                }
            };

            foreach (FileSystemInfo item in fileSystemVisitor)
            {
                Console.WriteLine(item.FullName);
            }
        }
    }
}
