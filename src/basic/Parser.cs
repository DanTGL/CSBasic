using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using CSBasic.Tokenization;

namespace CSBasic {
    public class Parser {

        public Queue<Token> ParseLine(String line) {
            return Tokenizer.Tokenize(line);
        }

        

        public static int ParseExpression(Queue<Token> tokens) {
            return int.Parse(tokens.Dequeue().GetValue());
        }

    }
}