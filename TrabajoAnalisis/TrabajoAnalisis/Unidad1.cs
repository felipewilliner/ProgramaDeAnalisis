using Calculus;
using Entidades;

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

                }


            }

            return resultado;
        }

        public Resultado AnalizarSintaxis(CerradosParam param)
        {
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
