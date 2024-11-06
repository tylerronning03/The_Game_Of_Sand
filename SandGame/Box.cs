using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SandGame
{
    internal class Box
    {

        private Element _element;
        private Snake? _snake;
        private bool _isHead;

        public Box()
        {
            // None represents an empty square
            _element = Element.none;
            _snake = null;
        }

        public Box(Element element)
        {
            _element = element;
            _snake = null;
        }

        public Box(Element element, Snake snake)
        {
            _element = element;
            _snake = snake;
        }

        /// <summary>
        /// Returns box element
        /// </summary>
        /// <returns></returns>
        public Element GetElement()
        {
            return _element;
        }

        /// <summary>
        /// Set the element of a box
        /// </summary>
        /// <param name="element">Name of the element</param>
        /// <exception cref="ArgumentException">Invalid element</exception>
        public void SetElement(Element element)
        {
            _element = element;
        }

        /// <summary>
        /// Returns the snake object in the box, if a snake exists.
        /// </summary>
        /// <returns>Snake object</returns>
        public Snake GetSnake()
        {
            return _snake;
        }

        /// <summary>
        /// Creates a snake object at this location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetSnake(int x, int y)
        {
            _snake = new Snake(x, y);
        }

        /// <summary>
        /// Sets the snake object in the box.
        /// </summary>
        /// <param name="snake"></param>
        public void SetSnake(Snake snake)
        {
            _snake = snake;
        }
        /// <summary>
        /// Returns if box element is a solid object--not needed?
        /// </summary>
        /// <returns></returns>
        public bool IsSolid()
        {
            return _element == Element.rock || _element == Element.magma || _element == Element.glass || _element == Element.snake;
        }

        /// <summary>
        /// Set the flag indicating if this box is the head of a snake.
        /// </summary>
        /// <param name="isHead">Box is head of a snake</param>
        public void SetIsHead(bool isHead)
        {
            _isHead = isHead;
        }

        /// <summary>
        /// Returns if this box is the head of a snake
        /// </summary>
        /// <returns>Box is head of a snake</returns>
        public bool IsHead()
        {
            return _isHead;
        }

        /// <summary>
        /// Moves snake
        /// </summary>
        public void Move()
        {
            if (_element != Element.snake)
            {
                return;
            }
        }

        /// <summary>
        /// Return a copy of a box
        /// </summary>
        /// <returns>Box copy</returns>
        public Box Copy()
        {
            Box copyBox = new Box(_element, _snake);
            return copyBox;
        }
    }
}
