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
        public MainWindow() {
            InitializeComponent();
        }

        void NewGame() {
            Clear();
            gameController = new GameController(GameCanvas, true, Text_Length, img_GameOver);
        }

        void Clear() {
            img_GameOver.Opacity = 0;
            GameCanvas.Children.Clear();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e) {
            NewGame();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e) {
            gameController.gameplay.KeyPressed(sender, e);
        }
    }

    
}
