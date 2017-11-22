namespace ZFNotePad
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {           
            this.InitializeComponent();
            var mainWindowViewModel = new MainWindowViewModel(new FileImpl(), new MessageBoxImpl(), new SaveFileImpl(), new OpenFileImpl(), new PathImpl());
            this.DataContext = mainWindowViewModel;
        }
    }
}
