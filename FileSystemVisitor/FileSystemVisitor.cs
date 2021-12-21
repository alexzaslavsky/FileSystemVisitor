using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FileSystemVisitor
{
    public class FileSystemVisitor
    {
        #region Public Fields

        public bool IsItemExcluded { get; set; }
        public bool IsSearchStopped { get; set; }

        #endregion

        #region Private Fields

        private readonly string _startPath;
        private readonly Func<FileSystemInfo, bool> _filter;

        #endregion

        #region EventHandlers

        public event EventHandler<EventArgs> Start;
        public event EventHandler<EventArgs> Finish;
        public event EventHandler<ItemFindedEventArgs> FileFinded;
        public event EventHandler<ItemFindedEventArgs> DirectoryFinded;
        public event EventHandler<ItemFindedEventArgs> FilteredFileFinded;
        public event EventHandler<ItemFindedEventArgs> FilteredDirectoryFinded;

        #endregion

        #region Constructors

        public FileSystemVisitor(string path)
        {
            _startPath = path;
        }

        public FileSystemVisitor(string path, Func<FileSystemInfo, bool> filter)
        {
            _startPath = path;
            _filter = filter;
        }

        #endregion

        public IEnumerator GetEnumerator()
        {
            OnStart();
            foreach (var file in GetFileSystemInfo(_startPath))
            {
                if (IsSearchStopped)
                {
                    yield break;
                }

                CallFileFindedEventHandler(file);

                if (FindedFileVerified(file))
                {
                    yield return file;
                }
                else
                {
                    yield break;
                }
            }
            OnFinish();
        }

        public IEnumerable<FileSystemInfo> GetFileSystemInfo(string path)
        {
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                yield return file;
            }

            foreach (var dir in directory.GetDirectories())
            {
                yield return dir;

                foreach (var file in GetFileSystemInfo(dir.FullName))
                {
                    yield return file;
                }
            }
        }

        #region Protected Methods

        protected virtual void OnStart()
        {
            Start?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFinish()
        {
            Finish?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFileFinded(ItemFindedEventArgs args)
        {
            FileFinded?.Invoke(this, args);
        }

        protected virtual void OnDirectoryFinded(ItemFindedEventArgs args)
        {
            DirectoryFinded?.Invoke(this, args);
        }

        protected virtual void OnFilteredFileFinded(ItemFindedEventArgs args)
        {
            FilteredFileFinded?.Invoke(this, args);
        }

        protected virtual void OnFilteredDirectoryFinded(ItemFindedEventArgs args)
        {
            FilteredDirectoryFinded?.Invoke(this, args);
        }

        #endregion

        #region Private Methods

        private bool FindedFileVerified(FileSystemInfo file)
        {
            if (!IsItemExcluded && (_filter is null || _filter(file)))
            {
                return true;
            }
            return false;
        }

        private void CallFileFindedEventHandler(FileSystemInfo file)
        {
            var type = file.GetType() == typeof(FileInfo) ? FileType.File : FileType.Directory;
            var typeNamePart = type == FileType.File ? "File" : "Directory";
            var filteredNamePart = _filter == null ? string.Empty : "Filtered";
            var handler = GetType().GetMethod(
                $"On{filteredNamePart}{typeNamePart}Finded",
                BindingFlags.Instance | BindingFlags.NonPublic);

            handler.Invoke(this, new[]
                {
                    new ItemFindedEventArgs(file.FullName, type)
                });
        }

        #endregion
    }
}
