[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/bDuW-ExC)
# Adv-CS-Lab-Project2

## Usage
To run:
* Execute /SandGame/SandGame.sln and build SandGame through the build tab.
* Navigate to the generated /bin folder and run the generated .exe

## Testing
To perform unit tests, open the Test tab in Visual Studio and run tests through the Test Explorer.

## Elements
* **Sand** follows laws of gravity and always falls down, down-left, or down-right, if possible. Sand is subject to gravity and brownian forces.
* **Rock** is not affected by gravity and can hold forces at bay.
* **Snake** traverses the map randomly and can eat sand to grow. Snake picks a random direction each frame and if it's a legal movement option, takes the move.
* **Magma** burns whatever touches it, turning snakes to sand and sand to glass.

## Work Delegation
* Daniel worked on anything that directly touched the world data structure. This includes files Box.cs, Direction.cs, Element.cs, Grid.cs, Snake.cs. Daniel also worked on the engine unit tests, EngineTest.cs.
* Tyler worked on anything that was shown on screen. This includes the xaml file MainWindow.xaml as well as the draw script MainWindow.xaml.cs, which correctly implements world API.
