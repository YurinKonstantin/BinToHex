using BinViewer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio.Tests
{
    [TestClass]
    public class SearchTests
    {
        private string _testFilePath;
        private VirtualFileBuffer _buffer;

        [TestInitialize]
        public void Setup()
        {
            _testFilePath = Path.GetTempFileName();
            // Создаем файл, содержащий текст и известные HEX-маркеры
            using (var fs = new FileStream(_testFilePath, FileMode.Create))
            {
                byte[] header = Encoding.ASCII.GetBytes("HEADER_START_");
                byte[] secretHex = new byte[] { 0x4D, 0x5A, 0x90, 0x00 }; // Сигнатура PE (MZ)
                byte[] footer = Encoding.ASCII.GetBytes("_FOOTER_END");

                fs.Write(header, 0, header.Length);
                fs.Write(secretHex, 0, secretHex.Length);
                fs.Write(footer, 0, footer.Length);
            }

            _buffer = new VirtualFileBuffer();
            _buffer.Open(_testFilePath);
        }

        [TestCleanup]
        public void TearDown()
        {
            _buffer?.Dispose();
            if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
        }

        [TestMethod]
        public async Task FindAllPatternsAsync_ShouldFindAsciiTextCorrectly()
        {
            // Arrange: Ищем слово "HEADER"
            byte[] pattern = Encoding.ASCII.GetBytes("HEADER");

            // Act
            List<long> results = await _buffer.FindAllPatternsAsync(pattern);

            // Assert
            Assert.AreEqual(1, results.Count, "Поисковый движок не нашел ASCII паттерн или нашел лишнее");
            Assert.AreEqual(0, results[0], "Координата найденного ASCII текста не совпадает с началом файла");
        }

        [TestMethod]
        public async Task FindAllPatternsAsync_ShouldFindHexSignatureCorrectly()
        {
            // Arrange: Ищем сигнатуру 4D 5A 90
            byte[] pattern = new byte[] { 0x4D, 0x5A, 0x90 };

            // Act
            List<long> results = await _buffer.FindAllPatternsAsync(pattern);

            // Assert
            Assert.AreEqual(1, results.Count, "Поисковый движок не обнаружил HEX-сигнатуру");
            // "HEADER_START_" — это 13 символов, значит сигнатура должна быть на 13-й позиции
            Assert.AreEqual(13, results[0], "Вычисленное смещение для HEX-сигнатуры неверно");
        }

        [TestMethod]
        public async Task FindAllPatternsAsync_ShouldReturnEmptyListIfPatternNotFound()
        {
            // Arrange: Ищем то, чего точно нет в файле
            byte[] pattern = Encoding.ASCII.GetBytes("NOT_EXIST_STRING_123");

            // Act
            List<long> results = await _buffer.FindAllPatternsAsync(pattern);

            // Assert
            Assert.AreEqual(0, results.Count, "Поиск вернул результаты для несуществующего паттерна");
        }
    }
}
