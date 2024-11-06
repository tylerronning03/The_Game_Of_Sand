using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SandGame
{
    internal class Engine
    {

        private Grid _world;

        private bool _gravity;

        private float _brownian;

        public Engine(int width, int height)
        {
            _world = new Grid(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _world[x, y] = new Box();
                }
            }
        }

        /// <summary>
        /// Apply physics to all boxes in grid
        /// </summary>
        public void Tick()
        {

            ApplyBurn();
            for (int y = _world.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = _world.GetLength(0) - 1; x >= 0; x--)
                {
                    if (_gravity)
                    {
                        ApplyGravity(x, y);
                    }
                    ApplyBrownian(x, y);
                }
            }


            MoveSnakes();

            return;
        }

        /// <summary>
        /// Returns the grid
        /// </summary>
        /// <returns>World array</returns>
        public Grid GetWorldData()
        {
            return _world;
        }

        /// <summary>
        /// Updates engine gravity
        /// </summary>
        /// <param name="gravity">New gravity state</param>
        public void SetGravity(bool gravity)
        {
            _gravity = gravity;
        }

        /// <summary>
        /// Updates engine brownian value
        /// </summary>
        /// <param name="brownian">New brownian value</param>
        public void SetBrownian(float brownian)
        {
            _brownian = brownian;

        }

        /// <summary>
        /// Set a box in the grid
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="element">box element</param>
        public void SetObject(int x, int y, Element element)
        {
            // This function goes to grid so that Unit tests can simulate grids
            _world.SetObject(x, y, element);
        }

        /// <summary>
        /// Apply gravity to box at location.
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        void ApplyGravity(int x, int y)
        {
            if (_world[x, y].GetElement() == Element.sand)
            {
                if (CanMoveDown(x, y))
                {
                    SetObject(x, y + 1, _world[x, y].GetElement());
                    SetObject(x, y, Element.none);
                }
                else if (CanMoveDownLeft(x, y))
                {
                    SetObject(x - 1, y + 1, _world[x, y].GetElement());
                    SetObject(x, y, Element.none);
                }
                else if (CanMoveDownRight(x, y))
                {
                    SetObject(x + 1, y + 1, _world[x, y].GetElement());
                    SetObject(x, y, Element.none);
                }
            }
        }

        /// <summary>
        /// Apply Brownian Force to box
        /// </summary>
        public void ApplyBrownian(int x, int y)
        {
            if (x >= 0 && x < _world.GetLength(0) &&
                y >= 0 && y < _world.GetLength(1))
            {


                if (_world[x, y].GetElement() == Element.sand)
                {
                    float random = RandomBrownian();

                    if (random < _brownian)
                    {
                        int coinFlip = CoinFlip();

                        if (coinFlip == 0)
                        {
                            if (CanMoveLeft(x, y))
                            {
                                SetObject(x - 1, y, _world[x, y].GetElement());
                                SetObject(x, y, Element.none);
                            }
                        }
                        else
                        {
                            if (CanMoveRight(x, y))
                            {
                                SetObject(x + 1, y, _world[x, y].GetElement());
                                SetObject(x, y, Element.none);
                                x++;    // do not brownian the same object twice
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Apply Brownian Force to world
        /// </summary>
        /// <param name="skew">Predestined Brownian comparison for testing</param>
        /// <param name="coin">Predestined coin flip for comparison for testing</param>
        public void ApplyBrownian(float skew, int coin)
        {
            for (int y = 0; y < _world.GetLength(1); y++)
            {
                for (int x = 0; x < _world.GetLength(0); x++)
                {
                    if (_world[x, y].GetElement() == Element.sand)
                    {
                        float random = (skew == -1) ? RandomBrownian() : skew;

                        if (random < _brownian)
                        {
                            int coinFlip = (coin == -1) ? CoinFlip() : coin;

                            if (coinFlip == 0)
                            {
                                if (CanMoveLeft(x, y))
                                {
                                    SetObject(x - 1, y, _world[x, y].GetElement());
                                    SetObject(x, y, Element.none);
                                }
                            }
                            else
                            {
                                if (CanMoveRight(x, y))
                                {
                                    SetObject(x + 1, y, _world[x, y].GetElement());
                                    SetObject(x, y, Element.none);
                                    x++;    // do not brownian the same object twice
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Apply effects of magma to every box in the world
        /// </summary>
        public void ApplyBurn()
        {

            for (int y = 0; y < _world.GetLength(1); y++)
            {
                for (int x = 0; x < _world.GetLength(0); x++)
                {
                    if (_world[x, y].GetElement() == Element.magma)
                    {
                        for (int targetX = x - 1; targetX <= x + 1; targetX++)
                        {
                            for (int targetY = y - 1; targetY <= y + 1; targetY++)
                            {
                                BurnBoxFrom(targetX, targetY, x, y);
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Turn snake into sand or sand into glass
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void BurnBoxFrom(int x, int y, int magmaX, int magmaY)
        {
            if (x >= 0 && x < _world.GetLength(0) &&
                y >= 0 && y < _world.GetLength(1))
            {
                if (_world[x, y].GetElement() == Element.snake)
                {
                    _world[x, y].GetSnake().BurnFrom(_world, magmaX, magmaY);
                }
                else if (_world[x, y].GetElement() == Element.sand)
                {
                    _world[x, y].SetElement(Element.glass);
                }
            }
        }

        /// <summary>
        /// Returns random float for brownian comparison
        /// </summary>
        /// <returns>Brownian Float</returns>
        public float RandomBrownian()
        {
            Random random = new Random();
            return random.NextSingle();
        }

        /// <summary>
        /// Returns a 0 or 1 randomly
        /// </summary>
        /// <returns>0 or 1</returns>
        public int CoinFlip()
        {
            Random random = new Random();
            return random.Next(0, 2);
        }

        /// <summary>
        /// Move each snake in the grid in a random direction, if possible
        /// </summary>
        public void MoveSnakes()
        {
            foreach (Snake snake in _world.GetSnakes())
            {
                Random random = new Random();
                (int x, int y) head = snake.GetBody()[snake.GetHeadIndex()];
                (int x, int y) nextElement = head;

                // Get random direction
                HashSet<int> directionSet = new HashSet<int>();

                // Randomly navigate to an option
                directionSet.Add(0);
                directionSet.Add(1);
                directionSet.Add(2);
                directionSet.Add(3);
                HashSet<int>.Enumerator directionEnumerator = directionSet.GetEnumerator();

                int index = random.Next(1, directionSet.Count + 1);
                for (int i = 0; i < index; i++)
                {
                    directionEnumerator.MoveNext();
                }

                bool willMove = true;
                switch (directionEnumerator.Current)
                {
                    case 0:
                        if (_world.CanMoveUp(head.x, head.y))
                        {
                            snake.SetDirection(Direction.Up);
                            nextElement = (head.x, head.y - 1);
                        }
                        else
                        {
                            willMove = false;
                        }
                        break;
                    case 1:
                        if (_world.CanMoveDown(head.x, head.y))
                        {
                            snake.SetDirection(Direction.Down);
                            nextElement = (head.x, head.y + 1);
                        }
                        else
                        {
                            willMove = false;
                        }
                        break;
                    case 2:
                        if (_world.CanMoveLeft(head.x, head.y))
                        {
                            snake.SetDirection(Direction.Left);
                            nextElement = (head.x - 1, head.y);
                        }
                        else
                        {
                            willMove = false;
                        }
                        break;
                    case 3:
                        if (_world.CanMoveRight(head.x, head.y))
                        {
                            snake.SetDirection(Direction.Right);
                            nextElement = (head.x + 1, head.y);
                        }
                        else
                        {
                            willMove = false;
                        }
                        break;
                }

                // Don't necessarily move every frame
                if (willMove)
                {
                    _world[head.x, head.y].SetIsHead(false);    // The old head is no longer a head if we are moving

                    if (_world[nextElement.x, nextElement.y].GetElement() == Element.sand)
                    {
                        snake.Eat(nextElement.x, nextElement.y);
                    }
                    else
                    {
                        // save old box so we can erase it
                        Box firstBox = _world[snake.GetBody()[0].x, snake.GetBody()[0].y];
                        snake.Move(nextElement.x, nextElement.y);
                        firstBox.SetElement(Element.none);
                        firstBox.SetSnake(null);
                    }
                    // Mark new head correctly
                    _world[snake.GetBody()[snake.GetHeadIndex()].x, snake.GetBody()[snake.GetHeadIndex()].y].SetElement(Element.snake);
                    _world[snake.GetBody()[snake.GetHeadIndex()].x, snake.GetBody()[snake.GetHeadIndex()].y].SetSnake(snake);
                    _world[snake.GetBody()[snake.GetHeadIndex()].x, snake.GetBody()[snake.GetHeadIndex()].y].SetIsHead(true);
                }
            }
        }

        /// <summary>
        /// Move snakes in a specified direction
        /// </summary>
        /// <param name="direction">Direction for snakes to move</param>
        public void MoveSnakesDebug(Direction direction)
        {
            foreach (Snake snake in _world.GetSnakes())
            {
                (int x, int y) head = snake.GetBody()[snake.GetHeadIndex()];
                (int x, int y) nextElement = head;

                switch (direction)
                {
                    case Direction.Up:
                        if (_world.CanMoveUp(head.x, head.y))
                        {
                            snake.SetDirection(Direction.Up);
                            nextElement = (head.x, head.y - 1);
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case Direction.Down:
                        if (_world.CanMoveDown(head.x, head.y))
                        {
                            snake.SetDirection(Direction.Down);
                            nextElement = (head.x, head.y + 1);
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case Direction.Left:
                        if (_world.CanMoveLeft(head.x, head.y))
                        {
                            snake.SetDirection(Direction.Left);
                            nextElement = (head.x - 1, head.y);
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case Direction.Right:
                        if (_world.CanMoveRight(head.x, head.y))
                        {
                            snake.SetDirection(Direction.Right);
                            nextElement = (head.x + 1, head.y);
                        }
                        else
                        {
                            return;
                        }
                        break;

                }
                _world[head.x, head.y].SetIsHead(false);
                if (_world[nextElement.x, nextElement.y].GetElement() == Element.sand)
                {
                    snake.Eat(nextElement.x, nextElement.y);
                }
                else
                {
                    Box firstBox = _world[snake.GetBody()[0].x, snake.GetBody()[0].y];
                    snake.Move(nextElement.x, nextElement.y);
                    firstBox.SetElement(Element.none);
                    firstBox.SetSnake(null);
                }
                // Mark new head correctly
                _world[snake.GetBody()[snake.GetHeadIndex()].x, snake.GetBody()[snake.GetHeadIndex()].y].SetElement(Element.snake);
                _world[snake.GetBody()[snake.GetHeadIndex()].x, snake.GetBody()[snake.GetHeadIndex()].y].SetSnake(snake);
                _world[snake.GetBody()[snake.GetHeadIndex()].x, snake.GetBody()[snake.GetHeadIndex()].y].SetIsHead(true);
            }
        }

        /// <summary>
        /// Returns if box at location can move down 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveDown(int x, int y)
        {
            return _world.CanMoveDown(x, y);
        }

        /// <summary>
        /// Returns if box at location can move left 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveLeft(int x, int y)
        {

            return _world.CanMoveLeft(x, y);
        }

        /// <summary>
        /// Returns if box at location can move right 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveRight(int x, int y)
        {
            return _world.CanMoveRight(x, y);
        }

        /// <summary>
        /// Returns if box at location can move down and left 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveDownLeft(int x, int y)
        {
            return _world.CanMoveDownLeft(x, y);
        }

        /// <summary>
        /// Returns if box at location can move down and right 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveDownRight(int x, int y)
        {
            return _world.CanMoveDownRight(x, y);
        }
    }
}
