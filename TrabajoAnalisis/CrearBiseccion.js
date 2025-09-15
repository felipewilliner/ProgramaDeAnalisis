document.addEventListener("DOMContentLoaded", function (event) {
    console.info("Formulario de Biseccion Listo");
});

document.getElementById('biseccionForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const funcion = document.getElementById('funcion').value;
    const xi = document.getElementById('xi').value;
    const xd = document.getElementById('xd').value;
    const iteraciones = document.getElementById('iteraciones').value;
    const tolerancia = document.getElementById('tolerancia').value;

    ggbApplet.evalCommand(`f(x) = ${funcion}`);

    const datosBiseccion = {
        Funcion: funcion,
        Xi: xi,
        Xd: xd,
        Iteraciones: iteraciones,
        Tolerancia: tolerancia
    };

    fetch('http://localhost:5125/api/Unidad1/biseccion', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(datosBiseccion)
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Error al cargar los datos');
            }
        })
        .then(data => {
            console.log('Respuesta de la API:', data);


            if (!data.success) {

                document.getElementById('resultado').innerHTML = `
                <p style="color:red;"><strong style ="color:red;">Error:</strong> La raiz no esta en el intervalo indicado. ${data.mensaje}</p>
            `;

            } else {
                document.getElementById('resultado').innerHTML = `
             <p>${data.mensaje}</p>
            <p><strong>Raiz:</strong> ${data.raiz ?? 'N/A'}</p>
              <p><strong>Iteraciones:</strong> ${data.iteraciones}</p>
              <p><strong>Error Relativo:</strong> ${data.error}</p>`;
                // Crear un punto en la ra�z sobre el eje X
                ggbApplet.evalCommand(`R = (${data.raiz}, 0)`);
                ggbApplet.setPointSize("R", 5); // Tama�o del punto
                ggbApplet.setColor("R", 255, 0, 0); // Rojo
            }
        })
        .catch(error => {
            console.error('Error al enviar los datos:', error);
            document.getElementById('resultado').innerHTML = `<p style="color:red;">Error al cargar los datos</p>`;
        });
});