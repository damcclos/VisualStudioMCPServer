using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using VisualStudioMCPServer.Tests.Mock;

namespace VisualStudioMCPServer.Tests
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void Log_WithIdentifier_PrependsIdentifierAndAppendsNewline()
        {
            // Arrange
            var mockLogger = new MockLogger();
            var logger = new Logger(mockLogger, "TestID");

            // Act
            logger.Log("test message");

            // Assert
            Assert.AreEqual(1, mockLogger.LoggedMessages.Count);
            Assert.AreEqual("[TestID]: test message\n", mockLogger.LoggedMessages[0]);
        }

        [TestMethod]
        public void Log_WithoutIdentifier_AppendsNewlineOnly()
        {
            // Arrange
            var mockLogger = new MockLogger();
            var logger = new Logger(mockLogger, null);

            // Act
            logger.Log("test message");

            // Assert
            Assert.AreEqual(1, mockLogger.LoggedMessages.Count);
            Assert.AreEqual("test message\n", mockLogger.LoggedMessages[0]);
        }

        [TestMethod]
        public void Log_WithEmptyIdentifier_AppendsNewlineOnly()
        {
            // Arrange
            var mockLogger = new MockLogger();
            var logger = new Logger(mockLogger, "");

            // Act
            logger.Log("test message");

            // Assert
            Assert.AreEqual(1, mockLogger.LoggedMessages.Count);
            Assert.AreEqual("test message\n", mockLogger.LoggedMessages[0]);
        }

        [TestMethod]
        public void Log_MultipleMessages_CallsUnderlyingLoggerForEach()
        {
            // Arrange
            var mockLogger = new MockLogger();
            var logger = new Logger(mockLogger, "ID");

            // Act
            logger.Log("first");
            logger.Log("second");
            logger.Log("third");

            // Assert
            Assert.AreEqual(3, mockLogger.LoggedMessages.Count);
            Assert.AreEqual("[ID]: first\n", mockLogger.LoggedMessages[0]);
            Assert.AreEqual("[ID]: second\n", mockLogger.LoggedMessages[1]);
            Assert.AreEqual("[ID]: third\n", mockLogger.LoggedMessages[2]);
        }

        [TestMethod]
        public void Log_EmptyMessage_StillFormatsCorrectly()
        {
            // Arrange
            var mockLogger = new MockLogger();
            var logger = new Logger(mockLogger, "Test");

            // Act
            logger.Log("");

            // Assert
            Assert.AreEqual(1, mockLogger.LoggedMessages.Count);
            Assert.AreEqual("[Test]: \n", mockLogger.LoggedMessages[0]);
        }
    }
}
