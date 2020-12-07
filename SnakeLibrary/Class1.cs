using System;
using System.Threading;
using System.Drawing;
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
    }

    public class ControllerGraphics {
        ControllerGameplay gameplay;
        Rectangle[] rects;

        int cellSize = 20;

        public ControllerGraphics(ControllerGameplay gameplay) {
            this.gameplay = gameplay;
            DrawMap();
        }

        void Start() {
            DrawMap();
        }

        void DrawMap() {
            int count = gameplay.mapSize * gameplay.mapSize, mapSize = gameplay.mapSize;
            rects = new Rectangle[count];

            for (int x = 0, y = 0, i = 0; y < mapSize; y++) {
                x = 0;
                while (x < mapSize) {
                    RectangleF rect = rects[i];
                    rect.Width = cellSize;
                    rect.Height = cellSize;
                    rect.X = x * cellSize;
                    rect.Y = y * cellSize;
                    //Fill 

                    i++;
                    x++;
                }
            }
        }
    }

    public class GameController {
        ControllerGameplay gameplay;
        ControllerGraphics graphics;

        public GameController(bool start = false) {
            if (start) Start();
        }

        void Start() {
            gameplay = new ControllerGameplay();
            graphics = new ControllerGraphics(gameplay);

            gameplay.Start();
        }
    }

    public class ControllerGameplay {
        int[] length = new int[2];
        Position position;
        Random random = new Random();
        int speed = 1, apples = 0;
        bool isAlive;
        string direction = "down";
        public int mapSize = 8;
        Cell[,] cells;


        void Die() {
            isAlive = false;
        }

        void SpawnApple() {
            Position pos = new Position(random.Next(0, mapSize), random.Next(0, mapSize));

            while (pos.Equals(position)) {
                pos = new Position(random.Next(0, mapSize), random.Next(0, mapSize));
            }

            cells[pos.x, pos.y].type = Cell.Type.apple;
        }

        void Collect() {
            apples += 1;
            cells[position.x, position.y].type = Cell.Type.empty;
        }

        void SpawnSnake() {
            position.x = random.Next(0, mapSize);
            position.y = random.Next(0, mapSize);
        }

        void Update() {
            Move();
            CheckObstacle();

            Draw();
        }

        void CheckObstacle() {
            Cell cell = cells[position.x, position.y];

            switch (cell.type) {
                case Cell.Type.empty: break;
                case Cell.Type.apple: Collect(); break;
                case Cell.Type.snake: Die(); break;
            }

            if (position.x == 0 && direction == "left") position = new Position(mapSize, position.y);
            if (position.x == mapSize && direction == "right") position = new Position(0, position.y);
            if (position.y == 0 && direction == "up") position = new Position(position.x, mapSize);
            if (position.y == mapSize && direction == "down") position = new Position(position.y, 0);


        }

        void Draw() {

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
            SpawnSnake();
            SpawnApple();

            while (isAlive) {
                Update();
                Thread.Sleep(1);
            }
        }

        public ControllerGameplay(bool start = false) {
            if (start) Start();
        }
    }
}
