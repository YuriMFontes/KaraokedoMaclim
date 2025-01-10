using System.Windows;
using System.Windows.Input;
using KaraokedoMaclim.Models;

namespace KaraokedoMaclim
{
    public partial class AdminPanel : Window
    {
        public AdminPanel()
        {
            InitializeComponent();
            CreditsInput.Focus(); // Foca no campo de digitação
        }

        private void CreditsInput_KeyDown(object sender, KeyEventArgs e)
        {
            // Verifica se a tecla pressionada é Enter
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(CreditsInput.Text, out int newCredits))
                {
                    KaraokeSession.Credits += newCredits; // Adiciona os créditos
                    this.Close(); // Fecha o painel de administração
                }
                else
                {
                    MessageBox.Show("Insira um valor válido para os créditos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
