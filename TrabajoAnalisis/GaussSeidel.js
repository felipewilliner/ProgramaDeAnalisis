document.addEventListener("DOMContentLoaded", function () {
    console.info("Formulario de Gauss-Seidel Listo");
    document.querySelector('button[onclick="generarMatriz()"]').addEventListener('click', generarMatriz);

    // Prevenir el envío del formulario y llamar a resolverGaussSeidel
    document.getElementById('gaussSeidelForm').addEventListener('submit', function (event) {
        event.preventDefault();
        resolverGaussSeidel();
    });

    generarMatriz();
});


function generarMatriz() {
    const n = parseInt(document.getElementById("dimension").value);
    if (isNaN(n) || n < 2 || n > 8) {
        alert("Por favor, ingrese una dimensión válida entre 2 y 8");
        return;
    }

    const container = document.getElementById("matriz-container");
    container.innerHTML = "";

    const tabla = document.createElement("table");
    tabla.style.margin = "10px 0";

    for (let i = 0; i < n; i++) {
        const fila = document.createElement("tr");
        for (let j = 0; j <= n; j++) {
            const celda = document.createElement("td");
            const input = document.createElement("input");
            input.type = "number";
            input.step = "any";
            input.id = `cell-${i}-${j}`;
            input.style.width = "60px";
            input.placeholder = `a${i + 1}${j + 1}`;
            input.value = "0";
            celda.appendChild(input);
            fila.appendChild(celda);

            if (j === n - 1) {
                const celdaIgual = document.createElement("td");
                celdaIgual.textContent = "=";
                celdaIgual.style.padding = "0 5px";
                celdaIgual.style.fontWeight = "bold";
                fila.appendChild(celdaIgual);
            }
        }
        tabla.appendChild(fila);
    }
    container.appendChild(tabla);
}

function resolverGaussSeidel() {
    const n = parseInt(document.getElementById("dimension").value);
    const tolerancia = parseFloat(document.getElementById("tolerancia").value) || 0.0001;
    const maxIteraciones = parseInt(document.getElementById("maxIteraciones").value) || 100;

    let matriz = [];
    for (let i = 0; i < n; i++) {
        matriz[i] = [];
        for (let j = 0; j <= n; j++) {
            const input = document.getElementById(`cell-${i}-${j}`);
            matriz[i][j] = parseFloat(input?.value) || 0;
        }
    }

    const parametros = {
        dimension: n,
        matriz: matriz,
        tolerancia: tolerancia,
        maxIteraciones: maxIteraciones
    };

    console.log('Enviando datos:', parametros);

    fetch('http://localhost:5125/api/Unidad2/gaussseidel', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(parametros)
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
            console.log('Respuesta:', data);
            mostrarResultadoGaussSeidel(data);
        })
        .catch(error => {
            console.error('Error completo:', error);
            document.getElementById('info').innerHTML = `
        <div style="color:red; padding:20px; background:#ffe6e6; border-radius:5px;">
            Error al conectar con el servidor: ${error.message}
        </div>`;
        });
}

function formatearNumero(numero) {
    if (Number.isInteger(numero)) {
        return numero.toString();
    } else {
        const partes = numero.toString().split('.');
        if (partes.length === 2) {
            const decimales = partes[1].substring(0, 6);
            const decimalesSinCeros = decimales.replace(/0+$/, '');
            return decimalesSinCeros ? `${partes[0]}.${decimalesSinCeros}` : partes[0];
        }
        return numero.toString();
    }
}

function mostrarResultadoGaussSeidel(data) {
    const infoDiv = document.getElementById('info');

    // Verificar diferentes posibles nombres de propiedades
    const success = data.succes !== undefined ? data.succes :
        data.success !== undefined ? data.success : false;

    const mensaje = data.mensaje || data.message || 'Error desconocido';
    const resultados = data.resultados || data.Resultados || [];
    const iteraciones = data.iteraciones || data.Iteraciones || 0;
    const errorFinal = data.errorFinal || data.ErrorFinal || 0;

    if (!success) {
        infoDiv.innerHTML = `
        <div style="color:red; padding:20px; background:#ffe6e6; border-radius:5px;">
            <strong>Error:</strong> ${mensaje}
            ${iteraciones ? `<br><strong>Iteraciones realizadas:</strong> ${iteraciones}` : ''}
            ${errorFinal ? `<br><strong>Error final:</strong> ${errorFinal.toFixed(6)}` : ''}
        </div>`;
        return;
    }

    let html = `
    <div style="background:white; padding:20px; border-radius:10px; box-shadow:0 2px 10px rgba(0,0,0,0.1);">
        <h2 style="color:#2980b9; text-align:center; margin-bottom:20px;">Resultados del Metodo Gauss-Seidel</h2>
        <div style="color:#2c3e50; margin-bottom:10px;">
            <strong>Estado:</strong> ${mensaje}
        </div>
        <div style="color:#7f8c8d; margin-bottom:10px;">
            <strong>Iteraciones:</strong> ${iteraciones}
        </div>
        <div style="color:#7f8c8d; margin-bottom:15px;">
            <strong>Error final:</strong> ${errorFinal.toFixed(6)}
        </div>
        <h3 style="color:#27ae60; border-bottom:2px solid #27ae60; padding-bottom:5px;">Solucion del Sistema:</h3>
        <div style="font-size:1.2rem; margin:20px 0;">`;

    for (let i = 0; i < resultados.length; i++) {
        const valor = formatearNumero(resultados[i]);
        html += `
            <div style="padding:8px 0; border-bottom:1px solid #eee;">
                <span style="font-weight:bold; color:#2980b9;">x${i + 1}</span> = 
                <span style="color:#e74c3c; font-weight:bold;">${valor}</span>
            </div>`;
    }

  
    </div>`;

    infoDiv.innerHTML = html;
}