using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace compilador.Controllers
{
    /*POO - Estamos creando un objeto de tipo Token que nos permita leer 
     * el objeto Token, que tiene el tipo y el valor, el tipo es el componente
     * que lo define como operador, identificador, etc..., y el valor es el valor real que 
     * tendría para el analizador. 
     */
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public Token(string type, string value) 
        {
            Type = type;
            Value = value;
            
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class Analizador_Lexico : ControllerBase
    {

        private readonly List<Token> tokens; //Se crea una lista de Tokens.
        private int position;//Se instancia la posición.
        private string codigo; 

        public Analizador_Lexico()
        {
            this.codigo = "";
            this.tokens = new List<Token>();
            this.position = 0;
        }

        private void AddToken(string type, string value)
        {
            tokens.Add(new Token(type, value)); 
        }
        /*
        [HttpPost]
        [Route("Tokenize")]
        public IActionResult AnalizarLexico([FromBody] string codigo)
        { 
            try
            {
                Console.WriteLine(codigo);
                // Lógica para el análisis léxico (usando la lógica del código anterior)
                List<Token> lexicoResults = Tokenize(codigo);

                return Ok(lexicoResults);
            }
            catch (Exception ex)
            {
                // Loguea la excepción para obtener más detalles
                Console.Error.WriteLine($"Error en el análisis léxico: {ex}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        */

        [HttpPost]
        [Route("Tokenize")]
        public IActionResult AnalizarLexico([FromBody] string codigo)
        {
            try
            {
                Console.WriteLine(codigo);
                // Dividir el código por saltos de línea
                string[] lines = codigo.Split('\n');

                // Lógica para el análisis léxico por línea
                List<List<Token>> lexicoResults = new List<List<Token>>();
                for (int i = 0; i < lines.Length; i++)
                {
                    // Verificar si la línea no está en blanco ni compuesta solo por espacios
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        List<Token> lineTokens = Tokenize(lines[i].Trim());
                        lexicoResults.Add(lineTokens); // Agregar la lista de tokens de la línea a la lista general
                    }
                }

                return Ok(lexicoResults);
            }
            catch (Exception ex)
            {
                // Loguea la excepción para obtener más detalles
                Console.Error.WriteLine($"Error en el análisis léxico: {ex}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        /*
        public List<Token> Tokenize(string line)

        //public List<Token> Tokenize(string codigo)
        {
            int position = 0;

            while (position < line.Length) 


            //while (position < codigo.Length) 
            {
                //char currentChar = codigo[position];
                char currentChar = line[position];


                if (char.IsWhiteSpace(currentChar))
                {
                    position++;
                    continue;
                }

                // Identificar palabras clave
                if (Regex.IsMatch(codigo.Substring(position), @"\b(int|float|bool|string)\b"))
                {
                    //Match match = Regex.Match(codigo.Substring(position), @"\b(int|float|bool|string)\b");
                    Match match = Regex.Match(line.Substring(position), @"\b(int|float|bool|string)\b");

                    AddToken(match.Value, match.Value);
                    position += match.Length;
                    continue;
                }


                // Identificar identificadores
                if (Regex.IsMatch(currentChar.ToString(), @"[a-zA-Z]"))
                {
                    //Match match = Regex.Match(codigo.Substring(position), @"\b(int|float|bool|string)\b|[a-zA-Z][a-zA-Z0-9_]*");
                    Match match = Regex.Match(line.Substring(position), @"\b(int|float|bool|string)\b|[a-zA-Z][a-zA-Z0-9_]*");

                    AddToken("IDENTIFIER", match.Value);
                    position += match.Length;
                    continue;
                }


                // Identificar literales de cadena
                if (currentChar == '\"')
                {
                    //Match match = Regex.Match(codigo.Substring(position + 1), @"[^""]*");
                    Match match = Regex.Match(line.Substring(position + 1), @"[^""]*");
                    AddToken("STRING_LITERAL", match.Value);
                    position += match.Length + 2; // Moverse al siguiente caracter después de las comillas de cierre
                    continue;
                }

                // Identificar punto y coma
                if (currentChar == ';')
                {
                    AddToken("SEMICOLON", currentChar.ToString());
                    position++;
                    continue;
                }

                // Identificar operadores
                if (Regex.IsMatch(currentChar.ToString(), @"[=+\-*\/]"))
                {
                    AddToken("OPERATOR", currentChar.ToString());
                    position++;
                    continue;
                }

                // Identificar literales numéricos
                if (Regex.IsMatch(currentChar.ToString(), @"\d"))
                {
                    //Match match = Regex.Match(codigo.Substring(position), @"\d+(\.\d+)?");

                    Match match = Regex.Match(line.Substring(position), @"\d+(\.\d+)?");
                    AddToken("NUMERIC_LITERAL", match.Value);
                    position += match.Length;
                    continue;
                }

                Console.WriteLine($"Error: Carácter inesperado '{currentChar}' en la posición {position}");
                position++;
            }
            LogTokens(tokens);
            return tokens;
        }
        */

        public List<Token> Tokenize(string line)
        {
            List<Token> lineTokens = new List<Token>();
            int position = 0;

            while (position < line.Length)
            {
                char currentChar = line[position];

                if (char.IsWhiteSpace(currentChar))
                {
                    position++;
                    continue;
                }

                // Identificar palabras clave
                if (Regex.IsMatch(line.Substring(position), @"\b(int|float|bool|string|char)\b"))
                {
                    Match match = Regex.Match(line.Substring(position), @"\b(int|float|bool|string|char)\b");
                    lineTokens.Add(new Token("KEYWORD", match.Value));
                    position += match.Length;
                    continue;
                }

                // Identificar identificadores
                if (Regex.IsMatch(line.Substring(position), @"[a-zA-Z]"))
                {
                    Match match = Regex.Match(line.Substring(position), @"[a-zA-Z][a-zA-Z0-9_]*");

                    // Verificar que no sea una palabra clave
                    if (!Regex.IsMatch(match.Value, @"\b(int|float|bool|string|char)\b"))
                    {
                        lineTokens.Add(new Token("IDENTIFIER", match.Value));
                    }

                    position += match.Length;
                    continue;
                }


                // Identificar literales de cadena
                if (currentChar == '\"')
                {
                    Match match = Regex.Match(line.Substring(position + 1), @"[^""]*");
                    lineTokens.Add(new Token("STRING_LITERAL", match.Value));
                    position += match.Length + 2; // Moverse al siguiente caracter después de las comillas de cierre
                    continue;
                }

                // Identificar punto y coma
                if (currentChar == ';')
                {
                    lineTokens.Add(new Token("SEMICOLON", currentChar.ToString()));
                    position++;
                    continue;
                }

                // Identificar operadores
                if (Regex.IsMatch(currentChar.ToString(), @"[=+\-*\/]"))
                {
                    lineTokens.Add(new Token("OPERATOR", currentChar.ToString()));
                    position++;
                    continue;
                }

                // Identificar literales numéricos
                if (Regex.IsMatch(currentChar.ToString(), @"\d"))
                {
                    Match match = Regex.Match(line.Substring(position), @"\d+(\.\d+)?");
                    lineTokens.Add(new Token("NUMERIC_LITERAL", match.Value));
                    position += match.Length;
                    continue;
                }

                // Verificar si hay un carácter inesperado y retornar el mensaje de error
                string errorMessage = $"Error: Carácter inesperado '{currentChar}' en la posición {position}";
                lineTokens.Add(new Token("ERROR", errorMessage));
                Console.WriteLine(errorMessage);
                return lineTokens; 
                
                //position++;
            }
            LogTokens(lineTokens);
            return lineTokens;
        }


        public static class TokenUtility
        {
            public static List<Token> GetTokens(List<Token> lineTokens)
            {
                return lineTokens;
            }
        }

        private void AddToken(List<Token> tokens, string type, string value)
        {
            tokens.Add(new Token(type, value));
        }

        private void LogTokens(List<Token> tokens)
        {
            
            foreach (var token in tokens)
            {
                Console.WriteLine($"Tipo: {token.Type}, Valor: {token.Value}");
            }
        }
    }
}
