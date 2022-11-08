using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace SurfaceTransform
{
    class Program : GameWindow
    {

        public static Camera camera;
        private bool freeCamera = true;

        private bool _firstMove = true;
        private Vector2 _lastPos;

        //private float[] circle;

        //список со всеми линиями - экземплярами класса Surface
        private List<Surface> Surfaces;
        //линия движения ровера
        private float[] roverLine;

        private String drawType = "Point";

        public Program()
            : base(800, 600, GraphicsMode.Default, "Rover Vision")
        {
            WindowState = WindowState.Maximized;//формат окна
        }

        static void Main(string[] args)
        {
            using (Program program = new Program())
            {
                //TextWriterTraceListener writer = new TextWriterTraceListener(System.Console.Out);
                //Trace.Listeners.Add(writer);
                program.Run();
                //TextWriterTraceListener writer = new TextWriterTraceListener(System.Console.Out);
                //Trace.Listeners.Add(writer);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            //цвет фона
            GL.ClearColor(1f, 1f, 1f, 1.0f);

            camera = new Camera(new Vector3(0, 2, 0), Width / (float)Height);//положение камеры начальное

            roverLine = new float[]
                    {
                        0f,0.7f,2f,
                        0.7f,0.3f,2f,

                        0.7f,0.3f,2f,
                        2.2f,1f,2f,

                        2.2f,1f,2f,
                        3.3f,0.2f,2f,

                        3.3f,0.2f,2f,
                        4f,2.5f,2f,

                        4f,2.5f,2f,
                        5.7f,2.7f,2f,

                        5.7f,2.7f,2f,
                        6.3f,2.9f,2f,

                        6.3f,2.9f,2f,
                        7.2f,3.3f,2f,

                        7.2f,3.3f,2f,
                        8.4f,2.5f,2f,

                        8.4f,2.5f,2f,
                        9f,2.3f,2f,

                        9f,2.3f,2f,
                        10f,2.1f,2f,
                    };
            //задаём Surfaces
            Surfaces = new List<Surface>()
            {
                //чтобы задать новую линию нужно дать массив вершин и цвет
                //первая линия
                new Surface(
                    new float[]
                    {
                        0f,0.5f,0f,
                        1f,1.4f,0f,

                        1f,1.4f,0f,
                        1.9f,0.4f,0f,

                        1.9f,0.4f,0f,
                        3.1f,1.5f,0f,

                        3.1f,1.5f,0f,
                        4.2f,3.1f,0f,

                        4.2f,3.1f,0f,
                        5f,2.4f,0f,

                        5f,2.4f,0f,
                        6.1f,1.9f,0f,

                        6.1f,1.9f,0f,
                        6.7f,1.2f,0f,

                        6.7f,1.2f,0f,
                        8f,0.7f,0f,

                        8f,0.7f,0f,
                        9.2f,1.3f,0f,

                        9.2f,1.3f,0f,
                        10f,2.1f,0f,
                    },
                    new Vector3(1f, 0.980f, 0.058f),
                    drawType),
                //вторая линия
                new Surface(
                    new float[]
                    {
                        0f,0.3f,1f,
                        0.9f,0f,1f,

                        0.9f,0f,1f,
                        2.4f,2f,1f,

                        2.4f,2f,1f,
                        3.5f,1.3f,1f,

                        3.5f,1.3f,1f,
                        3.8f,2.5f,1f,

                        3.8f,2.5f,1f,
                        5.1f,2.4f,1f,

                        5.1f,2.4f,1f,
                        5.7f,1.8f,1f,

                        5.7f,1.8f,1f,
                        6.5f,1.3f,1f,

                        6.5f,1.3f,1f,
                        8.1f,0.2f,1f,

                        8.1f,0.2f,1f,
                        9.4f,0.9f,1f,

                        9.4f,0.9f,1f,
                        10f,1.5f,1f,
                    },
                    new Vector3(0.058f, 0.203f, 1f),
                    drawType),
                //третья линия
                new Surface(
                    new float[]
                    {
                        0f,0.7f,2f,
                        0.7f,0.3f,2f,

                        0.7f,0.3f,2f,
                        2.2f,1f,2f,

                        2.2f,1f,2f,
                        3.3f,0.2f,2f,

                        3.3f,0.2f,2f,
                        4f,2.5f,2f,

                        4f,2.5f,2f,
                        5.7f,2.7f,2f,

                        5.7f,2.7f,2f,
                        6.3f,2.9f,2f,

                        6.3f,2.9f,2f,
                        7.2f,3.3f,2f,

                        7.2f,3.3f,2f,
                        8.4f,2.5f,2f,

                        8.4f,2.5f,2f,
                        9f,2.3f,2f,

                        9f,2.3f,2f,
                        10f,2.1f,2f,
                    },
                    new Vector3(1f, 0.211f, 0.058f),
                    drawType),
                //четвертая линия
                new Surface(
                    new float[]
                    {
                        0f,0.1f,3f,
                        0.5f,0.7f,3f,

                        0.5f,0.7f,3f,
                        1.4f,0.4f,3f,

                        1.4f,0.4f,3f,
                        2.9f,1.2f,3f,

                        2.9f,1.2f,3f,
                        4.3f,2.6f,3f,

                        4.3f,2.6f,3f,
                        5.1f,3.2f,3f,

                        5.1f,3.2f,3f,
                        6.2f,2.1f,3f,

                        6.2f,2.1f,3f,
                        6.9f,1.7f,3f,

                        6.9f,1.7f,3f,
                        8.3f,2.8f,3f,

                        8.3f,2.8f,3f,
                        8.8f,1.4f,3f,

                        8.8f,1.4f,3f,
                        10f,2.1f,3f,
                    },
                    new Vector3(0.066f, 1f, 0.058f),
                    drawType),
                //пятая линия
                new Surface(
                    new float[]
                    {
                        0f,0.2f,4f,
                        1.3f,0.5f,4f,

                        1.3f,0.5f,4f,
                        1.7f,1.2f,4f,

                        1.7f,1.2f,4f,
                        3.1f,1.5f,4f,

                        3.1f,1.5f,4f,
                        3.8f,2f,4f,

                        3.8f,2f,4f,
                        5.1f,2.2f,4f,

                        5.1f,2.2f,4f,
                        6.5f,2.8f,4f,

                        6.5f,2.8f,4f,
                        7.1f,2.5f,4f,

                        7.1f,2.5f,4f,
                        8f,3.1f,4f,

                        8f,3.1f,4f,
                        8.8f,2.4f,4f,

                        8.8f,2.4f,4f,
                        10f,1.5f,4f,
                    },
                    new Vector3(1f, 0.058f, 0.984f),
                    drawType),
                //далее черные линии соединяющие вершины ломаных с одинаковыми номерами
                new Surface(
                    new float[]
                    {
                        0f,0.5f,0f,
                        0f,0.3f,1f,

                        0f,0.3f,1f,
                        0f,0.7f,2f,

                        0f,0.7f,2f,
                        0f,0.1f,3f,

                        0f,0.1f,3f,
                        0f,0.2f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        1f,1.4f,0f,
                        0.9f,0f,1f,

                        0.9f,0f,1f,
                        0.7f,0.3f,2f,

                        0.7f,0.3f,2f,
                        0.5f,0.7f,3f,

                        0.5f,0.7f,3f,
                        1.3f,0.5f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        1.9f,0.4f,0f,
                        2.4f,2f,1f,

                        2.4f,2f,1f,
                        2.2f,1f,2f,

                        2.2f,1f,2f,
                        1.4f,0.4f,3f,

                        1.4f,0.4f,3f,
                        1.7f,1.2f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        3.1f,1.5f,0f,
                        3.5f,1.3f,1f,

                        3.5f,1.3f,1f,
                        3.3f,0.2f,2f,

                        3.3f,0.2f,2f,
                        2.9f,1.2f,3f,

                        2.9f,1.2f,3f,
                        3.1f,1.5f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        4.2f,3.1f,0f,
                        3.8f,2.5f,1f,

                        3.8f,2.5f,1f,
                        4f,2.5f,2f,

                        4f,2.5f,2f,
                        4.3f,2.6f,3f,

                        4.3f,2.6f,3f,
                        3.8f,2f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        5f,2.4f,0f,
                        5.1f,2.4f,1f,

                        5.1f,2.4f,1f,
                        5.7f,2.7f,2f,

                        5.7f,2.7f,2f,
                        5.1f,3.2f,3f,

                        5.1f,3.2f,3f,
                        5.1f,2.2f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        6.1f,1.9f,0f,
                        5.7f,1.8f,1f,

                        5.7f,1.8f,1f,
                        6.3f,2.9f,2f,

                        6.3f,2.9f,2f,
                        6.2f,2.1f,3f,

                        6.2f,2.1f,3f,
                        6.5f,2.8f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        6.7f,1.2f,0f,
                        6.5f,1.3f,1f,

                        6.5f,1.3f,1f,
                        7.2f,3.3f,2f,

                        7.2f,3.3f,2f,
                        6.9f,1.7f,3f,

                        6.9f,1.7f,3f,
                        7.1f,2.5f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        8f,0.7f,0f,
                        8.1f,0.2f,1f,

                        8.1f,0.2f,1f,
                        8.4f,2.5f,2f,

                        8.4f,2.5f,2f,
                        8.3f,2.8f,3f,

                        8.3f,2.8f,3f,
                        8f,3.1f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        9.2f,1.3f,0f,
                        9.4f,0.9f,1f,

                        9.4f,0.9f,1f,
                        9f,2.3f,2f,

                        9f,2.3f,2f,
                        8.8f,1.4f,3f,

                        8.8f,1.4f,3f,
                        8.8f,2.4f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),
                new Surface(
                    new float[]
                    {
                        10f,2.1f,0f,
                        10f,1.5f,1f,

                        10f,1.5f,1f,
                        10f,2.1f,2f,

                        10f,2.1f,2f,
                        10f,2.1f,3f,

                        10f,2.1f,3f,
                        10f,1.5f,4f,
                    },
                    new Vector3(0f, 0f, 0f),
                    drawType),

            };

            //задаём окружность
            //circle = new float[] {};
            //List<float> cir = new List<float>();
            //for (double i = 0; i < 2 * Math.PI; i += 0.2d)
            //{
            //    //circle.Append((float)Math.Cos(i));
            //    //circle.Append((float)Math.Sin(i));
            //    cir.Add((float)Math.Cos(i));
            //    cir.Add((float)Math.Sin(i));
            //    cir.Add(0f);
            //}
            //circle = cir.ToArray();

            //загружаем все Surface внутри Surfaces
            for (int i = 0; i < Surfaces.Count; i++)
            {
                Surfaces[i].load();
            }

            CursorVisible = false;

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var transform = Matrix4.Identity;

            //рендерим все Surface внутри Surfaces
            for (int i = 0; i < Surfaces.Count; i++)
            {
                Surfaces[i].render(e, transform);
            }

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape)) Exit();

            if (input.IsKeyDown(Key.F)) freeCamera = true;
            if (input.IsKeyDown(Key.G)) freeCamera = false;

            const float cameraSpeed = 3f;
            const float sensitivity = 0.2f;
            //движение при свободной камере
            if (freeCamera)
            {
                if (input.IsKeyDown(Key.W))
                {
                    camera.Position += camera.Front * cameraSpeed * (float)e.Time; // Forward
                }
                if (input.IsKeyDown(Key.S))
                {
                    camera.Position -= camera.Front * cameraSpeed * (float)e.Time; // Backwards
                }
                if (input.IsKeyDown(Key.A))
                {
                    camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
                }
                if (input.IsKeyDown(Key.D))
                {
                    camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
                }
                if (input.IsKeyDown(Key.Space))
                {
                    camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
                }
                if (input.IsKeyDown(Key.LShift))
                {
                    camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
                }
            }
            //движение при привязанной камере
            if (!freeCamera)
            {
                camera.Position =
                    new Vector3(camera.Position.X, camera.Ynofreecamera(roverLine, camera.Position.X) + 1f, 2f);
                if (input.IsKeyDown(Key.W))
                {
                    camera.Position += new Vector3(1f, 0f, 0f) * cameraSpeed * (float)e.Time; // Forward
                }
                if (input.IsKeyDown(Key.S))
                {
                    camera.Position -= new Vector3(1f, 0f, 0f) * cameraSpeed * (float)e.Time; // Backwards
                }
                //ограничиваем движение вне линии при привязанной камере
                if (camera.Position.X < roverLine[0]) camera.Position =
                        new Vector3(roverLine[0], camera.Position.Y, camera.Position.Z);
                if (camera.Position.X > roverLine[roverLine.Length - 3]) camera.Position =
                        new Vector3(roverLine[roverLine.Length - 3], camera.Position.Y, camera.Position.Z);
            }


            var mouse = Mouse.GetState();

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {

                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);


                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity;
                //здесь ограничиваем угол обзора(тангажа и рыскания) у несвободной камеры в 2*20 градусов 
                if (!freeCamera && camera.Yaw < -20) camera.Yaw = -20;
                if (!freeCamera && camera.Yaw > 20) camera.Yaw = 20;
                if (!freeCamera && camera.Pitch < -20) camera.Pitch = -20;
                if (!freeCamera && camera.Pitch > 20) camera.Pitch = 20;
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused)
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }
            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            camera.AspectRatio = Width / (float)Height;
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            //удаляем все Surface внутри Surfaces
            for (int i = 0; i < Surfaces.Count; i++)
            {
                Surfaces[i].destroy(e);
            }

            base.OnUnload(e);
        }
    }
}