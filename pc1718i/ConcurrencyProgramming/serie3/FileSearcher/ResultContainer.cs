using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace ConcurrencyProgramming.serie3.FileSearcher {
    public class ResultContainer {
        private readonly int MAX_SIZE;
        private object _lock = new object();
        private List<FileInfo> _queue;
        private FileInfo smallestFile;
        public int FilesCount { get; set; }
        
        public ResultContainer(int maxSize) {
            _queue = new List<FileInfo>();
            MAX_SIZE = maxSize;
        }

        public List<string> GetFiles() {
            return _queue.Select(file => file.FullName).ToList();
        }

        public int GetFilesFound() {
            return FilesCount;
        }

        public void TryAddFile(FileInfo file) {
            lock (_lock) {
                if (_queue.Count < MAX_SIZE)
                    _queue.Add(file);
                else {
                    _queue.Sort( (info, fileInfo) => (int) (info.Length - fileInfo.Length));
                    FileInfo smallest = _queue[0];
                    if (smallest.Length < file.Length)
                    {
                        _queue.Remove(smallest);
                        _queue.Add(file);
                    }                
                }
            }
        }
    }
}
