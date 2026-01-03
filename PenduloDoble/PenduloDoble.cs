using System.Numerics;
using Raylib_cs;

// Esta clase ha sido diseñada con ayuda de Gemini 3 pro
namespace PenduloDoble;

public class PenduloDoble
{
    // Variables estáticas. No cambian durante el movimiento
    private float _masa1;
    private float _masa2;
    private float _longitud1; // Longitud de varilla de la masa 1
    private float _longitud2 ; // Longitud de varilla de la masa 2
    private Vector2 _posicionAnclaje;
    private float _g; // Gravedad
    private float _friccion; // si la fricción no se incluye, entendemos que no hay, así que ponemos 1 por defecto


    // Variables dinámicas. Cambiarán durante el movimiento
    private float _theta1; // Ángulo con respecto a la vertical de la masa 1
    private float _theta2; // Ángulo con respecto a la vertical de la masa 2
    private float _omega1; // Velocidad angular de la masa 1
    private float _omega2; // Velocidad angular de la masa 2
    private float _alpha1; // Aceleración angular de la masa 1
    private float _alpha2; // Aceleración angular de la masa 2

    // Posiciones de las masas
    private Vector2 _posicionM1;
    private Vector2 _posicionM2;

    // Registro de posiciones para la traza de la masa 2
    Queue<Vector2> _traza = new Queue<Vector2>();
    const int LongitudTraza = 10000; // últimas 100 posiciones


    // Constructor
    public PenduloDoble(Vector2 anclaje, float m1, float m2, float l1, float l2, float gravedad, float the1, float the2,
        float omg1,
        float omg2,
        float friccion)
    {
        _posicionAnclaje = anclaje;
        _masa1 = m1;
        _masa2 = m2;
        _longitud1 = l1;
        _longitud2 = l2;
        _g = gravedad;
        _theta1 = the1 * MathF.PI / 180;
        _theta2 = the2 * MathF.PI / 180;
        _omega1 = omg1;
        _omega2 = omg2;
        _friccion = friccion;
    }

    // Actualización de físicas
    public void ActualizarFisica(float dt)
    {

        // 1. Cálculo de aceleraciones
        // Cálculo de Alpha 1 Está formado por tres numeradores y un denominador
        float num1 = -_g * (2 * _masa1 + _masa2) * MathF.Sin(_theta1);
        float num2 = -_masa2 * _g * MathF.Sin(_theta1 - 2 * _theta2);
        float num3 = -2 * MathF.Sin(_theta1 - _theta2) * _masa2 * (MathF.Pow(_omega2,2) * _longitud2 + MathF.Pow(_omega1, 2) * _longitud1 * MathF.Cos(_theta1 - _theta2));
        float den = _longitud1 * (2 * _masa1 + _masa2 - _masa2 * MathF.Cos(2 * _theta1 - 2 * _theta2));

        _alpha1 = (num1 + num2 + num3) / den;

        // Cálculo de Alpha 2
        float num4 = 2 * MathF.Sin(_theta1 - _theta2);
        float num5 = (MathF.Pow(_omega1, 2) * _longitud1 * (_masa1 + _masa2));
        float num6 = _g * (_masa1 + _masa2) * MathF.Cos(_theta1);
        float num7 = MathF.Pow(_omega2, 2) * _longitud2 * _masa2 * MathF.Cos(_theta1 - _theta2);
        float den2 = _longitud2 * (2 * _masa1 + _masa2 - _masa2 * MathF.Cos(2 * _theta1 - 2 * _theta2));

        _alpha2 = (num4 * (num5 + num6+ num7)) / den2;

        // 2. Aplicación del método de Euler
        // Velocidades
        _omega1 += _alpha1 * dt;
        _omega2 += _alpha2 * dt;
        // Ángulos
        _theta1 += _omega1 * dt;
        _theta2 += _omega2 * dt;

        // Fricción con el aire
        _omega1 *= _friccion;
        _omega2 *= _friccion;
    }

    public void CalcularPosiciones()
    {
        // Posición de la primera masa
        _posicionM1 = _posicionAnclaje +
                        new Vector2(_longitud1 * MathF.Sin(_theta1), _longitud1 * MathF.Cos(_theta1));
        // Posición de la segunda masa
        _posicionM2 = _posicionM1 + new Vector2(_longitud2 * MathF.Sin(_theta2), _longitud2 * MathF.Cos(_theta2));
    }

    public void Dibujar()
    {
        // Calculamos posiciones cartesianas
        CalcularPosiciones();
        // Añadir posición actual a la traza
        _traza.Enqueue(_posicionM2);
        if (_traza.Count > LongitudTraza)
            _traza.Dequeue();

        // Dibujamos brazos
        Raylib.DrawLineEx(_posicionAnclaje - new Vector2(50, 0),
            _posicionAnclaje + new Vector2(50, 0), 3.0f, Color.White); // Linea horizontal
        Raylib.DrawLineEx(_posicionAnclaje, _posicionM1, 3.0f, Color.White); // Brazo masa 1
        Raylib.DrawLineEx(_posicionM1, _posicionM2, 3.0f, Color.White); // Brazo masa 2

        // Dibujar la traza
        int i = 0;
        int totalPuntos = _traza.Count;
        foreach (Vector2 traza in _traza)
        {
            float close = (float)i / totalPuntos;
            int alphaValue = (int)(150 * close);
            Color colorTraza = new Color(255, 200, 100, alphaValue);
            Raylib.DrawCircleV(traza, 2.0f, colorTraza);
            i++;
        }

        // Dibujamos masas
        Raylib.DrawCircleV(_posicionAnclaje, 5.0f, Color.Green); // Clavo anclado
        Raylib.DrawCircleV(_posicionM1, 15.0f, Color.Red); // Masa 1
        Raylib.DrawCircleV(_posicionM2, 15.0f, Color.Blue); // Masa 2
    }
}