using System.Threading;
using NUnit.Framework;
using Serie1;

namespace Serie1Test
{
    [TestFixture]
    public class TransferQueueTests
    {
        [Test]
        public void TestTransferWithoutTake()
        {
            var trQueue = new TransferQueue<string>();
            var assert = true;

            var thread1 = new Thread(() => assert = trQueue.Transfer("test!", 10));
            thread1.Start();
            thread1.Join();
            
            Assert.IsFalse(assert);
        }

        [Test]
        public void TestTimeoutZero()
        {
            var trQueue = new TransferQueue<string>();
            var assert = true;

            var thread1 = new Thread(() => assert = trQueue.Transfer("test!", 0));
            thread1.Start();
            thread1.Join();
            
            Assert.IsFalse(assert);
        }

        [Test]
        public void TestTranferAndTakeMethodsTogether()
        {
            var trQueue = new TransferQueue<string>();
            var assert1 = false;
            var assert2 = false;
            string rmsg = null;

            var thread1 = new Thread(() => assert1 = trQueue.Transfer("test!", 10));
            var thread2 = new Thread(() => assert2 = trQueue.Take(10, out rmsg));
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();
            
            Assert.IsTrue(assert1);
            Assert.IsTrue(assert2);
            Assert.AreEqual("test!", rmsg);
        }
        
        [Test]
        public void TestTranferAndTakeMethodsTogetherWithTimeoutZero()
        {
            var trQueue = new TransferQueue<string>();
            var assert1 = true;
            var assert2 = true;
            string rmsg = null;

            var thread1 = new Thread(() => assert1 = trQueue.Transfer("test!", 0));
            var thread2 = new Thread(() => assert2 = trQueue.Take(0, out rmsg));
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();
            
            Assert.IsFalse(assert1);
            Assert.IsFalse(assert2);
            Assert.IsNull(rmsg);
        }

        [Test]
        public void TestPutAndTakeMethods()
        {
            var trQueue = new TransferQueue<string>();
            var assert1 = false;
            string rmsg = null;

            var thread1 = new Thread(() => trQueue.Put("test!"));
            var thread2 = new Thread(() => assert1 = trQueue.Take(10, out rmsg));
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();
            
            Assert.IsTrue(assert1);
            Assert.AreEqual("test!", rmsg);
        }

        [Test]
        public void TestMultipleThreadsWithoutInterrupt()
        {
            var trQueue = new TransferQueue<string>();
            var assert1 = false;
            var assert2 = false;
            var assert3 = false;
            var assert4 = false;
            string rmsg1 = null;
            string rmsg2 = null;

            var thread1 = new Thread(() => assert1 = trQueue.Transfer("test!", 10));
            var thread2 = new Thread(() => assert2 = trQueue.Transfer("test!", 10));
            var thread3 = new Thread(() => assert3 = trQueue.Take(10, out rmsg1));
            var thread4 = new Thread(() => assert4 = trQueue.Take(10, out rmsg2));
            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            
            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();
            
            Assert.IsTrue(assert1);
            Assert.IsTrue(assert2);
            Assert.IsTrue(assert3);
            Assert.IsTrue(assert4);
            Assert.AreEqual("test!", rmsg1);
            Assert.AreEqual("test!", rmsg2);
        }
    }
}