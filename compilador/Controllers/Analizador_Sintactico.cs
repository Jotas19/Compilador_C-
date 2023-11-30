/*
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static compilador.Controllers.Analizador_Lexico;

namespace compilador.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Analizador_Sintactico : ControllerBase
    {

        public List<Token> GetTokens_From_Lexico(List<Token> lineTokens)
        {
            // Llamada al método GetTokens
            List<Token> tokensObtenidos = TokenUtility.GetTokens(lineTokens);

            // Haz lo que necesites con la lista de tokens obtenidos
            foreach (var token in tokensObtenidos)
            {
                Console.WriteLine($"Tipo: {token.Type}, Valor: {token.Value}");
            }
            return lineTokens;
        }

        public class Report_Message
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public Report_Message(string type, string value)
            {
                Type = type;
                Value = value;

            }
        }


        [HttpPost]
        [Route("AnalizarSintaxis")]
        public IActionResult AnalizarSintaxis(List<Token> tokensObtenidos)
        {

            List<Report_Message> Report_Message = new List<Report_Message>;
            try
            {
                GetTokens_From_Lexico (tokensObtenidos);

                if (tokensObtenidos == null || !tokensObtenidos.Any())
                {
                    Console.WriteLine("Error: Lista de tokens vacía o nula.");

                    string errorMessage = $"Error: Lista de tokens vacía o nula.";
                    Report_Message.Add(new Report_Message("ERROR", errorMessage));
                    Console.WriteLine(errorMessage);
                    return Ok(Report_Message);
                }

                // Inicia el análisis sintáctico según las reglas gramaticales de MiniLang
                AnalizarDeclaraciones(tokensObtenidos);
                AnalizarOperaciones(tokensObtenidos);
                AnalizarEstructurasControl(tokensObtenidos);
                AnalizarFunciones(tokensObtenidos);

                Console.WriteLine("Análisis sintáctico exitoso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el análisis sintáctico: {ex.Message}");
            }
        }

        //Primer paso del analizador. 1. Analizador de Declaraciones
        private List<Report_Message> AnalizarDeclaraciones(List<Token> lineTokens)
        {
            List<Report_Message> Report_Message = new List<Report_Message>;

            int index = 0;

            while (index < lineTokens.Count)
            {
                // Verificar si el token actual es una palabra clave de tipo (int, float, bool, string)
                if (EsPalabraClaveDeTipo(lineTokens, index))
                {
                    // Obtener el tipo de la declaración (int, float, bool, string)
                    string tipo = lineTokens[index].Value;

                    // Avanzar al siguiente token
                    index++;

                    // Verificar si el siguiente token es un identificador
                    if (index < lineTokens.Count && lineTokens[index].Type == "IDENTIFIER")
                    {
                        // Obtener el nombre del identificador
                        string nombreVariable = lineTokens[index].Value;

                        // Avanzar al siguiente token
                        index++;

                        // Verificar si el siguiente token es un signo igual '='
                        if (index < lineTokens.Count && lineTokens[index].Type == "OPERATOR" && lineTokens[index].Value == "=")
                        {
                            // Avanzar al siguiente token
                            index++;

                            AnalizarExpresion(lineTokens, ref index);

                            // Avanzar al siguiente token después del valor asignado
                            index++;

                            // Verificar si el siguiente token es un punto y coma ';'
                            if (index < lineTokens.Count && lineTokens[index].Type == "SEMICOLON")
                            {
                                // La declaración es válida, puedes almacenarla o realizar otras acciones según sea necesario
                            }
                            else
                            {
                                string errorMessage = $"Error de sintaxis: Se esperaba un punto y coma al final de la declaración de '{nombreVariable}'";
                                Report_Message.Add(new Report_Message("ERROR", errorMessage));
                                Console.WriteLine(errorMessage);
                                // Error: Se esperaba un punto y coma al final de la declaración
                                Console.WriteLine($"Error de sintaxis: Se esperaba un punto y coma al final de la declaración de '{nombreVariable}'");
                            }
                        }
                        else
                        {
                            string errorMessage = $"Error de sintaxis: Falta el signo igual '=' después de '{nombreVariable}'";
                            Report_Message.Add(new Report_Message("ERROR", errorMessage));
                            Console.WriteLine(errorMessage);
                            // Error: Falta el signo igual '=' después del identificador
                            Console.WriteLine($"Error de sintaxis: Falta el signo igual '=' después de '{nombreVariable}'");
                        }
                    }
                    else
                    {
                        string errorMessage = $"Error de sintaxis: Se esperaba un identificador después de '{tipo}'";
                        Report_Message.Add(new Report_Message("ERROR", errorMessage));
                        Console.WriteLine(errorMessage);
                        // Error: Se esperaba un identificador después del tipo
                        Console.WriteLine($"Error de sintaxis: Se esperaba un identificador después de '{tipo}'");
                    }
                }
                else
                {
                    // Avanzar al siguiente token si no es una palabra clave de tipo
                    index++;
                }
                return Report_Message;
            }
        }
        //2. Analizador de operaciones.
        private void AnalizarOperaciones(List<Token> lineTokens)
        {
            int index = 0;

            while (index < lineTokens.Count)
            {
                Token tokenActual = lineTokens[index];

                if (EsOperacionAritmetica(tokenActual))
                {
                    // Analizar expresión aritmética
                    double resultadoExpresion = AnalizarExpresionAritmetica(lineTokens, ref index);

                    // Realizar acciones necesarias con el resultado, como asignarlo o imprimirlo
                    Console.WriteLine($"Operación aritmética: {resultadoExpresion}");
                }
                else if (EsOperacionLogica(tokenActual))
                {
                    // Analizar expresión lógica
                    bool resultadoExpresion = AnalizarExpresionLogica(lineTokens, ref index);

                    // Realizar acciones necesarias con el resultado, como asignarlo o imprimirlo
                    Console.WriteLine($"Operación lógica: {resultadoExpresion}");
                }
                else
                {
                    // Avanzar al siguiente token si no es una operación reconocida
                    index++;
                }
            }
        }

        private void AnalizarEstructurasControl(List<Token> lineTokens)
        {
            int index = 0;

            while (index < lineTokens.Count)
            {
                // Verificar si el token actual es la palabra clave "if"
                if (EsPalabraClaveIf(lineTokens, index))
                {
                    // Avanzar al siguiente token después de "if"
                    index++;

                    // Lógica para analizar la condición del "if"
                    if (AnalizarCondicion(lineTokens, ref index))
                    {
                        // Verificar si hay un bloque de código después de la condición
                        if (index < lineTokens.Count && lineTokens[index].Type == "LEFT_BRACE")
                        {
                            // Avanzar al siguiente token después de "{"
                            index++;

                            // Lógica para analizar el bloque de código dentro del "if"
                            AnalizarBloqueCodigo(lineTokens, ref index);

                            // Verificar si hay un bloque "else" después del bloque "if"
                            if (index < lineTokens.Count && EsPalabraClaveElse(lineTokens, index))
                            {
                                // Avanzar al siguiente token después de "else"
                                index++;

                                // Verificar si hay un bloque de código después de "else"
                                if (index < lineTokens.Count && lineTokens[index].Type == "LEFT_BRACE")
                                {
                                    // Avanzar al siguiente token después de "{"
                                    index++;

                                    // Lógica para analizar el bloque de código dentro del "else"
                                    AnalizarBloqueCodigo(lineTokens, ref index);
                                }
                                else
                                {
                                    // Error: Bloque de código esperado después de "else"
                                    Console.WriteLine("Error de sintaxis: Bloque de código esperado después de 'else'");
                                }
                            }
                        }
                        else
                        {
                            // Error: Bloque de código esperado después de la condición del "if"
                            Console.WriteLine("Error de sintaxis: Bloque de código esperado después de la condición 'if'");
                        }
                    }
                    else
                    {
                        // Error: Condición del "if" no válida
                        Console.WriteLine("Error de sintaxis: Condición del 'if' no válida");
                    }
                }
                else if (EsPalabraClaveWhile(lineTokens, index))
                {
                    // Avanzar al siguiente token después de "while"
                    index++;

                    // Lógica para analizar la condición del "while"
                    if (AnalizarCondicion(lineTokens, ref index))
                    {
                        // Verificar si hay un bloque de código después de la condición
                        if (index < lineTokens.Count && lineTokens[index].Type == "LEFT_BRACE")
                        {
                            // Avanzar al siguiente token después de "{"
                            index++;

                            // Lógica para analizar el bloque de código dentro del "while"
                            AnalizarBloqueCodigo(lineTokens, ref index);
                        }
                        else
                        {
                            // Error: Bloque de código esperado después de la condición del "while"
                            Console.WriteLine("Error de sintaxis: Bloque de código esperado después de la condición 'while'");
                        }
                    }
                    else
                    {
                        // Error: Condición del "while" no válida
                        Console.WriteLine("Error de sintaxis: Condición del 'while' no válida");
                    }
                }
                else
                {
                    // Avanzar al siguiente token si no es una estructura de control reconocida
                    index++;
                }
            }
        }

        private void AnalizarFunciones(List<Token> lineTokens)
        {
            int index = 0;

            while (index < lineTokens.Count)
            {
                // Verificar si el token actual es una palabra clave de tipo (int, float, bool, string)
                if (EsPalabraClaveDeTipo(lineTokens, index))
                {
                    // Avanzar al siguiente token después del tipo
                    index++;

                    // Verificar si el siguiente token es un identificador
                    if (index < lineTokens.Count && lineTokens[index].Type == "IDENTIFIER")
                    {
                        // Obtener el nombre del identificador (posible nombre de función)
                        string nombreFuncion = lineTokens[index].Value;

                        // Avanzar al siguiente token
                        index++;

                        // Verificar si el siguiente token es un paréntesis de apertura '('
                        if (index < lineTokens.Count && lineTokens[index].Type == "LEFT_PAREN")
                        {
                            // Avanzar al siguiente token después del paréntesis de apertura '('
                            index++;

                            // Lógica para analizar los parámetros de la función (si los hay)
                            AnalizarParametrosFuncion(lineTokens, ref index);

                            // Verificar si el siguiente token es un paréntesis de cierre ')'
                            if (index < lineTokens.Count && lineTokens[index].Type == "RIGHT_PAREN")
                            {
                                // Avanzar al siguiente token después del paréntesis de cierre ')'
                                index++;

                                // Verificar si hay un bloque de código después de la definición de la función
                                if (index < lineTokens.Count && lineTokens[index].Type == "LEFT_BRACE")
                                {
                                    // Avanzar al siguiente token después de '{'
                                    index++;

                                    // Lógica para analizar el bloque de código dentro de la función
                                    AnalizarBloqueCodigo(lineTokens, ref index);

                                    // Verificar si hay un paréntesis de cierre '}' después del bloque de código
                                    if (index < lineTokens.Count && lineTokens[index].Type == "RIGHT_BRACE")
                                    {
                                        // Avanzar al siguiente token después del paréntesis de cierre '}'
                                        index++;
                                    }
                                    else
                                    {
                                        // Error: Se esperaba un paréntesis de cierre '}' después del bloque de código
                                        Console.WriteLine("Error de sintaxis: Se esperaba '}' después del bloque de código de la función");
                                    }
                                }
                                else
                                {
                                    // Error: Bloque de código esperado después de la definición de la función
                                    Console.WriteLine("Error de sintaxis: Bloque de código esperado después de la definición de la función");
                                }
                            }
                            else
                            {
                                // Error: Se esperaba un paréntesis de cierre ')' después de los parámetros de la función
                                Console.WriteLine("Error de sintaxis: Se esperaba ')' después de los parámetros de la función");
                            }
                        }
                        else
                        {
                            // Error: Se esperaba un paréntesis de apertura '(' después del nombre de la función
                            Console.WriteLine("Error de sintaxis: Se esperaba '(' después del nombre de la función");
                        }
                    }
                    else
                    {
                        // Error: Se esperaba un identificador después del tipo
                        Console.WriteLine("Error de sintaxis: Se esperaba un identificador después del tipo");
                    }
                }
                else
                {
                    // Avanzar al siguiente token si no es una declaración de función
                    index++;
                }
            }
        }

        private void AnalizarParametrosFuncion(List<Token> tokens, ref int index)
        {
            // Lógica para analizar los parámetros de una función
            // Avanza el índice según los tokens procesados
        }

        private bool EsPalabraClaveIf(List<Token> tokens, int index)
        {
            // Verificar si el token actual es la palabra clave "if"
            return index < tokens.Count && tokens[index].Type == "KEYWORD" && tokens[index].Value == "if";
        }

        private bool EsPalabraClaveElse(List<Token> tokens, int index)
        {
            // Verificar si el token actual es la palabra clave "else"
            return index < tokens.Count && tokens[index].Type == "KEYWORD" && tokens[index].Value == "else";
        }

        private bool EsPalabraClaveWhile(List<Token> tokens, int index)
        {
            // Verificar si el token actual es la palabra clave "while"
            return index < tokens.Count && tokens[index].Type == "KEYWORD" && tokens[index].Value == "while";
        }

        private bool AnalizarCondicion(List<Token> tokens, ref int index)
        {
            // Lógica para analizar la condición, puedes usar AnalizarExpresionLogica u otro método según sea necesario
            // Avanza el índice según los tokens procesados
            return true; // Cambia esta línea según la lógica de tu analizador
        }

        private void AnalizarBloqueCodigo(List<Token> tokens, ref int index)
        {
            // Lógica para analizar un bloque de código encerrado por llaves {}
            // Avanza el índice según los tokens procesados
        }



        //1.2 Analizar la expresión del Analizador de deficiones. 
        private double AnalizarExpresion(List<Token> tokens, ref int index)
        {
            double resultado = 0;

            // Mantén una pila para evaluar la expresión en orden
            Stack<double> valores = new Stack<double>();
            // Mantén una pila para los operadores
            Stack<char> operadores = new Stack<char>();

            while (index < tokens.Count && tokens[index].Type != "SEMICOLON")
            {
                Token tokenActual = tokens[index];

                if (EsLiteralNumerico(tokenActual))
                {
                    // Si el token actual es un literal numérico, conviértelo y agrégalo a la pila de valores
                    if (double.TryParse(tokenActual.Value, out double valorNumerico))
                    {
                        valores.Push(valorNumerico);
                    }
                    else
                    {
                        // Manejar error de conversión
                        Console.WriteLine($"Error de sintaxis: No se pudo convertir el literal numérico '{tokenActual.Value}'");
                    }
                }
                else if (EsOperadorAritmetico(tokenActual))
                {
                    // Si el token actual es un operador aritmético, evalúa los operadores en la pila
                    while (operadores.Count > 0 && PrecedenciaOperador(operadores.Peek()) >= PrecedenciaOperador(tokenActual.Value[0]))
                    {
                        EvaluarOperador(valores, operadores.Pop());
                    }

                    // Agrega el operador actual a la pila
                    operadores.Push(tokenActual.Value[0]);
                }
                else
                {
                    // Manejar otros tipos de tokens o lanzar un error según sea necesario
                    Console.WriteLine($"Error de sintaxis: Token no reconocido '{tokenActual.Value}' en la expresión");
                }

                // Avanza al siguiente token
                index++;
            }

            // Evalúa cualquier operador restante en la pila
            while (operadores.Count > 0)
            {
                EvaluarOperador(valores, operadores.Pop());
            }

            // El resultado final debe estar en la cima de la pila de valores
            if (valores.Count == 1)
            {
                resultado = valores.Pop();
            }
            else
            {
                // Manejar error de evaluación
                Console.WriteLine("Error de sintaxis: Expresión inválida");
            }

            return resultado;
        }

        
        //2.2 Analizador de la expresión aritmetica. 
        private double AnalizarExpresionAritmetica(List<Token> tokens, ref int index)
        {
            double resultado = 0;

            // Mantén una pila para evaluar la expresión en orden
            Stack<double> valores = new Stack<double>();
            // Mantén una pila para los operadores
            Stack<char> operadores = new Stack<char>();

            while (index < tokens.Count && tokens[index].Type != "SEMICOLON")
            {
                Token tokenActual = tokens[index];

                if (EsLiteralNumerico(tokenActual))
                {
                    // Si el token actual es un literal numérico, conviértelo y agrégalo a la pila de valores
                    if (double.TryParse(tokenActual.Value, out double valorNumerico))
                    {
                        valores.Push(valorNumerico);
                    }
                    else
                    {
                        // Manejar error de conversión
                        Console.WriteLine($"Error de sintaxis: No se pudo convertir el literal numérico '{tokenActual.Value}'");
                    }
                }
                else if (EsOperacionAritmetica(tokenActual))
                {
                    // Si el token actual es un operador aritmético, evalúa los operadores en la pila
                    while (operadores.Count > 0 && PrecedenciaOperador(operadores.Peek()) >= PrecedenciaOperador(tokenActual.Value[0]))
                    {
                        EvaluarOperador(valores, operadores.Pop());
                    }

                    // Agrega el operador actual a la pila
                    operadores.Push(tokenActual.Value[0]);
                }
                else
                {
                    // Manejar otros tipos de tokens o lanzar un error según sea necesario
                    Console.WriteLine($"Error de sintaxis: Token no reconocido '{tokenActual.Value}' en la expresión");
                }

                // Avanza al siguiente token
                index++;
            }

            // Evalúa cualquier operador restante en la pila
            while (operadores.Count > 0)
            {
                EvaluarOperador(valores, operadores.Pop());
            }

            // El resultado final debe estar en la cima de la pila de valores
            if (valores.Count == 1)
            {
                resultado = valores.Pop();
            }
            else
            {
                // Manejar error de evaluación
                Console.WriteLine("Error de sintaxis: Expresión aritmética inválida");
            }

            return resultado;
        }

        private bool AnalizarExpresionLogica(List<Token> tokens, ref int index)
        {
            bool resultado = false;

            // Mantén una pila para evaluar la expresión lógica en orden
            Stack<bool> valores = new Stack<bool>();
            // Mantén una pila para los operadores lógicos
            Stack<string> operadoresLogicos = new Stack<string>();

            while (index < tokens.Count && tokens[index].Type != "SEMICOLON")
            {
                Token tokenActual = tokens[index];

                if (EsLiteralBooleano(tokenActual))
                {
                    // Si el token actual es un literal booleano, conviértelo y agrégalo a la pila de valores
                    if (bool.TryParse(tokenActual.Value, out bool valorBooleano))
                    {
                        valores.Push(valorBooleano);
                    }
                    else
                    {
                        // Manejar error de conversión
                        Console.WriteLine($"Error de sintaxis: No se pudo convertir el literal booleano '{tokenActual.Value}'");
                    }
                }
                else if (EsOperacionLogica(tokenActual))
                {
                    // Si el token actual es un operador lógico, evalúa los operadores en la pila
                    while (operadoresLogicos.Count > 0 && PrecedenciaOperadorLogico(operadoresLogicos.Peek()) >= PrecedenciaOperadorLogico(tokenActual.Value))
                    {
                        EvaluarOperadorLogico(valores, operadoresLogicos.Pop());
                    }

                    // Agrega el operador lógico actual a la pila
                    operadoresLogicos.Push(tokenActual.Value);
                }
                else
                {
                    // Manejar otros tipos de tokens o lanzar un error según sea necesario
                    Console.WriteLine($"Error de sintaxis: Token no reconocido '{tokenActual.Value}' en la expresión lógica");
                }

                // Avanza al siguiente token
                index++;
            }

            // Evalúa cualquier operador lógico restante en la pila
            while (operadoresLogicos.Count > 0)
            {
                EvaluarOperadorLogico(valores, operadoresLogicos.Pop());
            }

            // El resultado final debe estar en la cima de la pila de valores
            if (valores.Count == 1)
            {
                resultado = valores.Pop();
            }
            else
            {
                // Manejar error de evaluación
                Console.WriteLine("Error de sintaxis: Expresión lógica inválida");
            }

            return resultado;
        }
        //2.1 Verifica si es una operación aritmetica 
        private bool EsOperacionAritmetica(Token token)
        {
            // Comprobar si el token es un operador aritmético (+, -, *, /, etc.)
            string[] operadoresAritmeticos = { "+", "-", "*", "/" };
            return token.Type == "OPERATOR" && operadoresAritmeticos.Contains(token.Value);
        }

        private bool EsOperacionLogica(Token token)
        {
            // Comprobar si el token es un operador lógico (&&, ||, etc.)
            string[] operadoresLogicos = { "&&", "||" };
            return token.Type == "OPERATOR" && operadoresLogicos.Contains(token.Value);
        }
        //1.1 Es palabra Clave, de que tipo ?
        private bool EsPalabraClaveDeTipo(List<Token> lineTokens, int index)
        {
            // Verificar si el token actual es una palabra clave de tipo (int, float, bool, string)
            return index < lineTokens.Count && lineTokens[index].Type == "KEYWORD" &&
                   (lineTokens[index].Value == "int" || lineTokens[index].Value == "float" ||
                    lineTokens[index].Value == "bool" || lineTokens[index].Value == "string");
        }
        //1.2.1 Analizar si es literal númerico o no. Tambien usado por: 2.2
        private bool EsLiteralNumerico(Token token)
        {
            return token.Type == "NUMERIC_LITERAL";
        }

        //1.2.2 Analizar si posee un analizador Aritmetico.
        private bool EsOperadorAritmetico(Token token)
        {
            return token.Type == "OPERATOR" && "+-*///".Contains(token.Value);
