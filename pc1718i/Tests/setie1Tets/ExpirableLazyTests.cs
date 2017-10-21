using System;
using ConcurrencyProgramming.serie1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.setie1Tets {
    [TestClass]
    public class ExpirableLazyTests {
        private ExpirableLazy<string> exLazy;

        [TestMethod]
        public void TestSingleItemExpirable() {
            exLazy = new ExpirableLazy<string>(()=>"single result",new TimeSpan(1,1,1));
            Assert.AreEqual("single result",exLazy.Value);
        }

        [TestMethod]
        public void TestAfterTimeSpan() {
            exLazy = new ExpirableLazy<string>(() => "single result", new TimeSpan(0, 0, 0));
            Assert.AreEqual("single result", exLazy.Value);
        }
    }
}