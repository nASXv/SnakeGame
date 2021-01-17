using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using snakespace;
using SnakeLibrary;

namespace snakespace {
    public partial class MainWindow : Window {
        GameController gameController;

        string language = "en";

        public MainWindow() {
            InitializeComponent();
        }

        void NewGame() {
            Clear();
            if (gameController == null)
                gameController = new GameController(GameCanvas, true, Text_Length, img_GameOver, ScoreText, language);
            else gameController.Play();
        }

        void Clear() {
            img_GameOver.Opacity = 0;
            txt_Start.Opacity = 0;
            GameCanvas.Children.Clear();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e) {
            NewGame();

            btn_play_image.Source = new BitmapImage(new Uri(@"pack://application:,,/Resources/Restart.png"));
        }

        private void Window_KeyUp(object sender, KeyEventArgs e) {
            gameController.gameplay.KeyPressed(sender, e);
        }

        private void button_ru_Click(object sender, RoutedEventArgs e) {
            CanvasLanguage.Visibility = Visibility.Hidden;
            language = "ru";
            ScoreText.Text = "СЧЁТ: 0";
            txt_Start.Content = "Нажмите кнопку для старта";
        }

        private void button_en_Click(object sender, RoutedEventArgs e) {
            CanvasLanguage.Visibility = Visibility.Hidden;
        }
    }

    
}
