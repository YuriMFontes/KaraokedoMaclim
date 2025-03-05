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

        public ScoreWindow()
        {
            InitializeComponent();

            // Gera uma pontuação aleatória conforme as chances definidas
            _score = GenerateRandomScore();

            // Inicializa o texto com 0
            ScoreText.Text = "0";
            MessageText.Text = "";

            // Configura a animação para a pontuação
            StartScoreAnimation();

            // Inicia o timer para voltar ao menu inicial após 10 segundos
            StartReturnToMenuTimer();
        }

        private int GenerateRandomScore()
        {
            Random random = new Random();
            int chance = random.Next(100); // Gera um número de 0 a 99

            if (chance < 5) return random.Next(40, 50); // 5% de chance
            if (chance < 15) return random.Next(50, 60); // 10% de chance
            if (chance < 30) return random.Next(60, 70); // 15% de chance
            if (chance < 65) return random.Next(70, 80); // 35% de chance
            if (chance < 90) return random.Next(80, 90); // 25% de chance
            return random.Next(90, 100); // 10% de chance
        }

        private void StartScoreAnimation()
        {
            _scoreTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // Atualiza a cada 50ms
            };
            _scoreTimer.Tick += ScoreTimer_Tick;
            _scoreTimer.Start();
        }

        private void ScoreTimer_Tick(object sender, EventArgs e)
        {
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
            _returnTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            _returnTimer.Tick += ReturnToMenuTimer_Tick;
            _returnTimer.Start();
        }

        private void ReturnToMenuTimer_Tick(object sender, EventArgs e)
        {
            _returnTimer.Stop();
            ReturnToMenu();
        }


        private void ReturnToMenu()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
