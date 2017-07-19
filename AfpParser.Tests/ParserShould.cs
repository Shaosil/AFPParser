using AFPParser.StructuredFields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            file.ErrorEvent += (string msg) => { Console.WriteLine(msg); };
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
        public void DeleteFieldsSuccessfully()
        {
            // Load file
            file.LoadData(testFilePath, false);

            // Delete all presentation text containers
            List<StructuredField> textFields = file.Fields.Where(f => f.LowestLevelContainer != null 
                && f.LowestLevelContainer.Structures[0].GetType() == typeof(BPT)).ToList();
            Assert.IsTrue(textFields.Any());
            foreach (StructuredField f in textFields)
                file.DeleteField(f);

            // Make sure they are gone
            Assert.IsFalse(file.Fields.Any(f => f.LowestLevelContainer != null && f.LowestLevelContainer.Structures[0].GetType() == typeof(BPT)));
        }

        [TestMethod]
        public void AddFieldsSuccessfully()
        {
            // Load data
            file.LoadData(testFilePath, false);

            int oldCount = file.Fields.Count;
            int numNew = 0;
            List<NOP> newFields = new List<NOP>();

            // Add a bunch of NOPs to the beginning
            for (int i = 0; i < 10; i++)
            {
                NOP newNOP = StructuredField.New<NOP>();
                file.AddField(newNOP, 0);
                newFields.Add(newNOP);
                numNew++;
            }

            // Ensure they exist
            int newCount = oldCount + numNew;
            Assert.AreEqual(newCount, file.Fields.Count);
            foreach (NOP n in newFields)
                Assert.IsTrue(file.Fields.Contains(n));
        }

        [TestMethod]
        public void UpdateContainerInfo_WhenFieldIsAddedOrDeleted()
        {
            // Load a file (ignore data), check the first NOP field
            file.LoadData(testFilePath, false);
            NOP foundNOP = file.Fields.OfType<NOP>().First();

            // Get the container of this field for future assertions, and delete the field
            Container NOPContainer = foundNOP.LowestLevelContainer;
            file.DeleteField(foundNOP);

            // Ensure that no containers have the deleted structure
            foreach (Container c in file.Fields.Select(f => f.LowestLevelContainer).Distinct())
                Assert.IsFalse(c.Structures.Contains(foundNOP));

            // Create a new NOP field and insert it after the first detected field with a container
            NOP newNOP = StructuredField.New<NOP>();

            // Store the insert index
            int insertIndex = 0;
            for (int i = 0; i < file.Fields.Count; i++)
            {
                if (file.Fields[i].LowestLevelContainer != null)
                {
                    insertIndex = i + 1;
                    break;
                }
            }
            file.AddField(newNOP, insertIndex);

            // Ensure the new field has the expected container
            Assert.AreEqual(NOPContainer, newNOP.LowestLevelContainer);
        }

        [TestMethod]
        public void Validate()
        {
            // Load the sample file data
            file.LoadData(testFilePath, false);

            // Ensure it validates
            Assert.IsTrue(file.Validates());

            // Remove the first container information from all fields it contains
            Container c = file.Fields.First(f => f.LowestLevelContainer != null).LowestLevelContainer;
            foreach (DataStructure s in c.Structures)
                s.Containers.Remove(c);

            // it should no longer validate
            Assert.IsFalse(file.Validates());

            // Reload data
            file.LoadData(testFilePath, false);

            // Surround the file with BPF/EPF tags
            file.AddField(StructuredField.New<BPF>(), 0);
            file.AddField(StructuredField.New<EPF>(), file.Fields.Count);

            // Check it still validates (container info should have been updated)
            Assert.IsTrue(file.Validates());

            // Try adding a TLE field to the top of the file
            file.AddField(StructuredField.New<TLE>(), 0);

            // Ensure it does not validate
            Assert.IsFalse(file.Validates());
        }
    }
}