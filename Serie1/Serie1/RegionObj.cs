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
            if (isFull()) {
                return -1; //cant add to list
            }
            listOfThreads.AddLast(id);
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