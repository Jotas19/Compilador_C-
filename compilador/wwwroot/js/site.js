// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Obtener referencias a elementos HTML

const fileInput = document.getElementById('fileInput');
const codeEditor = document.getElementById('codeEditor');

// Agregar un evento change al input de archivo
fileInput.addEventListener('change', function () {
    const file = fileInput.files[0];
    const reader = new FileReader();

    reader.onload = function (e) {
        const content = e.target.result;
        codeEditor.textContent = content; // Llena el editor de código con el contenido del archivo
    };

    reader.readAsText(file);
});

//NUMERACION
function updateLineNumbers() {
    const codeEditor = document.getElementById('codeEditor');
    const lineNumbers = document.getElementById('lineNumbers');
    const lines = codeEditor.innerText.split('\n');

    // Elimina líneas vacías al final si las hay
    while (lines.length > 1 && lines[lines.length - 1].trim() === '') {
        lines.pop();
    }

    let lineNumbersText = '1\n';
    let lineNumber = 2;

    for (let i = 1; i < lines.length; i++) {
        lineNumbersText += lineNumber + '\n';
        lineNumber++;
    }

    lineNumbers.innerText = lineNumbersText;

    // Llamamos a syncLineNumbers para mantener la sincronización
    syncLineNumbers();
}

function syncLineNumbers() {
    const codeEditor = document.getElementById('codeEditor');
    const lineNumbers = document.getElementById('lineNumbers');
    
    const scrollTop = codeEditor.scrollTop;
    lineNumbers.scrollTop = scrollTop;
}

function loadFile() {
    const fileInput = document.getElementById('fileInput');
    const codeEditor = document.getElementById('codeEditor');
    const lineNumbers = document.getElementById('lineNumbers');

    const file = fileInput.files[0];
    const reader = new FileReader();

    reader.onload = function (e) {
        codeEditor.innerText = e.target.result;
        lineNumbers.innerText = '';
        updateLineNumbers();
    };

    reader.readAsText(file);
}

// Llamamos a updateLineNumbers al inicio
updateLineNumbers();

// Limpiar
const editorOutput = document.getElementById('codeEditor');
const lexicoOutput = document.getElementById('lexicoOutput');
const semanticoOutput = document.getElementById('semanticoOutput');
const sintacticoOutput = document.getElementById('sintacticoOutput');
const limpiarButton = document.getElementById('limpiarButton');
const lineNumbers = document.getElementById('lineNumbers');

limpiarButton.addEventListener('click', function () {
    editorOutput.innerHTML = '';
    lexicoOutput.innerHTML = '';
    semanticoOutput.innerHTML = '';
    sintacticoOutput.innerHTML = '';
    lineNumbers.innerText = '1';
});

// Limpiar analizadores
const codeEditor2 = document.getElementById('codeEditor');

codeEditor2.addEventListener('click', function () {
    limpiarAnalizadores();
});

function limpiarAnalizadores() {
    const lexicoOutput = document.getElementById('lexicoOutput');
    const semanticoOutput = document.getElementById('semanticoOutput');
    const sintacticoOutput = document.getElementById('sintacticoOutput');

    lexicoOutput.textContent = '';
    semanticoOutput.textContent = '';
    sintacticoOutput.textContent = '';
}



var lexicoResult;
var resultArray;





function generarArbolLexico(data) {
    var arbolLexico = [];

    data.forEach(function (resultArray) {
        var nodoPadre = { type: "ROOT", children: [] };

        resultArray.forEach(function (result) {
            var nodoHijo = { type: result.type, value: result.value };
            nodoPadre.children.push(nodoHijo);
        });

        arbolLexico.push(nodoPadre);
    });

    return { name: "ROOT", children: arbolLexico };
}

function mostrarArbolD3(data) {
    // Selecciona el contenedor para el árbol
    var container = d3.select("#arbolContainer");

    // Configura el layout del árbol
    var layout = d3.tree().size([500, 300]);

    // Crea una estructura de datos jerárquica
    var root = d3.hierarchy(data);

    // Calcula la disposición del árbol
    layout(root);

    // Crea un contenedor SVG
    var svg = container.append("svg")
        .attr("width", 600)
        .attr("height", 400)
        .append("g")
        .attr("transform", "translate(50, 20)");

    // Dibuja las conexiones entre nodos
    var links = svg.selectAll(".link")
        .data(root.links())
        .enter().append("path")
        .attr("class", "link")
        .attr("d", d3.linkHorizontal()
            .x(function (d) { return d.y; })
            .y(function (d) { return d.x; }));

    // Dibuja los nodos
    var nodes = svg.selectAll(".node")
        .data(root.descendants())
        .enter().append("g")
        .attr("class", "node")
        .attr("transform", function (d) { return "translate(" + d.y + "," + d.x + ")"; });

    // Agrega círculos a los nodos
    nodes.append("circle")
        .attr("r", 5);

    // Agrega etiquetas a los nodos
    nodes.append("text")
        .attr("dy", 3)
        .attr("x", function (d) { return d.children ? -8 : 8; })
        .style("text-anchor", function (d) { return d.children ? "end" : "start"; })
        .text(function (d) { return d.data.name; });
}



