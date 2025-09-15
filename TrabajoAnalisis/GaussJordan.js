document.addEventListener("DOMContentLoaded", function () {
    console.info("Formulario de Gauss Jordan Listo");
    document.querySelector('button[onclick="generarMatriz()"]').addEventListener('click', generarMatriz);
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

document.getElementById('gaussJordanForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const n = parseInt(document.getElementById("dimension").value);
    let matriz = [];

    for (let i = 0; i < n; i++) {
        matriz[i] = [];
        for (let j = 0; j <= n; j++) {
            const input = document.getElementById(`cell-${i}-${j}`);
            matriz[i][j] = parseFloat(input?.value) || 0;
        }
    }

    const datosGaussJordan = {
        dimension: n,
        matriz: matriz
    };

    console.log('Enviando datos:', datosGaussJordan);

    fetch('http://localhost:5125/api/Unidad2/gaussjordan', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(datosGaussJordan)
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
            mostrarResultado(data);
        })
        .catch(error => {
            console.error('Error completo:', error);
            document.getElementById('info').innerHTML = `
        <div style="color:red; padding:20px; background:#ffe6e6; border-radius:5px;">
            <strong>Error:</strong> ${error.message}
        </div>`;
        });
});

function mostrarResultado(data) {
    const infoDiv = document.getElementById('info');

    if (!data.success) {
        infoDiv.innerHTML = `
        <div style="color:red; padding:20px; background:#ffe6e6; border-radius:5px;">
            <strong>Error:</strong> ${data.mensaje || 'Error desconocido'}
        </div>`;
        return;
    }

    // Mostrar resultados en la columna derecha con mejor formato
    let html = `
    <div style="background:white; padding:20px; border-radius:10px; box-shadow:0 2px 10px rgba(0,0,0,0.1);">
        <h2 style="color:#2980b9; text-align:center; margin-bottom:20px;">Resultados del Metodo Gauss-Jordan</h2>
        <div style="color:#2c3e50; margin-bottom:15px;">
            <strong>Estado:</strong> ${data.mensaje || 'Solucion encontrada'}
        </div>
        <h3 style="color:#27ae60; border-bottom:2px solid #27ae60; padding-bottom:5px;">Solucion del Sistema:</h3>
        <div style="font-size:1.2rem; margin:20px 0;">`;

    // Mostrar cada variable con su resultado
    for (let i = 0; i < data.resultados.length; i++) {
        const formatearNumero = (num) => {
            if (Number.isInteger(num)) {
                return num.toString(); // Número entero, sin decimales
            } else {
                // Convertir a string y cortar hasta 6 decimales sin redondear
                const partes = num.toString().split('.');
                if (partes.length === 2) {
                    const decimales = partes[1].substring(0, 6);
                    // Eliminar ceros finales
                    const decimalesSinCeros = decimales.replace(/0+$/, '');
                    return decimalesSinCeros ? `${partes[0]}.${decimalesSinCeros}` : partes[0];
                }
                return num.toString();
            }
        };

        const valorFormateado = typeof data.resultados[i] === 'number' ?
            formatearNumero(data.resultados[i]) : 'N/A';
        html += `
            <div style="padding:8px 0; border-bottom:1px solid #eee;">
                <span style="font-weight:bold; color:#2980b9;">x${i + 1}</span> = 
                <span style="color:#e74c3c; font-weight:bold;">${valorFormateado}</span>
            </div>`;
    }

    html += `
        </div>
        <div style="margin-top:20px; padding:15px; background:#f8f9fa; border-radius:5px;">


        </div>
    </div>`;

    infoDiv.innerHTML = html;
}