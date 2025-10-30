document.addEventListener("DOMContentLoaded", function () {
    console.info("Formulario de Regresión Lineal Listo");

    // Inicializar con dos puntos de ejemplo
    agregarFilaPunto(1, 2);
    agregarFilaPunto(3, 4);

    document.getElementById('regresionLinealForm').addEventListener('submit', function (event) {
        event.preventDefault();
        calcularRegresion();
    });
});

// -------------------------------------------------------------------
//  1. VARIABLES GLOBALES
// (Para guardar los datos del primer cálculo y usarlos en el recálculo)
// -------------------------------------------------------------------
let puntosGlobales = [];
let toleranciaGlobal = 0.8;


// Contador global para IDs de puntos
let puntoIdCounter = 0;

function agregarFilaPunto(defaultX = 0, defaultY = 0) {
    const table = document.getElementById('puntos-tabla');
    const id = puntoIdCounter++;

    const row = document.createElement('div');
    row.id = `punto-row-${id}`;
    row.classList.add('punto-fila');
    row.style.marginBottom = '5px';
    row.style.display = 'flex';
    row.style.alignItems = 'center';

    row.innerHTML = `
        <label style="width: 15%;">Punto ${id + 1}:</label>
        <label style="width: 35%;">X: <input type="number" step="any" class="punto-x" value="${defaultX}" data-id="${id}"></label>
        <label style="width: 35%;">Y: <input type="number" step="any" class="punto-y" value="${defaultY}" data-id="${id}"></label>
        <button type="button" onclick="eliminarFilaPunto(${id})" style="padding: 5px; font-size: 0.8rem; margin-left: auto;">-</button>
    `;

    table.appendChild(row);
}

function eliminarFilaPunto(id) {
    const row = document.getElementById(`punto-row-${id}`);
    if (row) {
        // Nota: Decrementar el counter global aquí puede ser problemático si se borran 
        // puntos intermedios, pero seguimos la lógica del archivo original.
        id = puntoIdCounter--;
        row.remove();
    }
}

function obtenerPuntos() {
    const puntos = [];
    const inputsX = document.querySelectorAll('.punto-x');
    const inputsY = document.querySelectorAll('.punto-y');

    for (let i = 0; i < inputsX.length; i++) {
        const x = parseFloat(inputsX[i].value);
        const y = parseFloat(inputsY[i].value);

        if (!isNaN(x) && !isNaN(y)) {
            puntos.push({ X: x, Y: y });
        }
    }
    return puntos;
}

// -------------------------------------------------------------------
//  2. CÁLCULO ORIGINAL (MODIFICADO)
// (Guarda los puntos y la tolerancia en las variables globales)
// -------------------------------------------------------------------
function calcularRegresion() {
    // Guardar los valores en las variables globales
    toleranciaGlobal = parseFloat(document.getElementById('tolerancia').value);
    const grado = parseInt(document.getElementById('grado').value);
    puntosGlobales = obtenerPuntos(); // Guardar puntos

    const resultadoDiv = document.getElementById('resultado');

    // Validación mínima de datos
    if (puntosGlobales.length < 2) {
        resultadoDiv.innerHTML = `<p style="color:red;">Error: Se necesitan al menos 2 puntos para la regresión.</p>`;
        return;
    }

    const datosRegresion = {
        Tolerancia: toleranciaGlobal,
        Grado: grado,
        Puntos: puntosGlobales
    };

    console.log('Enviando datos:', datosRegresion);

    fetch('http://localhost:5125/api/Unidad3/regresionlineal', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(datosRegresion)
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
            console.log('Respuesta de la API:', data);
            // Usar los puntos globales para mostrar el resultado
            mostrarResultado(data, puntosGlobales);
        })
        .catch(error => {
            console.error('Error al enviar los datos:', error);
            resultadoDiv.innerHTML = `<p style="color:red;">Error de conexión con el servidor: ${error.message}</p>`;
        });
}

