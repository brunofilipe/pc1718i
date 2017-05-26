using System.Collections.Generic;

namespace Serie1 {
     private readonly int maxInside;
        private readonly int maxWaiting;
        private readonly int waitTimeout;
        private readonly Dictionary<int, RegionObj> regionThreads;

        public ThrottledRegion(int maxInside, int maxWaiting, int waitTimeout) {
            this.maxInside = maxInside;
            this.maxWaiting = maxWaiting;
            this.waitTimeout = waitTimeout;
            regionThreads = new Dictionary<int, RegionObj>();
        }

        public bool TryEnter(int key) {
            lock (this) {
                if (!regionThreads.ContainsKey(key)) {
                    RegionObj obj = new RegionObj(maxInside, maxWaiting);
                    regionThreads.Add(key, obj);
                }
                RegionObj regionThread = regionThreads[key];
                if (regionThread.permits > 0) {
                    regionThread.permits--;
                    return true;
                }
                //if thread doesn't have permits , we must checked if it reached timeout
                if (waitTimeout == 0) {
                    return false;
                }
                int lastTime = (waitTimeout != Timeout.Infinite) ? Environment.TickCount : 0;
                do {
                    try {
                        Monitor.Wait(this, waitTimeout);
                    }catch (ThreadInterruptedException) {
                        if (regionThread.permits > 0) {
                            Monitor.PulseAll(regionThread);
                        }
                        throw;
                    }

                   
                } while (true);

            }
        }

        public void Leave(int key) {
            lock (this) {
                RegionObj regionThread = regionThreads[key];
                if (regionThread == null) {
                    throw new Exception("There is no region with this key");
                }
                if (regionThread.permits < maxInside) { //if the region still has permits
                    regionThread.permits++;
                    Monitor.PulseAll(regionThread);
                }
                throw new Exception("The region doesn´t support more accesses");
            }
        }

    }
}