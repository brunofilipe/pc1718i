using System.Collections.Generic;
using System.Threading;

namespace Serie1 {
    public class RegionObj {
        public int permits;

        private int maxWaiting;

        private readonly LinkedList<int> listOfThreads;

        public RegionObj(int permits, int maxWaiting)
        {
            listOfThreads = new LinkedList<int>();
            this.permits = permits;
            this.maxWaiting = maxWaiting;
        }

        public int AddToList(int id) {
            if (maxWaiting == 0) { //no more threads can wait
                return -1;
            }
            listOfThreads.AddLast(id);
            maxWaiting--;
            return 1;
        }

        public int Remove(int id) {
            listOfThreads.Remove(id);
            return id;
        }

        public bool isFull() {
            return listOfThreads.Count == maxWaiting;
        }


    }
}