// -------------------------------------------------------------------
// 3. MOSTRAR RESULTADO (MODIFICADO)
// (Muestra la sección "Modificar Recta" y asigna el evento al nuevo botón)
// -------------------------------------------------------------------
function mostrarResultado(data, puntos) {
    const resultadoDiv = document.getElementById('resultado');

    // Referencias a los nuevos elementos del HTML
    const modificarContainer = document.getElementById('modificar-recta-container');
    const inputFuncionModificada = document.getElementById('funcion-modificada');
    const btnRecalcular = document.getElementById('btn-recalcular-r');
    const resultadoModificadoDiv = document.getElementById('resultado-modificado');

    // Limpiar resultado modificado anterior
    resultadoModificadoDiv.innerHTML = '';

    if (!data.funcion || data.funcion === "N/A") {
        resultadoDiv.innerHTML = `
            <div style="background:#f9e6e6; border:1px solid #e74c3c; color:#c0392b;">
                <strong>Error:</strong> ${data.mensajeEfectividadAjuste}
            </div>`;

        modificarContainer.style.display = 'none'; // Ocultar si hay error
        graficarRegresion(puntos, null); // Limpiar gráfico
        return;
    }

    // 1. Mostrar resultados Originales
    const efectividadColor = data.mensajeEfectividadAjuste.includes('EFECTIVO') ? '#2e7d32' : '#c0392b';
    const efectividadBg = data.mensajeEfectividadAjuste.includes('EFECTIVO') ? '#c8e6c9' : '#f9e6e6';

    resultadoDiv.innerHTML = `
        <div style="margin-top:20px;">
            <h3>Resultados de Regresion</h3>
            <p><strong>Funcion de la Recta:</strong> <code>${data.funcion}</code></p>
            <p><strong>Efectividad (r*100):</strong> ${data.porcentajeEfectividad}</p>
            <div style="padding: 10px; border-radius: 5px; background: ${efectividadBg}; color: ${efectividadColor}; border: 1px solid ${efectividadColor};">
                <strong>Mensaje de Ajuste:</strong> ${data.mensajeEfectividadAjuste}
            </div>
        </div>
    `;

    // 2. Graficar en GeoGebra (el gráfico original)
    graficarRegresion(puntos, data.funcion);

    // 3. Mostrar y configurar la sección de modificación
    modificarContainer.style.display = 'block'; // Mostrar la sección oculta
    inputFuncionModificada.value = data.funcion; // Llenar con la función redondeada

    // Asignar el evento al botón de recalcular
    btnRecalcular.onclick = function () {
        recalcularCorrelacion(inputFuncionModificada.value);
    };
}


// -------------------------------------------------------------------
//  4. NUEVA FUNCIÓN
// (Se llama al presionar el botón naranja "Recalcular")
// -------------------------------------------------------------------
function recalcularCorrelacion(funcionModificada) {

    const resultadoModificadoDiv = document.getElementById('resultado-modificado');
    resultadoModificadoDiv.innerHTML = '<p>Recalculando...</p>';

    // Usar los datos globales guardados
    const datosRecalculo = {
        FuncionModificada: funcionModificada,
        Puntos: puntosGlobales,
        Tolerancia: toleranciaGlobal
    };

    fetch('http://localhost:5125/api/Unidad3/recalcular-correlacion', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(datosRecalculo)
    })
        .then(response => response.json())
        .then(data => {
            // Mostrar el resultado del recálculo
            const efectividadColor = data.mensajeEfectividadAjuste.includes('EFECTIVO') ? '#2e7d32' : '#c0392b';
            const efectividadBg = data.mensajeEfectividadAjuste.includes('EFECTIVO') ? '#c8e6c9' : '#f9e6e6';

            resultadoModificadoDiv.innerHTML = `
            <h4 style="margin-bottom: 5px;">Resultado de la Recta Modificada</h4>
            <p><strong>Funcion:</strong> <code>${data.funcion}</code></p>
            <p><strong>Nueva Efectividad (r*100):</strong> ${data.porcentajeEfectividad}</p>
            <div style="padding: 10px; border-radius: 5px; background: ${efectividadBg}; color: ${efectividadColor}; border: 1px solid ${efectividadColor};">
                <strong>Mensaje de Ajuste:</strong> ${data.mensajeEfectividadAjuste}
            </div>
        `;

            // Graficar la NUEVA recta modificada en GeoGebra
            graficarRegresion(puntosGlobales, data.funcion);
        })
        .catch(error => {
            resultadoModificadoDiv.innerHTML = `<p style="color:red;">Error al recalcular: ${error.message}</p>`;
        });
}


// -------------------------------------------------------------------
// 5. GRAFICADOR (MODIFICADO)
// (Limpia los objetos antiguos antes de dibujar los nuevos)
// -------------------------------------------------------------------
function graficarRegresion(puntos, funcion) {
    const applet = ggbApplet;

    if (!applet || typeof applet.evalCommand !== 'function') {
        console.error("GeoGebra no está listo para recibir comandos.");
        return;
    }

    // 1. Limpieza de Gráfica:
    try {
        // Borrar la función 'f' anterior
        applet.deleteObject('f');

        // Borrar los puntos anteriores (P1, P2, ...)
        // (Asumimos un máximo de 50 puntos para limpiar)
        for (let i = 1; i <= 50; i++) {
            applet.deleteObject(`P${i}`);
        }
    } catch (e) {
        console.warn('Error al limpiar objetos (puede ser la primera ejecución).');
    }

    // Si la función es nula (en caso de error), solo limpiamos y salimos.
    if (!funcion) return;

    // 2. Graficar CADA PUNTO individualmente
    puntos.forEach((p, index) => {
        const puntoNombre = `P${index + 1}`;
        const comandoPunto = `${puntoNombre} = (${p.X}, ${p.Y})`;
        applet.evalCommand(comandoPunto);
        applet.setColor(puntoNombre, 0, 0, 255); // Azul
        applet.setPointSize(puntoNombre, 5);
    });

    // 3. Graficar la Recta de Regresión
    let funcionLimpia = funcion
        .replace(/,/g, '.') // Garantizar punto decimal
        .replace('y =', 'f(x) =')
        .replace(/\*x/g, 'x'); // Quitar el '*' si existe

    applet.evalCommand(funcionLimpia);

    // 4. Configurar el estilo de la recta 'f'
    applet.setColor('f', 255, 0, 0); // Color Rojo
    applet.setLineThickness('f', 7);
}