using MazeSolver.Filesystem;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver.Tests
{
    public class ArgumentParserTests
    {
        ILogger<ArgumentParser> _logger;
        IFileSystem _fileSystem;
        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<ArgumentParser>>();
            _fileSystem = Substitute.For<IFileSystem>();
        }

        private (IFileData, IFileData) SetupHappyPathFileData()
        {
            var sourceFileData = Substitute.For<IFileData>();
            var destFileData = Substitute.For<IFileData>();
            sourceFileData.Exists.ReturnsForAnyArgs(true);
            destFileData.Exists.ReturnsForAnyArgs(false);
            sourceFileData.Extension.ReturnsForAnyArgs(".png");
            destFileData.Extension.ReturnsForAnyArgs(".png");
            _fileSystem.GetFileData("").ReturnsForAnyArgs(sourceFileData, destFileData);

            return (sourceFileData,  destFileData);
        }

        [Test]
        [TestCase(new string[] {}, false)]
        [TestCase(new string[] {"foo"}, false)]
        [TestCase(new string[] { "foo", "bar" }, true)]
        [TestCase(new string[] { "foo", "bar", "fizz" }, false)]
        [TestCase(new string[] { "foo", "bar", "fizz", "buzz" }, false)]
        public void Parse_ReturnsFalse_IfNumberOfArgumentsIsnt2(string[] args, bool expectedResult)
        {
            SetupHappyPathFileData();

            var parser = new ArgumentParser(args, _logger, _fileSystem);
            Assert.That(parser.Parse(), Is.EqualTo(expectedResult));
        }

        [Test]
        public void Parse_ReturnsFalse_IfSourceFileDoesntExist()
        {
            var files = SetupHappyPathFileData();
            files.Item1.Exists.ReturnsForAnyArgs(false);

            var parser = new ArgumentParser(new string[] { "foo", "bar" }, _logger, _fileSystem);

            Assert.That(parser.Parse(), Is.EqualTo(false));
        }

        [Test]
        public void Parse_ReturnsFalse_IfDestinationFileExists()
        {
            var files = SetupHappyPathFileData();
            files.Item2.Exists.ReturnsForAnyArgs(true);

            var parser = new ArgumentParser(new string[] { "foo", "bar" }, _logger, _fileSystem);

            Assert.That(parser.Parse(), Is.EqualTo(false));
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("foo", false)]
        [TestCase("", false)]
        [TestCase(".tst", false)]
        [TestCase(".png", true)]
        [TestCase(".bmp", true)]
        [TestCase(".jpg", true)]
        public void Parse_ReturnsFalse_IfSourceFileExtensionIsWrong(string extension, bool expectedResult)
        {
            var files = SetupHappyPathFileData();
            files.Item1.Extension.ReturnsForAnyArgs(extension);

            var parser = new ArgumentParser(new string[] { "foo", "bar" }, _logger, _fileSystem);

            Assert.That(parser.Parse(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(null)]
        [TestCase("foo")]
        [TestCase("")]
        [TestCase(".tst")]
        [TestCase(".png")]
        [TestCase(".bmp")]
        [TestCase(".jpg")]
        public void Parse_DoesNotCheckDestinationExtension(string extension)
        {
            var files = SetupHappyPathFileData();
            files.Item2.Extension.ReturnsForAnyArgs(extension);

            var parser = new ArgumentParser(new string[] { "foo", "bar" }, _logger, _fileSystem);

            Assert.That(parser.Parse(), Is.EqualTo(true));
        }

        [Test]
        public void Parse_ReturnsFalse_IfFileDataThrowsException()
        {
            _fileSystem.GetFileData("").ReturnsForAnyArgs(x => { throw new Exception("Something bad happened"); });

            var parser = new ArgumentParser(new string[] { "foo", "bar" }, _logger, _fileSystem);

            Assert.That(parser.Parse(), Is.EqualTo(false));
        }
    }
}
