using BinViewer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BinStudio.Tests
{
    [TestClass]
    public class BufferTests
    {
        private string _testFilePath;
        private VirtualFileBuffer _buffer;
        private const int TestFileSize = 1024; // 1 КБ для быстрого теста

        [TestInitialize]
        public void Setup()
        {
            // Создаем временный файл, заполненный последовательными байтами (0, 1, 2, 3...)
            _testFilePath = Path.GetTempFileName();
            byte[] dummyData = new byte[TestFileSize];
            for (int i = 0; i < TestFileSize; i++)
            {
                dummyData[i] = (byte)(i % 256);
            }
            File.WriteAllBytes(_testFilePath, dummyData);

            // Инициализируем буфер
            _buffer = new VirtualFileBuffer();
            _buffer.Open(_testFilePath);
        }

        [TestCleanup]
        public void TearDown()
        {
            // Обязательно освобождаем файл и удаляем его с диска после теста
            _buffer?.Dispose();
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TestMethod]
        public void ReadByte_ShouldReturnCorrectOriginalByte()
        {
            // Act
            byte b5 = _buffer.ReadByte(5);
            byte b10 = _buffer.ReadByte(10);

            // Assert
            Assert.AreEqual((byte)5, b5, "Буфер вернул некорректный оригинальный байт на позиции 5");
            Assert.AreEqual((byte)10, b10, "Буфер вернул некорректный оригинальный байт на позиции 10");
        }

        [TestMethod]
        public void SetByte_ShouldApplyModificationInRamWithoutChangingFile()
        {
            // Act: Меняем байт на позиции 5 во внутреннем словаре
            _buffer.SetByte(5, 0xFF);

            // Assert 1: Буфер должен отдавать измененный байт
            Assert.AreEqual((byte)0xFF, _buffer.ReadByte(5), "Буфер не вернул измененный байт из ОЗУ");

            // Assert 2: Оригинальный файл на диске НЕ должен измениться до вызова Save
            _buffer.Dispose(); // Временно закрываем поток, чтобы прочитать напрямую
            byte[] fileBytes = File.ReadAllBytes(_testFilePath);
            Assert.AreEqual((byte)5, fileBytes[5], "Файл на диске был изменен до вызова метода Save");
        }

        [TestMethod]
        public void Save_ShouldPhysicallyCommitChangesToDisk()
        {
            // Arrange
            _buffer.SetByte(10, 0xAA);

            // Act
            _buffer.Save();
            _buffer.Dispose(); // Закрываем, чтобы проверить файл

            // Assert
            byte[] fileBytes = File.ReadAllBytes(_testFilePath);
            Assert.AreEqual((byte)0xAA, fileBytes[10], "Метод Save не зафиксировал изменения на жестком диске");
        }
    }
}
