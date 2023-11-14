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