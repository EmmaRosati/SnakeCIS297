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
        private bool cantChangeDirection;
        private bool stopUpdatingGame;

        public MainPage()
        {
            this.InitializeComponent();
            snake = new Snake();

            //Add method to keydown event
            Window.Current.CoreWindow.KeyDown += Canvas_KeyDown;
            cantChangeDirection = false;
            stopUpdatingGame = false;
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            //Draw Black Background
            Rect rect = new Rect();
            rect.X = 0;
            rect.Y = 0;
            rect.Width = 500;
            rect.Height = 500;

            args.DrawingSession.DrawRectangle(rect, Colors.Black);
            args.DrawingSession.FillRectangle(rect, Colors.Black);

            //Draw Game
            snake.drawGame(args.DrawingSession);
        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            if (!stopUpdatingGame)
            {
                snake.updateGame();
            }
        }

        //Runs when key is pressed down.
        private void Canvas_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            //Left key is pressed when the user was not already going left or right
            if (e.VirtualKey == Windows.System.VirtualKey.Left && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft && !cantChangeDirection)
            {
                cantChangeDirection = true;
                stopUpdatingGame = true;

                //Put a cover over turn
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                //Change direction
                snake.snakeHead.goingLeft = true;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;

                //Number of blocks lined up with snake head when it turns
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 5;

                //Calcuate how far each body segment should travel and which direction it should go
                //When it gets there
                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 5);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.L);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.L);
                    }
                }

                //Reset distanceSinceLastTurn since we just turned
                snake.snakeHead.distanceSinceLastTurn = 0;

                BodySegment firstBS = snake.bodySegments[0];
                Head snakeHead = snake.snakeHead;

                if (snake.covers.Count != 0 && snake.bodySegments[0].distancesTillTurns.Count >= 1)
                {
                    if (firstBS.goingUp)
                    {
                        if (!((firstBS.y - firstBS.distancesTillTurns[0]) == snakeHead.y))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingDown)
                    {
                        if (!((firstBS.y + firstBS.distancesTillTurns[0]) == snakeHead.y))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingLeft)
                    {
                        if (!((firstBS.x - firstBS.distancesTillTurns[0]) == snakeHead.x))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingRight)
                    {
                        if (!((firstBS.x + firstBS.distancesTillTurns[0]) == snakeHead.x))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }
                }

                cantChangeDirection = false;
                stopUpdatingGame = false;
            }

            //Right key is pressed when we are not going right or left
            else if (e.VirtualKey == Windows.System.VirtualKey.Right && !snake.snakeHead.goingLeft && !snake.snakeHead.goingRight && !cantChangeDirection)
            {
                cantChangeDirection = true;
                stopUpdatingGame = true;

                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = true;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 5;


                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 5);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.R);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.R);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;

                BodySegment firstBS = snake.bodySegments[0];
                Head snakeHead = snake.snakeHead;

                if (snake.covers.Count != 0 && snake.bodySegments[0].distancesTillTurns.Count >= 1)
                {
                    if (firstBS.goingUp)
                    {
                        if (!((firstBS.y - firstBS.distancesTillTurns[0]) == snakeHead.y))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingDown)
                    {
                        if (!((firstBS.y + firstBS.distancesTillTurns[0]) == snakeHead.y))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingLeft)
                    {
                        if (!((firstBS.x - firstBS.distancesTillTurns[0]) == snakeHead.x))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingRight)
                    {
                        if (!((firstBS.x + firstBS.distancesTillTurns[0]) == snakeHead.x))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }
                }

                cantChangeDirection = false;
                stopUpdatingGame = false;
            }

            //Down key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Down && !snake.snakeHead.goingUp && !snake.snakeHead.goingDown && !cantChangeDirection)
            {
                cantChangeDirection = true;
                stopUpdatingGame = true;

                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = true;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 5;

                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 5);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.D);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.D);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;

                BodySegment firstBS = snake.bodySegments[0];
                Head snakeHead = snake.snakeHead;

                if (snake.covers.Count != 0 && snake.bodySegments[0].distancesTillTurns.Count >= 1)
                {
                    if (firstBS.goingUp)
                    {
                        if (!((firstBS.y - firstBS.distancesTillTurns[0]) == snakeHead.y))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingDown)
                    {
                        if (!((firstBS.y + firstBS.distancesTillTurns[0]) == snakeHead.y))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingLeft)
                    {
                        if (!((firstBS.x - firstBS.distancesTillTurns[0]) == snakeHead.x))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingRight)
                    {
                        if (!((firstBS.x + firstBS.distancesTillTurns[0]) == snakeHead.x))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }
                }

                cantChangeDirection = false;
                stopUpdatingGame = false;
            }

            //Up key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Up && !snake.snakeHead.goingDown && !snake.snakeHead.goingUp && !cantChangeDirection)
            {
                cantChangeDirection = true;
                stopUpdatingGame = true;

                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = true;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 5;

                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 5);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.U);
                        ++bodySegmentNumber;
                    }

                    else
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(snake.snakeHead.distanceSinceLastTurn);
                        snake.bodySegments[i].waysToTurn.Enqueue(direction.U);
                    }
                }

                snake.snakeHead.distanceSinceLastTurn = 0;

                BodySegment firstBS = snake.bodySegments[0];
                Head snakeHead = snake.snakeHead;

                if (snake.covers.Count != 0 && snake.bodySegments[0].distancesTillTurns.Count >= 1)
                {
                    if (firstBS.goingUp)
                    {
                        if (!((firstBS.y - firstBS.distancesTillTurns[0]) == snakeHead.y))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingDown)
                    {
                        if (!((firstBS.y + firstBS.distancesTillTurns[0]) == snakeHead.y))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingLeft)
                    {
                        if (!((firstBS.x - firstBS.distancesTillTurns[0]) == snakeHead.x))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }

                    else if (firstBS.goingRight)
                    {
                        if (!((firstBS.x + firstBS.distancesTillTurns[0]) == snakeHead.x))
                        {
                            for (int i = 0; i < snake.bodySegments.Count; ++i)
                            {
                                snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] = snake.bodySegments[i].distancesTillTurns[snake.bodySegments[i].distancesTillTurns.Count - 1] + 5;
                            }
                        }
                    }
                }

                cantChangeDirection = false;
                stopUpdatingGame = false;
            }
        }
    }
}