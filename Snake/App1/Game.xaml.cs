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
using Windows.UI.ViewManagement;
using Microsoft.Graphics.Canvas.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Game : Page
    {
        //Game Object
        private Snake snake;

        private int gameOverCounter;

        private bool cantChangeDirection;
        private bool turningLeft;
        private bool turningRight;
        private bool turningDown;
        private bool turningUp;
        private bool gameOver;
        private bool gameIsRunning;

        public Game()
        {
            this.InitializeComponent();
            snake = new Snake();

            //Add method to keydown event
            Window.Current.CoreWindow.KeyDown += Canvas_KeyDown;
            cantChangeDirection = false;
            turningLeft = false;
            turningRight = false;
            turningDown = false;
            turningUp = false;
            gameOver = false;
            gameIsRunning = true; //make false later

            gameOverCounter = 0;

            //Set width and height of window
            ApplicationView.PreferredLaunchViewSize = new Size(800, 800);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
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

            if (gameIsRunning)
            {
                //Draw Game
                snake.drawGame(args.DrawingSession);
            }          

            else if (gameOver)
            {
                Rect locOfGameOverText = new Rect();
                locOfGameOverText.X = 200;
                locOfGameOverText.Y = 300;
                locOfGameOverText.Width = 400;
                locOfGameOverText.Height = 200;

                CanvasTextFormat textFormatOfGameOverText = new CanvasTextFormat()
                {
                    FontFamily = "Arial Black",
                    FontSize = 72
                };

                args.DrawingSession.DrawText("Game Over", locOfGameOverText, Colors.White, textFormatOfGameOverText);
            }
        }

        private async void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
           if(gameIsRunning)
           {
               updateGame();
           }

           else if (gameOver)
           {
                ++gameOverCounter;

                if (gameOverCounter == 1)
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        var element = new MediaElement();
                        var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                        var file = await folder.GetFileAsync("sm64_mario_game_over.wav");
                        var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                        element.SetSource(stream, "");
                        element.Play();
                    });
                }

                if (gameOverCounter == 360)
                {
                    gameOver = false;
                    snake.resetGame();
                    gameIsRunning = true;
                    gameOverCounter = 0;
                }
           }
        }

        private void updateGame()
        {
            snake.updateGame();

            if (turningLeft)
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
                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 20);
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


                turningLeft = false;
            }

            else if (turningRight)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = true;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 20);
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
                turningRight = false;
            }

            else if (turningDown)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = true;
                snake.snakeHead.goingUp = false;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 20);
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
                turningDown = false;
            }

            else if (turningUp)
            {
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l));

                snake.snakeHead.goingLeft = false;
                snake.snakeHead.goingRight = false;
                snake.snakeHead.goingDown = false;
                snake.snakeHead.goingUp = true;

                int bodySegmentNumber = 1;
                int numberOfBlocksInLineWithHead = snake.snakeHead.distanceSinceLastTurn / 20;

                for (int i = 0; i < snake.bodySegments.Count; ++i)
                {
                    if (bodySegmentNumber <= numberOfBlocksInLineWithHead)
                    {
                        snake.bodySegments[i].distancesTillTurns.Add(bodySegmentNumber * 20);
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
                turningUp = false;
            }

            //game ends if snake hits edge of window
            if (snake.snakeHead.x == 0 || snake.snakeHead.x == 780 ||
                snake.snakeHead.y == 0 || snake.snakeHead.y == 780)
            {
                gameOver = true;
                gameIsRunning = false;
            }
        }

        //Runs when key is pressed down.
        private void Canvas_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            //Left key is pressed when the user was not already going left or right
            if (e.VirtualKey == Windows.System.VirtualKey.Left && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft && !cantChangeDirection && gameIsRunning)
            {
                cantChangeDirection = true;
                turningLeft = true;
                cantChangeDirection = false;
            }

            //Right key is pressed when we are not going right or left
            else if (e.VirtualKey == Windows.System.VirtualKey.Right && !snake.snakeHead.goingLeft && !snake.snakeHead.goingRight && !cantChangeDirection && gameIsRunning)
            {
                cantChangeDirection = true;
                turningRight = true;
                cantChangeDirection = false;
            }

            //Down key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Down && !snake.snakeHead.goingUp && !snake.snakeHead.goingDown && !cantChangeDirection && gameIsRunning)
            {
                cantChangeDirection = true;
                turningDown = true;
                cantChangeDirection = false;
            }

            //Up key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Up && !snake.snakeHead.goingDown && !snake.snakeHead.goingUp && !cantChangeDirection && gameIsRunning)
            {
                cantChangeDirection = true;
                turningUp = true;
                cantChangeDirection = false;
            }
        }
    }
}