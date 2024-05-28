using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NaturalAndNutritious.Tests
{
    public class LocalStorageServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly LocalStorageService _localStorageService;

        public LocalStorageServiceTests()
        {
            _envMock = new Mock<IWebHostEnvironment>();
            _envMock.Setup(env => env.WebRootPath).Returns("wwwroot");
            _localStorageService = new LocalStorageService(_envMock.Object);
        }

        [Fact]
        public async Task UploadFileAsync_UploadsFileSuccessfully()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write("Test content");
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);

            var dirPath = "testDir";

            // Act
            var result = await _localStorageService.UploadFileAsync(dirPath, fileMock.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileName, result.FileName);
            Assert.Contains(dirPath, result.FullPath);
        }

        [Fact]
        public void HasFile_FileExists_ReturnsTrue()
        {
            // Arrange
            var dirPath = "testDir";
            var fileName = "test.txt";
            var path = Path.Combine("wwwroot", "uploads", dirPath, fileName);

            Directory.CreateDirectory(Path.Combine("wwwroot", "uploads", dirPath));
            File.WriteAllText(path, "Test content");

            // Act
            var result = _localStorageService.HasFile(dirPath, fileName);

            // Assert
            Assert.True(result);

            // Cleanup
            File.Delete(path);
            Directory.Delete(Path.Combine("wwwroot", "uploads", dirPath));
        }

        [Fact]
        public async Task DeleteFileAsync_FileExists_DeletesFileSuccessfully()
        {
            // Arrange
            var dirPath = "testDir";
            var fileName = "test.txt";
            var path = Path.Combine("wwwroot", "uploads", dirPath, fileName);

            Directory.CreateDirectory(Path.Combine("wwwroot", "uploads", dirPath));
            File.WriteAllText(path, "Test content");

            // Act
            await _localStorageService.DeleteFileAsync(dirPath, fileName);

            // Assert
            Assert.False(File.Exists(path));

            // Cleanup
            Directory.Delete(Path.Combine("wwwroot", "uploads", dirPath));
        }

        [Fact]
        public async Task UploadFilesAsync_UploadsMultipleFilesSuccessfully()
        {
            // Arrange
            var fileMock1 = new Mock<IFormFile>();
            var fileMock2 = new Mock<IFormFile>();
            var fileName1 = "test1.txt";
            var fileName2 = "test2.txt";

            var ms1 = new MemoryStream();
            var writer1 = new StreamWriter(ms1);
            writer1.Write("Test content 1");
            writer1.Flush();
            ms1.Position = 0;

            var ms2 = new MemoryStream();
            var writer2 = new StreamWriter(ms2);
            writer2.Write("Test content 2");
            writer2.Flush();
            ms2.Position = 0;

            fileMock1.Setup(f => f.OpenReadStream()).Returns(ms1);
            fileMock1.Setup(f => f.FileName).Returns(fileName1);
            fileMock1.Setup(f => f.Length).Returns(ms1.Length);

            fileMock2.Setup(f => f.OpenReadStream()).Returns(ms2);
            fileMock2.Setup(f => f.FileName).Returns(fileName2);
            fileMock2.Setup(f => f.Length).Returns(ms2.Length);

            var files = new FormFileCollection { fileMock1.Object, fileMock2.Object };
            var dirPath = "testDir";

            // Act
            var result = await _localStorageService.UploadFilesAsync(dirPath, files);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, f => f.FileName == fileName1);
            Assert.Contains(result, f => f.FileName == fileName2);
        }
    }
}