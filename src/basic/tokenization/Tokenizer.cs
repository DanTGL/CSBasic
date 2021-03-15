using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CSBasic.Tokenization {

    public class Tokenizer {
        
        public static readonly TokenType RELOP = new TokenType("^(<(>|=|ε)|>(=|ε)|=|<|>)");
        public static readonly TokenType NUMBER = new TokenType("^([0-9]+)");
        public static readonly TokenType VAR = new TokenType("^[A-Z]");
        public static readonly TokenType BINOP = new TokenType(@"^(\+|-|\*|/|ε)");
        public static readonly TokenType L_PAREN = new TokenType(@"^\(");
        public static readonly TokenType R_PAREN = new TokenType(@"^\)");
        public static readonly TokenType FUNC = new TokenType(@"^[A-Z][A-Z]+\(");
        public static readonly TokenType CMD = new TokenType("^[A-Z][A-Z]+");
        
        public static readonly TokenType[] tokenTypes = {
            L_PAREN,
            R_PAREN,
            FUNC,
            CMD,
            VAR,
            NUMBER,
            RELOP,
            BINOP
        };

        public static Match Match(String input, TokenType tokenType) {
            return tokenType.Match(input);
        }

        public static Queue<Token> Tokenize(String input) {
            Queue<Token> tokens = new Queue<Token>();
            while (input != "") {
                bool matched = false;
                input = input.TrimStart(' ');
                for (int i = 0; i < tokenTypes.Length; i++) {
                    Match match = Regex.Match(input, tokenTypes[i].GetPattern());

                    if (match.Success) {
                        tokens.Enqueue(new Token(tokenTypes[i], match.Value));
                        input = input.Substring(match.Length);
                        matched = true;
                        break;
                    }
                }

                if (!matched) {
                    Debug.WriteLine("Invalid syntax: " + input);
                    break;
                }
            }

            return tokens;
        }

    }

}
