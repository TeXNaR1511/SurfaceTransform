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
        private bool freeCamera = false;

        private bool _firstMove = true;
        private Vector2 _lastPos;

        //private float[] circle;

        //список со всеми линиями - экземплярами класса Surface
        private List<Surface> Surfaces;
        //линия движения ровера
        private float[] roverLine;

        private String drawType = "Line";

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
            GL.ClearColor(1f, 1f, 1f, 1f);

            camera = new Camera(new Vector3(0, 0, 3), Width / (float)Height);//положение камеры начальное

            //задаём Surfaces
            Surfaces = new List<Surface>()
            {
                //чтобы задать новую линию нужно дать массив вершин и цвет
                //первая линия
                new Surface(
                    new float[]
                    {
                        0f,0f,0f,
                        1f,2f,0f,
                        2f,1f,0f

                    },
                    new Vector3(0f,0f,0f),
                    "Line")
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

            CursorVisible = true;

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
            if (freeCamera)
            {
                CursorVisible = false;
                //Focused = true;
            }
            if(!freeCamera) CursorVisible = true;
            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            Console.Write(camera.Position);
            Console.Write("("+camera.return_pitch()+")");
            Console.WriteLine("("+camera.return_yaw()+")");
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape)) Exit();

            if (input.IsKeyDown(Key.F))
            {
                freeCamera = true;
                camera.Pitch = 0;
                camera.Yaw = MathHelper.RadiansToDegrees(-MathHelper.PiOver2);
            }
            if (input.IsKeyDown(Key.G))
            {
                freeCamera = false;
                camera.Position = new Vector3(0, 0, 3);
                camera.Pitch = 0;
                camera.Yaw = MathHelper.RadiansToDegrees(-MathHelper.PiOver2);
            }

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
                if (input.IsKeyDown(Key.A))
                {
                    camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
                }
                if (input.IsKeyDown(Key.D))
                {
                    camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
                }
                if (input.IsKeyDown(Key.W))
                {
                    camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
                }
                if (input.IsKeyDown(Key.S))
                {
                    camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
                }
                if (input.IsKeyDown(Key.Space))
                {
                    camera.Position += camera.Front * cameraSpeed * (float)e.Time; // Up
                }
                if (input.IsKeyDown(Key.LShift))
                {
                    camera.Position -= camera.Front * cameraSpeed * (float)e.Time; // Down
                }
            }


            var mouse = Mouse.GetState();

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                if(freeCamera)
                {
                    var deltaX = mouse.X - _lastPos.X;
                    var deltaY = mouse.Y - _lastPos.Y;
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    camera.Yaw += deltaX * sensitivity;
                    camera.Pitch -= deltaY * sensitivity;
                }
                //if(!freeCamera)
                //{
                //    camera.Pitch = 0;
                //    camera.Yaw = MathHelper.RadiansToDegrees(-MathHelper.PiOver2);
                //}
                //var deltaX = mouse.X - _lastPos.X;
                //var deltaY = mouse.Y - _lastPos.Y;
                //_lastPos = new Vector2(mouse.X, mouse.Y);


                //camera.Yaw += deltaX * sensitivity;
                //camera.Pitch -= deltaY * sensitivity;
                //здесь ограничиваем угол обзора(тангажа и рыскания) у несвободной камеры в 2*20 градусов 
                //if (freeCamera) camera.Yaw += deltaX * sensitivity;
                //if (freeCamera) camera.Pitch -= deltaY * sensitivity;
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused && freeCamera)
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