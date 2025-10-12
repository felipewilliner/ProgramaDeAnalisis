using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabajoAnalisis
{
    public class Unidad3
    {
        public Unidad3Resultado CalcularRegresionLineal(Unidad3Param param)
        {
            var resultado = new Unidad3Resultado();

            // 1. Obtener la cantidad de puntos de entrada (n)
            int n = param.Puntos.Count;

            if (n < 2)
            {
                // Necesitamos al menos dos puntos para una regresión lineal
                resultado.Funcion = "N/A";
                resultado.PorcentajeEfectividad = "0.00%";
                resultado.MensajeEfectividadAjuste = "Se requieren al menos 2 puntos para calcular la regresión lineal.";
                return resultado;
            }

            // Variables para las sumatorias (Puntos 2 al 5)
            double sumX = 0;   // Sumatoria de X
            double sumY = 0;   // Sumatoria de Y
            double sumXY = 0;  // Sumatoria de XY
            double sumX2 = 0;  // Sumatoria de X^2

            // Recorrer los PuntosCargados con un foreach y realizar las sumatorias
            foreach (var punto in param.Puntos)
            {
                double x = punto.X;
                double y = punto.Y;

                // 2. Calcular la sumatoria de X (SumX)
                sumX += x;

                // 3. Calcular la sumatoria de Y (SumY)
                sumY += y;

                // 4. Calcular la sumatoria de XY (SumXY)
                sumXY += x * y;

                // 5. Calcular la sumatoria de X² (SumX2)
                sumX2 += x * x;
            }

            // Calcular el denominador común D = (n * SumX2 - SumX * SumX)
            double denominador = (n * sumX2) - (sumX * sumX);

            if (denominador == 0)
            {
                // Ocurre si todos los puntos tienen la misma X (línea vertical)
                resultado.Funcion = "N/A";
                resultado.PorcentajeEfectividad = "0.00%";
                resultado.MensajeEfectividadAjuste = "No se puede calcular la regresión: las X son todas iguales.";
                return resultado;
            }

            // 6. Calcular a1 (pendiente)
            // a1 = (n * SumXY - SumX * SumY) / (n * SumX2 - SumX * SumY) -> ¡OJO! La fórmula del algoritmo tiene un error en el denominador, usa SumY en lugar de SumX2.
            // Utilizaremos la fórmula correcta de mínimos cuadrados: a1 = (n*SumXY - SumX*SumY) / (n*SumX2 - SumX*SumX)
            double a1 = (n * sumXY - sumX * sumY) / denominador;

            // 7. Calcular a0 (intercepto)
            // a0 = (SumY / n) - a1 * (SumX / n)
            double a0 = (sumY / n) - a1 * (sumX / n);

            // Variables para el cálculo de r (Punto 8 y 9)
            double st = 0; // Suma de los cuadrados totales (ST) - Usando la media de Y (Y_bar)
            double sr = 0; // Suma de los cuadrados de los residuos (SR)

            // Calcular la media de Y para ST
            double y_bar = sumY / n;

            // 8. Calcular sr y st
            // Esto requiere un nuevo recorrido sobre los puntos (o un ciclo)
            foreach (var punto in param.Puntos)
            {
                double x = punto.X;
                double y = punto.Y;

                // Valor predicho por la recta de regresión
                double y_predicho = a1 * x + a0;

                // a. St += (y - y_bar)^2
                st += Math.Pow((y - y_bar), 2);

                // b. Sr += (y - y_predicho)^2
                // Nota: El algoritmo original usa a1 y a0 como [1] y [0] respectivamente, que es confuso.
                // Sr es la suma de los errores al cuadrado (residuales).
                sr += Math.Pow((y - y_predicho), 2);
            }

            // 9. Calcular el coeficiente de correlación r
            double r_cuadrado;
            if (st == 0) // Todos los valores de Y son iguales. La correlación es 1 o 0 (mejor usar 1 si es un ajuste perfecto).
            {
                r_cuadrado = 1.0;
            }
            else
            {
                // r^2 = (St - Sr) / St  (Coeficiente de Determinación)
                r_cuadrado = (st - sr) / st;
            }

            // Coeficiente de correlación r = Math.Sqrt(r^2)
            // Usaremos el signo de la pendiente (a1) para el signo de r, aunque r^2 es más común para "efectividad".
            double r = Math.Sqrt(r_cuadrado);
            if (a1 < 0)
            {
                r = -r; // Se asigna el signo de la pendiente al coeficiente de correlación
            }

            // Calcular la efectividad como un porcentaje del coeficiente de determinación (r^2)
            // El algoritmo original calcula: Math.Sqrt((st-sr)/st) * 100.
            // Esto es un enfoque simplificado (r * 100), pero a menudo se usa r² * 100 como "porcentaje de variabilidad explicada". 
            // Siguiendo el algoritmo (Punto 9.a), calculamos r * 100.
            double efectividad = r * 100;

            // 10. Devolver los resultados
            resultado.Funcion = $"y = {a1:F4}x + {a0:F4}"; // Formato a 4 decimales
            resultado.PorcentajeEfectividad = $"{efectividad:F2}%"; // Formato a 2 decimales

            // a. Mensaje de efectividad de ajuste
            if (Math.Abs(r) >= param.Tolerancia)
            {
                resultado.MensajeEfectividadAjuste = $"El ajuste es EFECTIVO. |r| ({Math.Abs(r):F4}) es mayor o igual a la Tolerancia ({param.Tolerancia:F4}).";
            }
            else
            {
                resultado.MensajeEfectividadAjuste = $"El ajuste NO ES EFECTIVO. |r| ({Math.Abs(r):F4}) es menor que la Tolerancia ({param.Tolerancia:F4}).";
            }

            return resultado;
        }
    }
}
}
