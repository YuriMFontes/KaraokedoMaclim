using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using NAudio.Wave;

namespace KaraokedoMaclim
{
    public partial class KaraokeWindow : Window
    {
        private readonly string _lyricsFilePath;
        private readonly string _audioFilePath;
        private (TimeSpan Time, string Text)[] _lyricsTimedLines;
        private DispatcherTimer _lyricsTimer;
        private IWavePlayer _waveOutDevice;
        private AudioFileReader _audioFileReader;
        private int _currentLineIndex = 0;
        private int _highlightPosition = 0;

        public KaraokeWindow(string audioFilePath, string lyricsFilePath)
        {
            InitializeComponent();
            _audioFilePath = audioFilePath;
            _lyricsFilePath = lyricsFilePath;
            CarregarLetras();
            IniciarAudio();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Definir a janela para tela cheia
            this.WindowState = WindowState.Maximized;
            this.Topmost = true; // Para manter a janela sempre acima das outras
            this.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(@"C:\Users\yurim\source\repos\KaraokedoMaclim\KaraokedoMaclim\Imagens\background.png")),
                Stretch = Stretch.UniformToFill
            };
        }

        private void CarregarLetras()
        {
            if (File.Exists(_lyricsFilePath))
            {
                var lines = File.ReadAllLines(_lyricsFilePath);
                _lyricsTimedLines = ParseLyrics(lines);
            }
            else
            {
                LyricsDisplay.Text = "Arquivo de letras não encontrado.";
            }
        }

        private (TimeSpan Time, string Text)[] ParseLyrics(string[] lines)
        {
            return lines.Select(line =>
            {
                var parts = line.Split(new[] { ']' }, 2);
                if (parts.Length == 2 && TimeSpan.TryParseExact(parts[0].TrimStart('['), @"mm\:ss\.ff", null, out var time))
                {
                    return (time, parts[1]);
                }
                return (TimeSpan.Zero, string.Empty);
            }).ToArray();
        }

        private void IniciarAudio()
        {
            if (File.Exists(_audioFilePath))
            {
                try
                {
                    _audioFileReader = new AudioFileReader(_audioFilePath) { Volume = 0.5f };
                    _waveOutDevice = new WaveOutEvent();
                    _waveOutDevice.Init(_audioFileReader);
                    _waveOutDevice.Play();

                    _lyricsTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(200) // Atualização mais precisa
                    };
                    _lyricsTimer.Tick += AtualizarLetras;
                    _lyricsTimer.Start();
                }
                catch (Exception ex)
                {
                    LyricsDisplay.Text = $"Erro ao iniciar o áudio: {ex.Message}";
                }
            }
            else
            {
                LyricsDisplay.Text = "Arquivo de áudio não encontrado.";
            }
        }

        private void AtualizarLetras(object sender, EventArgs e)
        {
            if (_lyricsTimedLines == null || _audioFileReader == null) return;

            var currentTime = _audioFileReader.CurrentTime;

            // Atualiza a linha atual com letras sincronizadas
            if (_currentLineIndex < _lyricsTimedLines.Length &&
                _lyricsTimedLines[_currentLineIndex].Time <= currentTime)
            {
                AtualizarTextoComDestaque(_lyricsTimedLines[_currentLineIndex].Text);
            }
        }

        private void AtualizarTextoComDestaque(string linhaAtual)
        {
            // Marcar as letras já cantadas como amarelas
            if (_highlightPosition < linhaAtual.Length)
            {
                _highlightPosition++;
                var jaCantadas = linhaAtual.Substring(0, _highlightPosition);
                var restante = linhaAtual.Substring(_highlightPosition);

                LyricsDisplay.Inlines.Clear();
                LyricsDisplay.Inlines.Add(new Run(jaCantadas) { Foreground = Brushes.Yellow });
                LyricsDisplay.Inlines.Add(new Run(restante) { Foreground = Brushes.White });
            }
            else
            {
                _currentLineIndex++;
                _highlightPosition = 0;
            }
        }

        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            _waveOutDevice?.Stop();
            _audioFileReader?.Dispose();
            _waveOutDevice?.Dispose();
            this.Close();
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) // Pressionando "Space" para voltar à MainWindow
            {
                VoltarButton_Click(sender, e);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _waveOutDevice?.Stop();
            _audioFileReader?.Dispose();
            _waveOutDevice?.Dispose();
            base.OnClosed(e);
        }
    }
}
