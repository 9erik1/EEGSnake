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
            dir = SnakeMotivation.Up;
        }

        public void MoveDown()
        {
            dir = SnakeMotivation.Down;
        }

        public void MoveLeft()
        {
            dir = SnakeMotivation.Left;
        }

        public void MoveRight()
        {
            dir = SnakeMotivation.Right;
        }

        // todo : on a thread somewhere
        public void MoveSnake()
        {
            LinkedListNode<SnakeCube> currentNode = snakeUser.First;
            SnakeCube proxy;//for mod the positions
            SnakeCube lastPos = new SnakeCube();
            while (currentNode != null)
            {
                if (currentNode.Previous == null)
                {
                    switch (dir)
                    {
                        case SnakeMotivation.Up:
                            proxy = currentNode.Value;
                            proxy.Y--;
                            if (proxy.Y < 0)
                                proxy.Y = 19;
                            currentNode.Value = proxy;
                            break;
                        case SnakeMotivation.Down:
                            proxy = currentNode.Value;
                            proxy.Y++;
                            if (proxy.Y > 19)
                                proxy.Y = 0;
                            currentNode.Value = proxy;
                            break;
                        case SnakeMotivation.Right:
                            proxy = currentNode.Value;
                            proxy.X++;
                            if (proxy.X > 19)
                                proxy.X = 0;
                            currentNode.Value = proxy;
                            break;
                        case SnakeMotivation.Left:
                            proxy = currentNode.Value;
                            proxy.X--;
                            if (proxy.X < 0)
                                proxy.X = 19;
                            currentNode.Value = proxy;
                            break;
                        default:
                            Console.WriteLine("Ctrl+F 'fortnight' asap!!!!!!!");
                            break;
                    }
                }
                else
                {
                    switch (dir)
                    {

                        case SnakeMotivation.Up:
                            proxy = currentNode.Previous.Value;
                            proxy.Y++;
                            if (proxy.Y > 19)
                                proxy.Y = 0;
                            currentNode.Value = proxy;
                            break;
                        case SnakeMotivation.Down:
                            proxy = currentNode.Previous.Value;
                            proxy.Y--;
                            if (proxy.Y < 0)
                                proxy.Y = 19;
                            currentNode.Value = proxy;
                            break;
                        case SnakeMotivation.Right:
                            proxy = currentNode.Previous.Value;
                            proxy.X++;
                            if (proxy.X > 19)
                                proxy.X = 0;
                            currentNode.Value = proxy;
                            break;
                        case SnakeMotivation.Left:
                            proxy = currentNode.Previous.Value;
                            proxy.X--;
                            if (proxy.X < 0)
                                proxy.X = 19;
                            currentNode.Value = proxy;
                            break;
                        default:
                            Console.WriteLine("Ctrl+F 'fortnight' asap!!!!!!!");
                            break;
                    }
                }

                currentNode = currentNode.Next;
            }
        }

        public void Detect()
        {
            SnakeCube face = snakeUser.First.Value;
            if (face.Y == applePos.X && face.X == applePos.Y)
            {
                GrowLights();
                NewApplePos();
            }
        }

        private void NewApplePos()
        {
            applePos.X = randy.Next(1,20);
            applePos.Y = randy.Next(1,20);
        }

        private void GrowLights()
        {
            int xHit = 0;
            int yHit = 0;

            switch (dir)
            {
                case SnakeMotivation.Up:
                    yHit = 1;
                    break;
                case SnakeMotivation.Down:
                    yHit = -1;
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

            snakeUser.AddFirst(new SnakeCube(applePos.X+xHit, applePos.Y+yHit));
        }

    }
}
