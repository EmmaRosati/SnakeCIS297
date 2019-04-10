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
using Windows.System;

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
        private Window mainWindow;

        public void TestToMakeSureBodySegmentsAreGoingToRightSpot()
        {
            if (snake.covers.Count != 0 && snake.bodySegments[0].distancesTillTurns.Count != 0)
            {
                if (snake.bodySegments[0].goingUp)
                {
                    if (!((snake.bodySegments[0].y - snake.bodySegments[0].distancesTillTurns[0]) == snake.covers[0].Y))
                    {
                        throw new Exception("Snake is unaligned.");
                    }
                }

                else if (snake.bodySegments[0].goingDown)
                {
                    if (!((snake.bodySegments[0].y + snake.bodySegments[0].distancesTillTurns[0]) == snake.covers[0].Y))
                    {
                        throw new Exception("Snake is unaligned.");
                    }
                }

                else if (snake.bodySegments[0].goingLeft)
                {
                    if (!((snake.bodySegments[0].x - snake.bodySegments[0].distancesTillTurns[0]) == snake.covers[0].X))
                    {
                        throw new Exception("Snake is unaligned.");
                    }
                }

                if (snake.bodySegments[0].goingRight)
                {
                    if (!((snake.bodySegments[0].x + snake.bodySegments[0].distancesTillTurns[0]) == snake.covers[0].X))
                    {
                        throw new Exception("Snake is unaligned.");
                    }
                }
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            snake = new Snake();
            mainWindow = Window.Current;
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

            bool isLeftArrowPressed = mainWindow.CoreWindow.GetKeyState(VirtualKey.Left).HasFlag(CoreVirtualKeyStates.Down);
            bool isRightArrowPressed = mainWindow.CoreWindow.GetKeyState(VirtualKey.Right).HasFlag(CoreVirtualKeyStates.Down);
            bool isUpArrowPressed = mainWindow.CoreWindow.GetKeyState(VirtualKey.Up).HasFlag(CoreVirtualKeyStates.Down);
            bool isDownArrowPressed = mainWindow.CoreWindow.GetKeyState(VirtualKey.Down).HasFlag(CoreVirtualKeyStates.Down);
        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            //Update Game
            snake.updateGame();
            checkForKeyPress();
            TestToMakeSureBodySegmentsAreGoingToRightSpot();
        }

        private void checkForKeyPress()
        {
            var a = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().CoreWindow.Dispatcher;
            a.

            bool isRightArrowPressed = false;
            bool isUpArrowPressed = false;
            bool isDownArrowPressed = false;

            //Left key is pressed when the user was not already going left or right
            if (isLeftArrowPressed && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft)
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
            else if (isRightArrowPressed && !snake.snakeHead.goingLeft && !snake.snakeHead.goingRight)
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
            else if (isDownArrowPressed && !snake.snakeHead.goingUp && !snake.snakeHead.goingDown)
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
            else if (isUpArrowPressed && !snake.snakeHead.goingDown && !snake.snakeHead.goingUp)
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