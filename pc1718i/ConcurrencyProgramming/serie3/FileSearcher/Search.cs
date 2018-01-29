using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie3.FileSearcher {
    public class Search {
        public static ResultContainer ParallelGetBiggestFiles(string path, int numOfFiles, CancellationToken token, IProgress<ResultContainer> progress = null) {
            if (!Directory.Exists(path)) {
                throw new ArgumentException();
            }
            token.ThrowIfCancellationRequested();

            var result = new ResultContainer(numOfFiles);
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            result.FilesCount = files.Length;

            Parallel.ForEach(files, (filePath, loopState) => {
                if (token.IsCancellationRequested)
                    loopState.Break();

                var file = new FileInfo(filePath);
                result.TryAddFile(file);

                progress?.Report(result);
            });

            token.ThrowIfCancellationRequested();
            return result;
        }

        public static Task<ResultContainer> ParallelGetBiggestFilesAsync(string path, int numOfFiles, CancellationToken token, IProgress<ResultContainer> progress = null) {
            return Task.Factory.StartNew(() => ParallelGetBiggestFiles(path, numOfFiles, token, progress), token);
        }
    }
}