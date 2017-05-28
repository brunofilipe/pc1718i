using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serie1;

namespace Serie1Tests
{
    [TestClass]
    public class ThrottledRegionTest {

        private ThrottledRegion throttledRegion;

        [TestInitialize]
        public void Init() {
            throttledRegion = new ThrottledRegion(2,0,2000);
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
    }
}
