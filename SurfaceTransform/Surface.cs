using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SurfaceTransform
{
    class Surface
    {
        //private float[] surface;
        private List<float> surface; 
        //private int _vertexBufferObject;
        private Shader surfaceShader;
        private int surfaceVAO;
        //private int _vertexBufferObject;
        private int surfaceBufferObject;

        private Vector3 objectColor = new Vector3();

        private String drawType;

        public Surface(List<float> surface, Vector3 objectColor, String drawType)
        {
            this.surface = surface;
            this.objectColor = objectColor;
            this.drawType = drawType;
            surfaceShader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
        }

        public void load()
        {
            surfaceShader.Use();

            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.PointSmooth);
            GL.PointSize(20f);
            GL.LineWidth(6f);

            GL.GenVertexArrays(1, out surfaceVAO);
            GL.BindVertexArray(surfaceVAO);
            GL.GenBuffers(1, out surfaceBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, surfaceBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, surface.Count * sizeof(float), surface.ToArray(), BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public void render(FrameEventArgs e, Matrix4 model)
        {
            surfaceShader.Use();
 
            surfaceShader.SetMatrix4("view", Program.camera.GetViewMatrix());
            surfaceShader.SetMatrix4("projection", Program.camera.GetProjectionMatrix());
            surfaceShader.SetVector3("objectColor", objectColor);//цвет того, что рисуем
            surfaceShader.SetMatrix4("model", model);

            GL.BindVertexArray(surfaceVAO);
            //выбираем как будем рисовать
            if (drawType == "Line") GL.DrawArrays(PrimitiveType.LineStrip, 0, surface.Count/3);
            if (drawType == "Polygon") GL.DrawArrays(PrimitiveType.Polygon, 0, surface.Count/3);
            if (drawType == "Point") GL.DrawArrays(PrimitiveType.Points, 0, surface.Count/3);
        }

        public void destroy(EventArgs e)
        {
            GL.DeleteProgram(surfaceShader.Handle);
        }

        public void pushPoint(float a, float b,float c)
        {
            //Console.WriteLine(surface.Count);
            surface.Add(a);
            //Console.WriteLine(surface.Count);
            surface.Add(b);
            surface.Add(c);
            //Console.WriteLine(surface.Count);
        }

        public void writeArray()
        {
            for(int i=0;i<surface.Count;i++)
            {
                Console.Write(surface[i]+",");
            }
            Console.WriteLine();
        }

        public List<float> getArray()
        {
            return surface;
        }
        public void movePoint(bool XorY, float coord)
        {
            if(XorY)
            {
                surface[surface.Count - 3] += coord;
            }
            else
            {
                surface[surface.Count - 2] += coord;
            }
        }

        public List<float> getLastPoint()
        {
            return new List<float> { surface[surface.Count - 3], surface[surface.Count - 2], surface[surface.Count - 1] };
        }

        public int getSurfaceSize()
        {
            return surface.Count;
        }
    }
}



