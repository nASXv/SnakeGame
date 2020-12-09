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
        public Position(Position pos) {
            this.x = pos.x;
            this.y = pos.y;
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

   
    public class GameController {
        public ControllerGameplay gameplay;
        public ControllerGraphics graphics;
        public Label debug;
        Canvas canvas;
        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();
        int gameStep = 12;

        void LaunchTimers() {
            gameTickTimer.Tick += UpdateEvents;
            gameTickTimer.Interval = new TimeSpan(gameStep * 100000);
            gameTickTimer.Start();
        }

         void UpdateEvents(object source, EventArgs e) {
            gameplay.Update();
            graphics.Update();
            if (debug != null) Debug();
        }

        void Debug() {
            debug.Content = "Position: " + gameplay.position.x + "," + gameplay.position.y + "\nApples:  " + gameplay.apples+ "\nSize:  " + gameplay.bodyPositions.Length + "\n "+ 0 + " \n" + 0;
        }

        public GameController(Canvas canvas, bool start = false, Label debugLabel = null) {
            this.canvas = canvas;
            if (start) Start();
            debug = debugLabel;
        }

        void Start() {
            gameplay = new ControllerGameplay(true);
            graphics = new ControllerGraphics(gameplay, canvas);

            LaunchTimers();
            //gameplay.Start();
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
            for (int x = 0, y = 0, i = 0; y < gameplay.mapSize; y++) {
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

    public class ControllerGameplay {
        int[] length = new int[2];
        public Position position, oldPosition;
        public Position[] bodyPositions;
        Random random = new Random();
        
        public int apples = 0;
        bool isAlive = true, increaseSize = false;
        public string direction = "up", nextDirection = "up";
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
            apples ++;
            cells[position.x, position.y].type = Cell.Type.empty;

            SpawnApple();
            IncreaseSize();
        }

        public void KeyPressed(object sender, KeyEventArgs e) {
            string oldDirection = direction;
            switch (e.Key) {
                case Key.Up: if (direction != "down") nextDirection = "up";  break;
                case Key.Down: if (direction != "up") nextDirection = "down";  break;
                case Key.Left:  if (direction != "right") nextDirection = "left"; break;
                case Key.Right:  if (direction != "left") nextDirection = "right"; break;
                case Key.R: //restart
                    break;
            }
        }

        void IncreaseSize() {
            Position[] oldBody = new Position[bodyPositions.Length];
            for (int i = 0; i < oldBody.Length; i++)
                oldBody[i] = new Position(bodyPositions[i]);

            bodyPositions = new Position[bodyPositions.Length + 1];

            for (int i = 0; i < oldBody.Length; i++)
                bodyPositions[i] = new Position( oldBody[i].x, oldBody[i].y);
            bodyPositions[bodyPositions.Length - 1] = new Position(position);
        }

        void SpawnSnake() {
            isAlive = true;
            position = new Position(random.Next(0, mapSize - 1), random.Next(0, mapSize - 1));

            bodyPositions = new Position[2];
            bodyPositions[0] = position;
            bodyPositions[1] = new Position( bodyPositions[0].x, bodyPositions[0].y + 1);
            //bodyPositions[2] = new Position();
        }

        public void Update() {
            if (isAlive) {
                CheckBorder();
                Move();
                CheckObstacle();
            }

            FillCells();
        }

        void CheckBorder() {
            if (position.x == 0 && nextDirection == "left") position = new Position(mapSize, position.y);
            if (position.x == mapSize - 1 && nextDirection == "right") position = new Position(-1, position.y);
            if (position.y == 0 && nextDirection == "up") position = new Position(position.x, mapSize);
            if (position.y == mapSize - 1 && nextDirection == "down") position = new Position(position.x, -1);
        }

        void CheckObstacle() {
            Cell cell = cells[position.x, position.y];

            switch (cell.type) {
                case Cell.Type.empty: break;
                case Cell.Type.apple: Collect(); break;
                case Cell.Type.snake: Die(); break;
            }
        }

        void FillCells() {
            foreach(Cell x in cells) {
                if (x.type != Cell.Type.apple) x.type = Cell.Type.empty;
            }

            foreach (Position x in bodyPositions)
                cells[x.x, x.y].type = Cell.Type.snake;
        }

        void Move() {
            direction = nextDirection;

            Position movement = new Position();
            switch (direction) {
                case "up": movement.y = -1; break;
                case "down": movement.y = 1; break;
                case "left": movement.x = -1; break;
                case "right": movement.x = 1; break;
            }
            Position[] oldBody = new Position[bodyPositions.Length];
            for (int i = 0; i < oldBody.Length; i++) {
                oldBody[i] = new Position(bodyPositions[i].x, bodyPositions[i].y);
            }

            position.x += movement.x;
            position.y += movement.y;

            bodyPositions[0] = position;

            //move body
            for (int i = 1; i < bodyPositions.Length; i++) {
                bodyPositions[i] = oldBody[i-1];
            }
        }

        public void Start() {
            cells = new Cell[mapSize, mapSize];
            for(int x = 0, y = 0; y < mapSize; y++) {
                x = 0;
                while (x < mapSize) {
                    cells[x, y] = new Cell();

                    x++;
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
