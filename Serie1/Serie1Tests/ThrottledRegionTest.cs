using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serie1;

namespace Serie1Tests
{
    [TestClass]
    public class ThrottledRegionTest {

        private ThrottledRegion throttledRegion;

        [TestInitialize]
        public void Init() {
            throttledRegion = new ThrottledRegion(2,2,2000);
        }

        [TestMethod]
        public void TestEnterRegion() {
            bool enter = throttledRegion.TryEnter(1);
            Assert.IsTrue(enter);

        }

        [TestMethod]
        public void TestEnterFullRegion() {
            throttledRegion.TryEnter(1);
            throttledRegion.TryEnter(1);
            bool condition = throttledRegion.TryEnter(1);
            Assert.IsFalse(condition);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Collections.Generic.KeyNotFoundException))]
        public void TestLeaveFailureByNonExistingRegion() {
            throttledRegion.Leave(1);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestLeaveFailureByEmptyRegion() {
            throttledRegion.TryEnter(1);
            throttledRegion.Leave(1);
            throttledRegion.Leave(1);
        }

        [TestMethod]
        public void TestEnterFullRegionMultiThread() {
            bool[] condition = {false};
            Thread th1 = new Thread(th => {
                throttledRegion.TryEnter(1);
                throttledRegion.TryEnter(1);
            });
          
            Thread th2 = new Thread(th => {
                condition[0] = throttledRegion.TryEnter(1);

            });
            th1.Start();
            th2.Start();
            th1.Join();
            th2.Join();
            Assert.IsFalse(condition[0]);
        }

        [TestMethod]
        public void TestEnterAvailableRegionMultiThread()
        {
            bool[] condition = { false };
            Thread th1 = new Thread(th => {
                throttledRegion.TryEnter(1);
            });

            Thread th2 = new Thread(th => {
                condition[0] = throttledRegion.TryEnter(1);
            });
            th1.Start();
            th2.Start();
            th1.Join();
            th2.Join();
            Assert.IsFalse(condition[0]);
        }

    }
}
