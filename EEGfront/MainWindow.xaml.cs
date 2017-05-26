using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace EEGfront
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GLControl left;
        private GLControl right;
        private GLControl top;
        private GLControl bot;
        private GLControl game;

        Thread drawL;
        Thread drawR;
        Thread drawT;
        Thread drawB;

        private bool lTog = true;
        private bool rTog = true;
        private bool tTog = true;
        private bool bTog = true;

        public MainWindow()
        {
            this.WindowState = System.Windows.WindowState.Maximized;
            InitializeComponent();
            drawL = new Thread(new ThreadStart(DrawL));
            drawL.Start();
            drawR = new Thread(new ThreadStart(DrawR));
            drawR.Start();
            drawT = new Thread(new ThreadStart(DrawT));
            drawT.Start();
            drawB = new Thread(new ThreadStart(DrawB));
            drawB.Start();

            left = (GLControl)Lefty.Child;
            right = (GLControl)Rightey.Child;
            top = (GLControl)Heven.Child;
            bot = (GLControl)Hell.Child;
        }

        private async void DrawL()
        {
            while (true)
            {
                await Task.Delay(200);
                await Dispatcher.BeginInvoke((Action) (() =>
                {
                    //GLControl l = (GLControl)Lefty.Child;
                    left.MakeCurrent();
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    if (lTog)
                    {
                        GL.ClearColor(Color.White);
                        lTog = false;
                    }
                    else
                    {
                        GL.ClearColor(Color.Black);
                        lTog = true;
                    }
                    left.SwapBuffers();
                }));
            }
        }

        private async void DrawR()
        {
            while (true)
            {
                await Task.Delay(400);
                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    right.MakeCurrent();
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    if (rTog)
                    {
                        GL.ClearColor(Color.White);
                        rTog = false;
                    }
                    else
                    {
                        GL.ClearColor(Color.Black);
                        rTog = true;
                    }
                    right.SwapBuffers();
                }));
            }
        }

        private async void DrawT()
        {
            while (true)
            {
                await Task.Delay(800);
                await Dispatcher.BeginInvoke((Action)(() =>
                {
                    top.MakeCurrent();
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    if (tTog)
                    {
                        GL.ClearColor(Color.White);
                        tTog = false;
                    }
                    else
                    {
                        GL.ClearColor(Color.Black);
                        tTog = true;
                    }
                    top.SwapBuffers();
                }));
            }
        }

        private async void DrawB()
        {
            while (true)
            {
                await Task.Delay(1600);
                await Dispatcher.BeginInvoke((Action)(() =>
                {                    
                    bot.MakeCurrent();
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    if (bTog)
                    {
                        GL.ClearColor(Color.White);
                        bTog = false;
                    }
                    else
                    {
                        GL.ClearColor(Color.Black);
                        bTog = true;
                    }
                    bot.SwapBuffers();
                }));
            }
        }

        private void GLviewLeft(object sender, EventArgs e)
        {
            glInit(left, sender, "lefty");
        }

        private void GLviewRight(object sender, EventArgs e)
        {
            glInit(right, sender, "rightey");
        }

        private void GLviewTop(object sender, EventArgs e)
        {
            glInit(top, sender, "heven");
        }

        private void GLviewBot(object sender, EventArgs e)
        {
            glInit(bot, sender, "hell");
        }

        private void GLviewGame(object sender, EventArgs e)
        {
            glInit(game, sender, "game");
        }

        private void glInit(GLControl g, object sender, string name)
        {
            var flags = GraphicsContextFlags.Default;
            g = new GLControl(new GraphicsMode(32, 24), 2, 0, flags);
            g.Context.MakeCurrent(null);
            g.Name = name;
            g.Paint += GLControl_Paint;
            g.Dock = DockStyle.Fill;
            (sender as WindowsFormsHost).Child = g;
            GL.Viewport(0, 0, 200, 200);
            double aspect_ratio = Width / (double)Height;
            float fov = 1.0f;
            float near_distance = 1.0f;
            float far_distance = 1000.0f;
            Matrix4 perspective_matrix =
            Matrix4.CreatePerspectiveFieldOfView(fov, (float)aspect_ratio, near_distance, far_distance);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective_matrix);
        }

        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            var x = sender as GLControl;
            x.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.Red);
            GL.MatrixMode(MatrixMode.Projection);
            GL.Flush();
            x.SwapBuffers();
        }
    }
}