using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using StorageMangler.Domain.Infrastructure;
using StorageMangler.Domain.Model;
using StorageMangler.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace StorageMangler.Domain.Test
{
    [TestFixture]
    public class TestStorageService
    {
        private IStorageService _storageService;
        private Mock<IFileMetaDataRepository> _fileMetaDataRepository;
        private Mock<IFileStorage> _fileStorage;
        private ForbiddenNamesService _forbiddenNamesService;
        private Mock<IForbiddenPatternsRepository> _forbiddenPatternsRepository;
        private Mock<ILogger<ForbiddenNamesService>> _mocklogger;
        private NullLoggerFactory _nullLoggerFactory;
        /// <summary>
        /// TestStorageService constructor defined
        /// </summary>
        public TestStorageService()
        {

            _fileMetaDataRepository = new Mock<IFileMetaDataRepository>();
            _fileStorage = new Mock<IFileStorage>();
            _forbiddenPatternsRepository = new Mock<IForbiddenPatternsRepository>();
            _mocklogger = new Mock<ILogger<ForbiddenNamesService>>();
            _nullLoggerFactory = new NullLoggerFactory();
            _forbiddenNamesService = new ForbiddenNamesService(_forbiddenPatternsRepository.Object, _mocklogger.Object);
            _storageService = new StorageService(_fileMetaDataRepository.Object, _fileStorage.Object, _forbiddenNamesService, _nullLoggerFactory);
        }

        /// <summary>
        /// Check if file exist in the storage
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ListNonForbiddenFiles_ExistFiles()
        {
            //declare files objects
            var fileslist = new List<FileInfo>
            {
                new FileInfo
                {
                    Name = "document.pdf",
                    CreatedOn = DateTime.Now
                },
                new FileInfo
                {
                    Name = "presentation.ppt",
                    CreatedOn = DateTime.Now
                },
                new FileInfo
                {
                    Name = "textfile.txt",
                    CreatedOn = DateTime.Now
                },
                new FileInfo
                {
                    Name = "thefile.xlsx",
                    CreatedOn = DateTime.Now
                }
            }.AsQueryable();
            //declare pattern
            var patternlist = new List<ForbiddenPattern>
            {
                new ForbiddenPattern
                {
                     Pattern = "xlsx$",
                     Created= DateTime.Now
                },
               new ForbiddenPattern
                {
                     Pattern = "xls$",
                     Created= DateTime.Now
                },
                new ForbiddenPattern
                {
                     Pattern = "docx$",
                     Created= DateTime.Now
                },
                 new ForbiddenPattern
                {
                     Pattern = "doc$",
                     Created= DateTime.Now
                },
                 new ForbiddenPattern
                {
                     Pattern = "pptx$",
                     Created= DateTime.Now
                },
                 new ForbiddenPattern
                {
                     Pattern = "ppt$",
                     Created= DateTime.Now
                },
            }.AsQueryable();

            //setup PreStaging files
            var files = _fileStorage.Setup(mr => mr.ListPreStagingFiles()).Returns(Task.FromResult(fileslist.ToList()));
            //setup Fetch all pattern
            _forbiddenPatternsRepository.Setup(m => m.FetchAll()).Returns(Task.FromResult(patternlist.ToList()));
            //Act
            var result = _storageService.ListNonForbiddenFiles();
            try
            {
                Assert.IsTrue(result.Result.Count > 0);
            }
            catch (AssertFailedException)
            {
                Assert.Fail("Failed");
            }
        }
        /// <summary>
        /// Check if file not exist in the storage
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ListNonForbiddenFiles_If_NotExistFiles()
        {
            //declare files objects
            var fileslist = new List<FileInfo>
            {
                new FileInfo
                {
                    Name = "document.pdf",
                    CreatedOn = DateTime.Now
                },
                new FileInfo
                {
                    Name = "presentation.ppt",
                    CreatedOn = DateTime.Now
                },
                new FileInfo
                {
                    Name = "textfile.txt",
                    CreatedOn = DateTime.Now
                },
                new FileInfo
                {
                    Name = "thefile.xlsx",
                    CreatedOn = DateTime.Now
                }
            }.AsQueryable();
            //declare pattern
            var patternlist = new List<ForbiddenPattern>
            {
                new ForbiddenPattern
                {
                     Pattern = "csv",
                     Created= DateTime.Now
                },
               new ForbiddenPattern
                {
                     Pattern = "aspx",
                     Created= DateTime.Now
                },
                new ForbiddenPattern
                {
                     Pattern = "mrt$",
                     Created= DateTime.Now
                }
            }.AsQueryable();
            //setup PreStaging files
            var files = _fileStorage.Setup(mr => mr.ListPreStagingFiles()).Returns(Task.FromResult(fileslist.ToList()));
            //setup Fetch all pattern
            _forbiddenPatternsRepository.Setup(m => m.FetchAll()).Returns(Task.FromResult(patternlist.ToList()));
           
            //Act
            var result = _storageService.ListNonForbiddenFiles();
            
            try
            {
                Assert.IsFalse(result.Result.Count > 0);
            }
            catch (AssertFailedException)
            {
                Assert.Fail("Failed");
            }
        }
    }
}

