using System;

namespace CSBasic.Tokenization {
    public class Token {
        TokenType tokenType;
        String value;
        
        public Token(TokenType tokenType, String value) {
            this.tokenType = tokenType;
            this.value = value;
        }

        public override string ToString() {
            return value;
        }

        public TokenType GetTokenType() {
            return tokenType;
        }

        public String GetValue() {
            return value;
        }

    }
}