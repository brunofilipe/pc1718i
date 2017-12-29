using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ConcurrencyProgramming.serie1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.setie1Tets {
    [TestClass]
    public class ExpirableLazyTests {
        private ExpirableLazy<string> exLazy;
        private LinkedList<Exception> exList;

        [TestInitialize]
        public void init() {
            exList = new LinkedList<Exception>();
        }
        [TestMethod]
        public void TestSingleItemExpirable() {
            exLazy = new ExpirableLazy<string>(() => "single result", new TimeSpan(1, 1, 1));
            Assert.AreEqual("single result", exLazy.Value);
        }

        [TestMethod]
        public void TestAfterTimeSpan() {
            exLazy = new ExpirableLazy<string>(() => "single result", new TimeSpan(0, 0, 0));
            Assert.AreEqual("single result", exLazy.Value);
        }

        [TestMethod]
        public void TestParallelExpirableLazyWithException() {
            ExpirableLazy<String> expirableLazy = new ExpirableLazy<String>(() => {
                throw new InvalidOperationException();
            }, new TimeSpan(1000));

            Thread thread1 = new Thread(() => {
                Thread.Sleep(1000);
                try {
                    var a = expirableLazy.Value;
                }
                catch (Exception e) {
                    exList.AddLast(e);
                }
                
            });
            Thread thread2 = new Thread(() => {
                Thread.Sleep(1000);
                try{
                    var a = expirableLazy.Value;
                }
                catch (Exception e) {
                    exList.AddLast(e);
                }
            });
            Thread thread3 = new Thread(() => {
                Thread.Sleep(1000);
                try
                {
                   var a = expirableLazy.Value;
                }
                catch (Exception e)
                {
                    exList.AddLast(e);
                }
            });
            Thread thread4 = new Thread(() => {
                Thread.Sleep(1000);
                try
                {
                   var a =expirableLazy.Value;
                }
                catch (Exception e)
                {
                    exList.AddLast(e);
                }
            });

            thread1.Start();    // Will wait
            thread2.Start();    // Will wait
            thread3.Start();    // Will wait
            Trace.WriteLine("Everyone is waiting...");
            thread4.Start();    // I'm the first in queue, let's work

            thread1.Join();
            thread2.Join();
            thread3.Join();

            Assert.AreEqual(4,exList.Count);
        }

        [TestMethod]
        public void TestParallelExpirableLazyWithSuccess()
        {
            ExpirableLazy<String> expirableLazy = new ExpirableLazy<String>(() => "Ola", new TimeSpan(1000));

            Thread thread1 = new Thread(() => {
                Thread.Sleep(1000);
                try
                {
                    var a = expirableLazy.Value;
                }
                catch (Exception e)
                {
                    exList.AddLast(e);
                }

            });
            Thread thread2 = new Thread(() => {
                Thread.Sleep(1000);
                try
                {
                    var a = expirableLazy.Value;
                }
                catch (Exception e)
                {
                    exList.AddLast(e);
                }
            });
            Thread thread3 = new Thread(() => {
                Thread.Sleep(1000);
                try
                {
                    var a = expirableLazy.Value;
                }
                catch (Exception e)
                {
                    exList.AddLast(e);
                }
            });
            Thread thread4 = new Thread(() => {
                Thread.Sleep(1000);
                try
                {
                    var a = expirableLazy.Value;
                }
                catch (Exception e)
                {
                    exList.AddLast(e);
                }
            });

            thread1.Start();   
            thread2.Start();  
            thread3.Start();    
            thread1.Join();
            thread2.Join();
            thread3.Join();

            Assert.AreEqual(0, exList.Count);
        }

    }
}