document.addEventListener("DOMContentLoaded", function () {
    console.info("Formulario de Integraci�n Num�rica Listo");
});

document.getElementById('integracionForm').addEventListener('submit', function (event) {
    event.preventDefault();

    // Recolecci�n de datos
    const funcion = document.getElementById('funcion').value;
    const xi = parseFloat(document.getElementById('xi').value);
    const xd = parseFloat(document.getElementById('xd').value);
    const n = parseInt(document.getElementById('subintervalosN').value);
    const metodo = document.getElementById('metodo').value;

    const errorDiv = document.getElementById('mensaje-error');
    errorDiv.innerHTML = '';

    // Validaci�n m�nima
    if (isNaN(xi) || isNaN(xd) || xd <= xi) {
        errorDiv.innerHTML = `<p style="color:red;">Xi y Xd deben ser valores validos, y Xd debe ser mayor que Xi.</p>`;
        return;
    }

    const datosIntegracion = {
        Funcion: funcion,
        Xi: xi,
        Xd: xd,
        SubintervalosN: n,
        Metodo: metodo
    };

    // 1. Graficar funci�n en GeoGebra (similar a Unidad 1)
    if (window.ggbApplet) {
        try {
            // Limpiar y trazar la funci�n
            window.ggbApplet.deleteObject('a');
            window.ggbApplet.evalCommand(`f(x) = ${funcion}`);

            // Trazar el �rea para visualizaci�n
            window.ggbApplet.evalCommand(`Integral(f, ${xi}, ${xd})`);
           

        } catch (e) {
            console.warn("GeoGebra no pudo trazar la funci�n.", e);
        }
    }


    // 2. Llamada a la API
    fetch('http://localhost:5125/api/Unidad4/calcularintegral', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(datosIntegracion)
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    throw new Error(`HTTP ${response.status}: ${text}`);
                });
            }
            return response.json();
        })
        .then(data => {
           
            const errorDiv = document.getElementById('mensaje-error');

            if (!data.success) {
                errorDiv.innerHTML = `<p style="color:red;">Error de Calculo: ${data.mensaje}</p>`;
                
                errorDiv.style.background = '#f9e6e6';
                errorDiv.style.border = '1px solid red';

            } else {
                
                // CAMBIO CLAVE: Insertar mensaje y �rea en el mismo contenedor de mensaje (verde)
                errorDiv.style.background = '#c8e6c9';
                errorDiv.style.border = '1px solid #4caf50';

                errorDiv.innerHTML = `
            <p style="color:green; margin-bottom: 5px;">${data.mensaje}</p>
            <p style="font-size: 1.2rem; font-weight: bold; color: #2e7d32;">
                Resultado Area: ${data.area.toFixed(8)}
            </p>
        `;
            }
        })
        .catch(error => {
            console.error('Error al enviar los datos:', error);
            const errorDiv = document.getElementById('mensaje-error');
            errorDiv.style.background = '#f9e6e6';
            errorDiv.style.border = '1px solid red';
            errorDiv.innerHTML = `<p style="color:red;">Error de conexion: ${error.message}</p>`;
            // document.getElementById('resultado-area').innerHTML = `�rea: N/A`; // <--- ELIMINAR ESTA L�NEA
        });
});