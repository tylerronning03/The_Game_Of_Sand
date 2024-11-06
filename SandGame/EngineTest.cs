using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SandGame
{

    [TestClass]
    public class EngineTest
    {

        /// <summary>
        /// Tests if gravity can correctly make objects move down, down-left, and down-right
        /// </summary>
        [TestMethod]
        public void TestGravity()
        {
            Engine engine = new Engine(3, 3);
            engine.SetGravity(true);
            Grid testWorld = new Grid(3, 3);
            for (int y = 0; y < engine.GetWorldData().GetLength(1); y++)
            {
                for (int x = 0; x < engine.GetWorldData().GetLength(0); x++)
                {
                    testWorld[x, y] = new Box();
                }
            }

            // Gravity Down
            engine.SetObject(1, 1, Element.sand);
            engine.Tick();
            testWorld.SetObject(1, 2, Element.sand);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

            // Gravity Down Left
            engine.SetObject(1, 0, Element.sand);
            engine.Tick();
            engine.Tick();
            testWorld.SetObject(0, 2, Element.sand);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

            // Gravity Down Right
            engine.SetObject(1, 0, Element.sand);
            engine.Tick();
            engine.Tick();
            testWorld.SetObject(2, 2, Element.sand);

            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

        }

        /// <summary>
        /// Tests if multiple objects stacked on top of each other respond to gravity correctly
        /// </summary>
        [TestMethod]
        public void TestStackGravity()
        {
            Engine engine = new Engine(1, 4);
            engine.SetGravity(true);
            Grid testWorld = new Grid(1, 4);
            for (int y = 0; y < engine.GetWorldData().GetLength(1); y++)
            {
                for (int x = 0; x < engine.GetWorldData().GetLength(0); x++)
                {
                    testWorld[x, y] = new Box();
                }
            }

            engine.SetObject(0, 1, Element.sand);
            engine.SetObject(0, 2, Element.sand);
            engine.Tick();

            testWorld.SetObject(0, 2, Element.sand);
            testWorld.SetObject(0, 3, Element.sand);

            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));
        }

        /// <summary>
        /// Tests if the corner rule if correctly followed
        /// </summary>
        [TestMethod]
        public void TestCornerRule()
        {
            Engine engine = new Engine(3, 3);
            engine.SetGravity(true);
            Grid testWorld = new Grid(3, 3);
            for (int y = 0; y < engine.GetWorldData().GetLength(1); y++)
            {
                for (int x = 0; x < engine.GetWorldData().GetLength(0); x++)
                {
                    testWorld[x, y] = new Box();
                }
            }

            engine.SetObject(0, 1, Element.rock);
            engine.SetObject(2, 1, Element.rock);
            engine.SetObject(1, 2, Element.sand);
            engine.SetObject(1, 0, Element.sand);
            engine.Tick();
            engine.Tick();

            testWorld.SetObject(0, 1, Element.rock);
            testWorld.SetObject(2, 1, Element.rock);
            testWorld.SetObject(1, 2, Element.sand);
            testWorld.SetObject(1, 1, Element.sand);

            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));
        }

        /// <summary>
        /// Tests if Brownian behavior is correctly followed
        /// </summary>
        [TestMethod]
        public void TestBrownian()
        {
            Engine engine = new Engine(4, 4);
            engine.SetGravity(true);
            engine.SetBrownian(.5f);
            Grid testWorld = new Grid(4, 4);
            for (int y = 0; y < engine.GetWorldData().GetLength(1); y++)
            {
                for (int x = 0; x < engine.GetWorldData().GetLength(0); x++)
                {
                    testWorld[x, y] = new Box();
                }
            }

            // Brownian Left
            engine.SetObject(1, 2, Element.sand);
            engine.ApplyBrownian(.4f, 0);
            testWorld.SetObject(0, 2, Element.sand);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

            // Brownian Right
            engine.ApplyBrownian(.4f, 1);
            testWorld.SetObject(0, 2, Element.none);
            testWorld.SetObject(1, 2, Element.sand);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

            // Brownian Not
            engine.ApplyBrownian(.5f, 1);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

            // Brownian multiple next to each other
            engine.SetObject(2, 2, Element.sand);
            engine.ApplyBrownian(0f, 1);
            testWorld.SetObject(3, 2, Element.sand);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

        }

        /// <summary>
        /// Tests if the snake moves as expected
        /// </summary>
        [TestMethod]
        public void TestSnakeMove()
        {
            Engine engine = new Engine(3, 3);
            engine.SetGravity(true);
            Grid testWorld = new Grid(3, 3);
            for (int y = 0; y < engine.GetWorldData().GetLength(1); y++)
            {
                for (int x = 0; x < engine.GetWorldData().GetLength(0); x++)
                {
                    testWorld[x, y] = new Box();
                }
            }

            // Basic move
            engine.SetObject(1, 1, Element.snake);
            engine.MoveSnakesDebug(Direction.Up);
            testWorld.SetObject(1, 0, Element.snake);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

            // Eat sand
            engine.SetObject(1, 1, Element.sand);
            engine.MoveSnakesDebug(Direction.Down);
            testWorld.SetObject(1, 1, Element.snake);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

            // Stop at rock
            engine.SetObject(1, 2, Element.rock);
            engine.MoveSnakesDebug(Direction.Down);
            testWorld.SetObject(1, 2, Element.rock);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

            // No move into self
            engine.MoveSnakesDebug(Direction.Left);
            engine.MoveSnakesDebug(Direction.Right);
            testWorld.SetObject(1, 0, Element.none);
            testWorld.SetObject(0, 1, Element.snake);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));
        }

        [TestMethod]
        public void TestMagma()
        {
            Engine engine = new Engine(3, 3);
            engine.SetGravity(true);
            Grid testWorld = new Grid(3, 3);
            for (int y = 0; y < engine.GetWorldData().GetLength(1); y++)
            {
                for (int x = 0; x < engine.GetWorldData().GetLength(0); x++)
                {
                    testWorld[x, y] = new Box();
                }
            }


            // test full glassification of snake
            engine.SetObject(0, 0, Element.snake);
            engine.SetObject(0, 1, Element.sand);
            engine.MoveSnakesDebug(Direction.Down);
            engine.SetObject(1, 1, Element.magma);
            engine.Tick();
            testWorld.SetObject(1, 1, Element.magma);
            testWorld.SetObject(0, 1, Element.glass);
            testWorld.SetObject(0, 0, Element.glass);
            Assert.IsTrue(engine.GetWorldData().IsWorldCopy(testWorld));

        }
    }
}
