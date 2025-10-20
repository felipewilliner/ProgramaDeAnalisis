using Calculus;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace TrabajoAnalisis
{
    public class Unidad4
    {
        private Calculo AnalizadorDeFunciones = new Calculo();

        // Helper para manejar la sintaxis y el reemplazo de e^x (similar a Unidad1.cs)
        private string PreprocesarFuncion(string funcion)
        {
            // Reemplazar e^x por exp(x) (similar a Unidad1.cs)
            funcion = Regex.Replace(funcion, @"e\^(\([^()]*(?:\([^()]*\)[^()]*)*\)|[^\s\+\-\*\/\)]+)", "exp($1)");
            funcion = Regex.Replace(funcion, @"e\^([^()\s\+\-\*\/]+)", "exp($1)");

            // Reemplazar e solo por su valor (similar a Unidad1.cs)
            funcion = Regex.Replace(funcion, @"(?<![a-zA-Z0-9_\^])e(?![a-zA-Z0-9_\^])", Math.E.ToString());

           

            if (AnalizadorDeFunciones.Sintaxis(funcion, 'x'))
            {
                // Devolvemos un mensaje de error si la sintaxis falla
                return "Función mal ingresada o sintaxis incorrecta. Código: ";
            }
            return string.Empty;
        }

        // --- Método Auxiliar para Trapecios Simple (Punto 171) ---
        private double TrapeciosSimple(double xi, double xd)
        {
            // Fórmula: (f(xi) + f(xd)) * (xd - xi) / 2
            double fa = AnalizadorDeFunciones.EvaluaFx(xi);
            double fb = AnalizadorDeFunciones.EvaluaFx(xd);

            return (fa + fb) * (xd - xi) / 2;
        }

        // --- Método Auxiliar para Trapecios Múltiple (Punto 183) ---
        private double TrapeciosMultiple(double xi, double xd, int n)
        {
            // h = (xd - xi) / n
            double h = (xd - xi) / n;
            double sum = 0;

            // Sumatoria de f(xi) para i=1 hasta n-1
            for (int i = 1; i < n; i++)
            {
                // xi + h*i
                sum += AnalizadorDeFunciones.EvaluaFx(xi + h * i);
            }

            // Fórmula: (h/2) * (f(a) + 2*sum + f(b)) 
            // Corregido del error en el pseudocódigo PDF (usa h/2 y no (h/2)^2)
            return (h / 2) * (AnalizadorDeFunciones.EvaluaFx(xi) + 2 * sum + AnalizadorDeFunciones.EvaluaFx(xd));
        }

        // --- Método Auxiliar para Simpson 1/3 Simple (Punto 200) ---
        private double Simpson1_3Simple(double xi, double xd)
        {
            // n=2, h = (xd - xi) / 2
            double h = (xd - xi) / 2;

            // Fórmula: (h/3) * (f(a) + 4*f(a+h) + f(b))
            return (h / 3) * (AnalizadorDeFunciones.EvaluaFx(xi) +
                             4 * AnalizadorDeFunciones.EvaluaFx(xi + h) +
                                 AnalizadorDeFunciones.EvaluaFx(xd));
        }

        // --- Método Auxiliar para Simpson 1/3 Múltiple (Punto 211) ---
        private double Simpson1_3Multiple(double xi, double xd, int n)
        {
            double h = (xd - xi) / n;
            double sumPares = 0;
            double sumImpares = 0;

            // Se itera desde i=1 hasta n-1
            for (int i = 1; i < n; i++)
            {
                double x_i = xi + h * i;
                if (i % 2 == 0) // Índices pares (x2, x4, ...)
                {
                    sumPares += AnalizadorDeFunciones.EvaluaFx(x_i); // 2*sumPares
                }
                else // Índices impares (x1, x3, ...)
                {
                    sumImpares += AnalizadorDeFunciones.EvaluaFx(x_i); // 4*sumImpares
                }
            }

            // Fórmula: (h/3) * (f(a) + 4*sumImpares + 2*sumPares + f(b))
            return (h / 3) * (AnalizadorDeFunciones.EvaluaFx(xi) +
                             4 * sumImpares +
                             2 * sumPares +
                             AnalizadorDeFunciones.EvaluaFx(xd));
        }

        // --- Método Auxiliar para Simpson 3/8 (Punto 231) ---
        private double Simpson3_8(double xi, double xd)
        {
            // n=3, h = (xd - xi) / 3
            double h = (xd - xi) / 3;

            // Fórmula: (3*h/8) * (f(a) + 3*f(a+h) + 3*f(a+2*h) + f(b))
            return (3 * h / 8) * (AnalizadorDeFunciones.EvaluaFx(xi) +
                                3 * AnalizadorDeFunciones.EvaluaFx(xi + h) +
                                3 * AnalizadorDeFunciones.EvaluaFx(xi + 2 * h) +
                                AnalizadorDeFunciones.EvaluaFx(xd));
        }

        // --- Método Auxiliar para Simpson Combinado (Punto 243) ---
        private double SimpsonCombinado(double xi, double xd, int n)
        {
            if (n % 2 == 0)
            {
                // Si n es par, usamos directamente el Simpson 1/3 Múltiple
                return Simpson1_3Multiple(xi, xd, n);
            }

            // Caso n impar: Usar Simpson 3/8 para los últimos 3 subintervalos, 
            // y Simpson 1/3 Múltiple para el resto.

            // 1. Aplicar Simpson 3/8 a los últimos 3 subintervalos
            // n_combinado = n - 3
            double h_total = (xd - xi) / n;
            double nuevoXi = xi + h_total * (n - 3); // Extremo izquierdo del segmento 3/8

            double resultado3_8 = Simpson3_8(nuevoXi, xd); // Calcular Integral 3/8

            // 2. Aplicar Simpson 1/3 Múltiple al segmento restante
            int n_restante = n - 3; // La cantidad de intervalos restantes (n-3) es par
            double resultado1_3Multiple = Simpson1_3Multiple(xi, nuevoXi, n_restante);

            // 3. Sumar resultados
            return resultado3_8 + resultado1_3Multiple; // Sumar ambos resultados
        }


        // --- Método de Despacho Principal ---
        public Unidad4Resultado CalcularIntegral(Unidad4Param param)
        {
            var resultado = new Unidad4Resultado { Success = false };

            // 1. Preprocesar y Analizar Sintaxis
            string error = PreprocesarFuncion(param.Funcion);

            if (!string.IsNullOrEmpty(error))
            {
                resultado.Mensaje = error;
                return resultado;
            }

            double area = 0;

            // Validación general
            if (param.Xd <= param.Xi)
            {
                resultado.Mensaje = "El extremo derecho (Xd) debe ser mayor al izquierdo (Xi).";
                return resultado;
            }

            // 2. Despacho del Método
            switch (param.Metodo)
            {
                case "Trapecios Simple":
                    area = TrapeciosSimple(param.Xi, param.Xd);
                    break;

                case "Trapecios Múltiple":
                    if (param.SubintervalosN <= 0)
                    {
                        resultado.Mensaje = "Cantidad de subintervalos (n) debe ser mayor a 0.";
                        return resultado;
                    }
                    area = TrapeciosMultiple(param.Xi, param.Xd, param.SubintervalosN);
                    break;

                case "Simpson 1/3 Simple":
                    area = Simpson1_3Simple(param.Xi, param.Xd);
                    break;

                case "Simpson 1/3 Múltiple":
                    if (param.SubintervalosN <= 0)
                    {
                        resultado.Mensaje = "Cantidad de subintervalos (n) debe ser mayor a 0.";
                        return resultado;
                    }
                    if (param.SubintervalosN % 2 != 0)
                    {
                        resultado.Mensaje = "Simpson 1/3 Múltiple requiere una cantidad PAR de subintervalos (n). Para impares use 'Simpson Combinado'.";
                        return resultado;
                    }
                    area = Simpson1_3Multiple(param.Xi, param.Xd, param.SubintervalosN);
                    break;

                case "Simpson 3/8 Simple":
                    // Simpson 3/8 Simple siempre tiene 3 intervalos (implícito n=3)
                    area = Simpson3_8(param.Xi, param.Xd);
                    break;

                case "Simpson Combinado":
                    if (param.SubintervalosN <= 0)
                    {
                        resultado.Mensaje = "Cantidad de subintervalos (n) debe ser mayor a 0.";
                        return resultado;
                    }
                    if (param.SubintervalosN < 3 && param.SubintervalosN % 2 != 0)
                    {
                        resultado.Mensaje = "Para n impar, n debe ser al menos 3 para usar la combinación 1/3 y 3/8.";
                        return resultado;
                    }
                    // Despachar al método combinado que maneja el caso par/impar
                    area = SimpsonCombinado(param.Xi, param.Xd, param.SubintervalosN);
                    break;

                default:
                    resultado.Mensaje = "Método de integración no reconocido.";
                    return resultado;
            }

            // 3. Devolver Resultado
            if (double.IsNaN(area) || double.IsInfinity(area))
            {
                resultado.Mensaje = "El cálculo resultó en un valor no numérico (NaN/Infinito). Revise la función y los límites.";
                return resultado;
            }

            resultado.Area = area;
            resultado.Success = true;
            resultado.Mensaje = "Área calculada con éxito.";
            return resultado;
        }
    }
}

 