using System;
using System.Collections.Generic;
using System.Windows;

namespace EEGfront
{
    public class Snake
    {

        private LinkedList<Vector> snakeUser;
        private SnakeMotivation dir;

        public Snake()
        {
            snakeUser = new LinkedList<Vector>();
            snakeUser.AddFirst(new Vector(0.0d, 0.0d));
            dir = SnakeMotivation.Down;
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
                            proxy.Y += 0.1d;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Down:
                            proxy = currentNode.Value;
                            proxy.Y -= 0.1d;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Right:
                            proxy = currentNode.Value;
                            proxy.X += 0.1d;
                            snakeUser.First.Value = proxy;
                            break;
                        case SnakeMotivation.Left:
                            proxy = currentNode.Value;
                            proxy.X -= 0.1d;
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

        private void Detect()
        {

        }

        private void NewApple()
        {

        }

    }
}
