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
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.Gaming.Input;
using Microsoft.Graphics.Canvas;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Game : Page
    {
        private Snake snake;

        private MediaPlayer backgroundMusicPlayer;
        private MediaPlayer gameoverSoundEffect;
        private MediaPlayer thankYouSoundEffect;

        private Gamepad controller;

        menuSelector menuSelector;

        private int gameOverCounter;
        private int turnCounter;
        private int loadCounter;
        private int thankYouSoundEffectCounter;

        private bool cantChangeDirection;
        private bool turningLeft;
        private bool turningRight;
        private bool turningDown;
        private bool turningUp;
        private bool gameOver;
        private bool gameIsRunning;
        private bool canTurn;
        private bool startPageDisplaying;
        private bool settingsPageDisplaying;
        private bool howToPlayDisplaying;
        private bool highScoreMenu;
        private bool loading;
        private bool credits;

        public Game()
        {
            this.InitializeComponent();
            snake = new Snake(Colors.DarkOrange, Colors.Black);

            //Add method to keydown event
            Window.Current.CoreWindow.KeyDown += Canvas_KeyDown;
            cantChangeDirection = false; //prevents key down event from firing off twice
            turningLeft = false;
            turningRight = false;
            turningDown = false;
            turningUp = false;
            gameOver = false;
            gameIsRunning = false;
            canTurn = true; //Serves different purpose than cantChangeDirection
            startPageDisplaying = false;
            settingsPageDisplaying = false;
            howToPlayDisplaying = false;
            highScoreMenu = false;
            loading = true;
            credits = false;

            gameOverCounter = 0;
            turnCounter = 0;
            thankYouSoundEffectCounter = 0;

            //Set width and height of window
            ApplicationView.PreferredLaunchViewSize = new Size(600, 400);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            //Start background music
            backgroundMusicPlayer = new MediaPlayer();
            backgroundMusicPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/duck_tales_music.wav"));
            backgroundMusicPlayer.Volume = 0.06;
            backgroundMusicPlayer.MediaEnded += resetSong;
            backgroundMusicPlayer.Play();

            //Gameover sound effect
            gameoverSoundEffect = new MediaPlayer();
            gameoverSoundEffect.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sm64_mario_game_over.wav"));

            //Thank you sound effect
            thankYouSoundEffect = new MediaPlayer();
            thankYouSoundEffect.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/sm64_mario_thank_you.wav"));

            menuSelector = new menuSelector();
        }

        private void resetSong(MediaPlayer sender, object args)
        {
            backgroundMusicPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(0);
            backgroundMusicPlayer.Play();
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            //Draw Black Background
            Rect rect = new Rect();
            rect.X = 0;
            rect.Y = 0;
            rect.Width = 600;
            rect.Height = 400;

            args.DrawingSession.DrawRectangle(rect, snake.foregroundColor);
            args.DrawingSession.FillRectangle(rect, snake.backgroundColor);

            if (gameIsRunning)
            {
                Rect scoreBoard = new Rect();
                scoreBoard.X = 401;
                scoreBoard.Y = 0;
                scoreBoard.Width = 199;
                scoreBoard.Height = 400;
                args.DrawingSession.DrawRectangle(scoreBoard, snake.foregroundColor);
                args.DrawingSession.FillRectangle(scoreBoard, snake.foregroundColor);

                Rect playerScoreRec = new Rect();
                playerScoreRec.X = 441;
                playerScoreRec.Y = 181;
                playerScoreRec.Width = 160;
                playerScoreRec.Height = 40;

                CanvasTextFormat textFormatOfScoreText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 72
                };

                args.DrawingSession.DrawText($"{snake.playerScore}", playerScoreRec, snake.backgroundColor, textFormatOfScoreText);

                //Draw Game
                snake.drawGame(args.DrawingSession);
            }          

            else if (gameOver)
            {
                Rect locOfGameOverText = new Rect();
                locOfGameOverText.X = 200;
                locOfGameOverText.Y = 125;
                locOfGameOverText.Width = 200;
                locOfGameOverText.Height = 200;

                CanvasTextFormat textFormatOfGameOverText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 72
                };

                args.DrawingSession.DrawText("GAME OVER", locOfGameOverText, Colors.White, textFormatOfGameOverText);
            }

            else if (startPageDisplaying)
            {
                Rect titleRec = new Rect();
                titleRec.X = 25;
                titleRec.Y = 20;
                titleRec.Width = 400;
                titleRec.Height = 100;

                Rect selectionText = new Rect();
                selectionText.X = 60;
                selectionText.Y = 200;
                selectionText.Width = 400;
                selectionText.Height = 200;

                Rect imageRec = new Rect();
                imageRec.X = 300;
                imageRec.Y = 40;
                imageRec.Width = 216;
                imageRec.Height = 216;


                CanvasTextFormat textFormatOfTitleText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 72
                };

                CanvasTextFormat textFormatOfSelectionText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 44
                };

                string selectionString = "PLAY!!!\nSETTINGS!!!\nHOW TO PLAY???\nCREDITS!!!\n";
                args.DrawingSession.DrawText("SNAKE!!!", titleRec, Colors.White, textFormatOfTitleText);
                args.DrawingSession.DrawText(selectionString, selectionText, Colors.White, textFormatOfSelectionText);
                menuSelector.draw(args.DrawingSession);
            }

            else if(settingsPageDisplaying)
            {
                Rect HorriblyFormattedTextPleaseRemove = new Rect();
                HorriblyFormattedTextPleaseRemove.X = 100;
                HorriblyFormattedTextPleaseRemove.Y = 100;
                HorriblyFormattedTextPleaseRemove.Width = 100;
                HorriblyFormattedTextPleaseRemove.Height = 100;

                CanvasTextFormat HorribleFormattingPleaseRemove = new CanvasTextFormat()
                {
                    FontFamily = "Arial",
                    FontSize = 12
                };

                args.DrawingSession.DrawText("Settings", HorriblyFormattedTextPleaseRemove, Colors.White, HorribleFormattingPleaseRemove);
            }

            else if(howToPlayDisplaying)
            {
                Rect howToPlayRect = new Rect();
                    howToPlayRect.X = 25;
                    howToPlayRect.Y = 25;
                    howToPlayRect.Width = 550;
                    howToPlayRect.Height = 300;

                CanvasTextFormat rectFormat = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 24
                };

                args.DrawingSession.DrawText("HOW TO PLAY XBOX/WINDOWS:\n\n" +
                                             "D-PAD UP/UP ARROW = GO UP\n" +
                                             "D-PAD DOWN/DOWN ARROW = GO DOWN\n" +
                                             "D-PAD LEFT/LEFT ARROW = GO LEFT\n" +
                                             "D-PAD RIGHT/RIGHT ARROW = GO RIGHT\n" +
                                             "ENTER/A BUTTON = SELECT/BACK/ADVANCE\n\n" +
                                             "PRESS ENTER OR A TO GO TO START MENU", howToPlayRect, Colors.White, rectFormat);
            }

            else if(highScoreMenu)
            {
                Rect HorriblyFormattedTextPleaseRemove = new Rect();
                HorriblyFormattedTextPleaseRemove.X = 100;
                HorriblyFormattedTextPleaseRemove.Y = 100;
                HorriblyFormattedTextPleaseRemove.Width = 100;
                HorriblyFormattedTextPleaseRemove.Height = 100;

                CanvasTextFormat HorribleFormattingPleaseRemove = new CanvasTextFormat()
                {
                    FontFamily = "Arial",
                    FontSize = 12
                };

                args.DrawingSession.DrawText("High score", HorriblyFormattedTextPleaseRemove, Colors.White, HorribleFormattingPleaseRemove);
            }

            else if(loading)
            {
                Rect locOfLoadText = new Rect();
                locOfLoadText.X = 125;
                locOfLoadText.Y = 150;
                locOfLoadText.Width = 400;
                locOfLoadText.Height = 200;

                CanvasTextFormat textFormatOfLoadText = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 56,
                    FontStyle = Windows.UI.Text.FontStyle.Italic
                };

                args.DrawingSession.DrawText("LOADING...", locOfLoadText, Colors.White, textFormatOfLoadText);
            }

            else if (credits)
            {
                Rect creditsRec = new Rect();
                creditsRec.Y = 20;
                creditsRec.X = 20;
                creditsRec.Width = 600;
                creditsRec.Height = 400;

                CanvasTextFormat textFormatOfCredits = new CanvasTextFormat()
                {
                    FontFamily = "Courier New",
                    FontSize = 32
                };

                string creditsString = "THE CREW:\n\nALEX ROSATI (SUPER COOL)\nAVIAN CALADO (SLEEPY)\nPETER SCHUBERT (COOL)\nNISARG"
                                        + " PATEL (COOL) \n\nPRESS ENTER OR A TO GO BACK";

                args.DrawingSession.DrawText(creditsString, creditsRec, Colors.White, textFormatOfCredits);
            }
        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
           if(gameIsRunning)
           {
               controllerGameLogic();

               if (!canTurn)
               {
                    ++turnCounter;

                    if(turnCounter == 6)
                    {
                        canTurn = true;
                        turnCounter = 0;
                    }
               }

               updateGame();
           }

           else if (gameOver)
           {
                ++gameOverCounter;

                if (gameOverCounter == 1)
                {
                    gameoverSoundEffect.Play();
                }

                if (gameOverCounter == 360)
                {
                    gameOver = false;
                    snake.resetGame();
                    highScoreMenu = true;
                    gameOverCounter = 0;
                }
           }

           else if(startPageDisplaying)
           {

           }

           else if(settingsPageDisplaying)
           {

           }

           else if(howToPlayDisplaying)
           {
                
           }

           else if(highScoreMenu)
           {

           }

           else if(loading)
           {
                ++loadCounter;

                if(loadCounter == 80)
                {
                    loading = false;
                    howToPlayDisplaying = true;
                }
           }

           else if (credits)
           {
          
           }
        }

        private void updateGame()
        {
            snake.updateGame();

            if (turningLeft)
            {
                //Put a cover over turn
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l, snake.foregroundColor));

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
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l, snake.foregroundColor));

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
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l, snake.foregroundColor));
         
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
                snake.covers.Add(new Cover(snake.snakeHead.x, snake.snakeHead.y, snake.snakeHead.l, snake.foregroundColor));

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

            //Game ends if player hits themself
            if (snake.playerRanIntoThemself())
            {
                gameOver = true;
                gameIsRunning = false;
            }

            //Game ends if snake hits edge of window
            if (snake.snakeHead.x == 0 || snake.snakeHead.x == 380 ||
                snake.snakeHead.y == 0 || snake.snakeHead.y == 380)
            {
                gameOver = true;
                gameIsRunning = false;
            }
        }

        //Runs when key is pressed down.
        private void Canvas_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            bool startedAtStartPage = startPageDisplaying;

            //When game is running
            //Left key is pressed when the user was not already going left or right
            if (e.VirtualKey == Windows.System.VirtualKey.Left && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft 
                && !cantChangeDirection&& gameIsRunning && canTurn)
            {
                cantChangeDirection = true;
                turningLeft = true;
                cantChangeDirection = false;
                canTurn = false;
            }

            //Right key is pressed when we are not going right or left
            else if (e.VirtualKey == Windows.System.VirtualKey.Right && !snake.snakeHead.goingLeft && !snake.snakeHead.goingRight 
                && !cantChangeDirection && gameIsRunning && canTurn)
            {
                cantChangeDirection = true;
                turningRight = true;
                cantChangeDirection = false;
                canTurn = false;
            }

            //Down key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Down && !snake.snakeHead.goingUp && !snake.snakeHead.goingDown 
                && !cantChangeDirection && gameIsRunning && canTurn)
            {
                cantChangeDirection = true;
                turningDown = true;
                cantChangeDirection = false;
                canTurn = false;
            }

            //Up key is pressed when we are not going up or down
            else if (e.VirtualKey == Windows.System.VirtualKey.Up && !snake.snakeHead.goingDown && !snake.snakeHead.goingUp 
                && !cantChangeDirection && gameIsRunning && canTurn)
            {
                cantChangeDirection = true;
                turningUp = true;
                cantChangeDirection = false;
                canTurn = false;
            }

            if(startPageDisplaying)
            {
                if(e.VirtualKey == Windows.System.VirtualKey.Up)
                {
                    menuSelector.moveUp();
                }

                else if (e.VirtualKey == Windows.System.VirtualKey.Down)
                {
                    menuSelector.moveDown();
                }

                if (e.VirtualKey == Windows.System.VirtualKey.Enter)
                {
                    if (menuSelector.selection == startPageSelection.Play)
                    {
                        startPageDisplaying = false;
                        gameIsRunning = true;
                    }

                    else if (menuSelector.selection == startPageSelection.Settings)
                    {
                        startPageDisplaying = false;
                        settingsPageDisplaying = true;
                    }

                    else if (menuSelector.selection == startPageSelection.HowToPlay)
                    {
                        startPageDisplaying = false;
                        howToPlayDisplaying = true;
                    }

                    else if (menuSelector.selection == startPageSelection.Credits)
                    {
                        startPageDisplaying = false;
                        credits = true;
                        thankYouSoundEffect.Play();
                    }
                }
            }

            //Go to start menu from how to play menu
            if (!startedAtStartPage && howToPlayDisplaying && e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                howToPlayDisplaying = false;
                startPageDisplaying = true;
                menuSelector = new menuSelector();
            }

            //Go back to start menu from credits
            if (!startedAtStartPage && credits && e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                credits = false;
                startPageDisplaying = true;
                menuSelector = new menuSelector();
            }

            //Go back to start menu from settings page
            if (!startedAtStartPage && settingsPageDisplaying && e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                settingsPageDisplaying = false;
                startPageDisplaying = true;
                menuSelector = new menuSelector();
            }

            //From high score menu to start menu
            if (highScoreMenu && e.VirtualKey == Windows.System.VirtualKey.Enter)
            {
                highScoreMenu = false;
                startPageDisplaying = true;
            }
        }

        //Makes Snake Turn when d-pad on controller is pressed
        private void controllerGameLogic()
        {
            if (Gamepad.Gamepads.Count > 0)
            {
                controller = Gamepad.Gamepads.First();
                GamepadReading reading = controller.GetCurrentReading();

                if (reading.Buttons.HasFlag(GamepadButtons.DPadLeft) && !snake.snakeHead.goingRight && !snake.snakeHead.goingLeft
                && canTurn)
                {
                    turningLeft = true;
                    canTurn = false;
                }

                else if (reading.Buttons.HasFlag(GamepadButtons.DPadRight) && !snake.snakeHead.goingLeft && !snake.snakeHead.goingRight
                && canTurn)
                {
                    turningRight = true;
                    canTurn = false;
                }

                else if (reading.Buttons.HasFlag(GamepadButtons.DPadDown) && !snake.snakeHead.goingUp && !snake.snakeHead.goingDown
                && canTurn)
                {
                    turningDown = true;
                    canTurn = false;
                }

                else if (reading.Buttons.HasFlag(GamepadButtons.DPadUp) && !snake.snakeHead.goingDown && !snake.snakeHead.goingUp
                && canTurn)
                {
                    turningUp = true;
                    canTurn = false;
                }
            }
        }
    }
}