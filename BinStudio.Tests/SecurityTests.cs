using BinViewer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BinStudio.Tests
{
    [TestClass]
    public class SecurityTests
    {
        private string _testFilePath;
        private VirtualFileBuffer _buffer;

        [TestInitialize]
        public void Setup()
        {
            _testFilePath = Path.GetTempFileName();
            File.WriteAllBytes(_testFilePath, new byte[] { 1, 2, 3, 4, 5 });

            _buffer = new VirtualFileBuffer();
            _buffer.Open(_testFilePath);
        }

        [TestCleanup]
        public void TearDown()
        {
            _buffer?.Dispose();
            if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
            if (File.Exists(_testFilePath + ".bak")) File.Delete(_testFilePath + ".bak");
        }

        [TestMethod]
        public void CreateBackup_ShouldGenerateBakFileWithSameContent()
        {
            // Arrange
            string expectedBakPath = _testFilePath + ".bak";

            // Act
            _buffer.CreateBackup(_testFilePath);

            // Assert
            Assert.IsTrue(File.Exists(expectedBakPath), "Файл резервной копии .bak не был создан физически");

            byte[] originalBytes = File.ReadAllBytes(_testFilePath);
            byte[] backupBytes = File.ReadAllBytes(expectedBakPath);

            CollectionAssert.AreEqual(originalBytes, backupBytes, "Содержимое файла бэкапа отличается от оригинала");
        }
    }
}
