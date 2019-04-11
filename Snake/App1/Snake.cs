using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace App1
{
    enum direction { R, L, U, D};

    //Blocks that the snake hits to grow
    class Apple
    {
        public int x;
        public int y;
        public int l;

        public Apple()
        {
            //Give apple random x and y
            Random RNG = new Random();
            x = RNG.Next(20, 440);
            y = RNG.Next(20, 440);
            l = 5;
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
            canvas.DrawRectangle(rectForApple, Colors.White);
            canvas.FillRectangle(rectForApple, Colors.White);
        }
    }

    class Snake
    {
        public Head snakeHead;
        public Apple apple;
        public List<BodySegment> bodySegments;
        public List<Cover> covers;

        public Snake()
        {
            snakeHead = new Head();
            apple = new Apple();
            bodySegments = new List<BodySegment>();
            covers = new List<Cover>();

            //Initialize snake to have six body segments.
            bodySegments.Add(new BodySegment(snakeHead.x - snakeHead.l, snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (2 * snakeHead.l), snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (3 * snakeHead.l), snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (4 * snakeHead.l), snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (5 * snakeHead.l), snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (6 * snakeHead.l), snakeHead.y, snakeHead.l));
        }

        public void updateGame()
        {
            //Update snake
            snakeHead.update();

            //If the snake head collides with the apple, start drawing a new apple
            while (apple.collides(snakeHead.x, snakeHead.y, snakeHead.l))
            {
                apple = new Apple();
            }


            //If a cover collides with the last body segment, stop drawing it.
            BodySegment lastBodySegment = bodySegments[bodySegments.Count() - 1];

            try
            {

                for (int i = 0; i < covers.Count; ++i)
                {
                    if (covers[i].collides(lastBodySegment.x, lastBodySegment.y))
                    {
                        covers.RemoveAt(i);
                    }
                }


            }

            catch (NullReferenceException e)
            {
                if (covers == null)
                {
                    throw new Exception("covers is null");
                }

                else if (covers[0] == null)
                {
                    throw new Exception("covers[0] is null");
                }

                else if (lastBodySegment == null)
                {
                    throw new Exception("lastBodySegment is null");
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

            try
            {
                //Draw all the covers
                for (int i = 0; i < covers.Count; ++i)
                {
                    covers[i].draw(canvas);
                }
            }
            
            catch (NullReferenceException e)
            {
                if (covers == null)
                {
                    throw new Exception("covers is null");
                }

                else if (covers[0] == null)
                {
                    throw new Exception("covers[0] is null");
                }
            }
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

        public Head()
        {
            x = 300;
            y = 300;
            l = 5;

            //There's a reason this is 100. Don't change the value.
            distanceSinceLastTurn = 30;

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
                y -= 5;
            }

            else if(goingDown)
            {
                y += 5;
            }

            else if (goingRight)
            {
                x += 5;
            }

            else if (goingLeft)
            {
                x -= 5;
            }

            distanceSinceLastTurn += 5;
        }

        public void draw(CanvasDrawingSession canvas)
        {
            //Draw snake head
            Rect rectForSnakeHead = new Rect();
            rectForSnakeHead.X = x;
            rectForSnakeHead.Y = y;
            rectForSnakeHead.Width = l;
            rectForSnakeHead.Height = l;
            canvas.DrawRectangle(rectForSnakeHead, Colors.White);
            canvas.FillRectangle(rectForSnakeHead, Colors.White);
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

        //Keep track of distances that the body segment has to travel and then turn at
        public List<int> distancesTillTurns;

        //Keep track of directions that body segment has to turn
        public Queue<direction> waysToTurn;

        public BodySegment(int sx, int sy, int sl)
        {
            x = sx;
            y = sy;
            l = sl;
            goingRight = true;
            goingLeft = false;
            goingUp = false;
            goingDown = false;
            distancesTillTurns = new List<int>();
            waysToTurn = new Queue<direction>();
        }

        public void update()
        {
            if (goingUp)
            {
                y -= 5;
            }

            else if (goingDown)
            {
                y += 5;
            }

            else if (goingRight)
            {
                x += 5;
            }

            else if (goingLeft)
            {
                x -= 5;
            }

            //Don't go any further if the body segment is lined up with the head
            if (distancesTillTurns.Count() == 0)
            {
                return;
            }

            //Subtract 4 from distance body segment is currently traveling
            distancesTillTurns[0] -= 5;

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
            canvas.DrawRectangle(rectForBodySegment, Colors.White);
            canvas.FillRectangle(rectForBodySegment, Colors.White);
        }
    }

    class Cover
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int L { get; private set; }

        public Cover(int x, int y, int l)
        {
            X = x;
            Y = y;
            L = l;
        }

        public void draw(CanvasDrawingSession canvas)
        {
            //Draw cover
            Rect recForCover = new Rect();
            recForCover.X = X;
            recForCover.Y = Y;
            recForCover.Width = L;
            recForCover.Height = L;
            canvas.DrawRectangle(recForCover, Colors.White);
            canvas.FillRectangle(recForCover, Colors.White);
        }

        public bool collides (int x, int y)
        {
            //Returns true is snake head is directly on top of cover
            return X == x && Y == y;
        }
    }
}