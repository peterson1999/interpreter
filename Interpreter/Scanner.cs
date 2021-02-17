using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Scanner
    {
        private readonly string[] source;
        private readonly List<Token> tokens;
        private int count;
        private int char_counter;
        private int line;

        public Scanner(string sourceCode)
        {
            tokens = new List<Token>();
            source = sourceCode.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            count = 0;
            line = 1;
            char_counter = 0;
        }
        public void Process()
        {
            int i = 0;
            while (i < source.Length)
            {
                ProccessLine();
                i++;
                line++;
            }
        }

        public void NextChar()
        {

        }

        public void ProccessLine()
        {
            int i = 0;
            string a = source[count];
            for(;i<a.Length;i++)
            {
                switch (a[i])
                {
                    case '+':
                        break;
                    case '-':
                        break;
                    case '/':
                        break;
                    case '*':
                        break;
                    case '(':
                        break;
                    case ')':
                        break;
                    case '>':
                        break;
                    case '<':
                        break;
                    case '=':
                        break;
                    case ' ':
                        break;
                    case ',':
                        break;

                    default:
                        if (isDigit(a[i])){

                            isValue(a[i]);
                            break;
                        }
                        else if (isAlpha(a[i]))
                        {
                            isIdentifier(a[i]);
                            break;
                        }
                        break;
                }
            }
        }

        private bool isValue(char a)    //checks whether a digit is a float or integer
        {

            return true;
        }

        private bool isIdentifier(char a) //checks whether a string of alphanumeric characters a variable name of keyword
        {

            return true;
        }

        private bool isDigit(char a)
        {
            return a >= '0' && a <= '9';
        }

        private bool isAlpha(char a)
        {
            return (a >= 'a' && a <= 'z') || (a >= 'A' && a <= 'Z') || a=='_';
        }

        //Used only to check if the string has been properly split
        public string[] source
        {
            get
            {
                return source;
            }    
        }
    }
}
