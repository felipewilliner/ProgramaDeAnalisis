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

function calcularRegresion() {
    const tolerancia = parseFloat(document.getElementById('tolerancia').value);
    const grado = parseInt(document.getElementById('grado').value);
    const puntos = obtenerPuntos();
    const resultadoDiv = document.getElementById('resultado');

    // Validación mínima de datos
    if (puntos.length < 2) {
        resultadoDiv.innerHTML = `<p style="color:red;">Error: Se necesitan al menos 2 puntos para la regresión.</p>`;
        return;
    }

    const datosRegresion = {
        Tolerancia: tolerancia,
        Grado: grado,
        Puntos: puntos
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
            mostrarResultado(data, puntos);
        })
        .catch(error => {
            console.error('Error al enviar los datos:', error);
            resultadoDiv.innerHTML = `<p style="color:red;">Error de conexión con el servidor: ${error.message}</p>`;
        });
}

function mostrarResultado(data, puntos) {
    const resultadoDiv = document.getElementById('resultado');

    if (!data.funcion || data.funcion === "N/A") {
        resultadoDiv.innerHTML = `
            <div style="background:#f9e6e6; border:1px solid #e74c3c; color:#c0392b;">
                <strong>Error:</strong> ${data.mensajeEfectividadAjuste}
            </div>`;

        // Limpiar el gráfico
        ggbApplet.evalCommand('ClearAll()');
        return;
    }

    // 1. Mostrar resultados
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

    // 2. Graficar en GeoGebra
    graficarRegresion(puntos, data.funcion);
}

function graficarRegresion(puntos, funcion) {
    // 1. Obtener la referencia global de GeoGebra
    const applet = ggbApplet;

    if (!applet || typeof applet.evalCommand !== 'function') {
        console.error("GeoGebra no está listo para recibir comandos.");
        return;
    }

    // Limpieza de Gráfica: Usamos 'BorraTodo' (comando en español).
    // Si falla (como ZoomContent), lo envolvemos en un try/catch para que no detenga el script.
 

    // 2. Graficar CADA PUNTO individualmente (Estilo Unidad 1)
    const nombreListaPuntos = [];
    puntos.forEach((p, index) => {
        const puntoNombre = `P${index + 1}`;
        nombreListaPuntos.push(puntoNombre);

        // Comando: PuntoReg1 = (X, Y)
        const comandoPunto = `${puntoNombre} = (${p.X}, ${p.Y})`;
        applet.evalCommand(comandoPunto);

        // Configurar estilo del punto
        applet.setColor(puntoNombre, 0, 0, 255); // Azul
        applet.setPointSize(puntoNombre, 5);
    });


    // 3. Graficar la Recta de Regresión

    // Paso a. Limpiar la cadena: reemplazamos 'y = ' por 'f(x) = ', y quitamos '*' y comas.
    let funcionLimpia = funcion
        .replace(/,/g, '.') // Garantizar punto decimal, si C# no lo hizo
        .replace('y =', 'f(x) =')
        .replace(/\*x/g, 'x'); // Quitar el '*' si existe, para que quede 1.0000x

    // Comando: f(x) = 1.0000x + 1.0000
    applet.evalCommand(funcionLimpia);

    // 4. Configurar el estilo de la recta 'f'
    applet.setColor('f', 255, 0, 0); // Color Rojo
    applet.setLineThickness('f', 7);
}