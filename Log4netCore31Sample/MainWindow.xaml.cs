using System.ComponentModel;
using System.Windows;
using Logging;

namespace Log4netCore31Sample
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LogSpooler l;

        public MainWindow()
        {
            InitializeComponent();
            l = new LogSpooler();
            l.SetSafe(30);
            l.Start();
        }

        private void TestInfo_OnClick(object sender, RoutedEventArgs e)
        {
            testInfoHandler();
        }

        private void TestWarn_OnClick(object sender, RoutedEventArgs e)
        {
            testWarnHandler();
        }

        private void TestError_OnClick(object sender, RoutedEventArgs e)
        {
            testErrorHandler();
        }

        private void testInfoHandler()
        {
            l.AppendInfo(@"test info");
        }

        private void testWarnHandler()
        {
            l.AppendWarn(@"test warn");
        }

        private void testErrorHandler()
        {
            l.AppendError(@"test error");
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            l.Dispose();
        }
    }
}