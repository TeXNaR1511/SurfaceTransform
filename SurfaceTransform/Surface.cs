using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SurfaceTransform
{
    class Surface
    {
        private readonly float[] surface;
        //private int _vertexBufferObject;
        private Shader surfaceShader;
        private int surfaceVAO;
        //private int _vertexBufferObject;
        private int surfaceBufferObject;

        private Vector3 objectColor = new Vector3();

        private String drawType;

        public Surface(float[] surface, Vector3 objectColor, String drawType)
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
            GL.LineWidth(4f);

            GL.GenVertexArrays(1, out surfaceVAO);
            GL.BindVertexArray(surfaceVAO);
            GL.GenBuffers(1, out surfaceBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, surfaceBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, surface.Length * sizeof(float), surface, BufferUsageHint.StaticDraw);

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
            if (drawType == "Line") GL.DrawArrays(PrimitiveType.LineStrip, 0, surface.Length);
            if (drawType == "Polygon") GL.DrawArrays(PrimitiveType.Polygon, 0, surface.Length);
            if (drawType == "Point") GL.DrawArrays(PrimitiveType.Points, 0, surface.Length);
        }

        public void destroy(EventArgs e)
        {
            GL.DeleteProgram(surfaceShader.Handle);
        }

    }
}



