using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AFPParser.Tests
{
    [TestClass]
    public class ParserShould
    {
        private AFPFile file = null;
        private static string testFilePath = string.Empty;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            testFilePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\Sample Files\Sample 1.afp");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            file = new AFPFile();
        }

        [TestMethod]
        public void DecodeSuccessfully_WithoutParsingData()
        {
            // Load an AFP file without exceptions
            Assert.IsTrue(file.LoadData(testFilePath, false));
            Assert.IsNotNull(file.Fields);
            Assert.AreNotEqual(0, file.Fields.Count);
        }

        [TestMethod]
        public void DecodeAndParseDataSuccessfully()
        {
            // Load an AFP file, and parse its data into custom properties and objects without exceptions
            Assert.IsTrue(file.LoadData(testFilePath, true));
            Assert.IsNotNull(file.Fields);
            Assert.AreNotEqual(0, file.Fields.Count);
        }

        [TestMethod]
        public void DecodeAndParseData_ThenReencode()
        {
            byte[] rawFile = File.ReadAllBytes(testFilePath);

            // Load an AFP file, parse its data, and save it to an in-memory byte stream
            Assert.IsTrue(file.LoadData(testFilePath, true));
            Assert.IsNotNull(file.Fields);
            Assert.AreNotEqual(0, file.Fields.Count);

            // Compare original raw bytes with reencoded byte stream - they must be identical
            byte[] encoded = file.EncodeData();
            Assert.IsTrue(rawFile.SequenceEqual(encoded));
        }

        [TestMethod]
        public void UpdateContainerInfo_WhenFieldIsAddedOrDeleted()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void DeleteFieldsSuccessfully()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void AddFieldsSuccessfully()
        {
            throw new NotImplementedException();
        }
    }
}