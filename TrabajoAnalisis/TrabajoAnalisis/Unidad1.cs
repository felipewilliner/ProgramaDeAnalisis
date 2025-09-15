using Calculus;
using Entidades;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TrabajoAnalisis
{
    public class Unidad1
    {
        Calculo AnalizadorDeFunciones = new Calculo();

        public Resultado Biseccion(CerradosParam param)
        {
            Resultado resultado = AnalizarSintaxis(param);
            if (!string.IsNullOrEmpty(resultado.Mensaje))
            {
                return resultado;
            }
            else
            {


                double xrAnterior = 0;
                double xr = 0;
                double error = 0;

                for (int i = 0; i < param.Iteraciones; i++)
                {
                    xr = (param.Xi + param.Xd) / 2;
                    error = Math.Abs((xr - xrAnterior) / xr);
                    if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(xr)) < param.Tolerancia || error < param.Tolerancia)
                    {
                        resultado.Raiz = xr;
                        resultado.Success = true;
                        resultado.Mensaje = "Raiz encontrada";
                        return resultado;

                    }
                    else if (AnalizadorDeFunciones.EvaluaFx(param.Xi) * AnalizadorDeFunciones.EvaluaFx(xr) > 0)
                    {
                        param.Xi = xr;
                    }
                    else
                    {
                        param.Xd = xr;
                    }
                    xrAnterior = xr;

                    resultado.Error = error;
                    resultado.Iteraciones = i + 1;
                }
            }

            return resultado;

        }

        public Resultado ReglaFalsa(CerradosParam param)
        {

            Resultado resultado = AnalizarSintaxis(param);
            if (!string.IsNullOrEmpty(resultado.Mensaje))
            {
                return resultado;
            }
            else
            {
                double xrAnterior = 0;
                double xr = 0;
                double error = 0;

                for (int i = 0; i < param.Iteraciones; i++)
                {
                    xr = ((AnalizadorDeFunciones.EvaluaFx(param.Xd) * param.Xi) - (AnalizadorDeFunciones.EvaluaFx(param.Xi) * param.Xd)) /
                        (AnalizadorDeFunciones.EvaluaFx(param.Xd) - AnalizadorDeFunciones.EvaluaFx(param.Xi));

                    error = Math.Abs((xr - xrAnterior) / xr);
                    if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(xr)) < param.Tolerancia || error < param.Tolerancia)
                    {
                        resultado.Raiz = xr;
                        resultado.Success = true;
                        resultado.Mensaje = "Raiz encontrada";
                        return resultado;

                    }
                    else if (AnalizadorDeFunciones.EvaluaFx(param.Xi) * AnalizadorDeFunciones.EvaluaFx(xr) > 0)
                    {
                        param.Xi = xr;
                    }
                    else
                    {
                        param.Xd = xr;
                    }
                    xrAnterior = xr;
                    resultado.Iteraciones = i + 1;
                    resultado.Error = error;
                }


            }

            return resultado;
        }

        public Resultado NewtonRaphson(CerradosParam param)
        {
            param.Funcion = Regex.Replace(param.Funcion, @"e\^(.+?)(?=[\s\+\-\*\/\)]|$)", "exp($1)");
            AnalizadorDeFunciones.Sintaxis(param.Funcion, 'x');
            Resultado resultado = new Resultado();
            if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(param.Xi)) <= param.Tolerancia)
            {
                resultado.Success = true;
                resultado.Raiz = param.Xi;
                resultado.Mensaje = "Raiz encontrada";
                resultado.Iteraciones = 0;
                resultado.Error = 0;
                return resultado;
            }
            else
            {
                double xrAnterior2 = 0;
                double xrAnterior = 0;
                double xr = 0;
                double error = 0;
                for (int i = 0; i < param.Iteraciones; i++)
                {
                    double derivada = AnalizadorDeFunciones.Dx(param.Xi);
                    if (double.IsNaN(derivada) || Math.Abs(derivada) <= param.Tolerancia)
                    {
                        resultado.Success = false;
                        resultado.Raiz = xr;
                        resultado.Mensaje = "El metodo diverge";
                        resultado.Iteraciones = i + 1;
                        resultado.Error = error;
                        break;
                    }
                    else
                    {
                        xr = param.Xi - (AnalizadorDeFunciones.EvaluaFx(param.Xi) / derivada);
                        if ((xrAnterior2 == xr && i >= 2))
                        {
                            resultado.Success = false;
                            resultado.Mensaje = "El metodo diverge por bucle";
                            resultado.Iteraciones = i + 1;
                            resultado.Error = error;
                            break;
                        }
                    }

                    if (double.IsNaN(xr))
                    {
                        resultado.Success = false;
                        resultado.Mensaje = "El metodo diverge";
                        resultado.Raiz = xr;
                        resultado.Iteraciones = i + 1;
                        resultado.Error = error;
                        break;
                    }
                    error = Math.Abs((xr - xrAnterior) / xr);
                    if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(xr)) <= param.Tolerancia || error <= param.Tolerancia)
                    {
                        resultado.Raiz = xr;
                        resultado.Success = true;
                        resultado.Mensaje = "Raiz encontrada";
                        resultado.Iteraciones = i + 1;
                        resultado.Error = error;
                        break;
                    }
                    xrAnterior2 = xrAnterior;
                    param.Xi = xr;
                    xrAnterior = xr;
                }
                if (string.IsNullOrEmpty(resultado.Mensaje))
                {
                    resultado.Success = false;
                    resultado.Mensaje = "Máximo de iteraciones alcanzado";
                    resultado.Iteraciones = param.Iteraciones;
                    resultado.Error = error;
                    resultado.Raiz = xr;
                }

                return resultado;
            }

        }

        public Resultado Secante(CerradosParam param)
        {
            param.Funcion = Regex.Replace(param.Funcion, @"e\^(.+?)(?=[\s\+\-\*\/\)]|$)", "exp($1)");
            AnalizadorDeFunciones.Sintaxis(param.Funcion, 'x');
            Resultado resultado = new Resultado();
            if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(param.Xi)) <= param.Tolerancia)
            {
                resultado.Success = true;
                resultado.Raiz = param.Xi;
                resultado.Mensaje = "Raiz encontrada";
                resultado.Iteraciones = 0;
                resultado.Error = 0;
                return resultado;
            }
            else if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(param.Xd)) <= param.Tolerancia)
            {
                resultado.Success = true;
                resultado.Raiz = param.Xd;
                resultado.Mensaje = "Raiz encontrada";
                resultado.Iteraciones = 0;
                resultado.Error = 0;
                return resultado;
            }
            else
            {
                double xrAnterior2 = 0;
                double xrAnterior = 0;
                double xr = 0;
                double error = 0;
                for (int i = 0; i < param.Iteraciones; i++)
                {

                    xr = ((AnalizadorDeFunciones.EvaluaFx(param.Xd) * param.Xi) - (AnalizadorDeFunciones.EvaluaFx(param.Xi) * param.Xd)) /
                            (AnalizadorDeFunciones.EvaluaFx(param.Xd) - AnalizadorDeFunciones.EvaluaFx(param.Xi));


                    if (double.IsNaN(xr) || AnalizadorDeFunciones.EvaluaFx(param.Xi) == AnalizadorDeFunciones.EvaluaFx(param.Xd) || double.IsNaN(AnalizadorDeFunciones.EvaluaFx(xr)) || double.IsInfinity(xr) || Math.Abs(xr) > 1e10)
                    {
                        resultado.Success = false;
                        resultado.Mensaje = "El metodo diverge";
                        resultado.Iteraciones = i + 1;
                        resultado.Error = error;
                        resultado.Raiz = xr;
                        return resultado;
                    }
                    if (Math.Abs(xrAnterior2 - xr) < param.Tolerancia && i >= 2)
                    {
                        resultado.Success = false;
                        resultado.Mensaje = "El metodo diverge por bucle";
                        resultado.Iteraciones = i + 1;
                        resultado.Error = error;
                        break;
                    }
                    if (xr != 0) { error = Math.Abs((xr - xrAnterior) / xr); }
                    double aver = Math.Abs(AnalizadorDeFunciones.EvaluaFx(xr));
                    if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(xr)) <= param.Tolerancia || error <= param.Tolerancia)
                    {
                        Debug.WriteLine($"xr = {xr}, f(xr) = {AnalizadorDeFunciones.EvaluaFx(xr)}, error = {error}, tol = {param.Tolerancia}");
                        resultado.Raiz = xr;
                        resultado.Success = true;
                        resultado.Mensaje = "Raiz encontrada";
                        resultado.Iteraciones = i + 1;
                        resultado.Error = error;
                        return resultado;
                    }
                    xrAnterior2 = xrAnterior;
                    param.Xi = param.Xd;
                    param.Xd = xr;
                    xrAnterior = xr;
                }

                if (string.IsNullOrEmpty(resultado.Mensaje))
                {
                    resultado.Success = true;
                    resultado.Mensaje = "Máximo de iteraciones alcanzado";
                    resultado.Iteraciones = param.Iteraciones;
                    resultado.Error = error;
                    resultado.Raiz = xr;

                }
                return resultado;

            }

        }



        public Resultado AnalizarSintaxis(CerradosParam param)
        {

            // Primero: Reemplazar e^x por exp(x)
            param.Funcion = Regex.Replace(param.Funcion, @"e\^(.+?)(?=[\s\+\-\*\/\)]|$)", "exp($1)");

            // Segundo: Reemplazar x^e (o cualquier expresión^e) por expresión^valor_de_e
            param.Funcion = Regex.Replace(
                param.Funcion,
                @"\^e(?![a-zA-Z0-9_])", // 'e' después de ^ y no seguida de letras/números
                "^" + Math.E.ToString("F10")
            );

            // Tercero: Reemplazar 'e' standalone (que no esté después de ^ ni antes de ^)
            param.Funcion = Regex.Replace(
                param.Funcion,
                @"(?<![a-zA-Z0-9_\^])e(?![a-zA-Z0-9_\^])",
                Math.E.ToString("F10")
            );

            Resultado resultado = new Resultado();
            AnalizadorDeFunciones.Sintaxis(param.Funcion, 'x');
            if (AnalizadorDeFunciones.EvaluaFx(param.Xi) * AnalizadorDeFunciones.EvaluaFx(param.Xd) > param.Tolerancia)
            {
                resultado.Success = false;
                resultado.Mensaje = "Ingrese otros valores de xi xd";
            }
            else if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(param.Xi)) <= param.Tolerancia)
            {
                resultado.Raiz = param.Xi;
                resultado.Success = true;
                resultado.Mensaje = "Raíz encontrada";
            }
            else if (Math.Abs(AnalizadorDeFunciones.EvaluaFx(param.Xd)) <= param.Tolerancia)
            {
                resultado.Raiz = param.Xd;
                resultado.Success = true;
                resultado.Mensaje = "Raíz encontrada";
            }

            return resultado;

        }


    }



}
