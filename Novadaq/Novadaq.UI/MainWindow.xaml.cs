using Novadaq.Core;
using Novadaq.UI.Model;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace Novadaq.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileWatcher _fileWatcher;
        private bool _isFileWatcherRunning = false;
        private IDisposable _subscription;
        private Session _session;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _session;
        }
        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderSelectDialog();
            folderDialog.InitialDirectory = _fileWatcher?.InputFolder ?? Environment.CurrentDirectory;

            // Show open file dialog box
            var result = folderDialog.ShowDialog();

            // Process open file dialog box results
            if (result)
            {
                // Open document    
                var folderName = folderDialog.FolderName;
                lbFolder.Text = folderName;

                btnStart.Content = "Start";
                _isFileWatcherRunning = false;

                //Dispose all outstanding tasks (if any)
                _fileWatcher?.Dispose();
                _fileWatcher = new FileWatcher(folderName);


                //Dispose all subscription if it exists.
                _subscription?.Dispose();
                _subscription = _fileWatcher.InputValue.ObserveOnDispatcher().Subscribe((msg) =>
                {
                    _session?.Messages.Add(new Message()
                    {
                        Content = msg,
                        Timestamp = DateTime.Now
                    });
                });

                //Create new session for output to UI
                _session = new Session(folderName);
                DataContext = _session;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_fileWatcher == null)
            {
                MessageBox.Show("Please select input folder first", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (_isFileWatcherRunning)
            {
                btnStart.Content = "Start";
                _fileWatcher.StopMonitoring();
            }
            else
            {
                btnStart.Content = "Stop";
                _fileWatcher.StartMonitoring();

            }
            _isFileWatcherRunning = !_isFileWatcherRunning;
        }
    }
}
