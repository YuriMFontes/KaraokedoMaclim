using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using KaraokedoMaclim.Models;

namespace KaraokedoMaclim
{
    public partial class MainWindow : Window
    {
        private const string UnlimitedCreditsPassword = "246800"; // Senha para desbloquear créditos ilimitados
        private const string AdminPanelPassword = "212223"; // Senha para abrir o painel administrativo
        private List<Musica> _musicas;
        private int _previousCredits = 0; // Armazena os créditos antes de desbloquear créditos ilimitados
        private bool _isAdminPanelOpen = false;

        public MainWindow()
        {
            InitializeComponent();
            _musicas = Musica.CarregarMusicas(@"C:\Users\yurim\source\repos\KaraokedoMaclim\KaraokedoMaclim\Musicas\musicas.txt");
            AtualizarCreditos();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MusicNumberInput.Focus();
        }

        private void AtualizarCreditos()
        {
            CreditsTextBlock.Text = KaraokeSession.Credits == int.MaxValue
                ? "Créditos: Ilimitados"
                : $"Créditos: {KaraokeSession.Credits}";
        }

        private void MusicNumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _); // Permitir apenas números
        }

        private void MusicNumberInput_TextChanged(object sender, EventArgs e)
        {
            string numeroMusica = MusicNumberInput.Text.PadLeft(4, '0');
            if (numeroMusica.Length == 4)
            {
                var musica = _musicas.Find(m => m.Numero == numeroMusica);
                MusicDetails.Text = musica != null
                    ? $"{musica.Titulo}\n{musica.Cantor}"
                    : "Música não encontrada.";
            }
        }

        private void MusicNumberInput_KeyDown(object sender, KeyEventArgs e)
        {
            string numeroMusica = MusicNumberInput.Text.PadLeft(4, '0');

            if (e.Key == Key.Enter)
            {
                // Desbloqueio de créditos ilimitados
                if (numeroMusica == UnlimitedCreditsPassword)
                {
                    if (KaraokeSession.Credits == int.MaxValue)
                    {
                        KaraokeSession.Credits = _previousCredits;
                        MessageBox.Show("Créditos ilimitados desativados.", "Modo Normal", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        _previousCredits = KaraokeSession.Credits;
                        KaraokeSession.Credits = int.MaxValue;
                        MessageBox.Show("Créditos ilimitados desbloqueados!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    AtualizarCreditos();
                    MusicNumberInput.Clear();
                    return;
                }

                // Abrir painel administrativo
                if (numeroMusica == AdminPanelPassword && !_isAdminPanelOpen)
                {
                    var adminPanel = new AdminPanel();
                    _isAdminPanelOpen = true;
                    adminPanel.ShowDialog();
                    _isAdminPanelOpen = false;
                    AtualizarCreditos();
                    MusicNumberInput.Clear();
                    return;
                }

                // Reproduzir música se houver créditos suficientes
                if (KaraokeSession.Credits > 0 || KaraokeSession.Credits == int.MaxValue)
                {
                    var musica = _musicas.Find(m => m.Numero == numeroMusica);

                    if (musica != null)
                    {
                        if (KaraokeSession.Credits != int.MaxValue)
                        {
                            KaraokeSession.Credits -= 1;
                            AtualizarCreditos();
                        }

                        string musicDirectory = @"D:\Musicas";
                        string audioPath = Path.Combine(musicDirectory, $"{numeroMusica}.mp4");

                        if (File.Exists(audioPath))
                        {
                            var musicWindow = new MusicWindow(audioPath);
                            musicWindow.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show($"Arquivo de música não encontrado: {audioPath}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Música não encontrada. Insira um número válido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Você não tem créditos suficientes!", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
