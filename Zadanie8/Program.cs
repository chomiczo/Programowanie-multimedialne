using OpenTK;
using OpenTK.Graphics.OpenGL4;
using GLFW;
using GlmSharp;

using Shaders;
using Models;

namespace PMLabs
{
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
        static Torus torusLeft1 = new Torus();
        static Torus torusLeft2 = new Torus();
        static Torus torusLeft3 = new Torus();

        static Torus torusRight1 = new Torus();
        static Torus torusRight2 = new Torus();
        static Torus torusRight3 = new Torus();

        static Cube cube1 = new Cube();

        static float angleTorusLeft1 = 0.0f;
        static float angleTorusLeft2 = 0.0f;
        static float angleTorusLeft3 = 0.0f;

        static float angleTorusRight1 = 0.0f;
        static float angleTorusRight2 = 0.0f;
        static float angleTorusRight3 = 0.0f;

        public static void InitOpenGLProgram(Window window)
        {
            GL.ClearColor(0, 0, 0, 1);
            DemoShaders.InitShaders("Shaders\\");
        }

        public static void DrawScene(Window window, float time)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mat4 V = mat4.LookAt(
                new vec3(0.0f, 0.0f, -5.0f),
                new vec3(0.0f, 0.0f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));
            mat4 P = mat4.Perspective(glm.Radians(50.0f), 1.0f, 1.0f, 50.0f);

            DemoShaders.spConstant.Use();
            GL.UniformMatrix4(DemoShaders.spConstant.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(DemoShaders.spConstant.U("V"), 1, false, V.Values1D);

            // Rysowanie torusów po lewej stronie
            DrawTorus(torusLeft1, new vec3(-0.8f, 1.4f, 0.0f), angleTorusLeft1, time, 0.8f);
            DrawTorus(torusLeft2, new vec3(-1.4f, 0.0f, 0.0f), angleTorusLeft2, time, 0.8f);
            DrawTorus(torusLeft3, new vec3(-0.8f, -1.5f, 0.0f), angleTorusLeft3, time, 0.8f);

            // Rysowanie torusów po prawej stronie
            DrawTorus(torusRight1, new vec3(0.8f, 1.4f, 0.0f), angleTorusRight1, time, 0.8f);
            DrawTorus(torusRight2, new vec3(1.4f,0.0f, 0.0f), angleTorusRight2, time, 0.8f);
            DrawTorus(torusRight3, new vec3(0.8f, -1.5f, 0.0f), angleTorusRight3, time, 0.8f);

            Glfw.SwapBuffers(window);
        }

        public static void FreeOpenGLProgram(Window window)
        {
            // Możesz dodać odpowiednie czyszczenie zasobów tutaj, jeśli jest to konieczne
        }

        static void Main(string[] args)
        {
            Glfw.Init();
            Window window = Glfw.CreateWindow(500, 500, "Programowanie multimedialne", GLFW.Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);
            GL.LoadBindings(new BC());

            InitOpenGLProgram(window);

            Glfw.Time = 0;

            while (!Glfw.WindowShouldClose(window))
            {
                DrawScene(window, (float)Glfw.Time);
                Glfw.PollEvents();
            }

            FreeOpenGLProgram(window);
            Glfw.Terminate();
        }

        static void DrawTeeth(mat4 torusMatrix)
        {
            for (int i = 0; i < 12; i++)
            {
                float angle = glm.Radians(30.0f * i);
                mat4 Mk = torusMatrix * mat4.Rotate(angle, new vec3(0.0f, 0.0f, 1.0f)) * mat4.Translate(new vec3(0.9f, 0.0f, 0.0f)) * mat4.Scale(new vec3(0.15f));
                GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, Mk.Values1D);
                cube1.drawWire();
            }
        }

        static void DrawTorus(Torus torus, vec3 translation, float angle, float time, float scale)
        {
            mat4 M = mat4.Identity;
            M *= mat4.Translate(translation);
            M *= mat4.Rotate(glm.Radians(100.0f * time + angle), new vec3(0.0f, 0.0f, 1.0f));
            M *= mat4.Scale(new vec3(scale));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, M.Values1D);
            torus.drawWire();

            DrawTeeth(M);
        }
    }
}
