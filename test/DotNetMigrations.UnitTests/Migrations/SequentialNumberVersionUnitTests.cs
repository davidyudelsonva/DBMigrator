﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetMigrations.Core;
using DotNetMigrations.Core.Migrations;
using DotNetMigrations.UnitTests.Mocks;
using DotNetMigrations.UnitTests.Stubs;
using NUnit.Framework;

namespace DotNetMigrations.UnitTests.Migrations
{
    [TestFixture]
    public class SequentialNumberVersionUnitTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _configManager = new InMemoryConfigurationManager();
            _subject = new SequentialNumberVersion();
        }

        #endregion

        private SequentialNumberVersion _subject;
        private InMemoryConfigurationManager _configManager;

        [Test]
				[Ignore("Not supporting sequential version numbers")]
        public void GetNewVersionNumber_should_return_1_when_directory_is_empty()
        {
            // arrange
            var migDir = new MigrationDirectory(_configManager,"C:\\temp\\");
            
            // act
            var num = _subject.GetNewVersionNumber(migDir);

            // assert
            Assert.AreEqual(1, num);
        }
    }
}