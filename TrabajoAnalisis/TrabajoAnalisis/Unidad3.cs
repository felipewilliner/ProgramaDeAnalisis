using Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            double efectividad = Math.Abs(r) * 100;

            // 10. Devolver los resultados
            resultado.Funcion = $"y = {a1:F4}*x + {a0:F4}"; // Formato a 4 decimales
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


        private readonly Unidad2 _unidadesSolver = new Unidad2();

        // --- Método GenerarMatrizPolinomial (Punto 1 del Algoritmo) ---
        private double[][] GenerarMatrizPolinomial(List<xy> puntosCargados, int grado)
        {
            int n = puntosCargados.Count;
            int dimension = grado + 1; // n+1 incógnitas (a0, a1, ..., an)

            // La matriz es (grado + 1) x (grado + 2)
            double[,] matriz = new double[dimension, dimension + 1];

            // Realizar las sumatorias
            for (int i = 0; i <= 2 * grado; i++) // Sumatorias de X^0 hasta X^(2*grado)
            {
                double sumXi = 0; // Sumatoria de X^i
                double sumYi = 0; // Sumatoria de Y*X^i (solo para i <= grado)

                foreach (var punto in puntosCargados)
                {
                    double x = punto.X;
                    double y = punto.Y;

                    sumXi += Math.Pow(x, i); // Sum(X^i)

                    if (i <= grado)
                    {
                        sumYi += y * Math.Pow(x, i); // Sum(Y*X^i)
                    }
                }

                // Llenar la matriz de ecuaciones normales
                for (int fila = 0; fila < dimension; fila++)
                {
                    // Llenar coeficientes (matriz cuadrada)
                    for (int col = 0; col < dimension; col++)
                    {
                        // La matriz en [fila, col] es la sumatoria de X^(fila + col)
                        if (fila + col == i)
                        {
                            matriz[fila, col] = sumXi;
                        }
                    }

                    // Llenar la columna de términos independientes (matriz[fila, dimension] = Sum(Y*X^fila))
                    if (fila == i && i <= grado)
                    {
                        matriz[fila, dimension] = sumYi;
                    }
                }
            }

            // Convertir double[,] a double[][] para usar con Unidad2/EcuacionesParam
            double[][] matrizResultante = new double[dimension][];
            for (int i = 0; i < dimension; i++)
            {
                matrizResultante[i] = new double[dimension + 1];
                for (int j = 0; j <= dimension; j++)
                {
                    matrizResultante[i][j] = matriz[i, j];
                }
            }

            return matrizResultante;
        }

        // --- Nuevo Método Principal para Regresión Polinomial ---
        public Unidad3Resultado CalcularRegresionPolinomial(Unidad3Param param)
        {
            var resultado = new Unidad3Resultado();
            int grado = param.Grado;
            int n = param.Puntos.Count;

            if (n < grado + 1)
            {
                resultado.Funcion = "N/A";
                resultado.PorcentajeEfectividad = "0.00%";
                resultado.MensajeEfectividadAjuste = $"Se requieren al menos {grado + 1} puntos para un polinomio de grado {grado}.";
                return resultado;
            }

            // 1. Generar la matriz de ecuaciones normales
            double[][] matrizPolinomial = GenerarMatrizPolinomial(param.Puntos, grado);

            // 2. Resolver la matriz con Gauss-Jordan
            EcuacionesParam ecuacionParam = new EcuacionesParam
            {
                Dimension = grado + 1,
                Matriz = matrizPolinomial
            };

            // Asume que Unidad2.ResolverGaussJordan toma EcuacionesParam y devuelve ResultadoEcuaciones
            var resultadoGauss = _unidadesSolver.ResolverGaussJordan(ecuacionParam);

            if (!resultadoGauss.Success)
            {
                resultado.Funcion = "N/A";
                resultado.PorcentajeEfectividad = "0.00%";
                resultado.MensajeEfectividadAjuste = "Fallo al resolver el sistema de ecuaciones con Gauss-Jordan: " + resultadoGauss.Mensaje;
                return resultado;
            }

            double[] coeficientes = resultadoGauss.Resultados; // Vector [a0, a1, a2, ...]

            // 3. Formar la función polinomial (Punto 2.a del Algoritmo)
            StringBuilder sb = new StringBuilder("y = ");
            for (int i = 0; i < coeficientes.Length; i++)
            {
                double ai = coeficientes[i];
                if (Math.Abs(ai) < 1e-6) continue; // Ignorar coeficientes muy cercanos a cero

                string ai_str = Math.Abs(ai).ToString("F4", CultureInfo.InvariantCulture);
                string signo = ai >= 0 ? "+" : "-";

                if (i == 0)
                {
                    // Término independiente a0
                    sb.Append($"{signo} {ai_str}");
                }
                else
                {
                    // Términos a1x, a2x^2, ...
                    sb.Append($" {signo} {ai_str}x");
                    if (i > 1)
                    {
                        sb.Append($"^{i}");
                    }
                }
            }
            // Limpiar el '+' inicial si a0 > 0
            resultado.Funcion = sb.ToString().Replace("y = +", "y = ").Trim();
            if (resultado.Funcion.StartsWith("y = -")) // Si es negativo, el signo ya está bien
            {
                resultado.Funcion = "y = " + resultado.Funcion.Substring(4).Trim();
            }


            // 4. Calcular el coeficiente de correlación r (Punto 2.b del Algoritmo)
            double sumY = 0;
            double st = 0; // Suma de los cuadrados totales
            double sr = 0; // Suma de los cuadrados de los residuos

            foreach (var punto in param.Puntos)
            {
                sumY += punto.Y;
            }
            double y_bar = sumY / n;

            foreach (var punto in param.Puntos)
            {
                double x = punto.X;
                double y = punto.Y;

                // Calcular y_predicho (suma = a0 + a1x + a2x^2 + ...)
                double y_predicho = 0;
                for (int i = 0; i < coeficientes.Length; i++)
                {
                    y_predicho += coeficientes[i] * Math.Pow(x, i);
                }

                // st += (y - y_bar)^2
                st += Math.Pow((y - y_bar), 2);

                // sr += (y - y_predicho)^2
                sr += Math.Pow((y - y_predicho), 2);
            }

            // Cálculo de r
            double r_cuadrado = 0;
            if (st > 1e-10) // Evitar división por cero si todas las Y son iguales
            {
                // Coeficiente de Determinación (r^2)
                r_cuadrado = (st - sr) / st;
            }
            else
            {
                r_cuadrado = 1.0; // Ajuste perfecto si todas las Y son iguales.
            }

            // Coeficiente de correlación r = Math.Sqrt(r^2)
            double r = Math.Sqrt(Math.Abs(r_cuadrado)); // Usamos Math.Abs() para prevenir r^2 negativo por errores numéricos

            // Efectividad y mensaje
            double efectividad = r * 100;

            resultado.PorcentajeEfectividad = $"{efectividad:F2}%";

            if (r >= param.Tolerancia)
            {
                resultado.MensajeEfectividadAjuste = $"El ajuste es EFECTIVO. |r| ({r:F4}) es mayor o igual a la Tolerancia ({param.Tolerancia:F4}).";
            }
            else
            {
                resultado.MensajeEfectividadAjuste = $"El ajuste NO ES EFECTIVO. |r| ({r:F4}) es menor que la Tolerancia ({param.Tolerancia:F4}).";
            }

            return resultado;
        }



        // --- Método Helper para extraer coeficientes de la recta (Punto 5 del Algoritmo) ---
        private Tuple<double, double> ObtenerCoeficientesFuncion(string funcion)
        {
            if (string.IsNullOrWhiteSpace(funcion))
                throw new ArgumentException("La función no puede estar vacía.");

            // Expresión regular adaptada para manejar espacios y signos opcionales
            var regex = new Regex(@"y\s*=\s*([+-]?\d+(?:[.,]\d+)?)(\*?x)?\s*([+-]\s*\d+(?:[.,]\d+)?)?");
            var match = regex.Match(funcion.Replace(" ", "")); // Limpiar espacios

            if (!match.Success)
            {
                // Intento alternativo (si solo es 'y = a1x' o 'y = a0')
                regex = new Regex(@"y\s*=\s*([+-]?\d+(?:[.,]\d+)?)(\*?x)?");
                match = regex.Match(funcion.Replace(" ", ""));

                if (!match.Success)
                {
                    // Intento solo a0
                    regex = new Regex(@"y\s*=\s*([+-]?\d+(?:[.,]\d+)?)");
                    match = regex.Match(funcion.Replace(" ", ""));
                    if (!match.Success)
                        throw new FormatException("Formato inválido. Ejemplo esperado: y = 2.5x - 1.3");
                }
            }

            string a1Str = "0";
            string a0Str = "0";

            // Lógica de parsing adaptada
            if (match.Groups[2].Value.Contains("x")) // Si 'x' está presente, el grupo 1 es a1
            {
                a1Str = match.Groups[1].Value.Replace(',', '.');
                if (match.Groups[3].Success) // Si hay un grupo 3, es a0
                {
                    a0Str = match.Groups[3].Value.Replace(',', '.');
                }
            }
            else // Si 'x' no está presente, el grupo 1 es a0
            {
                a0Str = match.Groups[1].Value.Replace(',', '.');
            }

            // Manejo de 'y = x' (a1 = 1) o 'y = -x' (a1 = -1)
            if (a1Str == "+" || a1Str == "") a1Str = "1";
            if (a1Str == "-") a1Str = "-1";

            double a1 = double.Parse(a1Str, CultureInfo.InvariantCulture);
            double a0 = double.Parse(a0Str, CultureInfo.InvariantCulture);

            return Tuple.Create(a1, a0);
        }

        // --- Método Principal para Recalcular R (Punto 5 del Algoritmo) ---
        public Unidad3Resultado CalcularCorrelacionRectaModificada(Unidad3ModificadaParam param)
        {
            var resultado = new Unidad3Resultado();
            List<xy> PuntosCargados = param.Puntos;
            int n = PuntosCargados.Count;

            if (n == 0)
            {
                resultado.MensajeEfectividadAjuste = "No hay puntos cargados.";
                return resultado;
            }

            try
            {
                double sumY = 0;
                // Adaptado a List<xy>
                foreach (var punto in PuntosCargados)
                {
                    sumY += punto.Y;
                }

                // Obtener coeficientes a1 y a0 de la función modificada
                var coeficientes = ObtenerCoeficientesFuncion(param.FuncionModificada);
                double a1 = coeficientes.Item1;
                double a0 = coeficientes.Item2;

                double st = 0;
                double sr = 0;
                double y_bar = sumY / n;

                // Adaptado a List<xy>
                foreach (var punto in PuntosCargados)
                {
                    double x = punto.X;
                    double y = punto.Y;

                    st += Math.Pow(y - y_bar, 2); // Fórmula St correcta

                    double y_predicho = a1 * x + a0;
                    sr += Math.Pow(y - y_predicho, 2);
                }

                // Cálculo de r
                double r_cuadrado;
                if (st == 0)
                {
                    r_cuadrado = 1.0;
                }
                else
                {
                    r_cuadrado = (st - sr) / st;
                }

                double r = Math.Sqrt(Math.Abs(r_cuadrado));
                if (a1 < 0 && r > 0) r = -r; // Asignar signo de la pendiente

                double efectividad = Math.Abs(r) * 100;

                resultado.Funcion = param.FuncionModificada;
                resultado.PorcentajeEfectividad = $"{efectividad:F2}%";

                if (Math.Abs(r) >= param.Tolerancia)
                {
                    resultado.MensajeEfectividadAjuste = $"El ajuste MODIFICADO es EFECTIVO. |r| ({Math.Abs(r):F4}) es mayor o igual a la Tolerancia ({param.Tolerancia:F4}).";
                }
                else
                {
                    resultado.MensajeEfectividadAjuste = $"El ajuste MODIFICADO NO ES EFECTIVO. |r| ({Math.Abs(r):F4}) es menor a la Tolerancia ({param.Tolerancia:F4}).";
                }
            }
            catch (Exception ex)
            {
                resultado.MensajeEfectividadAjuste = "Error al recalcular: " + ex.Message;
            }

            return resultado;
        }
    }
}






