namespace ZFNotePad
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Win32;
    using RentalCars.WPF;
    using ZFNotePad.Annotations;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // CTRL + M, O collapses all regions & CTRL + M, L expands all regions
        #region Interfaces
        private readonly IFile file;

        private readonly IMessageBox messageBox;

        private readonly ISaveFile saveFile;

        private readonly IOpenFile openFile;

        private readonly IPath path;

        #endregion

        #region Setting default fields
        private const int defaultFontSize = 20;

        private const string defaultFontColor = "Black";

        private const string defaultFontFamily = "Arial";

        private const string defaultFileName = "Untitled.txt";
        #endregion

        #region Backing fields
        private string fileName = defaultFileName;

        private string text = string.Empty;

        private string fontFamily = defaultFontFamily;

        private int fontSize = defaultFontSize;

        private string fontColor = defaultFontColor;
        #endregion

        #region Constructor
        public MainWindowViewModel(IFile file, IMessageBox messageBox, ISaveFile saveFile, IOpenFile openFile, IPath path)
        {
            this.file = file;
            this.messageBox = messageBox;
            this.saveFile = saveFile;
            this.openFile = openFile;
            this.path = path;
        }
        #endregion

        #region Long Hand Properties
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                this.OnPropertyChanged(nameof(this.Text));
                this.isDirty = true;
            }
        }

        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                if (value != null)
                {
                    this.fileName = value;
                }

                else
                {
                    this.fileName = defaultFileName;
                }
                this.OnPropertyChanged(nameof(this.FileName));
            }
        }

        public string FontFamily
        {
            get
            {
                return this.fontFamily;
            }
            set
            {
                this.fontFamily = value;
                this.OnPropertyChanged(nameof(this.FontFamily));
            }
        }

        public int FontSize
        {
            get
            {
                return this.fontSize;
            }

            set
            {
                this.fontSize = value;
                this.OnPropertyChanged(nameof(this.FontSize));
            }
        }

        public string FontColor
        {
            get
            {
                return this.fontColor;
            }

            set
            {
                this.fontColor = value;
                this.OnPropertyChanged(nameof(this.FontColor));
            }
        }
        #endregion

        #region Open About Window Functionality
        private ICommand openAboutBoxCommand;

        public ICommand OpenAboutBoxCommand
        {
            get
            {
                if (this.openAboutBoxCommand == null)
                {
                    this.openAboutBoxCommand = new RelayCommand(param => this.OpenAbout(), param => this.CanOpenAbout);
                }

                return this.openAboutBoxCommand;
            }
        }

        public AboutWindow OpenAbout()
        {        
            var about = new AboutWindow();
            about.ShowDialog();
            return about;
        }

        public bool CanOpenAbout
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Reset Display Settings Functionality
        private ICommand resetDisplaySettingsCommand;

        public ICommand ResetDisplaySettingsCommand
        {
            get
            {
                if (this.resetDisplaySettingsCommand == null)
                {
                    this.resetDisplaySettingsCommand = new RelayCommand(param => this.ResetDisplaySettings(), param => this.CanResetDisplaySettings);
                }

                return this.resetDisplaySettingsCommand;
            }
        }

        public bool CanResetDisplaySettings
        {
            get
            {
                return true;
            }
        }

        public void ResetDisplaySettings()
        {
            this.FontSize = defaultFontSize;
            this.FontFamily = defaultFontFamily;
            this.FontColor = defaultFontColor;
        }
        #endregion

        #region New File Functionality
        private ICommand newFileCommand;

        public ICommand NewFileCommand
        {
            get
            {
                if (this.newFileCommand == null)
                {
                    this.newFileCommand = new RelayCommand(param => this.NewFile(), param => this.CanNewFile);
                }

                return this.newFileCommand;
            }
        }

        private bool isDirty;

        public void NewFile()
        {
                if (this.isDirty == true)
                {
                    var result = this.messageBox.Show("Do you want to save changes made?", "Just Checking", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.SaveFile();
                    }
                }
                this.Text = string.Empty;
                this.isDirty = false;
                this.FileName = defaultFileName;      
        }

        public bool CanNewFile
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Open File Functionality
        private ICommand openFileCommand;

        public ICommand OpenFileCommand
        {
            get
            {
                if (this.openFileCommand == null)
                {
                    this.openFileCommand = new RelayCommand(param => this.OpenFile(), param => this.CanOpenFile);
                }

                return this.openFileCommand;
            }
        }

        public void OpenFile()
        {
            if (this.openFile.ShowDialog().Value)
            {
                try
                {
                    if (this.path.GetExtension(this.openFile.FileName) == ".txt")
                    {
                        this.FileName = this.openFile.FileName;
                        this.Text = this.file.ReadAllText(this.FileName);
                        this.isDirty = false;
                    }
                    else
                    {
                        this.messageBox.Show("Please only open text files", "Warning");
                    }

                }
                catch (Exception exception)
                {
                    this.messageBox.Show(exception.Message, "Unable to load the file!");
                }
            }
        }

        public bool CanOpenFile
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Exit App Functionality
        private ICommand exitAppCommand;

        public ICommand ExitAppCommand
        {
            get
            {
                if (this.exitAppCommand == null)
                {
                    this.exitAppCommand = new RelayCommand(param => this.Exit(), param => this.CanExit);
                }

                return this.exitAppCommand;
            }
        }

        public void Exit()
        {
            if (this.isDirty == true)
            {
               var result = this.messageBox.Show("Do you want to save changes made?", "Just Checking", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    this.SaveFile();
                }                
            }
            Application.Current.Shutdown();
        }

        public bool CanExit
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Lower Font Size Functionality
        private ICommand lowerFontSizeCommand;

        public ICommand LowerFontSizeCommand
        {
            get
            {
                if (this.lowerFontSizeCommand == null)
                {
                    this.lowerFontSizeCommand = new RelayCommand(param => this.LowerFont(), param => this.CanLowerFont);
                }

                return this.lowerFontSizeCommand;
            }
        }

        public void LowerFont()
        {
            if (this.FontSize <= 1)
            {
                return;
            }
            this.FontSize--;
        }

        public bool CanLowerFont
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Increase Font Size Functionality
        private ICommand increaseFontSizeCommand;

        public ICommand IncreaseFontSizeCommand
        {
            get
            {
                if (this.increaseFontSizeCommand == null)
                {
                    this.increaseFontSizeCommand = new RelayCommand(
                        param => this.IncreaseFont(),
                        param => this.CanIncreaseFont);
                }

                return this.increaseFontSizeCommand;
            }
        }

        public void IncreaseFont()
        {
            this.FontSize++;
        }

        public bool CanIncreaseFont
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Save File and Save File As Functionality
        private ICommand saveFileCommand;

        public ICommand SaveFileCommand
        {
            get
            {
                if (this.saveFileCommand == null)
                {
                    this.saveFileCommand = new RelayCommand(param => this.SaveFile(), param => this.CanSaveFile);
                }

                return this.saveFileCommand;
            }
        }

        public void SaveFile()
        {
            if (this.FileName != defaultFileName)
            {
                try
                {
                    this.file.WriteAllText(this.FileName, this.Text);
                }
                catch (Exception exception)
                {
                    this.messageBox.Show(exception.Message, "File save failed");
                }
            }
            else
            {
                this.SaveFileAs();
            }
        }

        public bool CanSaveFile
        {
            get
            {
                return true;
            }
        }

        private ICommand saveFileAsCommand;

        public ICommand SaveFileAsCommand
        {
            get
            {
                if (this.saveFileAsCommand == null)
                {
                    this.saveFileAsCommand = new RelayCommand(param => this.SaveFileAs(), param => this.CanSaveFileAs);
                }

                return this.saveFileAsCommand;
            }
        }

        public void SaveFileAs()
        {
            if (this.saveFile.ShowDialog().Value)
            {
                this.FileName = this.saveFile.FileName;
                this.SaveFile();
            }
            else
            {
                this.messageBox.Show("No file was selected", "Warning");
            }
        }

        public bool CanSaveFileAs
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Delete File Functionality
        private ICommand deleteFileCommand;

        public ICommand DeleteFileCommand
        {
            get
            {
                if (this.deleteFileCommand == null)
                {
                    this.deleteFileCommand = new RelayCommand(param => this.DeleteFile(), param => this.CanDeleteFile);
                }

                return this.deleteFileCommand;
            }
        }

        public void DeleteFile()
        {         
            var result = this.messageBox.Show("Are you sure you want to delete?", "Just Checking", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (this.FileName != defaultFileName)
                {
                    try
                    {
                        this.file.DeleteCurrentFile(this.FileName);
                        this.Text = string.Empty;
                        this.isDirty = false;
                        this.FileName = defaultFileName;
                    }
                    catch (Exception exception)
                    {
                        this.messageBox.Show(exception.Message, "Error, could not delete file!");
                    }
                }
                else
                {
                    this.messageBox.Show("You need a current file showing in order to delete", "Error");
                }
            }
                
            
        }

        public bool CanDeleteFile
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion

        #region Framework Wrappers
    public interface IMessageBox
    {
        MessageBoxResult Show(string message, string caption);

        MessageBoxResult Show(string message, string caption, MessageBoxButton choice);
    }

    public class MessageBoxImpl : IMessageBox
    {
        public MessageBoxResult Show(string message, string caption)
        {
            return MessageBox.Show(message, caption);
        }
        public MessageBoxResult Show(string message, string caption, MessageBoxButton choice)
        {
            return MessageBox.Show(message, caption, choice);
        }
    }

    public interface IFile
    {
        void WriteAllText(string fileName, string text);
        void DeleteCurrentFile(string fileName);

        string ReadAllText(string fileName);
    }

    public class FileImpl : IFile
    {
        public void WriteAllText(string fileName, string text)
        {
            File.WriteAllText(fileName, text);
        }

        public void DeleteCurrentFile(string fileName)
        {
            File.Delete(fileName);
        }

        public string ReadAllText(string fileName)
        {
            return File.ReadAllText(fileName);
        }
    }

    public interface ISaveFile
    {
        bool? ShowDialog();

        string FileName { get; set; }
    }

    public class SaveFileImpl : ISaveFile
    {
        private SaveFileDialog saveFileDialog;
        public SaveFileImpl()
        {
            this.saveFileDialog = new SaveFileDialog();
            this.saveFileDialog.Filter = "Text|*.txt|All|*.*";
        }

        public bool? ShowDialog()
        {
            return this.saveFileDialog.ShowDialog();
        }

        public string FileName
        {
            get
            {
                return this.saveFileDialog.FileName;
            }
            set
            {
                this.saveFileDialog.FileName = value;
            }
        }
    }

    public interface IOpenFile
    {
        bool? ShowDialog();
        string FileName { get; set; }
    }

    public class OpenFileImpl : IOpenFile
    {
        private OpenFileDialog openFileDialog;
        public OpenFileImpl()
        {
            this.openFileDialog = new OpenFileDialog();
            this.openFileDialog.Filter = "Text|*.txt|All|*.*";
        }

        public bool? ShowDialog()
        {
            return this.openFileDialog.ShowDialog();
        }

        public string FileName
        {
            get
            {
                return this.openFileDialog.FileName;
            }
            set
            {
                this.openFileDialog.FileName = value;
            }
        }
    }

    public interface IPath
    {
        string GetExtension(string path);
    }

    public class PathImpl : IPath
    {
        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }
    }
    #endregion

}
