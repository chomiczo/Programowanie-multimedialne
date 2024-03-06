using GLFW;
using GlmSharp;
using Models;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Shaders;

namespace PMLabs
{
    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenGL do współpracy z OpenTK.
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
        static Sphere sun = new Sphere(0.5f, 12, 12);
        static Sphere planet = new Sphere(0.2f, 12, 12);
        static Sphere moon = new Sphere(0.1f, 12, 12);
        static Sphere planet2 = new Sphere(0.25f, 12, 12);
        static Sphere moon2 = new Sphere(0.07f, 12, 12);
        public static void InitOpenGLProgram(Window window)
        {
            // Czyszczenie okna na kolor czarny
            GL.ClearColor(0, 0, 0, 1);

            // Ładowanie programów cieniujących
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

            //Sun
            mat4 M = mat4.Identity;
            mat4 N = mat4.Identity;
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, M.Values1D);
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, N.Values1D);

            sun.drawWire();

            M *= mat4.Rotate(glm.Radians(60.0f * time), new vec3(0.0f, 1.0f, 0.0f));
            N *= mat4.Rotate(glm.Radians(60.0f * time), new vec3(0.0f, 0.0f, 1.0f));

            //Planet1
            M *= mat4.Translate(new vec3(1.5f, 0.0f, 0.0f));
            M *= mat4.Rotate(glm.Radians(100.0f * time), new vec3(0.0f, 1.0f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, M.Values1D);

            planet.drawWire();

            //Moon1
            M *= mat4.Translate(new vec3(0.5f, 0.0f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, M.Values1D);
            moon.drawWire();

            //Planet2
            N *= mat4.Translate(new vec3(0.0f, 2.0f, 0.0f));
            N *= mat4.Rotate(glm.Radians(100.0f * time), new vec3(1.0f, 0.0f, 0.0f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, N.Values1D);

            planet2.drawWire();

            //Moon2
            N *= mat4.Translate(new vec3(0.0f, 0.0f, 0.3f));
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, N.Values1D);
            moon2.drawWire();

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

            GL.LoadBindings(new BC()); //Pozyskaj adresy implementacji poszczególnych procedur OpenGL

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


    }
}