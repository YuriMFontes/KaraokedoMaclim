using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KaraokedoMaclim
{
    public partial class MusicWindow : Window
    {
        private string _videoPath;

        public MusicWindow(string videoPath)
        {
            InitializeComponent();
            _videoPath = videoPath;

            VideoPlayer.Source = new Uri(_videoPath, UriKind.Absolute);
            VideoPlayer.LoadedBehavior = MediaState.Manual;
            VideoPlayer.Play();
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            var scoreWindow = new ScoreWindow();
            scoreWindow.Show();
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mainWindow?.Show();
                this.Close();
            }
        }
    }
}
