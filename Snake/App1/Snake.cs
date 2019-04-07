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

    class Apple
    {
        public int x;
        public int y;
        public int l;

        public Apple()
        {
            Random RNG = new Random();
            x = RNG.Next(20, 440);
            y = RNG.Next(20, 440);
            l = 20;
        }

        public bool collides(int hx, int hy, int hl)
        {
            return ((x + l) >= hx && (x + l) <= (hx + hl)) && ((y + l) >= hy && (y + l) <= (hy + hl)) ||
                   ((x >= hx && x <= (hx + hl)) && (y >= hy && y <= (hy + hl))) ||
                   (((x + l) >= hx && (x + l) <= (hx + hl)) && (y >= hy && y <= (hy + hl))) ||
                   ((x >= hx && x <= (hx + hl)) && ((y + l) >= hy && (y + l) <= (hy + hl)));
        }

        public void draw(CanvasDrawingSession canvas)
        {
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
            bodySegments.Add(new BodySegment(snakeHead.x - snakeHead.l, snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (2 * snakeHead.l), snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (3 * snakeHead.l), snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (4 * snakeHead.l), snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (5 * snakeHead.l), snakeHead.y, snakeHead.l));
            bodySegments.Add(new BodySegment(snakeHead.x - (6 * snakeHead.l), snakeHead.y, snakeHead.l));
        }

        public void updateGame()
        {
            snakeHead.update();
            while (apple.collides(snakeHead.x, snakeHead.y, snakeHead.l))
            {
                apple = new Apple();
            }

            BodySegment lastBodySegment = bodySegments[bodySegments.Count() - 1];
            
            for (int i = 0; i < covers.Count; ++i)
            {
                if (covers[i].collides(lastBodySegment.x, lastBodySegment.y))
                {
                    covers.RemoveAt(i);
                }
            }

            foreach (BodySegment bs in bodySegments)
            {
                bs.update();
            }
        }
            
        public void drawGame(CanvasDrawingSession canvas)
        {
            snakeHead.draw(canvas);
            apple.draw(canvas);
            foreach (BodySegment bs in bodySegments)
            {
                bs.draw(canvas);
            }

            foreach (Cover c in covers)
            {
                c.draw(canvas);
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
            l = 20;
            distanceSinceLastTurn = 100;
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

            if (goingRight)
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

        public List<int> distancesTillTurns;
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
                y -= 4;
            }

            else if (goingDown)
            {
                y += 4;
            }

            if (goingRight)
            {
                x += 4;
            }

            else if (goingLeft)
            {
                x -= 4;
            }

            if (distancesTillTurns.Count() == 0)
            {
                return;
            }

            distancesTillTurns[0] -= 4;

            if (distancesTillTurns[0] == 0)
            {
                distancesTillTurns.Remove(0);

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
            return X == x && Y == y;
        }
    }
}