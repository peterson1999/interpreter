using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFPL_Interpreter
{
    class Parser
    {
        private List<Tokens> tokens;
        int tokenPos = 0;

        public Parser(List<Tokens> tokens)
        {
            this.tokens = tokens;
        }

        public void nextSymbol()
        {
            tokenPos++;
        }

        public Boolean accept(TokenType t)
        {
            if (tokens[tokenPos].Type == t)
            {
                nextSymbol();
                return true;
            }
            else return false;
        }

        public Boolean expect(TokenType t)
        {
            if (accept(t)) return true;
            //CFPL.error(Scanner.line, "Unexpected symbol " + tokens.get(tokenPos).getLexeme());
            return false;
        }

        public void varExpression()
        {
            if (expect(TokenType.VAR))
            {
                nextSymbol();
            }
            else
            {

            }
        }

    }
}
