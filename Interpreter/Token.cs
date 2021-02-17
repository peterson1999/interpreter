using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Token
    {
        private TokenType type;
        private String lexeme;
        private Object literal;
        private int line;

        public Token(TokenType type, String lexeme, Object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public TokenType Type{
            get
            {
                return type;
            }

        }

        public String Lexeme
        {
            get
            {
                return lexeme;
            }
        }

        public Object Literal
        {
            get
            {
                return literal;
            }
        }
        
        public int Line
        {
            get
            {
                return line;
            }
        }

    }
}
