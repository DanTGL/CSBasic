using System;
using System.Text.RegularExpressions;

namespace CSBasic.Tokenization {
    public class TokenType {
        String pattern;

        public TokenType(String pattern) {
            this.pattern = pattern;
        }

        public Match Match(String input) {
            return Regex.Match(input, pattern);
        }

        public String GetPattern() {
            return pattern;
        }
    }
}