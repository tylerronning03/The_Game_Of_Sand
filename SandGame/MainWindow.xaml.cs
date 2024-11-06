using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.CodeDom;


namespace SandGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _selectedButton = "";
        private bool _gravity = true;
        private float _brownian = 0.5F;
        private float _brownianSlider = 0.0f;
        private int _gridRows = 50;
        private int _gridColumns = 50;
        private int _xOffset = 0;
        private int _yOffset = 0;
        private int _sideLength = 10;
        private DateTime _lastTime = DateTime.Now;
        private bool _brownianBool = true;
        private Rectangle[,] _grid;
        private Engine gameEngine;
        private int _mouseX;
        private int _mouseY;
        private bool _mouseDown;
        private int[] _lastCord = [0, 0];
        private int _elipseRadius = 30;

        /// <summary>
        /// Inital set up of values, and timer to callback Tick Function
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Set's up gameGrid
            _grid = MakeGrid(_gridColumns, _gridRows);
            // create GameEngine, and sets it's Gravity and Brownian
            gameEngine = new Engine(_gridColumns, _gridRows);
            gameEngine.SetGravity(_gravity);
            gameEngine.SetBrownian(_brownian);

            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (s, e) => Tick();
            timer.Interval = (TimeSpan.FromSeconds(1.0 / 60.0));
            Loaded += (s, e) => timer.Start();
            FPS.Text = "0";

        }

        // *********************************************************************** Methods ************************************************************************************************************

        /// <summary>
        /// Function to be called back every timer.interval occurance
        /// Calls the functions that drive the UI and advance the GameEngine
        /// </summary>
        private void Tick()
        {
            // FPS counter
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            TimeSpan delta = DateTime.Now - _lastTime;
            _lastTime = DateTime.Now;

            // checks mouse left click status
            if (_mouseDown)
            {
                // Depending on mouse inputs, call needed function
                if (_selectedButton == "Sand" || _selectedButton == "Rock" || _selectedButton == "Snake" || _selectedButton == "Magma")
                {
                    PlaceRect();
                }
                else if (_selectedButton == "Erase")
                {
                    EraseSingle();
                }
                else if (_selectedButton == "BigErase")
                {
                    EraseBig();
                }
            }


            if (delta.Milliseconds != 0)
            {
                FPS.Text = $" {(1000 / delta.Milliseconds)}";
            }
            milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // updating the UI visual then Ticking Engine
            UpdateGrid();
            gameEngine.Tick();

        }

        /// <summary>
        /// When Called, tells the GameEngine to place a block at a given position
        /// </summary>
        private void PlaceRect()
        {

            int[] i = CalcPos();
            //Console.WriteLine($"Placing {elem}, at {i[1]}, {i[0]}");
            if (_selectedButton == "Sand")
            {
                gameEngine.SetObject(i[0], i[1], Element.sand);
            }
            else if (_selectedButton == "Rock")
            {
                gameEngine.SetObject(i[0], i[1], Element.rock);

            }
            else if (_selectedButton == "Snake")
            {
                gameEngine.SetObject(i[0], i[1], Element.snake);
            }
            else if (_selectedButton == "Magma")
            {
                gameEngine.SetObject(i[0], i[1], Element.magma);
            }

        }

        /// <summary>
        /// Erases a single block from the board
        /// </summary>
        private void EraseSingle()
        {
            // remove color from source block
            // point.X/10 =  in terms of our blocks, but we need a range


            int[] i = CalcPos();
            gameEngine.SetObject(i[0], i[1], Element.none);

            gameEngine.SetObject(i[0], i[1], Element.none);

        }

        /// <summary>
        /// Performs the big Erase function, erasing a 5x5 section at once
        /// </summary>
        private void EraseBig()
        {
            int[] i = CalcPos();

            for (int x = i[0] - 2; x <= i[0] + 2; x++)
            {
                for (int y = i[1] - 2; y <= i[1] + 2; y++)
                {
                    if (x >= 0 && x < _gridColumns && y >= 0 && y < _gridRows)

                        if (_grid[x, y].Fill != Brushes.White)
                        {
                            gameEngine.SetObject(x, y, Element.none);
                        }
                }
            }
        }

        /// <summary>
        /// position of the mouse - the offset from where the world determines 0,0 is / side length rounded down
        /// will give is the box that the mouse is over
        /// </summary>
        /// <returns>list[0] = x, list[1] = y</returns>
        private int[] CalcPos()
        {
            int x = (int)Math.Floor(((double)_mouseX - _xOffset) / _sideLength);
            int y = (int)Math.Floor(((double)_mouseY - _yOffset) / _sideLength);
            return [x, y];
        }

        /// <summary>
        /// Fills a 2D array with white rectangles and populates the GameGrid object with the Rectangles
        /// </summary>
        /// <param name="cols">What will be turned into the x value</param>
        /// <param name="rows">What will be turned int0 the y value</param>
        /// <returns>Returns a 2D array of Rectangles</returns>
        private Rectangle[,] MakeGrid(int cols, int rows)
        {
            // creating and filling the gameGrid with Rectangles
            Rectangle[,] newArray = new Rectangle[cols, rows];
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Rectangle newRect = new Rectangle();
                    newRect.Fill = Brushes.White;

                    newRect.Height = _sideLength;
                    newRect.Width = _sideLength;
                    Canvas.SetLeft(newRect, i * _sideLength);
                    Canvas.SetTop(newRect, j * _sideLength);

                    GameGrid.Children.Add(newRect);
                    newArray[i, j] = newRect;
                }
            }

            return newArray;
        }

        /// <summary>
        /// Called to update the UI with the new world data from the engine
        /// </summary>
        private void UpdateGrid()
        {
            Grid world = gameEngine.GetWorldData();
            for (int i = 0; i < _gridColumns; i++)
            {
                for (int j = 0; j < _gridRows; j++)
                {
                    Element currElement = world[i, j].GetElement();
                    if (currElement == Element.sand)
                    {
                        if (_grid[i, j].Fill != Brushes.Gold)
                        {
                            // set color of rectange
                            _grid[i, j].Fill = Brushes.Gold;
                        }

                    }
                    else if (currElement == Element.rock)
                    {
                        if (_grid[i, j].Fill != Brushes.DimGray)
                        {
                            _grid[i, j].Fill = Brushes.DimGray;
                        }
                    }

                    else if (currElement == Element.none)
                    {
                        if (_grid[i, j].Fill != Brushes.White)
                        {
                            _grid[i, j].Fill = Brushes.White;
                        }
                    }
                    else if (currElement == Element.snake)
                    {
                        if (world[i, j].IsHead())
                        {
                            _grid[i, j].Fill = Brushes.DarkSlateGray;
                        }
                        else
                        {
                            _grid[i, j].Fill = Brushes.ForestGreen;
                        }

                    }
                    else if (currElement == Element.magma)
                    {
                        if (_grid[i, j].Fill != Brushes.OrangeRed)
                        {
                            _grid[i, j].Fill = Brushes.OrangeRed;
                        }
                    }
                    else if (currElement == Element.glass)
                    {
                        if (_grid[i, j].Fill != Brushes.Lavender)
                        {
                            _grid[i, j].Fill = Brushes.Lavender;
                        }
                    }

                }
            }

        }

        // ************************************************************************ Event Handlers ***********************************************************************************************************

        /// <summary>
        /// Gets the name of the type of block currently selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton? selectedRadioButton = sender as RadioButton;
            if (selectedRadioButton != null)
            {
                _selectedButton = selectedRadioButton.Name;
            }

        }

        /// <summary>
        /// When Checked, gravit acts as per the GameEngine rules.
        /// When Unchecked, objects acted upon by gravity float.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GravityButton_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox? gravityButton = sender as CheckBox;
            if (gravityButton.IsChecked == true)
            {
                _gravity = true;
            }
            else
            {
                _gravity = false;
            }
            if (gameEngine != null)
            {

                gameEngine.SetGravity(_gravity);
            }
        }

        /// <summary>
        /// When the Slider is moved, this is triggered, updating _brownian
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Brownian_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider? brownianSlider = sender as Slider;
            if (_brownianBool)
            {
                _brownian = (float)brownianSlider.Value;
                _brownianSlider = _brownian;
            }
            else
            {
                _brownian = 0.0f;
            }
            if (gameEngine != null)
            {
                gameEngine.SetBrownian(_brownian);

            }

        }

        /// <summary>
        /// When the mouse is moved in the GameGrid space, it will update its X,Y here for use in the program.
        /// Adds a "Shadow" (boarder) to the current box the mouse is over
        /// Updates the location of the Elipse representing BigErase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = Mouse.GetPosition(GameGrid);
            _mouseX = (int)mousePos.X;
            _mouseY = (int)mousePos.Y;

            int[] i = CalcPos();
            if ((i[0] != _lastCord[0] || i[1] != _lastCord[1]) && _selectedButton != "BigErase" && (i[0] < _gridColumns && i[0] >= 0 && i[1] >= 0 && i[1] < _gridRows))
            {
                _grid[_lastCord[0], _lastCord[1]].Stroke = Brushes.White;
                _grid[_lastCord[0], _lastCord[1]].StrokeThickness = 0;
                _lastCord = i;
                _grid[i[0], i[1]].Stroke = Brushes.Gray;
                _grid[i[0], i[1]].StrokeThickness = 1;

            }
            else if (_selectedButton == "BigErase")
            {
                _grid[_lastCord[0], _lastCord[1]].Stroke = Brushes.White;
                _grid[_lastCord[0], _lastCord[1]].StrokeThickness = 0;
            }

            if (_mouseX < _gridColumns * _sideLength - (_elipseRadius + 3) && _mouseX >= _elipseRadius)
            {
                Canvas.SetLeft(bigE, _mouseX - _xOffset - 30);
            }
            if (_mouseY < _gridRows*_sideLength - (_elipseRadius + 3) && _mouseY >= _elipseRadius)
            {
                Canvas.SetTop(bigE, _mouseY - _yOffset - 30);
            }

            //Console.WriteLine($"Mouse Pos: X = {mousePos.X}, Y = {mousePos.Y}");
        }

        /// <summary>
        /// handles left click being pressed and passing for use in placing blocks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameGrid_MouseleftDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
            if (_selectedButton == "BigErase")
            {
                bigE.Visibility = Visibility.Visible;
            }

            //Console.WriteLine("Left Click DOWN");
        }

        /// <summary>
        /// Handles Leftclick being released, sets _mouseDown to false, so no blocks will be placed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
            if (_selectedButton == "BigErase")
            {
                bigE.Visibility = Visibility.Collapsed;
            }
            // Console.WriteLine("Left Click UP");
        }

        /// <summary>
        /// Whent the Brownian Check Box is checked, brownian will act as usual
        /// When the box is Unchecked, Brownian will be passed as 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrownianCheck_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox? brownButton = sender as CheckBox;
            if (brownButton.IsChecked == true)
            {
                _brownianBool = true;

                if (gameEngine != null)
                {
                    gameEngine.SetBrownian(_brownianSlider);
                }
            }
            else
            {
                _brownianBool = false;
                _brownian = 0.0f;
                gameEngine.SetBrownian(_brownian);

            }
        }
    }



}