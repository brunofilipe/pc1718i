using System;
using System.Collections.Generic;
using System.Threading;

namespace Serie1 {
    public class ThrottledRegion {
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
                int threadId = Thread.CurrentThread.ManagedThreadId;
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
                if (regionThread.AddToList(threadId) == -1) {
                    return false;
                }
                int time = waitTimeout;
                int lastTime = (time != Timeout.Infinite) ? Environment.TickCount : 0;
                do {
                    try {
                        Monitor.Wait(this, time);
                    }
                    catch (ThreadInterruptedException) {
                        regionThread.Remove(threadId);
                        if (regionThread.permits > 0) {
                            Monitor.PulseAll(regionThread);
                        }
                        throw;
                    }
                    if (SyncUtils.AdjustTimeout(ref lastTime, ref time) == 0) {
                        regionThread.Remove(threadId);
                        return false;
                    }
                    if (regionThread.isFull()) {
                        return false;
                    }

                } while (true);

            }
        }

        public void Leave(int key) {
            lock (this) {
                    RegionObj regionThread = regionThreads[key];
                    if (regionThread.permits < maxInside) {
                        //if the region still has permits
                        regionThread.permits++;
                        Monitor.PulseAll(this);
                        return;
                    }
                throw new Exception("The region doesn´t support more accesses");
            }
        }
    }
}