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
using System.Timers;
//using System.Threading;
//using System.Drawing;
namespace SnakeLibrary {

    public class Position {
        public int x = 0, y = 0;
        public Position(int x = 0, int y = 0) {
            this.x = x;
            this.y = y;
        }
        public bool Equals(Position pos) {
            return (pos.x == x && pos.y == y);
        }
    }

    public class Cell {
        public enum Type {
            empty,
            snake,
            apple
        }
        public Type type = Type.empty;
        public Cell() {

        }
    }

    public class ControllerGraphics {
        ControllerGameplay gameplay;
        Rectangle[] rects;
        Canvas canvas;

        Color Empty = Colors.Black;
        Color Snake = Colors.Green;
        Color Apple = Colors.Red;

        int cellSize = 20;

        public ControllerGraphics(ControllerGameplay gameplay, Canvas canvas) {
            this.gameplay = gameplay;
            this.canvas = canvas;

            Start();
        }

        void Start() {
            CreateMap();
        }

        public void Update() {
            UpdateMap();
        }
        
        void UpdateMap() {
            for (int x = 0, y = 0, i= 0; y < gameplay.mapSize; y++) {
                x = 0;
                Color color = Colors.White;

                while (x < gameplay.mapSize) {

                    Rectangle rect = rects[i];
                    switch (gameplay.cells[x, y].type) {
                        case Cell.Type.apple: color = Apple; break;
                        case Cell.Type.empty: color = Empty; break;
                        case Cell.Type.snake: color = Snake; break;
                        default: color = Colors.Black; break;
                    }
                    
                    rect.Fill = new SolidColorBrush(color);

                    x++;
                    i++;
                    //Thread.Sleep(100);
                }
            }
        }

        void CreateMap() {
            int count = gameplay.mapSize * gameplay.mapSize, mapSize = gameplay.mapSize;
            rects = new Rectangle[count];

            for (int x = 0, y = 0, i = 0; y < mapSize; y++) {
                x = 0;
                while (x < mapSize) {
                   // Rectangle rect = rects[i];
                    Rectangle rect = new Rectangle();
                    rects[i] = rect;
                    
                    rect.Width = cellSize;
                    rect.Height = cellSize;

                    canvas.Children.Insert(i, rect);
                    Canvas.SetTop(rect, y * cellSize);
                    Canvas.SetLeft(rect, x * cellSize);

                    rect.Fill = new SolidColorBrush(Colors.AliceBlue);

                    i++;
                    x++;

                    //Thread.Sleep(100);
                }
            }
        }
    }

    public class GameController {
        ControllerGameplay gameplay;
        ControllerGraphics graphics;
        Canvas canvas;
        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();
        int gameStep = 5;

        void LaunchTimers() {
            gameTickTimer.Tick += UpdateEvents;
            gameTickTimer.Interval = new TimeSpan(gameStep * 1000000);
            gameTickTimer.Start();
        }

         void UpdateEvents(object source, EventArgs e) {
            gameplay.Update();
            graphics.Update();
        }

        public GameController(Canvas canvas, bool start = false) {
            this.canvas = canvas;
            if (start) Start();
        }

        void Start() {
            gameplay = new ControllerGameplay(true);
            graphics = new ControllerGraphics(gameplay, canvas);

            LaunchTimers();
            //gameplay.Start();
        }
    }

    public class ControllerGameplay {
        int[] length = new int[2];
        Position position = new Position();
        Random random = new Random();
        int gameSpeed = 1000, apples = 0;
        bool isAlive = true;
        string direction = "down";
        public int mapSize = 16;
        public Cell[,] cells;


        void Die() {
            isAlive = false;
        }

        void SpawnApple() {
            Position pos = new Position(random.Next(0, mapSize), random.Next(0, mapSize));

            while (cells[pos.x, pos.y].type != Cell.Type.empty) {
                pos = new Position(random.Next(0, mapSize), random.Next(0, mapSize));
            }

            cells[pos.x, pos.y].type = Cell.Type.apple;
        }

        void Collect() {
            apples += 1;
            cells[position.x, position.y].type = Cell.Type.empty;
        }

        void SpawnSnake() {
            isAlive = true;
            position.x = random.Next(0, mapSize-1);
            position.y = random.Next(0, mapSize-1);
        }

        public void Update() {
            Move();
            CheckObstacle();

            FillCells();
        }

        void CheckObstacle() {
            Cell cell = cells[position.x, position.y];

            switch (cell.type) {
                case Cell.Type.empty: break;
                case Cell.Type.apple: Collect(); break;
                case Cell.Type.snake: Die(); break;
            }

            if (position.x == 1 && direction == "left") position = new Position(mapSize-1, position.y);
            if (position.x == mapSize-1 && direction == "right") position = new Position(1, position.y);
            if (position.y == 1 && direction == "up") position = new Position(position.x, mapSize-1);
            if (position.y == mapSize-1 && direction == "down") position = new Position(position.y, 1);

        }

        void FillCells() {
            cells[position.x, position.y].type = Cell.Type.snake;
        }

        void Move() {
            Position movement = new Position();
            switch (direction) {
                case "up": movement.y = 1; break;
                case "down": movement.y = -1; break;
                case "left": movement.x = 1; break;
                case "right": movement.x = -1; break;
            }

            position.x += movement.x;
            position.y += movement.y;
        }

        public void Input(string direction) {
            this.direction = direction;
        }


        public void Start() {
            cells = new Cell[mapSize, mapSize];
            for(int x = 0, y = 0; x < mapSize; x++) {
                y = 0;
                while (y < mapSize) {
                    cells[x, y] = new Cell();

                    y++;
                }
            }

            SpawnSnake();
            SpawnApple();
        }

        public ControllerGameplay(bool start = false) {
            if (start) Start();
        }
    }
}
