using System;
using System.Threading;
using ConcurrencyProgramming.serie3.FileSearcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.serie3Tests.FileSearcherTests {
    [TestClass]
    public class SearchTests {
        [TestMethod]
        public void TestBlockingParallelGetBiggestFiles() {
            var res = Search.ParallelGetBiggestFiles("C:\\Users\\nunob\\OneDrive\\Universidade\\Ano4\\Semestre1\\PC\\Testes", 3, new CancellationToken());
            Assert.AreEqual(res.FilesCount, 52);
            Assert.IsTrue(res.GetFiles().Contains("C:\\Users\\nunob\\OneDrive\\Universidade\\Ano4\\Semestre1\\PC\\Testes\\PC_1415i_2.pdf"));
            Assert.IsTrue(res.GetFiles().Contains("C:\\Users\\nunob\\OneDrive\\Universidade\\Ano4\\Semestre1\\PC\\Testes\\PC_1415i_EE.pdf"));
            Assert.IsTrue(res.GetFiles().Contains("C:\\Users\\nunob\\OneDrive\\Universidade\\Ano4\\Semestre1\\PC\\Testes\\PC_1314i_2.pdf"));
        }
    }
}
