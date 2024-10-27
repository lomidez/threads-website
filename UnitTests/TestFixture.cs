using System.IO;
using NUnit.Framework;

namespace UnitTests
{
    [SetUpFixture]
    public class TestFixture
    {
        // Path to the Web Root
        public static string DataWebRootPath = "./wwwroot";

        // Path to the data folder for the content
        public static string DataContentRootPath = "./data/";

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            // Define paths
            var DataWebPath = "../../../../src/bin/Debug/net7.0/wwwroot/data"; // Adjusted path for net7.0
            var DataUTDirectory = "wwwroot";
            var DataUTPath = Path.Combine(DataUTDirectory, "data");

            // Ensure the destination directory is fresh
            if (Directory.Exists(DataUTDirectory))
            {
                Directory.Delete(DataUTDirectory, true);
            }

            // Create necessary directories
            Directory.CreateDirectory(DataUTPath);

            // Copy all files from DataWebPath to DataUTPath
            var filePaths = Directory.GetFiles(DataWebPath);
            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath); // Get just the file name
                var destFilePath = Path.Combine(DataUTPath, fileName); // Combine with destination path

                // Copy the file
                File.Copy(filePath, destFilePath, overwrite: true);
            }
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            // Optional: Clean up or log after tests complete
        }
    }
}
