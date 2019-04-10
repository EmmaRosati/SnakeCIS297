using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Game Object
        private Snake snake;

        public void TestForErrors()
        {
            bool allRight = true;
            bool allSameY = true;
            int YCordOfSnakeHead = snake.snakeHead.y;

            for (int i = 0; i < snake.bodySegments.Count; ++i)
            {
                if (snake.bodySegments[i].goingRight == false)
                {
                    allRight = false;
                    break;
                }
            }

            if (allRight)
            {
                if (snake.snakeHead.goingRight == false)
                {
                    allRight = false;
                }
            }

            for (int i = 0; i < snake.bodySegments.Count; ++i)
            {
                if(snake.bodySegments[i].y != YCordOfSnakeHead)
                {
                    allSameY = false;
                    break;
                }
            }

            if (allRight && !allSameY)
            {
                throw new Exception("Aha!");
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            snake = new Snake();

            //Add method to keydown event
            Window.Current.CoreWindow.KeyDown += Canvas_KeyDown;
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            //Draw Black Background
            Rect rect = new Rect();
            rect.X = 0;
            rect.Y = 0;
            rect.Width = 800;
            rect.Height = 800;

            args.DrawingSession.DrawRectangle(rect, Colors.Black);
            args.DrawingSession.FillRectangle(rect, Colors.Black);

            //Draw Game
            snake.drawGame(args.DrawingSession);
        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            //Update Game
            TestForErrors();
            snake.updateGame();
        }

        //Runs when key is pressed down.
        private void Canvas_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            //Set justTurned to true so that the snake doesn't become unaligned.
            snake.snakeHead.Turning();

            //Left key is pressed when the user was not already going left or right
            if (e.VirtualKey == Windows.System.VirtualKey.Left && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft)
            {
                //Put a cover over turn
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                //Change direction
                snake.snakeHead.goingLeft = true;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;

                //Number of blocks lined up with snake head when it turns
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                //Calcuate how far each body segment should travel and which direction it should go
                //When it gets there
                foreach (BodySegment bs in snake.bodySegments)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        bs.distancesTillTurns.Add(bodySegmentNumber * 20);
                        bs.waysToTurn.Enqueue(direction.L);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        bs.distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        bs.waysToTurn.Enqueue(direction.L);
                    }
                }

                //Reset distanceSinceLastTurn since we just turned
                snake.snakeHead.distanceSinceLastTurn = 0;
            }

            //Right key is pressed when we are not going right or left
            else if (e.VirtualKey == Windows.System.VirtualKey.Right && !snake.snakeHead.goingLeft && !snake.snakeHead.goingRight)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = true;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                foreach (BodySegment bs in snake.bodySegments)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        bs.distancesTillTurns.Add(bodySegmentNumber * 20);
                        bs.waysToTurn.Enqueue(direction.R);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        bs.distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        bs.waysToTurn.Enqueue(direction.R);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;
            }

            //Down key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Down && !snake.snakeHead.goingUp && !snake.snakeHead.goingDown)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = true;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                foreach (BodySegment bs in snake.bodySegments)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        bs.distancesTillTurns.Add(bodySegmentNumber * 20);
                        bs.waysToTurn.Enqueue(direction.D);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        bs.distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        bs.waysToTurn.Enqueue(direction.D);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;
            }

            //Up key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Up && !snake.snakeHead.goingDown && !snake.snakeHead.goingUp)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = true;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                foreach (BodySegment bs in snake.bodySegments)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        bs.distancesTillTurns.Add(bodySegmentNumber * 20);
                        bs.waysToTurn.Enqueue(direction.U);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        bs.distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        bs.waysToTurn.Enqueue(direction.U);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;
            }
        }
    }
}