using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using serie2;

namespace Serie2Test {
    
    [TestFixture]
    public class ExpirableLazyTests {
        
        private ExpirableLazy<string> _expirableLazy;
        private LinkedList<Exception> _exceptionList;

        private void AddExceptionToList()
        {
            try
            {
                Assert.AreEqual("test!", _expirableLazy.Value);
            }
            catch (Exception e)
            {
                _exceptionList.AddLast(e);
            }
        }
        
        [SetUp]
        public void Init()
        {
            _exceptionList = new LinkedList<Exception>();
        }
        
        [Test]
        public void TestOneTimeRun()
        {
            _expirableLazy = new ExpirableLazy<string>(() => "test!", new TimeSpan(10));

            Assert.AreEqual("test!", _expirableLazy.Value);
        }

        [Test]
        public void TestProviderThrowsException()
        {
            _expirableLazy = new ExpirableLazy<string>(() => {throw new Exception("test!");}, new TimeSpan(10));

            AddExceptionToList();

            Assert.AreEqual(1, _exceptionList.Count);
        }
        
        [Test]
        public void TestMoreThanOneThreadWithExceptions()
        {
            var numberOfFails = 3;    //number of times that will throw exception
            
            _expirableLazy = new ExpirableLazy<string>(() =>
            {
                if (0 < numberOfFails--) throw new Exception("Exception!");
                return "test!";
            }, new TimeSpan(10));

            var threads = new List<Thread>(10);
            
            for (var i = 0; i < threads.Capacity; i++)
                threads.Add(new Thread(AddExceptionToList));
            
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
            
            Assert.AreEqual(3, _exceptionList.Count);
        }
        
        [Test]
        public void TestMoreThanOneThreadWithSucess()
        {
            _expirableLazy = new ExpirableLazy<string>(() => "test!", new TimeSpan(10));

            var threads = new List<Thread>(10);
            
            for (var i = 0; i < threads.Capacity; i++)
                threads.Add(new Thread(AddExceptionToList));
            
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
            
            Assert.AreEqual(0, _exceptionList.Count);
        }
    }
}