using System.Numerics;
using Raylib_cs;

namespace PenduloDoble;

class Program
{
    static void Main(string[] args)
    {
        const int width = 1024;
        const int height = 720;
        // creamos nuevo péndulo doble
        PenduloDoble pendulo = new PenduloDoble(
            new Vector2(width / 2, 100),
            m1:10.0f,
            m2:15.0f,
            l1:200.0f,
            l2:150.0f,
            gravedad:500.0f,
            the1:90.1f,
            the2:90f,
            omg1:0,
            omg2:0,
            friccion:1.0f
        );

        Raylib.InitWindow(width, height, "Física - Péndulo simple");
        Raylib.SetTargetFPS(60);



        while (!Raylib.WindowShouldClose())
        {
            // Obteniendo delta de t para los cálculos
            float dt = Raylib.GetFrameTime();

            // Actualizando físicas
            pendulo.ActualizarFisica(dt);



            // Empezamos a dibujar el frame
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Dibujamos el péndulo
            pendulo.Dibujar();

            // Fin del dibujo
            string texto = "Péndulo Doble";
            Raylib.DrawText(texto, 10, 10, 20, Color.White);
            Raylib.EndDrawing();
        }
    }
}