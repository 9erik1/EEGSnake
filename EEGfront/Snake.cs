using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace EEGfront
{

    public struct SnakeCube
    {

        public SnakeCube(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;
    }

    public class Snake
    {

        private Random randy;
        private LinkedList<SnakeCube> snakeUser;
        private SnakeCube applePos;
        private SnakeMotivation dir;

        public Snake()
        {
            snakeUser = new LinkedList<SnakeCube>();
            snakeUser.AddFirst(new SnakeCube(0, 0));
            snakeUser.AddFirst(new SnakeCube(0, 1));
            snakeUser.AddFirst(new SnakeCube(0, 2));
            randy = new Random();
            NewApplePos();
            dir = SnakeMotivation.Down;
        }

        public LinkedList<SnakeCube> GetSnakeList()
        {
            return snakeUser;
        }

        public SnakeCube GetApplePos()
        {
            return applePos;
        }

        public int GetX()
        {
            return (int)snakeUser.First.Value.X * 10;
        }

        public int GetY()
        {
            return (int)snakeUser.First.Value.Y * 10;
        }

        public void MoveUp()
        {
            if (dir == SnakeMotivation.Down)
                return;
            dir = SnakeMotivation.Up;
        }

        public void MoveDown()
        {
            if (dir == SnakeMotivation.Up)
                return;
            dir = SnakeMotivation.Down;
        }

        public void MoveLeft()
        {
            if (dir == SnakeMotivation.Right)
                return;
            dir = SnakeMotivation.Left;
        }

        public void MoveRight()
        {
            if (dir == SnakeMotivation.Left)
                return;
            dir = SnakeMotivation.Right;
        }

        // todo : on a thread somewhere
        public void MoveSnake()
        {
            LinkedListNode<SnakeCube> currentNode = snakeUser.First;
            SnakeCube proxy;//for mod the positions
            SnakeCube curProcy;//for mod the positions
            SnakeCube lastPos = new SnakeCube();
            while (currentNode != null)
            {
                if (currentNode.Previous == null)
                {
                    proxy = currentNode.Value;
                    lastPos = proxy;
                    switch (dir)
                    {
                        case SnakeMotivation.Up:
                            proxy.Y--;
                            if (proxy.Y < 0)
                                proxy.Y = 19;
                            break;
                        case SnakeMotivation.Down:
                            proxy.Y++;
                            if (proxy.Y > 19)
                                proxy.Y = 0;
                            break;
                        case SnakeMotivation.Right:
                            proxy.X++;
                            if (proxy.X > 19)
                                proxy.X = 0;
                            break;
                        case SnakeMotivation.Left:
                            proxy.X--;
                            if (proxy.X < 0)
                                proxy.X = 19;
                            break;
                        default:
                            Console.WriteLine("Ctrl+F 'fortnight' asap!!!!!!!");
                            break;
                    }

                    currentNode.Value = proxy;
                }
                else
                {
                    var pro = currentNode.Value;
                    currentNode.Value = lastPos;
                    lastPos = pro;
                }
                currentNode = currentNode.Next;
            }
        }

        public void Detect()
        {
            SnakeCube face = snakeUser.First.Value;

            var shmelts = snakeUser.Where(x => x.X == face.X && x.Y == face.Y);

            if(shmelts.Count()>1)
            {
                // reset snake
                dir = SnakeMotivation.Down;
                snakeUser.Clear();
                snakeUser.AddFirst(new SnakeCube(0, 0));
                snakeUser.AddFirst(new SnakeCube(0, 1));
                snakeUser.AddFirst(new SnakeCube(0, 2));
            }


            if (face.Y == applePos.X && face.X == applePos.Y)
            {
                GrowLights();
                NewApplePos();
            }
        }

        private void NewApplePos()
        {
            applePos.X = randy.Next(1, 19);
            applePos.Y = randy.Next(1, 19);
        }

        private void GrowLights()
        {
            int xHit = 0;
            int yHit = 0;

            switch (dir)
            {
                case SnakeMotivation.Up:
                    yHit = -1;
                    break;
                case SnakeMotivation.Down:
                    yHit = 1;
                    break;
                case SnakeMotivation.Right:
                    xHit = 1;
                    break;
                case SnakeMotivation.Left:
                    xHit = -1;
                    break;
                default:
                    Console.WriteLine("Ctrl+F 'fortnight' asap!!!!!!!");
                    break;
            }


            snakeUser.AddFirst(new SnakeCube(applePos.Y + xHit, applePos.X + yHit));
        }

    }
}
