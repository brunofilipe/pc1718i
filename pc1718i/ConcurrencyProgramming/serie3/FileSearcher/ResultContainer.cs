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
        private  ConcurrentQueue<FileInfo> queue;
        private volatile int filesCount;
        private readonly int MAX_SIZE;

        public ResultContainer(int maxSize) {
            queue = new ConcurrentQueue<FileInfo>();
            MAX_SIZE = maxSize;
        }

        public List<string> GetFiles() {
            return queue.Select(file => file.FullName).ToList();
        }

        public int GetFilesFound() {
            return filesCount;
        }

        public void TryAddFile(FileInfo file) {
            Interlocked.Increment(ref filesCount);
            //add File...
            //sorting elements in queue
            var list = queue.ToList();
            list = list.OrderBy(f => f.Length).ToList();
            queue = new ConcurrentQueue<FileInfo>(list);
            throw new NotImplementedException();
        }
    }
}
