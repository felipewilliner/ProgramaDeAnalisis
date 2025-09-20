using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabajoAnalisis
{
    public class Unidad2
    {
        public ResultadoEcuaciones ResolverGaussJordan(EcuacionesParam ecuacion)
        {
            ResultadoEcuaciones resultado = new ResultadoEcuaciones(ecuacion.Dimension);
            int n = ecuacion.Dimension;
            double[][] matrizQueNoUso = ecuacion.Matriz;

            double[,] matriz = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    matriz[i, j] = matrizQueNoUso[i][j];
                }
            }

            // Algoritmo Gauss-Jordan
            for (int rowDiag = 0; rowDiag < n; rowDiag++)
            {
                double coefDiagonal = matriz[rowDiag, rowDiag];

                if (coefDiagonal == 0)
                {
                    resultado.Success = false;
                    resultado.Mensaje = "No se puede dividir por 0";
                    return resultado;
                }

                // Normalizar la fila actual
                for (int col = 0; col <= n; col++)
                {
                    matriz[rowDiag, col] /= coefDiagonal;
                }

                // Hacer ceros en las demás filas
                for (int row = 0; row < n; row++)
                {
                    if (row != rowDiag)
                    {
                        double coefCero = matriz[row, rowDiag];
                        for (int col = 0; col <= n; col++)
                        {
                            matriz[row, col] -= coefCero * matriz[rowDiag, col];
                        }
                    }
                }
            }

            // Extraer el resultado (última columna)

            for (int i = 0; i < n; i++)
            {

                resultado.Resultados[i] = matriz[i, n];
            }
            resultado.Success = true;
            return resultado;
        }

        public ResultadoGaussSeidel ResolverGaussSeidel(GaussSeidelParam parametros)
        {
            int n = parametros.Dimension;
            double[][] matrizEntrante = parametros.Matriz;
            double tolerancia = parametros.Tolerancia;
            int maxIteraciones = parametros.MaxIteraciones;

            ResultadoGaussSeidel resultado = new ResultadoGaussSeidel(n);

            // Convertir double[][] a double[,] para trabajar más fácil
            double[,] matriz = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    matriz[i, j] = matrizEntrante[i][j];
                }
            }

            // Verificar si la matriz es válida usando la matriz convertida
            if (matriz.GetLength(0) != n || matriz.GetLength(1) != n + 1)
            {
                resultado.Success = false;
                resultado.Mensaje = "La matriz no tiene las dimensiones correctas";
                return resultado;
            }

            // Verificar si la matriz es diagonalmente dominante usando la matriz convertida
            bool esDiagonalDominante = true;
            for (int i = 0; i < n; i++)
            {
                double diagonal = Math.Abs(matriz[i, i]);
                double suma = 0;

                for (int j = 0; j < n; j++)
                {
                    if (i != j) suma += Math.Abs(matriz[i, j]);
                }

                if (diagonal <= suma)
                {
                    esDiagonalDominante = false;
                    break;
                }
            }

            if (!esDiagonalDominante)
            {
                resultado.Success = false;
                resultado.Mensaje = "La matriz no es diagonalmente dominante. Se requiere pivotear manualmente.";
                return resultado;
            }

            double[] vectorResultado = new double[n];
            double[] vectorAnterior = new double[n];
            bool convergio = false;
            int iteracion = 0;
            double error = 0;

            // Inicializar el vector resultado con ceros
            Array.Fill(vectorResultado, 0);

            while (iteracion < maxIteraciones && !convergio)
            {
                iteracion++;

                // Copiar el vector actual al anterior
                Array.Copy(vectorResultado, vectorAnterior, n);

                // Iterar sobre cada ecuación usando la matriz convertida
                for (int i = 0; i < n; i++)
                {
                    double suma = 0;

                    // Sumar los términos de las otras incógnitas (usando los valores más recientes)
                    for (int j = 0; j < n; j++)
                    {
                        if (i != j)
                        {
                            suma += matriz[i, j] * vectorResultado[j];
                        }
                    }

                    // Calcular nuevo valor para la incógnita actual
                    vectorResultado[i] = (matriz[i, n] - suma) / matriz[i, i];
                }

                // Verificar convergencia después de la primera iteración - CORREGIDO
                if (iteracion > 1)
                {

                    convergio = true;  // Asumimos que converge hasta que se demuestre lo contrario

                    for (int i = 0; i < n; i++)
                    {
                        if (Math.Abs(vectorResultado[i]) > 1e-10) // Evitar división por cero
                        {

                            double errorAbsoluto = Math.Abs(vectorResultado[i] - vectorAnterior[i]);
                            error = errorAbsoluto / Math.Abs(vectorResultado[i]);



                            // Si algún error supera la tolerancia, NO converge
                            if (error > tolerancia)
                            {
                                convergio = false;
                            }
                        }
                    }
                }
            }

            // Preparar resultado final
            resultado.Iteraciones = iteracion;
            resultado.ErrorFinal = error;

            if (convergio)
            {
                resultado.Success = true;
                resultado.Mensaje = $"Solución convergente encontrada en {iteracion} iteraciones";
                Array.Copy(vectorResultado, resultado.Resultados, n);
            }
            else
            {
                resultado.Success = false;
                resultado.Mensaje = $"El método no convergió después de {iteracion} iteraciones. Error máximo: {error:F6}";
                // Aún así retornamos los valores actuales
                Array.Copy(vectorResultado, resultado.Resultados, n);
            }

            return resultado;
        }


    }
}

