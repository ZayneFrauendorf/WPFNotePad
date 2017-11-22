namespace ZFNotePad.Tests
{
    using System;
    using System.Windows;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class MainWindowViewModelTests
    {
        private MainWindowViewModel systemUnderTest;

        private Mock<IFile> mockFile;

        private Mock<IMessageBox> mockMessage;

        private Mock<ISaveFile> mockSaveFile;

        private Mock<IOpenFile> mockOpenFile;

        private Mock<IPath> mockPath;

        private const string defaultFileName = "Untitled.txt";

        private const int defaultFontSize = 20;

        private const string defaultFontFamily = "Arial";

        private const string defaultFontColor = "Black";

        MockRepository mockRepository = new MockRepository(MockBehavior.Strict);

        [SetUp]
        public void SetUp()
        {
            this.mockFile = this.mockRepository.Create<IFile>();
            this.mockMessage = this.mockRepository.Create<IMessageBox>();
            this.mockSaveFile = this.mockRepository.Create<ISaveFile>();
            this.mockOpenFile = this.mockRepository.Create<IOpenFile>();
            this.mockPath = this.mockRepository.Create<IPath>();       
            this.systemUnderTest = new MainWindowViewModel(this.mockFile.Object, this.mockMessage.Object, this.mockSaveFile.Object, this.mockOpenFile.Object, this.mockPath.Object); 
        }

        // Have input this here, so don't have to verify mocks in each test
        [TearDown]
        public void TearDown()
        {
            this.mockRepository.VerifyAll();
        }

        // Testing my Lower Font Functionality
        [Test]
        public void LowerFont_Always_LowersFont()
        {
            var expectedFontSize = this.systemUnderTest.FontSize - 1;
            this.systemUnderTest.LowerFont();
            Assert.That(this.systemUnderTest.FontSize, Is.EqualTo(expectedFontSize));
        }

        [TestCase (20)]
        [TestCase (50)]
        public void LowerFont_Never_GoesBelowOne(int invokeCount)
        {
            for (int i = 0; i < invokeCount; i++)
            {
                this.systemUnderTest.LowerFont();
            }

            Assert.That(this.systemUnderTest.FontSize, Is.GreaterThan(0));
        }

        [Test]
        public void CanLowerFont_Always_ReturnsTrue()
        {
            Assert.IsTrue(this.systemUnderTest.CanLowerFont);
        }

        [Test]
        public void LowerFontSizeCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.LowerFontSizeCommand, Is.Not.Null);
        }

        // Testing my Increase font functionality
        [Test]
        public void IncreaseFont_Always_IncreasesFont()
        {
            var expectedFontSize = this.systemUnderTest.FontSize + 1;
            this.systemUnderTest.IncreaseFont();
            Assert.That(this.systemUnderTest.FontSize, Is.EqualTo(expectedFontSize));
        }

        [Test]
        public void CanIncreaseFont_Always_ReturnsTrue()
        {
            Assert.IsTrue(this.systemUnderTest.CanIncreaseFont);
        }

        [Test]
        public void IncreaseFontSizeCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.IncreaseFontSizeCommand, Is.Not.Null);
        }

        // Testing Save File & Save File As functionality
        [Test]
        public void CanSaveFile_Always_ReturnsTrue()
        {
            Assert.IsTrue(this.systemUnderTest.CanSaveFile);
        }

        [Test]
        public void CanSaveFileAs_Always_ReturnsTrue()
        {
            Assert.IsTrue(this.systemUnderTest.CanSaveFileAs);
        }

        [Test]
        public void SaveFileCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.SaveFileCommand, Is.Not.Null);
        }

        [Test]
        public void SaveFileAsCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.SaveFileAsCommand, Is.Not.Null);
        }

        [Test]
        public void SaveFile_WhenFileNameNotDefault_PerformsExpectedWork()
        {
            this.systemUnderTest.FileName = "FileName";
            this.systemUnderTest.Text = "Text";
            // Hey Mockfile, expect me to call write all text and fail if we dont
            this.mockFile.Setup(p => p.WriteAllText("FileName", "Text"));
            this.systemUnderTest.SaveFile();
        }

        [Test]
        public void SaveFile_WhenFileThrows_PerformsExpectedWork()
        {
            this.systemUnderTest.FileName = "FileName";
            var exception = new Exception("Some message");
            this.mockMessage.Setup(p => p.Show(exception.Message, "File save failed")).Returns(MessageBoxResult.OK);
            // Hey Mockfile, no matter what i give you, throw an exception
            this.mockFile.Setup(p => p.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Throws(exception);
            this.systemUnderTest.SaveFile();
        }

        [Test]
        public void SaveFile_WhenFileNameIsDefault_PerformsExpectedWork()
        {
            this.systemUnderTest.Text = "Text";
            this.mockSaveFile.Setup(p => p.ShowDialog()).Returns(true);
            this.mockSaveFile.SetupGet(p => p.FileName).Returns("Any");
            this.mockFile.Setup(p => p.WriteAllText("Any", "Text"));
            this.systemUnderTest.SaveFile();
        }

        [Test]
        public void SaveFileAs_Always_PerformsExpectedWork()
        {
            this.systemUnderTest.Text = "Text";
            this.mockSaveFile.Setup(p => p.ShowDialog()).Returns(true);
            this.mockSaveFile.SetupGet(p => p.FileName).Returns("Any");
            this.mockFile.Setup(p => p.WriteAllText("Any", "Text"));
            this.systemUnderTest.SaveFileAs();
        }

        [Test]
        public void SaveFileAs_WhenUserCancelsDialogue_PerformsExpectedWork()
        {
            this.systemUnderTest.Text = "Text";
            this.mockSaveFile.Setup(p => p.ShowDialog()).Returns(false);
            this.mockMessage.Setup(p => p.Show("No file was selected", "Warning")).Returns(MessageBoxResult.OK);
            this.systemUnderTest.SaveFileAs();
        }

        // Testing Open About Window functionality
        [Test]
        public void CanOpenAbout_Always_ReturnsTrue()
        {
            Assert.IsTrue(this.systemUnderTest.CanOpenAbout);
        }

        [Test]
        public void OpenAboutBoxCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.OpenAboutBoxCommand, Is.Not.Null);
        }

        // Testing Open File Functionality
        [Test]
        public void CanOpenFile()
        {
            Assert.IsTrue(this.systemUnderTest.CanOpenFile);
        }

        [Test]
        public void OpenFileCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.OpenFileCommand, Is.Not.Null);
        }

        [Test]
        public void OpenFile_Always_PerformsExpectedWork()
        {
            this.mockOpenFile.Setup(p => p.ShowDialog()).Returns(true);
            this.mockOpenFile.SetupGet(p => p.FileName).Returns("Any");
            this.mockPath.Setup(p => p.GetExtension(It.IsAny<string>())).Returns(".txt");
            this.mockFile.Setup(p => p.ReadAllText("Any")).Returns("Text");
            this.systemUnderTest.OpenFile();
            Assert.That(this.systemUnderTest.FileName, Is.EqualTo("Any"));
            Assert.That(this.systemUnderTest.Text, Is.EqualTo("Text"));
        }

        // Testing New File Functionality
        [Test]
        public void CanNewFile_Always_ReturnsTrue()
        {
            Assert.IsTrue(this.systemUnderTest.CanNewFile);
        }

        [Test]
        public void NewFileCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.NewFileCommand, Is.Not.Null);
        }

        [Test]
        public void NewFile_Always_PerformsExpectedWork()
        {
            this.systemUnderTest.FileName = "FileName";
            this.systemUnderTest.Text = "Text";
            this.mockMessage.Setup(p => p.Show("Do you want to save changes made?", "Just Checking", MessageBoxButton.YesNo)).Returns(MessageBoxResult.Yes);
            this.mockFile.Setup(p => p.WriteAllText("FileName", "Text"));
            this.systemUnderTest.NewFile();
            Assert.That(this.systemUnderTest.FileName, Is.EqualTo(defaultFileName));
            Assert.That(this.systemUnderTest.Text, Is.EqualTo(string.Empty));
        }

        // Testing Delete File Functionality
        public void CanDeleteFile_Always_ReturnsTrue()
        {
            Assert.IsTrue(this.systemUnderTest.CanDeleteFile);
        }

        [Test]
        public void DeleteFileCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.DeleteFileCommand, Is.Not.Null);
        }

        [Test]
        public void DeleteFile_WhenFileNameNotDefault_PerformsExpectedWork()
        {
            this.systemUnderTest.FileName = "FileName";
            this.mockFile.Setup(p => p.DeleteCurrentFile("FileName"));
            this.mockMessage.Setup(p => p.Show("Are you sure you want to delete?", "Just Checking", MessageBoxButton.YesNo)).Returns(MessageBoxResult.Yes);
            this.systemUnderTest.DeleteFile();
            Assert.That(this.systemUnderTest.FileName, Is.EqualTo(defaultFileName));
            Assert.That(this.systemUnderTest.Text, Is.EqualTo(string.Empty));
        }

        [Test]
        public void DeleteFile_WhenFileNameIsDefault_PerformExpectedWork()
        {
            this.mockMessage.Setup(p => p.Show("Are you sure you want to delete?", "Just Checking", MessageBoxButton.YesNo)).Returns(MessageBoxResult.Yes);
            this.mockMessage.Setup(p => p.Show("You need a current file showing in order to delete", "Error"))
                .Returns(MessageBoxResult.OK);
            this.systemUnderTest.DeleteFile();
        }
       
        // Testing Exit File Functionality
        [Test]
        public void ExitAppCommand_Never_ReturnsNull()
        {          
            Assert.That(this.systemUnderTest.ExitAppCommand, Is.Not.Null);
        }

        [Test]
        public void CanExit_Always_ReturnsTrue()
        {
            Assert.IsTrue(this.systemUnderTest.CanExit);
        }

        // Testing Defaults
        [Test]
        public void Text_AfterConstruction_HasExpectedDefault()
        {
            Assert.That(this.systemUnderTest.Text, Is.EqualTo(string.Empty));
        }     

        [Test]
        public void FileName_WhenSetToNull_PerformExpectedWork()
        {
            this.systemUnderTest.FileName = null;
            Assert.That(this.systemUnderTest.FileName, Is.EqualTo(defaultFileName));
        }

        // Testing Reset Display Settings Functionality
        [Test]
        public void ResetDisplaySettings_Always_PerformsExpectedWork()
        {
            this.systemUnderTest.FontSize = 42;
            this.systemUnderTest.FontFamily = "Tahoma";
            this.systemUnderTest.FontColor = "Blue";
            this.systemUnderTest.ResetDisplaySettings();
            Assert.That(this.systemUnderTest.FontSize, Is.EqualTo(defaultFontSize));
            Assert.That(this.systemUnderTest.FontFamily, Is.EqualTo(defaultFontFamily));
            Assert.That(this.systemUnderTest.FontColor, Is.EqualTo(defaultFontColor));
        }

        [Test]
        public void ResetDisplaySettingsCommand_Never_ReturnsNull()
        {
            Assert.That(this.systemUnderTest.ResetDisplaySettingsCommand, Is.Not.Null);
        }

        [Test]
        public void CanResetDisplaySettings_Always_ReturnsTrue()
        {
            Assert.That(this.systemUnderTest.CanResetDisplaySettings, Is.EqualTo(true));
        }
    }
}
