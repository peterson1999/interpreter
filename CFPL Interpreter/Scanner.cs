using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFPL_Interpreter
{
    class Scanner
    {
        private readonly string[] source;
        private readonly List<Tokens> tokens;
        private static string currString;
        private static int charArrLength;
        private static int char_counter;
        private static int line;

        public Scanner(string source)
        {
            tokens = new List<Tokens>();
            this.source = source.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            line = 0;
            char_counter = 0;
            currString = "";
        }
        public void Process()
        {
            int i = 0;
            while (i < source.Length)
            {
                ProccessLine();
                i++;
                line++;
                char_counter = 0;
            }
        }

        public char NextChar()
        {
            if (char_counter + 1 < charArrLength)
                return currString[char_counter + 1];
            else
                return '|';
        }

        public void ProccessLine()
        {
            currString = source[line];
            charArrLength = currString.Length;
            while (char_counter < charArrLength)
            {
                char a = currString[char_counter];
                Console.WriteLine(a);
                switch (a)
                {
                    case '+':
                        tokens.Add(new Tokens(TokenType.ADD, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case '-':
                        tokens.Add(new Tokens(TokenType.SUBT, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case '/':
                        tokens.Add(new Tokens(TokenType.DIV, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case '*':
                        tokens.Add(new Tokens(TokenType.MULT, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case '(':
                        tokens.Add(new Tokens(TokenType.LEFT_PAREN, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case ')':
                        tokens.Add(new Tokens(TokenType.RIGHT_PAREN, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case '>':
                        if (NextChar() == '=')
                        {
                            string temp = "" + a + NextChar();
                            tokens.Add(new Tokens(TokenType.GREATER_EQUAL, temp, null, line));
                            char_counter += 2;
                        }
                        else
                        {
                            tokens.Add(new Tokens(TokenType.GREATER, a.ToString(), null, line));
                        }
                        break;
                    case '<':
                        if (NextChar() == '=')
                        {
                            string temp = "" + a + NextChar();
                            tokens.Add(new Tokens(TokenType.LESSER_EQUAL, temp, null, line));
                            char_counter += 2;
                        }
                        else
                        {
                            tokens.Add(new Tokens(TokenType.LESSER, a.ToString(), null, line));
                            char_counter++;
                        }
                        break;
                    case '=':
                        if (NextChar() == '=')
                        {
                            string temp = "" + a + NextChar();
                            tokens.Add(new Tokens(TokenType.EQUAL, temp, null, line));
                            char_counter += 2;
                        }
                        else
                        {
                            tokens.Add(new Tokens(TokenType.EQUALS, a.ToString(), null, line));
                            char_counter++;
                        }
                        break;
                    case ' ':
                        char_counter++;
                        break;
                    case ',':
                        tokens.Add(new Tokens(TokenType.COMMA, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case ':':
                        tokens.Add(new Tokens(TokenType.COLON, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case '%':
                        tokens.Add(new Tokens(TokenType.MOD, a.ToString(), null, line));
                        char_counter++;
                        break;
                    case ';':
                        if (char_counter != charArrLength)
                        {
                            //error statement here
                            char_counter++;
                            break;
                        }
                        char_counter++;
                        break;
                    default:
                        if (isDigit(a))
                        {
                            isValue(a);
                            break;
                        }
                        else if (isAlpha(a))
                        {
                            isIdentifier(a);
                            break;
                        }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                        break;
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
                        else if (isBoolChar(a))
                        {
                            boolCharVal(a);
                            break;
                        }
                        else
                        {
                            errorMsg.Add(string.Format("Encountered unsupported character \"{0}\" at line {1}.\n", a, line + 1));
                            char_counter++;
                            break;
                        }
>>>>>>> Stashed changes
                }
            }
        }

        private void isValue(char a)    //checks whether a digit is a float or integer
        {
            var t = TokenType.INT_LIT;
            string temp = "";
            while (isDigit(a))
            {
                temp += a;
                a = NextChar();
                char_counter++;
            }
            if (a == '.')
            {
                char_counter++;
                t = TokenType.FLOAT_LIT;
                a = NextChar();
                while (isDigit(a))
                {
                    temp += a;
                    a = NextChar();
                    char_counter++;
                }

            }
            if (t == TokenType.INT_LIT)
                tokens.Add(new Tokens(t, temp, Convert.ToInt32(temp), line));
            else
                tokens.Add(new Tokens(t, temp, Convert.ToDouble(temp), line));
        }

        private void boolCharVal (char a)
        {
            int charCount = 0;
            var t = TokenType.BOOL_LIT;
            string temp = "";
            string temp2 = "";
            while (isBoolChar(a))
            {
                if (charCount == 1)
                {
                    temp2 += a;
                }
                temp += a;
                a = NextChar();
                char_counter++;
                charCount++;
            }
            if (charCount-2 == 1)
            {
                t = TokenType.CHAR_LIT;
                tokens.Add(new Tokens(t, temp2, char.Parse(temp2), line));
            }
            else
            {
                if (temp == "\'TRUE\'" || temp == "\' TRUE \'" || temp == "\'TRUE \'" || temp == "\' TRUE\'")
                {
                    tokens.Add(new Tokens(t, temp, Convert.ToString("TRUE"), line));
                }
                else if (temp == "\'FALSE\'" || temp == "\' FALSE \'" || temp == "\'FALSE \'" || temp == " \'FALSE\'")
                {
                    tokens.Add(new Tokens(t, temp, Convert.ToString("FALSE"), line));
                }
                else
                {
                    errorMsg.Add(string.Format("Invalid value at line {0}.", line+1));
                }
            }
            
        }

        private void isIdentifier(char a) //checks whether a string of alphanumeric characters a variable name of keyword
        {
            string temp = "";
            while (isAlpha(a) || isDigit(a))
            {
                temp += a;
                a = NextChar();
                char_counter++;
            }
            switch (temp)
            {
                case "START":
                    tokens.Add(new Tokens(TokenType.START, temp, null, line));
                    break;
                case "STOP":
                    tokens.Add(new Tokens(TokenType.STOP, temp, null, line));
                    break;
                case "INT":
                    tokens.Add(new Tokens(TokenType.INT, temp, null, line));
                    break;
                case "FLOAT":
                    tokens.Add(new Tokens(TokenType.FLOAT, temp, null, line));
                    break;
                case "VAR":
                    tokens.Add(new Tokens(TokenType.VAR, temp, null, line));
                    break;
                case "AS":
                    tokens.Add(new Tokens(TokenType.AS, temp, null, line));
                    break;
                case "CHAR":
<<<<<<< Updated upstream
                    tokens.Add(new Tokens(TokenType.CHAR, temp, null, line));
=======
                    if (tokens[tokens.Count - 1].Type == TokenType.AS)
                        tokens.Add(new Tokens(TokenType.CHAR, temp, null, line));
                    else
                        errorMsg.Add(string.Format("Invalid usage of a reserved word CHAR at line {0}.", line));
                    break;
                case "BOOL":
                    if (tokens[tokens.Count - 1].Type == TokenType.AS)
                        tokens.Add(new Tokens(TokenType.BOOL, temp, null, line));
                    else
                        errorMsg.Add(string.Format("Invalid usage of a reserved word BOOL at line {0}.", line));
                    break;
                case "INPUT":
                    tokens.Add(new Tokens(TokenType.INPUT, temp, null, line));
                    break;
                case "OUTPUT":
                    tokens.Add(new Tokens(TokenType.OUTPUT, temp, null, line));
>>>>>>> Stashed changes
                    break;
                default:
                    tokens.Add(new Tokens(TokenType.IDENTIFIER, temp, null, line));
                    break;
            }
        }

        private bool isDigit(char a)
        {
            return a >= '0' && a <= '9';
        }

        private bool isAlpha(char a)
        {
            return (a >= 'a' && a <= 'z') || (a >= 'A' && a <= 'Z') || a == '_';
        }

        private bool isBoolChar(char a)
        {
            return (a == '\'' || (a >= 'a' && a <= 'z') || (a >= 'A' && a <= 'Z') || a == '_' || (a >= '0' && a <= '9'));
        }

        private bool isBoolean(string a)
        {
            bool x;
            if (a == "true" || a == "True" || a == "TRUE")
                x = true;
            else
                x = false;
            return x;
        }
        //Used only to check if the string has been properly split
        public string[] Source
        {
            get
            {
                return source;
            }
        }
        //Used only to check if token list data are correct
        public List<Tokens> Tokens
        {
            get
            {
                return tokens;
            }
        }
    }
}
