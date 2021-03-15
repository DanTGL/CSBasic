using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CSBasic.Tokenization;

namespace CSBasic {
    public class Interpreter {

        public SortedList<int, String> lines;
        public int curLine = 0;

        private Parser parser;

        public Dictionary<String, int> variables;

        public Random random;

        public Interpreter() {
            lines = new SortedList<int, string>();
            variables = new Dictionary<string, int>();
            parser = new Parser();
            random = new Random();
        }

        public void LoadCode(String[] code) {
            lines.Clear();
            ExtractLabels(code);
        }

        public void LoadFile(String filename) {
            lines.Clear();
            String[] code = File.ReadAllLines(filename);
            ExtractLabels(code);
        }

        public static int GetPrecedence(String op) {
            switch (op) {
                case "/":
                case "*":
                    return 1;

                case "+":
                case "-":
                    return 0;
            }

            return -1;
        }

        public int EvaluateFunction(Token func, Queue<Token> tokens) {
            int result = 0;
            switch (func.GetValue()) {
                case "RND(":
                    int arg = EvaluateExpression(tokens);
                    result = random.Next(arg);
                    break;
            }

            tokens.Dequeue();
            return result;
        }

        public int EvaluatePrimary(Queue<Token> tokens) {
            Token token = tokens.Dequeue();
            
            if (token.GetTokenType().Equals(Tokenizer.FUNC)) {
                return EvaluateFunction(token, tokens);
            } else if (token.GetTokenType().Equals(Tokenizer.L_PAREN)) {
                int result = EvaluateExpression(tokens);
                tokens.Dequeue();
                return result;
            } else if (token.GetTokenType().Equals(Tokenizer.NUMBER)) {
                return int.Parse(token.GetValue());
            } else if (token.GetTokenType().Equals(Tokenizer.VAR)) {
                return variables[token.GetValue()];
            }

            return 0;
        }

        public int EvaluateExpression(Queue<Token> tokens) {
            return EvaluateExpression1(tokens, EvaluatePrimary(tokens), 0);
        }

        public int EvaluateExpression1(Queue<Token> tokens, int lhs, int min_precedence) {
            if (tokens.Count == 0) return lhs;
            Token lookahead = tokens.Peek();
            while (tokens.Count != 0 && GetPrecedence(lookahead.GetValue()) >= min_precedence) {
                String op = lookahead.GetValue();
                if (tokens.Count == 0) return lhs;
                tokens.Dequeue();
                int rhs = EvaluatePrimary(tokens);

                if (tokens.Count > 0) {
                    lookahead = tokens.Peek();

                    while (GetPrecedence(lookahead.GetValue()) > GetPrecedence(op)) {
                        rhs = EvaluateExpression1(tokens, rhs, min_precedence + 1);

                        if (tokens.Count == 0) break;
                        lookahead = tokens.Peek();
                    }
                }

                switch (op) {
                    case "+":
                        lhs = lhs + rhs;
                        break;

                    case "-":
                        lhs = lhs - rhs;
                        break;

                    case "*":
                        lhs = lhs * rhs;
                        break;

                    case "/":
                        lhs = lhs / rhs;
                        break;
                }
            }

            return lhs;
        }

        void ExtractLabels(String[] code) {
            for (int i = 0; i < code.Length; i++) {
                Debug.WriteLine("Line: " + code[i]);
                String[] splitLine = code[i].Split(new char[] {' '}, 2);
                lines.Add(int.Parse(splitLine[0]), splitLine[1]);
            }
        }

        public void ExecuteStatement(Queue<Token> tokens) {
            //Debug.Log("test: " + tokens.Peek().GetValue());
            switch (tokens.Dequeue().GetValue()) {
                case "PRINT":
                    Console.WriteLine(EvaluateExpression(tokens));
                    break;
                case "IF":
                    int operand1 = EvaluateExpression(tokens);
                    String relop = tokens.Dequeue().GetValue();
                    int operand2 = EvaluateExpression(tokens);

                    bool result = false;
                    switch (relop) {
                        case "<":
                            result = operand1 < operand2;
                            break;
                        case ">":
                            result = operand1 > operand2;
                            break;
                        case "=":
                            result = operand1 == operand2;
                            break;

                    }

                    if (result) {
                        ExecuteStatement(tokens);
                    }

                    break;
                case "GOTO":
                    curLine = lines.IndexOfKey(EvaluateExpression(tokens)) - 1;
                    break;

                case "LET":
                    String variable = tokens.Dequeue().GetValue();
                    Debug.Assert(tokens.Dequeue().GetValue().Equals("="));
                    variables[variable] = EvaluateExpression(tokens);
                    break;
            }
        }

        public void Execute() {
            while (curLine < lines.Count) {
                int key = lines.Keys[curLine];
                String statement = lines.Values[curLine];
                ExecuteStatement(parser.ParseLine(statement));

                curLine++;
            }
        }
    }
}
