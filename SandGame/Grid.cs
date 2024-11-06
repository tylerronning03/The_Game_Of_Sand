using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SandGame
{
    internal class Grid
    {

        private Box[,] _world;
        private HashSet<Snake> _snakes;

        public Box this[int x, int y]
        {
            get
            {
                return _world[x, y];
            }
            set
            {
                _world[x, y] = value;
            }
        }

        public Grid(int width, int height)
        {
            _world = new Box[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _world[x, y] = new Box();
                }
            }
            _snakes = new HashSet<Snake>();
        }

        /// <summary>
        /// Set a box in the grid
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="element">box element</param>
        public void SetObject(int x, int y, Element element)
        {

            if (x < _world.GetLength(0) && x >= 0 &&
                y < _world.GetLength(1) && y >= 0)
            {

                if (_world[x, y].GetElement() == Element.none || element == Element.none)
                {
                    if (_world[x, y].GetElement() == Element.snake)
                    {
                        if (element == Element.none)
                        {
                            _snakes.Remove(_world[x, y].GetSnake());
                            _world[x, y].GetSnake().DeleteFrom(this);
                        }
                    }
                    else if (element == Element.snake)
                    {
                        _world[x, y].SetSnake(x, y);
                        _snakes.Add(_world[x, y].GetSnake());
                    }
                    _world[x, y].SetElement(element);
                }
            }


            return;
        }

        /// <summary>
        /// Returns if box at location can move down 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveDown(int x, int y)
        {
            if (x < _world.GetLength(0) && x >= 0 &&
                y < _world.GetLength(1) && y >= 0)
            {

                // these could be really big booleans, but this is more readable
                if (y + 1 < _world.GetLength(1))
                {
                    if (_world[x, y + 1].GetElement() == Element.none)
                    {
                        return true;
                    }
                    else if (_world[x, y].GetElement() == Element.snake && _world[x, y + 1].GetElement() == Element.sand)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns if box at location can move left 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveLeft(int x, int y)
        {

            if (x < _world.GetLength(0) && x >= 0 &&
                y < _world.GetLength(1) && y >= 0)
            {
                if (x - 1 >= 0)
                {
                    if (_world[x - 1, y].GetElement() == Element.none)
                    {
                        return true;
                    }
                    else if (_world[x, y].GetElement() == Element.snake && _world[x - 1, y].GetElement() == Element.sand)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns if box at location can move right 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveRight(int x, int y)
        {

            if (x < _world.GetLength(0) && x >= 0 &&
                y < _world.GetLength(1) && y >= 0)
            {
                if (x + 1 < _world.GetLength(0))
                {
                    if (_world[x + 1, y].GetElement() == Element.none)
                    {
                        return true;
                    }
                    else if (_world[x, y].GetElement() == Element.snake && _world[x + 1, y].GetElement() == Element.sand)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns if box at location can move up 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveUp(int x, int y)
        {
            if (x < _world.GetLength(0) && x >= 0 &&
                y < _world.GetLength(1) && y >= 0)
            {
                if (y - 1 >= 0)
                {
                    if (_world[x, y - 1].GetElement() == Element.none)
                    {
                        return true;
                    }
                    else if (_world[x, y].GetElement() == Element.snake && _world[x, y - 1].GetElement() == Element.sand)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns if box at location can move down and left 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveDownLeft(int x, int y)
        {
            if (x < _world.GetLength(0) && x >= 0 &&
                y < _world.GetLength(1) && y >= 0)
            {
                return (x - 1 >= 0 && y + 1 < _world.GetLength(1) &&
                    _world[x - 1, y + 1].GetElement() == Element.none &&
                    // corner rule
                    _world[x - 1, y].GetElement() == Element.none);
            }
            return false;
        }

        /// <summary>
        /// Returns if box at location can move down and right 1 step
        /// </summary>
        /// <param name="x">x coordinate of box</param>
        /// <param name="y">y coordinate of box</param>
        /// <returns></returns>
        public bool CanMoveDownRight(int x, int y)
        {
            if (x < _world.GetLength(0) && x >= 0 &&
                y < _world.GetLength(1) && y >= 0)
            {
                return (x + 1 < _world.GetLength(0) && y + 1 < _world.GetLength(1) &&
                    _world[x + 1, y + 1].GetElement() == Element.none &&
                    // corner rule
                    _world[x + 1, y].GetElement() == Element.none);
            }
            return false;
        }

        /// <summary>
        /// Compares two grids for testing purposes
        /// </summary>
        /// <param name="testWorld">Grid comparing against</param>
        /// <returns>Are the contents of the grid elements the same?</returns>
        public bool IsWorldCopy(Grid testWorld)
        {
            for (int y = 0; y < testWorld.GetLength(1); y++)
            {
                for (int x = 0; x < testWorld.GetLength(0); x++)
                {
                    if (testWorld[x, y].GetElement() != _world[x, y].GetElement())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Copy constructor--may not be necessary
        /// </summary>
        /// <returns></returns>
        public Grid Copy()
        {
            Grid worldCopy = new Grid(GetLength(0), GetLength(1));
            for (int y = 0; y < worldCopy.GetLength(1); y++)
            {
                for (int x = 0; x < worldCopy.GetLength(0); x++)
                {
                    worldCopy[x, y] = _world[x, y].Copy();
                }
            }
            return worldCopy;
        }

        /// <summary>
        /// Returns length of grid
        /// </summary>
        /// <param name="x">0 for width, 1 for height</param>
        /// <returns>Grid length</returns>
        public int GetLength(int x)
        {
            return _world.GetLength(x);
        }

        /// <summary>
        /// Returns al snakes in the world
        /// </summary>
        /// <returns>Snake set</returns>
        public HashSet<Snake> GetSnakes()
        {
            return _snakes;
        }
    }
}