function mostrarArbolLexico(data) {
    // Crear contenido HTML para la modal
    let modalContent = '<div class="modal-content">';

    // Agregar un nuevo contenedor para el árbol
    modalContent += '<div id="arbolContainer"></div>';

    data.forEach(function (resultArray, index) {
        modalContent += `<p>Árbol ${index + 1}:</p>`;

        resultArray.forEach(function (result) {
            modalContent += `<p>Type: ${result.type}, Value: ${result.value}</p>`;
        });

        modalContent += '<hr>';
    });

    modalContent += '</div>';

    // Crear modal
    let modal = `<div class="modal" tabindex="-1" role="dialog">
                    <div class="modal-dialog" role="document">
                        ${modalContent}
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                    </div>
                </div>`;

    // Agregar modal al cuerpo del documento
    $('body').append(modal);

    // Mostrar modal
    $('.modal').modal('show');

    // Eliminar modal del DOM después de cerrarlo
    $('.modal').on('hidden.bs.modal', function () {
        $(this).remove();
    });

    // Llamar a la función para mostrar el árbol después de que se haya mostrado la modal
    mostrarArbolD3(data);
}











function analyzeLexico() {
    // Obtener el código del editor
    var codeEditorContent = document.getElementById('codeEditor').innerText;
    console.log('Código de entrada:', codeEditorContent);


    // Llamar directamente a la función Tokenize del controlador
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        url: '/api/Analizador_Lexico/Tokenize',
        data: JSON.stringify(codeEditorContent), // Solo envía el contenido directamente, sin un objeto con una propiedad 'codigo'
        success: function (data) {
            console.log('Respuesta del servidor:', data);

            lexicoResult = data;
            // Mostrar los resultados en lexicoOutput
            var lexicoOutputElement = document.getElementById('lexicoOutput');
            lexicoOutputElement.innerHTML = ''; // Limpiar contenido anterior

            if (data.length > 0) {
                data.forEach(function (resultArray) {
                    var resultDiv = document.createElement('div');
                    resultArray.forEach(function (result) {
                        resultDiv.textContent += `Type: ${result.type}, Value: ${result.value} | \n`;
                    });
                    resultDiv.textContent += '\n';

                    lexicoOutputElement.appendChild(resultDiv);
                });
            } else {
                lexicoOutputElement.textContent = 'No se ha detectado nada en la caja de texto para analizar.';
            }
        },
        error: function (error) {
            console.error('Error al llamar a Tokenize:', error);
        }
    });
}

function analyzeSintactico() {
    $.ajax({
        type: 'GET',
        url: '/api/Analizador_Sintactico/ObtenerReporte', // Reemplaza 'TuControlador' con el nombre correcto de tu controlador
        success: function (data) {
            console.log('Reporte del servidor:', data);
            var sintacticoOutputElement = document.getElementById('sintacticoOutput');
            sintacticoOutputElement.innerHTML = ''; // Limpiar contenido anterior
            if (data.length > 0) {
                data.forEach(function (result) {
                    var resultDiv = document.createElement('div');
                    resultDiv.textContent = `Type: ${result.type}, Value: ${result.value} | \n`;

                    sintacticoOutputElement.appendChild(resultDiv);
                });
            } else {
                sintacticoOutputElement.textContent = 'No se ha detectado nada en la caja de texto para analizar.';
            }
        },
        error: function (error) {
            console.error('Error al obtener el reporte:', error);
        }
    });
}

function analyzeSemantico() {
    $.ajax({
        type: 'GET',
        url: '/api/Analizador_Semantico/ObtenerReporte_Semantico', // Reemplaza 'TuControlador' con el nombre correcto de tu controlador
        success: function (data) {
            console.log('Reporte del servidor:', data);
            var semanticoOutputElement = document.getElementById('semanticoOutput');
            semanticoOutputElement.innerHTML = ''; // Limpiar contenido anterior
            if (data.length > 0) {
                data.forEach(function (result) {
                    var resultDiv = document.createElement('div');
                    resultDiv.textContent = `Type: ${result.type}, Value: ${result.value} | \n`;

                    semanticoOutputElement.appendChild(resultDiv);
                });
            } else {
                semanticoOutputElement.textContent = 'No se ha detectado nada en la caja de texto para analizar.';
            }
        },
        error: function (error) {
            console.error('Error al obtener el reporte:', error);
        }
    });
}
