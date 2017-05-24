using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;

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

        public MainWindow()
        {
            this.WindowState = System.Windows.WindowState.Maximized;
            InitializeComponent();
        }

        private void GLviewLeft(object sender, EventArgs e)
        {
            glInit(left, sender);
        }

        private void GLviewRight(object sender, EventArgs e)
        {
            glInit(right, sender);
        }

        private void GLviewTop(object sender, EventArgs e)
        {
            glInit(top, sender);
        }

        private void GLviewBot(object sender, EventArgs e)
        {
            glInit(bot, sender);
        }

        private void glInit(GLControl g, object sender)
        {
            var flags = GraphicsContextFlags.Default;
            g = new GLControl(new GraphicsMode(32, 24), 2, 0, flags);
            g.Context.MakeCurrent(null);
            g.Paint += GLControl_Paint;
            g.Dock = DockStyle.Fill;
            (sender as WindowsFormsHost).Child = g;
            GL.Viewport(0, 0, 200, 200);
            double aspect_ratio = Width / (double)Height;
            float fov = 1.0f;
            float near_distance = 1.0f;
            float far_distance = 1000.0f;
            OpenTK.Matrix4 perspective_matrix =
            OpenTK.Matrix4.CreatePerspectiveFieldOfView(fov, (float)aspect_ratio, near_distance, far_distance);
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
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
            GL.Begin(BeginMode.Triangles);
            GL.Color3(Color.MidnightBlue);
            GL.Vertex2(-1.0f, 1.0f);
            GL.Color3(Color.SpringGreen);
            GL.Vertex2(0.0f, -1.0f);
            GL.Color3(Color.Ivory);
            GL.Vertex2(1.0f, 1.0f);
            GL.End();
            GL.Flush();
            x.SwapBuffers();
        }
    }
}


//GL.LoadIdentity();
//            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
//            GL.Begin(BeginMode.Triangles);
//            GL.Color3(Color.MidnightBlue);
//            GL.Vertex2(-1.0f, 1.0f);
//            GL.Color3(Color.SpringGreen);
//            GL.Vertex2(0.0f, -1.0f);
//            GL.Color3(Color.Ivory);
//            GL.Vertex2(1.0f, 1.0f);
//            GL.End();