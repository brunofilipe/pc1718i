using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;

namespace ConcurrencyProgramming.serie3.FileSearcher {
    class ResultContainer {
        private readonly int MAX_SIZE;
        private object _lock = new object();
        private List<FileInfo> _queue;
        //private ConcurrentQueue<FileInfo> _queue;
        private FileInfo smallestFile;
        public int FilesCount { get; set; }
        
        public ResultContainer(int maxSize) {
            //_queue = new ConcurrentQueue<FileInfo>();
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
                    _queue.Remove(_queue[_queue.Count-1]);
                    _queue.Sort( (info, fileInfo) => (int) (file.Length - fileInfo.Length));
                   
                }
            }
            //add File...
            //sorting elements in queue
            //var list = _queue.ToList();
            //list = list.OrderBy(f => f.Length).ToList();
            //_queue = new ConcurrentQueue<FileInfo>(list);
        }
    }
}
