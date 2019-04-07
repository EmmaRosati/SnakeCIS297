using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
        private Snake snake;

        public MainPage()
        {
            this.InitializeComponent();
            snake = new Snake();
            Window.Current.CoreWindow.KeyDown += Canvas_KeyDown;
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            Rect rect = new Rect();
            rect.X = 0;
            rect.Y = 0;
            rect.Width = 800;
            rect.Height = 800;

            args.DrawingSession.DrawRectangle(rect, Colors.Black);
            args.DrawingSession.FillRectangle(rect, Colors.Black);
            snake.drawGame(args.DrawingSession);
        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            snake.updateGame();
        }

        private void Canvas_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (e.VirtualKey == Windows.System.VirtualKey.Left && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = true;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

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

                snake.snakeHead.distanceSinceLastTurn = 0;
            }

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