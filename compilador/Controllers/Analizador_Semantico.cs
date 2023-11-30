using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static compilador.Controllers.Analizador_Sintactico;


namespace compilador.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class Analizador_Semantico : ControllerBase

    {

        public class Report_Message_Semantico
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public Report_Message_Semantico(string type, string value)
            {
                Type = type;
                Value = value;

            }
        }

        public class ReportMessage_Semantico_Service
        {
            private static List<Report_Message> semanticReportMessages = new List<Report_Message>();

            public static void AddSemanticReportMessage(Report_Message message)
            {
                semanticReportMessages.Add(message);
            }

            public static List<Report_Message> GetSemanticReportMessages()
            {
                return semanticReportMessages;
            }
            public static void ClearSemanticReportMessages()
            {
                semanticReportMessages.Clear();
            }

            public static void PrintSemanticReportMessages()
            {

                foreach (var message in semanticReportMessages)
                {
                    Console.WriteLine($"Semantic - Type: {message.Type}, Value: {message.Value}");
                }
            }

        }


        // Este diccionario almacena información sobre las variables declaradas
        private Dictionary<string, Type> variableTable;

        public Analizador_Semantico()
        {
            variableTable = new Dictionary<string, Type>();
        }

        public void AnalizarSemantica(List<Token> tokens)
        {
            foreach (var token in tokens)
            {
                if (token.Type == "KEYWORD" && token.Value == "int")
                {
                    // Identificar una declaración de variable entera
                    ProcessVariableDeclaration(tokens, Type.Int);
                }
                else if (token.Type == "KEYWORD" && token.Value == "float")
                {
                    // Identificar una declaración de variable flotante
                    ProcessVariableDeclaration(tokens, Type.Float);
                }
                else if (token.Type == "KEYWORD" && token.Value == "bool")
                {
                    // Identificar una declaración de variable booleana
                    ProcessVariableDeclaration(tokens, Type.Bool);
                }
                else if (token.Type == "KEYWORD" && token.Value == "string")
                {
                    // Identificar una declaración de variable de cadena
                    ProcessVariableDeclaration(tokens, Type.String);
                }
                else if (token.Type == "IDENTIFIER")
                {
                    // Identificador encontrado, verificar si se ha declarado previamente
                    CheckVariableDeclaration(token.Value);
                }
                else if (token.Type == "OPERATOR" && (token.Value == "=" || token.Value == "+"))
                {
                    // Operador de asignación o suma encontrado
                    ProcessAssignmentOrSum(tokens);
                }
                else if (token.Type == "KEYWORD" && token.Value == "if")
                {
                    // Inicio de una estructura de control "if"
                    ProcessIfStatement(tokens);
                }
                else if (token.Type == "KEYWORD" && token.Value == "while")
                {
                    // Inicio de un bucle "while"
                    ProcessWhileLoop(tokens);
                }
                else if (token.Type == "KEYWORD" && token.Value == "return")
                {
                    // Palabra clave "return" encontrada
                    ProcessReturnStatement(tokens);
                }
                // Puedes agregar más casos según las características del lenguaje MiniLang
            }
        }

        private void ProcessVariableDeclaration(List<Token> tokens, Type variableType)
        {
            // Esperamos un identificador después de la palabra clave de tipo
            int currentIndex = tokens.FindIndex(t => t.Type == "KEYWORD" && t.Value == variableType.ToString().ToLower()) + 1;

            if (currentIndex < tokens.Count && tokens[currentIndex].Type == "IDENTIFIER")
            {
                // Verificar si la variable ya ha sido declarada
                string variableName = tokens[currentIndex].Value;
                if (variableTable.ContainsKey(variableName))
                {
                    ReportMessage_Semantico_Service.AddSemanticReportMessage(new Report_Message("Error semántico: ", $" Variable '{variableName}' ya ha sido declarada."));
                    Console.WriteLine($"Error semántico: Variable '{variableName}' ya ha sido declarada.");
                }
                else
                {
                    // Agregar la variable a la tabla de variables
                    variableTable.Add(variableName, variableType);
                }
            }
            else
            {
                ReportMessage_Semantico_Service.AddSemanticReportMessage(new Report_Message("Error semántico: ", $" Se esperaba un identificador después de la palabra clave '{variableType}'."));
                Console.WriteLine($"Error semántico: Se esperaba un identificador después de la palabra clave '{variableType}'.");
            }
        }

        private void CheckVariableDeclaration(string variableName)
        {
            // Verificar si la variable ha sido declarada previamente
            if (!variableTable.ContainsKey(variableName))
            {
                ReportMessage_Semantico_Service.AddSemanticReportMessage(new Report_Message("Error semántico: ", $"Variable '{variableName}' no ha sido declarada."));
                Console.WriteLine($"Error semántico: Variable '{variableName}' no ha sido declarada.");
            }
        }

        private void ProcessAssignmentOrSum(List<Token> tokens)
        {
            // Implementa la lógica para verificar y analizar asignaciones o sumas
            // Puedes adaptar esto según las reglas semánticas de MiniLang
        }

        private void ProcessIfStatement(List<Token> tokens)
        {
            // Implementa la lógica para verificar y analizar estructuras "if"
            // Puedes adaptar esto según las reglas semánticas de MiniLang
        }

        private void ProcessWhileLoop(List<Token> tokens)
        {
            // Implementa la lógica para verificar y analizar bucles "while"
            // Puedes adaptar esto según las reglas semánticas de MiniLang
        }

        private void ProcessReturnStatement(List<Token> tokens)
        {
            // Implementa la lógica para verificar y analizar instrucciones "return"
            // Puedes adaptar esto según las reglas semánticas de MiniLang
        }


        [HttpGet]
        [Route("ObtenerReporte_Semantico")]
        public IActionResult ObtenerReporte_Semantico()
        {
            var reportMessages = ReportMessage_Semantico_Service.GetSemanticReportMessages();

            return Ok(reportMessages);
        }
    }

    

    // Enumeración para representar los tipos de datos en MiniLang
    public enum Type
    {
        Int,
        Float,
        Bool,
        String
    }

    

}

