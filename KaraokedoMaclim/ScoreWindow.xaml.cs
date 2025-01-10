using System;
using System.Windows;
using System.Windows.Threading;

namespace KaraokedoMaclim
{
    public partial class ScoreWindow : Window
    {
        private int _score;
        private int _currentScore = 0;
        private DispatcherTimer _scoreTimer;
        private DispatcherTimer _returnTimer;
        private DateTime _startTime;

        public ScoreWindow()
        {
            InitializeComponent();

            // Inicializa o tempo de início
            _startTime = DateTime.Now;

            // Pontuação inicial
            _score = 50;

            // Inicializa o texto com 0
            ScoreText.Text = "0";
            MessageText.Text = "";

            // Configura a animação para a pontuação
            StartScoreAnimation();

            // Inicia o timer para voltar ao menu inicial após 10 segundos
            StartReturnToMenuTimer();
        }

        private void StartScoreAnimation()
        {
            // Cria o timer para animar a pontuação
            _scoreTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // Atualiza a cada 50ms
            };
            _scoreTimer.Tick += ScoreTimer_Tick;

            _scoreTimer.Start();
        }

        private void ScoreTimer_Tick(object sender, EventArgs e)
        {
            // Simula uma pontuação com base no tempo de canto (quanto mais rápido, maior a pontuação)
            TimeSpan elapsedTime = DateTime.Now - _startTime;
            _score += (int)(elapsedTime.TotalSeconds * 2); // Aumenta a pontuação conforme o tempo

            // Para limitar a pontuação a um valor entre 50 e 100
            if (_score > 100) _score = 100;
            if (_score < 50) _score = 50;

            // Atualiza a pontuação no display
            ScoreText.Text = _score.ToString();

            // Para limitar a pontuação a um valor entre 50 e 100
            if (_score < 50) _score = 50;

            // Adiciona uma mensagem baseada na pontuação final
            if (_currentScore < _score)
            {
                _currentScore++;
                ScoreText.Text = _currentScore.ToString();
            }
            else
            {
                _scoreTimer.Stop();

                // Exibe a mensagem conforme a pontuação
                if (_score >= 90)
                {
                    MessageText.Text = "Perfeito!";
                }
                else if (_score >= 80)
                {
                    MessageText.Text = "Excelente!";
                }
                else if (_score >= 70)
                {
                    MessageText.Text = "Muito Bom!";
                }
                else
                {
                    MessageText.Text = "Quase lá";
                }
            }
        }

        private void StartReturnToMenuTimer()
        {
            // Cria o timer para retornar ao menu inicial após 10 segundos
            _returnTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            _returnTimer.Tick += ReturnToMenuTimer_Tick;
            _returnTimer.Start();
        }

        private void ReturnToMenuTimer_Tick(object sender, EventArgs e)
        {
            _returnTimer.Stop(); // Para o timer
            ReturnToMenu(); // Volta ao menu inicial
        }

        private void ReturnToMenu()
        {
            // Cria uma nova instância da MainWindow e exibe
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            // Fecha a janela atual (ScoreWindow)
            this.Close();
        }
    }
}
