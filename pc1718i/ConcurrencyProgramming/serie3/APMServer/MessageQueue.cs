using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyProgramming.serie3.APMServer {
    class MessageQueue {
        class PutRequest {
            public String Msg { get; set; }
            public bool IsCompleted { get; set; }
            public PutRequest(String _msg) {
                Msg = _msg;
            }
        }
        class ReadRequest {
            public bool IsReadAvailable { get; set; }
            public String MsgToRead { get; set; }
        }
        String[] messageList;
        LinkedList<ReadRequest> readers;
        LinkedList<PutRequest> writers;
        int maxQueueSize;
        int curQueueSize;
        int readIdx;
        int writeIdx;
        bool withPutWait;
        object _lock;

        public MessageQueue(int maxSz) {
            maxQueueSize = maxSz;
            messageList = new String[maxSz];
            readers = new LinkedList<ReadRequest>();
            writers = withPutWait ? new LinkedList<PutRequest>() : null;
            _lock = new object();
        }

        public bool TryPut(String msg) {
            lock (_lock) {
                if (maxQueueSize > curQueueSize) {
                    if (readers.Count > 0) {
                        ReadRequest rq = readers.First.Value;
                        readers.RemoveFirst();
                        rq.MsgToRead = msg;
                        rq.IsReadAvailable = true;
                        Monitor.PulseAll(this);
                    } else {
                        messageList[writeIdx] = msg;
                        writeIdx = writeIdx < maxQueueSize ? writeIdx++ : 0;
                        curQueueSize++;
                    }
                    return true;
                }
                return false;
            }
        }

        public String Read() {
            lock (_lock) {
                if (curQueueSize > 0) {
                    String s = messageList[0];
                    curQueueSize--;
                    readIdx = readIdx < (maxQueueSize - 1) ? readIdx++ : 0;
                    return s;
                }
                if (withPutWait) {
                    if (writers.Count > 0) {
                        return null;
                    }
                }
                ReadRequest rq = new ReadRequest();
                readers.AddLast(rq);
                do {
                    try {
                        Monitor.Wait(_lock);
                        if (rq.IsReadAvailable) {
                            return rq.MsgToRead;
                        }
                    } catch (ThreadInterruptedException) {
                        if (rq.IsReadAvailable) {
                            Thread.CurrentThread.Interrupt();
                            return rq.MsgToRead;
                        } else {
                            readers.Remove(rq);
                            throw;
                        }
                    }
                } while (true);

            }
        }
    }
}
