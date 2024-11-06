using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandGame
{
    internal class Snake
    {

        private List<(int x, int y)> _body;
        private int _head;

        // used for debugging
        Direction _direction;

        public Snake(int x, int y)
        {
            _body = new List<(int x, int y)>();
            _body.Add((x, y));
            _head = 0;
        }

        /// <summary>
        /// Sets direction snake is facing
        /// </summary>
        /// <param name="direction"></param>
        public void SetDirection(Direction direction)
        {
            _direction = direction;
        }



        /// <summary>
        /// Deletes all snake segments from this snake in the world
        /// </summary>
        /// <param name="world">Grid to delete from</param>
        public void DeleteFrom(Grid world)
        {
            foreach ((int x, int y) in _body)
            {
                world[x, y].SetSnake(null);
                world[x, y].SetElement(Element.none);
            }
        }

        /// <summary>
        /// Move the snake towards (x, y)
        /// </summary>
        /// <param name="x">New head x-coordinate</param>
        /// <param name="y">New head y-coordinate</param>
        public void Move(int x, int y)
        {
            for (int i = 0; i < _head; i++)
            {
                _body[i] = (_body[i + 1].x, _body[i + 1].y);
            }
            _body[_head] = (x, y);
        }

        /// <summary>
        /// Add a new head segment at the front of the snake
        /// </summary>
        /// <param name="x">New head x-coordinate</param>
        /// <param name="y">New head y-coordinate</param>
        public void Eat(int x, int y)
        {
            _head++;
            _body.Add((x, y));
        }

        /// <summary>
        /// Turns snake in world into sand
        /// </summary>
        /// <param name="world">World snake is a member of</param>
        public void BurnFrom(Grid world, int magmaX, int magmaY)
        {
            foreach ((int x, int y) in _body)
            {
                world[x, y].SetSnake(null);
                // if touching magma
                if ((x - 1, y - 1) == (magmaX, magmaY) ||
                    (x, y - 1) == (magmaX, magmaY) ||
                    (x + 1, y - 1) == (magmaX, magmaY) ||
                    (x - 1, y) == (magmaX, magmaY) ||
                    (x + 1, y) == (magmaX, magmaY) ||
                    (x - 1, y + 1) == (magmaX, magmaY) ||
                    (x, y + 1) == (magmaX, magmaY) ||
                    (x + 1, y + 1) == (magmaX, magmaY))
                {
                    world[x, y].SetElement(Element.glass);
                }
                else
                {

                    world[x, y].SetElement(Element.sand);
                }
            }
            world.GetSnakes().Remove(this);
        }

        /// <summary>
        /// Return the List of snake coordinates
        /// </summary>
        /// <returns>Snake body</returns>
        public List<(int x, int y)> GetBody()
        {
            return _body;
        }

        /// <summary>
        /// Returns index of head segment
        /// </summary>
        /// <returns>Head index</returns>
        public int GetHeadIndex()
        {
            return _head;
        }
    }
}
