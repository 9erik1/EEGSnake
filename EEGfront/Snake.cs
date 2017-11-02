using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace EEGfront
{
    public class Snake
    {

        private Random randy;
        private LinkedList<Vector> snakeUser;
        private Vector applePos;
        private SnakeMotivation dir;

        public Snake()
        {
            snakeUser = new LinkedList<Vector>();
            snakeUser.AddFirst(new Vector(0.0d, 0.0d));
            randy = new Random();
            NewApplePos();
            dir = SnakeMotivation.Down;
        }

        public LinkedList<Vector> GetSnakeList()
        {
            return snakeUser;
        }

        public Vector GetApplePos()
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
            LinkedListNode<Vector> currentNode = snakeUser.First;
            Vector proxy;//for mod the positions
            while (currentNode != null)
            {
                if (currentNode.List.Count == 1)
                {
                    switch (dir)
                    {
                        case SnakeMotivation.Up:
                            proxy = currentNode.Value;
                            proxy.Y -= 0.1;
                            if (proxy.Y < 0)
                                proxy.Y = 1.9;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Down:
                            proxy = currentNode.Value;
                            proxy.Y += 0.1;
                            if (proxy.Y > 1.9)
                                proxy.Y = 0.0;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Right:
                            proxy = currentNode.Value;
                            proxy.X += 0.1;
                            if (proxy.X > 1.9)
                                proxy.X = 0.0;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Left:
                            proxy = currentNode.Value;
                            proxy.X -= 0.1;
                            if (proxy.X < 0)
                                proxy.X = 1.9;
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
                    Vector lastPos = currentNode.Previous.Value;
                    Vector currentPos = currentNode.Value;

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

            Vector face = snakeUser.First.Value;
            face.X = Math.Round(face.X, 1);
            face.Y = Math.Round(face.Y, 1);
            if (face.Y == applePos.X && face.X == applePos.Y)
            {
                NewApplePos();
            }
        }

        private void NewApplePos()
        {
            applePos.X = Math.Round(randy.NextDouble() * 2, 1);
            applePos.Y = Math.Round(randy.NextDouble() * 2, 1);
        }

        private void NewApple()
        {

        }

    }
}
