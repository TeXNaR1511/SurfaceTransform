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

        private float pointSensitivity = 1f;

        private bool isPressedBefore = false;

        private bool isEditPoints = false;

        private bool isFrontLine = true;

        private int numberOfLine = 0;

        private int MaxNumberOfLine;

        private bool isEnterPressedBefore = false;

        private int firstEditingPoint = 0;

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

            camera = new Camera(new Vector3(0, 0, 2), Width / (float)Height);//положение камеры начальное

            //задаём Surfaces
            Surfaces = new List<Surface>()
            {
                //чтобы задать новую линию нужно дать массив вершин и цвет
                //первая линия типо левое колесо
                new Surface(
                    new List<float>
                    {
                        0f,0f,0f,
                        1f,0f,0f
                    },
                    new Vector3(0f,0f,0f),
                    "Point"),
                new Surface(
                    new List<float>
                    {
                        0f,0f,0f,
                        1f,0f,0f
                    },
                    new Vector3(0f,1f,0f),
                    "Line"),
                //вторая линия типо над ней брюхо едет
                new Surface(
                    new List<float>
                    {
                        0f,0f,2f,
                        1f,0f,2f
                    },
                    new Vector3(0f,0f,0f),
                    "Point"),
                new Surface(
                    new List<float>
                    {
                        0f,0f,2f,
                        1f,0f,2f
                    },
                    new Vector3(1f,0f,0f),
                    "Line"),
                //третья линия типо правое колесо
                new Surface(
                    new List<float>
                    {
                        0f,0f,4f,
                        1f,0f,4f
                    },
                    new Vector3(0f,0f,0f),
                    "Point"),
                new Surface(
                    new List<float>
                    {
                        0f,0f,4f,
                        1f,0f,4f
                    },
                    new Vector3(0f,0f,1f),
                    "Line")
            };
            MaxNumberOfLine = Surfaces.Count / 2;
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

            //Console.Write(camera.Position);
            //Console.Write("("+camera.return_pitch()+")");
            //Console.WriteLine("("+camera.return_yaw()+")");
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
                if (input.IsKeyDown(Key.E))
                {
                    isEditPoints = true;
                    numberOfLine= 0;
                    isPressedBefore = false;
                    isEnterPressedBefore= false;
                    firstEditingPoint = 0;
                }
                if (input.IsKeyDown(Key.R)) isEditPoints = false;
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
                if(input.IsKeyDown(Key.Enter) && !isEnterPressedBefore)
                {
                    numberOfLine++;
                    isEnterPressedBefore = true;
                    if (numberOfLine > MaxNumberOfLine) numberOfLine = MaxNumberOfLine;
                    if(isEditPoints) firstEditingPoint = 0;
                }
                if(numberOfLine<MaxNumberOfLine)
                {
                    if (input.IsKeyDown(Key.RShift) && !isPressedBefore)
                    {
                        if (!isEditPoints)
                        {
                            Surfaces[0 + 2 * numberOfLine].pushPoint(Surfaces[0 + 2 * numberOfLine].getLastPoint()[0], Surfaces[0 + 2 * numberOfLine].getLastPoint()[1], Surfaces[0 + 2 * numberOfLine].getLastPoint()[2]);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].pushPoint(Surfaces[1 + 2 * numberOfLine].getLastPoint()[0], Surfaces[1 + 2 * numberOfLine].getLastPoint()[1], Surfaces[1 + 2 * numberOfLine].getLastPoint()[2]);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        if (isEditPoints && firstEditingPoint < Surfaces[0 + 2 * numberOfLine].getSurfaceSize() / 3 - 1)
                        {
                            firstEditingPoint++;
                            Console.WriteLine(Surfaces[0 + 2 * numberOfLine].getSurfaceSize() / 3 + " " + firstEditingPoint);
                        }
                        //Console.WriteLine(Surfaces[0].getSurfaceSize());
                        isPressedBefore = true;
                    }
                    if (input.IsKeyDown(Key.Up))
                    {
                        if (!isEditPoints)
                        {
                            Surfaces[0 + 2 * numberOfLine].moveLastPoint(false, pointSensitivity * (float)e.Time);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].moveLastPoint(false, pointSensitivity * (float)e.Time);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        else
                        {
                            Surfaces[0 + 2 * numberOfLine].moveSpecificPoint(false, firstEditingPoint, pointSensitivity * (float)e.Time);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].moveSpecificPoint(false, firstEditingPoint, pointSensitivity * (float)e.Time);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        isPressedBefore = false;
                        isEnterPressedBefore = false;
                    }
                    if (input.IsKeyDown(Key.Down))
                    {
                        if(!isEditPoints)
                        {
                            Surfaces[0 + 2 * numberOfLine].moveLastPoint(false, -pointSensitivity * (float)e.Time);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].moveLastPoint(false, -pointSensitivity * (float)e.Time);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        else
                        {
                            Surfaces[0 + 2 * numberOfLine].moveSpecificPoint(false, firstEditingPoint, -pointSensitivity * (float)e.Time);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].moveSpecificPoint(false, firstEditingPoint, -pointSensitivity * (float)e.Time);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        isPressedBefore = false;
                        isEnterPressedBefore = false;
                    }
                    if (input.IsKeyDown(Key.Left))
                    {
                        if(!isEditPoints)
                        {
                            Surfaces[0 + 2 * numberOfLine].moveLastPoint(true, -pointSensitivity * (float)e.Time);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].moveLastPoint(true, -pointSensitivity * (float)e.Time);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        else
                        {
                            Surfaces[0 + 2 * numberOfLine].moveSpecificPoint(true, firstEditingPoint, -pointSensitivity * (float)e.Time);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].moveSpecificPoint(true, firstEditingPoint, -pointSensitivity * (float)e.Time);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        isPressedBefore = false;
                        isEnterPressedBefore = false;
                    }
                    if (input.IsKeyDown(Key.Right))
                    {
                        if(!isEditPoints)
                        {
                            Surfaces[0 + 2 * numberOfLine].moveLastPoint(true, pointSensitivity * (float)e.Time);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].moveLastPoint(true, pointSensitivity * (float)e.Time);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        else
                        {
                            Surfaces[0 + 2 * numberOfLine].moveSpecificPoint(true, firstEditingPoint, pointSensitivity * (float)e.Time);
                            Surfaces[0 + 2 * numberOfLine].load();
                            Surfaces[1 + 2 * numberOfLine].moveSpecificPoint(true, firstEditingPoint, pointSensitivity * (float)e.Time);
                            Surfaces[1 + 2 * numberOfLine].load();
                        }
                        isPressedBefore = false;
                        isEnterPressedBefore = false;
                    }
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

            //processing mouse button events

            //2if(!freeCamera)
            //2{
            //2    //for(int i = 0; i < Surfaces[0].getArray().Length;i+=2)
            //2    //{
            //2    //    Console.WriteLine(camera.GetProjectionMatrix()*camera.GetViewMatrix() * new Vector4(Surfaces[0].getArray()[i], Surfaces[0].getArray()[i + 1], 0f, 0f));
            //2    //}
            //2    // 2d Viewport Coordinates
            //2    //Console.WriteLine(Vector3.Project(new Vector3(mouse.X,mouse.Y,0f),0,0,Width,Height, 0.01f, 100000f,Matrix4.Mult(camera.GetViewMatrix(),camera.GetOrthoProjectionMatrix())));
            //2    if (mouse.IsButtonDown(MouseButton.Left))
            //2    {
            //2        //Console.WriteLine((mouse.X-Width/2f)/(Width/2f) + " " + (-mouse.Y - Height / 2f) / (Height / 2f));
            //2        //Console.WriteLine();
            //2        //Surfaces[0].writeArray();
            //2        //Surfaces[1].writeArray();
            //2        //Surfaces[0].pushPoint((float)mouse.X, (float)mouse.Y);
            //2        //Surfaces[1].pushPoint((float)mouse.X, (float)mouse.Y);
            //2        //Surfaces[0].load();
            //2        //Surfaces[1].load();
            //2        //Surfaces[0].render(e, Matrix4.Identity);
            //2        //Surfaces[1].render(e, Matrix4.Identity);
            //2    }
            //2}

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