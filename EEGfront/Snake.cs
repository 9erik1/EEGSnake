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
            while (currentNode != null)
            {
                if (currentNode.List.Count == 1)
                {
                    switch (dir)
                    {
                        case SnakeMotivation.Up:
                            proxy = currentNode.Value;
                            proxy.Y --;
                            if (proxy.Y < 0)
                                proxy.Y = 19;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Down:
                            proxy = currentNode.Value;
                            proxy.Y ++;
                            if (proxy.Y > 19)
                                proxy.Y = 0;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Right:
                            proxy = currentNode.Value;
                            proxy.X ++;
                            if (proxy.X > 19)
                                proxy.X = 0;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Left:
                            proxy = currentNode.Value;
                            proxy.X --;
                            if (proxy.X < 0)
                                proxy.X = 19;
                            snakeUser.First.Value = proxy;
                            break;
                        default:
                            Console.WriteLine("Ctrl+F 'fortnight' asap!!!!!!!");
                            break;
                    }
                    break;
                }
                else if (currentNode.Previous == null)
                {
                    currentNode = currentNode.Next;
                }
                else
                {
                    SnakeCube lastPos = currentNode.Previous.Value;
                    SnakeCube currentPos = currentNode.Value;

                    if (lastPos.X == currentPos.X)
                    {
                        if (lastPos.X < currentPos.X)
                            currentPos.X--;
                        else
                            currentPos.X++;
                    }
                    if (lastPos.Y == currentPos.Y)
                    {
                        if (lastPos.Y < currentPos.Y)
                            currentPos.Y--;
                        else
                            currentPos.Y++;
                    }

                    currentNode.Value = currentPos;

                    currentNode = currentNode.Next;
                }
            }
        }

        public void Detect()
        {
            //if (SnakeX < 0)
            //{
            //    SnakeX = XMAX - 1;
            //    return false;
            //}
            //if (SnakeY < 0)
            //{
            //    SnakeY = YMAX - 1;
            //    return false;
            //}

            //if (SnakeX > 19)
            //{
            //    SnakeX = 0;
            //    return false;
            //}
            //if (SnakeY > 19)
            //{
            //    SnakeY = 0;
            //    return false;
            //}

            SnakeCube face = snakeUser.First.Value;
            if (face.Y == applePos.X && face.X == applePos.Y)
            {
                NewApplePos();
            }
        }

        private void NewApplePos()
        {
            applePos.X = randy.Next(1,20);
            applePos.Y = randy.Next(1,20);
        }

        private void NewApple()
        {

        }

    }
}
