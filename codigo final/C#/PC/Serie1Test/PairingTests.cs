using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using Serie1;

namespace Serie1Test
{
    [TestFixture]
    public class PairingTests
    {
        private Pairing<string, int> _pairing;
        
        [SetUp]
        public void Init()
        {
            _pairing = new Pairing<string, int>();
        }
        
        [Test]
        public void TestSimplePairing()
        {
            Tuple<string, int> tuple1 = null, tuple2 = null;

            const string _string = "test1";
            const int value = 20;
            
            var thread1 = new Thread(() => {tuple1 = _pairing.Provide(_string, 10);});
            var thread2 = new Thread(() => {tuple2 = _pairing.Provide(value, 10);});
            
            thread1.Start();
            thread2.Start();
            
            thread1.Join();
            thread2.Join();
            
            Assert.AreEqual(_string, tuple1.Item1);
            Assert.AreEqual(value, tuple1.Item2);
            Assert.AreEqual(_string, tuple2.Item1);
            Assert.AreEqual(value, tuple2.Item2);
        }

        [Test]
        public void TestPairingWithoutTimeoutInFirstThread()
        {
            Tuple<string, int> tuple1 = null, tuple2 = null;

            const string _string = "test1";
            const int value = 20;
            
            var thread1 = new Thread(() => {tuple1 = _pairing.Provide(_string, 0);});
            var thread2 = new Thread(() => {tuple2 = _pairing.Provide(value, 10);});
            
            thread1.Start();
            thread2.Start();
            
            thread1.Join();
            thread2.Join();
            
            //TimeoutException is thrown in both methods
            Assert.IsNull(tuple1);
            Assert.IsNull(tuple2);
        }
        
        [Test]
        public void TestPairingWithoutTimeoutInSecondThread()
        {
            Tuple<string, int> tuple1 = null, tuple2 = null;

            const string _string = "test1";
            const int value = 20;
            
            var thread1 = new Thread(() => {tuple1 = _pairing.Provide(_string, 10);});
            var thread2 = new Thread(() => {tuple2 = _pairing.Provide(value, 0);});
            
            thread1.Start();
            thread2.Start();
            
            thread1.Join();
            thread2.Join();
            
            Assert.AreEqual(_string, tuple1.Item1);
            Assert.AreEqual(value, tuple1.Item2);
            Assert.AreEqual(_string, tuple2.Item1);
            Assert.AreEqual(value, tuple2.Item2);
        }
        
        [Test]
        public void TestPairingWithInterruptedThread()
        {
            Tuple<string, int> tuple1 = null, tuple2 = null;

            const string _string = "test1";
            const int value = 20;
            
            var thread1 = new Thread(() => {tuple1 = _pairing.Provide(_string, 10);});
            var thread2 = new Thread(() => {tuple2 = _pairing.Provide(value, 10);});
            
            thread1.Start();
            thread2.Start();
            
            thread1.Interrupt();
            
            thread1.Join();
            thread2.Join();
            
            Assert.IsNull(tuple1);
            Assert.AreEqual(_string, tuple2.Item1);
            Assert.AreEqual(value, tuple2.Item2);
        }
        
        [Test]
        public void TestPairingWithMultipleThreadsWithSameValue()
        {
            
            //tuple3 and tuple4 should return null, because both are using same values as tuple1 and tuple2
            
            Tuple<string, int> tuple1 = null, tuple2 = null, tuple3 = null, tuple4 = null;

            const string _string = "test1";
            const int value = 20;
            
            var thread1 = new Thread(() => {tuple1 = _pairing.Provide(_string, 10);});
            var thread2 = new Thread(() => {tuple2 = _pairing.Provide(value, 10);});
            var thread3 = new Thread(() => {tuple3 = _pairing.Provide(_string, 10);});
            var thread4 = new Thread(() => {tuple4 = _pairing.Provide(value, 10);});
            
            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            
            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();
            
            Assert.AreEqual(_string, tuple1.Item1);
            Assert.AreEqual(value, tuple1.Item2);
            Assert.AreEqual(_string, tuple2.Item1);
            Assert.AreEqual(value, tuple2.Item2);
            Assert.IsNull(tuple3);
            Assert.IsNull(tuple4);
        }
        
        [Test]
        public void TestPairingWithMultipleThreadsWithDiferentValues()
        {
            Tuple<string, int> tuple1 = null, tuple2 = null, tuple3 = null, tuple4 = null;

            const string _string1 = "test1", _string2 = "test2";
            const int value1 = 20, value2 = 30;
            
            var thread1 = new Thread(() => {tuple1 = _pairing.Provide(_string1, 10);});
            var thread2 = new Thread(() => {tuple2 = _pairing.Provide(value1, 10);});
            var thread3 = new Thread(() => {tuple3 = _pairing.Provide(_string2, 10);});
            var thread4 = new Thread(() => {tuple4 = _pairing.Provide(value2, 10);});
            
            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();
            
            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();
            
            Assert.AreEqual(_string1, tuple1.Item1);
            Assert.AreEqual(value1, tuple1.Item2);
            Assert.AreEqual(_string1, tuple2.Item1);
            Assert.AreEqual(value1, tuple2.Item2);
            Assert.AreEqual(_string2, tuple3.Item1);
            Assert.AreEqual(value2, tuple3.Item2);
            Assert.AreEqual(_string2, tuple4.Item1);
            Assert.AreEqual(value2, tuple4.Item2);
        }
    }
}