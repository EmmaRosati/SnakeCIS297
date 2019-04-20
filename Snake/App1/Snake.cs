using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Gaming.Input;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace App1
{
    enum direction { R, L, U, D};
    public enum startPageSelection { Play, Settings, HowToPlay, Credits };
    public enum ColorSelection {DarkOrange, Green, Cyan, HotPink };
    public enum MusicSelection {Song1, Song2, Song3 };

    //Blocks that the snake hits to grow
    class Apple
    {
        public int x;
        public int y;
        public int l;
        public Color appleColor;

        public Apple(Color color)
        {
            //Give apple random x and y
            Random RNG = new Random();
            x = RNG.Next(40, 360);
            y = RNG.Next(40, 360);
            l = 20;
            appleColor = color;
        }

        public bool collides(int hx, int hy, int hl)
        {
            //Returns true if one of the four corners of the apple is inside of the snake head
            return ((x + l) >= hx && (x + l) <= (hx + hl)) && ((y + l) >= hy && (y + l) <= (hy + hl)) ||
                   ((x >= hx && x <= (hx + hl)) && (y >= hy && y <= (hy + hl))) ||
                   (((x + l) >= hx && (x + l) <= (hx + hl)) && (y >= hy && y <= (hy + hl))) ||
                   ((x >= hx && x <= (hx + hl)) && ((y + l) >= hy && (y + l) <= (hy + hl)));
        }

        public void draw(CanvasDrawingSession canvas)
        {
            //Draw apple
            Rect rectForApple = new Rect();
            rectForApple.X = x;
            rectForApple.Y = y;
            rectForApple.Width = l;
            rectForApple.Height = l;
            canvas.DrawRectangle(rectForApple, appleColor);
            canvas.FillRectangle(rectForApple, appleColor);
        }
    }

    class Snake
    {
        public Head snakeHead;
        public Apple apple;
        public List<BodySegment> bodySegments;
        public List<Cover> covers;
        public Color foregroundColor;
        public Color backgroundColor;
        public bool growSnake;
        private MediaPlayer biteSoundEffect;
        public int playerScore;

        public Snake(Color foregroundColor, Color backgroundColor)
        {
            snakeHead = new Head(foregroundColor);
            apple = new Apple(foregroundColor);
            bodySegments = new List<BodySegment>();
            covers = new List<Cover>();
            this.foregroundColor = foregroundColor;
            this.backgroundColor = backgroundColor;
            growSnake = false;
            playerScore = 0;

            //Set up bite sound effect
            biteSoundEffect = new MediaPlayer();
            biteSoundEffect.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/chomp_sound_effect.wav"));


            //Initialize snake to have six body segments.
            bodySegments.Add(new BodySegment(snakeHead.x - snakeHead.l, snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (2 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (3 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (4 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (5 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (6 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
        }

        public void resetGame()
        {
            snakeHead = new Head(foregroundColor);
            apple = new Apple(foregroundColor);
            bodySegments = new List<BodySegment>();
            covers = new List<Cover>();
            playerScore = 0;

            //Initialize snake to have six body segments.
            bodySegments.Add(new BodySegment(snakeHead.x - snakeHead.l, snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (2 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (3 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (4 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (5 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
            bodySegments.Add(new BodySegment(snakeHead.x - (6 * snakeHead.l), snakeHead.y, snakeHead.l, foregroundColor, direction.R));
        }

        public void updateGame()
        {
            //Update snake
            snakeHead.update();

            if (appleCollidesWithSnakeHead())
            {
                //When the snake head collides with an apple
                while (appleCollidesWithSnakeHead() || appleCollidesWithBodyOfSnake() || appleIsToCloseToSnakeHead())
                {
                    apple = new Apple(foregroundColor);
                    growSnake = true;
                }
            }

            //Snake grows if player just hit an apple
            if (growSnake)
            {
                biteSoundEffect.Play();
                lengthenSnake();
                growSnake = false;
                ++playerScore;
            }


            //If a cover collides with the last body segment, stop drawing it.
            BodySegment lastBodySegment = bodySegments[bodySegments.Count() - 1];


            for (int i = 0; i < covers.Count; ++i)
            {
                 if (covers[i].collides(lastBodySegment.x, lastBodySegment.y))
                 {
                     covers.RemoveAt(i);
                 }
            }

            //Update all the body segments
            for (int i = 0; i < bodySegments.Count; ++i)
            {
                bodySegments[i].update();
            }
        }
            
        public void drawGame(CanvasDrawingSession canvas)
        {
            //Draw the snake head and apple
            snakeHead.draw(canvas);
            apple.draw(canvas);

            //Draw all the body segments
            for (int i = 0; i < bodySegments.Count; ++i)
            {
                bodySegments[i].draw(canvas);
            }

            //Draw all the covers
            for (int i = 0; i < covers.Count; ++i)
            {
                covers[i].draw(canvas);
            }
        }

        //Description: Determines if the apple is colliding with any of the body segment
        private bool appleCollidesWithBodyOfSnake()
        {
            foreach(BodySegment bs in bodySegments)
            {
                if (apple.collides(bs.x, bs.y, bs.l))
                {
                    return true;
                }
            }

            return false;
        }

        private bool appleCollidesWithSnakeHead()
        {
            return apple.collides(snakeHead.x, snakeHead.y, snakeHead.l);
        }

        //Stop apple from spawing too close to snake head. This ensures sound effect plays correctly.
        //Simple use of pythagerean theorem.
        private bool appleIsToCloseToSnakeHead()
        {
            return ((Math.Pow((apple.x - snakeHead.x), 2.0) + Math.Pow((apple.y - snakeHead.y), 2.0)) < 14400);
        }

        private void lengthenSnake()
        {
            //Make deep copy of end body segment
            BodySegment endBodySegment = bodySegments[bodySegments.Count - 1];
            BodySegment newEndBodySegment = endBodySegment.deepCopy();

            //Modify deep copy's coordinates and distance till next turn
            if (endBodySegment.goingUp)
            {
                newEndBodySegment.y += 20;
            }

            else if (endBodySegment.goingDown)
            {
                newEndBodySegment.y -= 20;
                
            }

            else if (endBodySegment.goingLeft)
            {
                newEndBodySegment.x += 20;
            }

            else if (endBodySegment.goingRight)
            {
                newEndBodySegment.x -= 20;
            }

            //Add new body segment to list of body segments if snake is not lined up perfectly
            if (endBodySegment.distancesTillTurns.Count != 0)
            {
                newEndBodySegment.distancesTillTurns[0] += 20;
            }

            bodySegments.Add(newEndBodySegment);
        }

        public bool playerRanIntoThemself()
        {
            //Start at second body segment b/c first will overlap with head when it turns
            //and the head can't collide with the first body segment anyway
            for (int i = 1; i < bodySegments.Count; ++i)
            {
                if (bodySegments[i].collides(snakeHead.x, snakeHead.y, snakeHead.l))
                {
                    return true;
                }
            }

            return false;
        }
    }

    class Head
    {
        public int x;
        public int y;
        public int l;
        public bool goingUp;
        public bool goingDown;
        public bool goingRight;
        public bool goingLeft;
        public int distanceSinceLastTurn;
        public Color headColor;

        public Head(Color color)
        {
            x = 200;
            y = 200;
            l = 20;
            headColor = color;

            //There's a reason this is 120. Don't change the value.
            distanceSinceLastTurn = 120;

            //Start off by moving right
            goingUp = false;
            goingDown = false;
            goingRight = true;
            goingLeft = false;
        }

        public void update()
        {
            if (goingUp)
            {
                y -= 4;
            }

            else if(goingDown)
            {
                y += 4;
            }

            else if (goingRight)
            {
                x += 4;
            }

            else if (goingLeft)
            {
                x -= 4;
            }

            distanceSinceLastTurn += 4;
        }

        public void draw(CanvasDrawingSession canvas)
        {
            //Draw snake head
            Rect rectForSnakeHead = new Rect();
            rectForSnakeHead.X = x;
            rectForSnakeHead.Y = y;
            rectForSnakeHead.Width = l;
            rectForSnakeHead.Height = l;
            canvas.DrawRectangle(rectForSnakeHead, headColor);
            canvas.FillRectangle(rectForSnakeHead, headColor);
        }
    }

    class BodySegment
    {
        public int x;
        public int y;
        public int l;
        public bool goingUp;
        public bool goingDown;
        public bool goingRight;
        public bool goingLeft;
        public Color bodySegmentColor;

        //Keep track of distances that the body segment has to travel and then turn at
        public List<int> distancesTillTurns;

        //Keep track of directions that body segment has to turn
        public Queue<direction> waysToTurn;

        public BodySegment(int sx, int sy, int sl, Color color, direction dir)
        {
            x = sx;
            y = sy;
            l = sl;

            if (dir == direction.R)
            {
                goingRight = true;
                goingLeft = false;
                goingUp = false;
                goingDown = false;
            }

            else if (dir == direction.L)
            {
                goingRight = false;
                goingLeft = true;
                goingUp = false;
                goingDown = false;
            }

            else if (dir == direction.U)
            {
                goingRight = false;
                goingLeft = false;
                goingUp = true;
                goingDown = false;
            }

            else if (dir == direction.D)
            {
                goingRight = false;
                goingLeft = false;
                goingUp = false;
                goingDown = true;
            }

            bodySegmentColor = color;
            distancesTillTurns = new List<int>();
            waysToTurn = new Queue<direction>();
        }

        //Description: Returns deep copy of calling body segment.
        public BodySegment deepCopy()
        {
            //Determine which way calling body segment is going and then tell copy to move same way
            direction dir = direction.L;

            if (goingUp)
            {
                dir = direction.U;
            }

            else if (goingDown)
            {
                dir = direction.D;
            }

            else if (goingLeft)
            {
                dir = direction.L;
            }

            else if (goingRight)
            {
                dir = direction.R;
            }

            BodySegment deepCopyOfBodySegment = new BodySegment(x, y, l, bodySegmentColor, dir);

            if (distancesTillTurns.Count != 0)
            {
                for (int i = 0; i < distancesTillTurns.Count; i++)
                {
                    deepCopyOfBodySegment.distancesTillTurns.Add(distancesTillTurns[i]);
                }
            }

            if (waysToTurn.Count != 0)
            {
                foreach(direction d in waysToTurn)
                {
                    deepCopyOfBodySegment.waysToTurn.Enqueue(d);
                }
            }

            return deepCopyOfBodySegment;
        }

        public bool collides(int hx, int hy, int hl)
        {
            //Returns true if one of the four corners of the body segment is inside of the snake head
            return ((x + l) > hx && (x + l) < (hx + hl)) && ((y + l) > hy && (y + l) < (hy + hl)) ||
                   ((x > hx && x < (hx + hl)) && (y > hy && y < (hy + hl))) ||
                   (((x + l) > hx && (x + l) < (hx + hl)) && (y > hy && y < (hy + hl))) ||
                   ((x > hx && x < (hx + hl)) && ((y + l) > hy && (y + l) < (hy + hl)));
        }

        public void update()
        {
            if (goingUp)
            {
                y -= 4;
            }

            else if (goingDown)
            {
                y += 4;
            }

            else if (goingRight)
            {
                x += 4;
            }

            else if (goingLeft)
            {
                x -= 4;
            }

            //Don't go any further if the body segment is lined up with the head
            if (distancesTillTurns.Count() == 0)
            {
                return;
            }

            //Subtract 4 from distance body segment is currently traveling
            distancesTillTurns[0] -= 4;

            //If it's time to turn
            if (distancesTillTurns[0] == 0)
            {
                //Get rid of zero. Will start subtracting from next distance.
                distancesTillTurns.Remove(0);

                //Get direction body segment is turning.
                direction newDirection = waysToTurn.Dequeue();

                if (newDirection == direction.U)
                {
                    goingRight = false;
                    goingLeft = false;
                    goingUp = true;
                    goingDown = false;
                }

                else if (newDirection == direction.D)
                {
                    goingRight = false;
                    goingLeft = false;
                    goingUp = false;
                    goingDown = true;
                }

                else if (newDirection == direction.L)
                {
                    goingRight = false;
                    goingLeft = true;
                    goingUp = false;
                    goingDown = false;
                }

                else if (newDirection == direction.R)
                {
                    goingRight = true;
                    goingLeft = false;
                    goingUp = false;
                    goingDown = false;
                }
            }
        }

        public void draw(CanvasDrawingSession canvas)
        {
            //Draw body segment
            Rect rectForBodySegment = new Rect();
            rectForBodySegment.X = x;
            rectForBodySegment.Y = y;
            rectForBodySegment.Width = l;
            rectForBodySegment.Height = l;
            canvas.DrawRectangle(rectForBodySegment, bodySegmentColor);
            canvas.FillRectangle(rectForBodySegment, bodySegmentColor);
        }
    }

    class Cover
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int L { get; private set; }
        public Color coverColor;

        public Cover(int x, int y, int l, Color color)
        {
            X = x;
            Y = y;
            L = l;
            coverColor = color;
        }

        public void draw(CanvasDrawingSession canvas)
        {
            //Draw cover
            Rect recForCover = new Rect();
            recForCover.X = X;
            recForCover.Y = Y;
            recForCover.Width = L;
            recForCover.Height = L;
            canvas.DrawRectangle(recForCover, coverColor);
            canvas.FillRectangle(recForCover, coverColor);
        }

        public bool collides (int x, int y)
        {
            //Returns true is snake head is directly on top of cover
            return X == x && Y == y;
        }
    }

    public class menuSelector_Settings
    {
        private bool atTop_Color;
        private bool atBottom_Color;

        private bool atBottom_Music;
        private bool atTop_Music;

        private Rect selector_color;
        private Rect selector_music;
        public ColorSelection selection_color;
        public MusicSelection selection_music;
        public menuSelector_Settings()
        {
            atTop_Color = true;
            atBottom_Color = false;

            atTop_Music = true;
            atBottom_Music = false;

            selector_color = new Rect();
            selector_color.X = 25;
            selector_color.Y = 105;
            selector_color.Height = 15;
            selector_color.Width = 15;

            selector_music = new Rect();
            selector_music.X = 445;
            selector_music.Y = 105;
            selector_music.Height = 15;
            selector_music.Width = 15;

            selection_color = ColorSelection.DarkOrange;
            selection_music = MusicSelection.Song1;
        }
        public void moveUp_Color()
        {
            if (!atTop_Color)
            {
                selector_color.Y -= 56;
                atBottom_Color = false;
                --selection_color;

                if (selector_color.Y == 105)
                {
                    atTop_Color = true;
                }
            }
        }
        public void moveDown_Color()
        {
            if (!atBottom_Color)
            {
                selector_color.Y += 56;
                atTop_Color = false;
                ++selection_color;

                if (selector_color.Y == 273)
                {
                    atBottom_Color = true;
                }
            }
        }
        public void moveUp_Music()
        {
            if (!atTop_Music)
            {
                selector_music.Y -= 56;
                atBottom_Music = false;
                --selection_music;

                if (selector_music.Y == 105)
                {
                    atTop_Music = true;
                }
            }
        }
        public void moveDown_Music()
        {
            if (!atBottom_Music)
            {
                selector_music.Y += 56;
                atTop_Music = false;
                ++selection_music;

                if (selector_music.Y == 217)
                {
                    atBottom_Music = true;
                }
            }
        }
        public void draw(CanvasDrawingSession canvas)
        {   // color selector
            canvas.DrawRectangle(selector_color, Colors.White);
            canvas.FillRectangle(selector_color, Colors.White);
            // song selector
            canvas.DrawRectangle(selector_music, Colors.White);
            canvas.FillRectangle(selector_music, Colors.White);
        }
    }
    public class menuSelector
    {
        private bool atTop;
        private bool atBottom;
        private Rect selector;
        public startPageSelection selection;
     

        public menuSelector()
        {
            atTop = true;
            atBottom = false;

            selector = new Rect();
            selector.X = 25;
            selector.Y = 210;
            selector.Height = 20;
            selector.Width = 20;

            selection = startPageSelection.Play;
        }

        public void moveUp()
        {
            if (!atTop)
            {
                selector.Y -= 50;
                atBottom = false;
                --selection;

                if (selector.Y == 210)
                {
                    atTop = true;
                }
            }
        }

        public void moveDown()
        {
            if (!atBottom)
            {
                selector.Y += 50;
                atTop = false;
                ++selection;

                if (selector.Y == 360)
                {
                    atBottom = true;
                }
            }
        }
     

        public void draw(CanvasDrawingSession canvas)
        {
            canvas.DrawRectangle(selector, Colors.White);
            canvas.FillRectangle(selector, Colors.White);
        }
    }
}