/*
        }
    
        //1.2.3 Analizar la precedencia del operador.
        private int PrecedenciaOperador(char operador)
        {
            switch (operador)
            {
                case '+':
                case '-':
                    return 1;
                case '*':
                case '/':
                    return 2;
                default:
                    return 0; // Precedencia mínima para otros casos
            }
        }

        private int PrecedenciaOperadorLogico(string operador)
        {
            switch (operador)
            {
                case "&&":
                    return 1;
                case "||":
                    return 2;
                default:
                    return 0; // Precedencia mínima para otros casos
            }
        }

        private bool EsLiteralBooleano(Token token)
        {
            // Comprobar si el token es un literal booleano (true o false)
            return token.Type == "BOOLEAN_LITERAL";
        }

        private void EvaluarOperadorLogico(Stack<bool> valores, string operador)
        {
            if (valores.Count >= 2)
            {
                bool segundoOperando = valores.Pop();
                bool primerOperando = valores.Pop();

                switch (operador)
                {
                    case "&&":
                        valores.Push(primerOperando && segundoOperando);
                        break;
                    case "||":
                        valores.Push(primerOperando || segundoOperando);
                        break;
                    default:
                        // Manejar error de operador lógico no reconocido
                        Console.WriteLine($"Error: Operador lógico no reconocido '{operador}'");
                        break;
                }
            }
            else
            {
                // Manejar error de falta de operandos
                Console.WriteLine("Error: Falta de operandos para el operador lógico en la expresión lógica");
            }
        }



        private void EvaluarOperador(Stack<double> valores, char operador)
        {
            if (valores.Count >= 2)
            {
                double segundoOperando = valores.Pop();
                double primerOperando = valores.Pop();

                switch (operador)
                {
                    case '+':
                        valores.Push(primerOperando + segundoOperando);
                        break;
                    case '-':
                        valores.Push(primerOperando - segundoOperando);
                        break;
                    case '*':
                        valores.Push(primerOperando * segundoOperando);
                        break;
                    case '/':
                        if (segundoOperando != 0)
                        {
                            valores.Push(primerOperando / segundoOperando);
                        }
                        else
                        {
                            // Manejar error de división por cero
                            Console.WriteLine("Error: División por cero en la expresión");
                        }
                        break;
                }
            }
            else
            {
                // Manejar error de falta de operandos
                Console.WriteLine("Error: Falta de operandos para el operador en la expresión");
            }
        }


    }
}
*/