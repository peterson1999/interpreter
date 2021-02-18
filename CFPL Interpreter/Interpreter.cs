using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    class Interpreter
    {
        private List<Token> tokens;
        private static bool hasStop;
        private static bool hasStart;
        private static int tCounter;
        private static bool varAfterStart;
        private Dictionary<string, object> map;

        public Interpreter(List<Token> t)
        {
            tokens = new List<Token>(t);
            map = new Dictionary<string, object>();
            tCounter = 0;
            hasStart = hasStop = varAfterStart=  false;
        }
        public int Parse()
        {
            List<string> varList = new List<string>();
            object temp;
            Console.WriteLine(tokens.Count);
           while(tCounter < tokens.Count && !varAfterStart)
            {
                switch (tokens[tCounter].Type)
                {
                    case TokenType.VAR:
                        if (!hasStart)
                        {
                            tCounter++;
                            while (tokens[tCounter].Type == TokenType.IDENTIFIER || tokens[tCounter].Type == TokenType.COMMA)
                            {
                                if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                {
                                    varList.Add(tokens[tCounter].Lexeme);
                                    tCounter++;
                                }
                                else if (tokens[tCounter].Type == TokenType.COMMA)
                                {
                                    tCounter++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    case TokenType.INT_LIT:
                        temp = (int)tokens[tCounter].Literal;
                        tCounter++;
                        break;
                    case TokenType.FLOAT_LIT:
                        temp = (float)tokens[tCounter].Literal;
                        tCounter++;
                        break;
                    case TokenType.AS:
                        tCounter++;
                        if (tokens[tCounter].Type == TokenType.INT)
                        {
                            foreach(string a in varList)
                            {
                                map.Add(a, 0);
                            }
                            tCounter++;
                            varList.Clear();  //varList is a list of variable declaration in one line of code;
                                              //so after adding them to the hashmap we clear the list to read another line of variable declaration
                        }
                        else if (tokens[tCounter].Type == TokenType.FLOAT)
                        {
                            foreach (var a in varList)
                            {
                                map.Add(a, 0.0);
                            }
                            tCounter++;
                            varList.Clear();  //varList is a list of variable declaration in one line of code;
                                              //so after adding them to the hashmap we clear the list to read another line of variable declaration
                        }
                        else
                        {
                            //error statement
                        }
                        break;
                    case TokenType.IDENTIFIER:
                        string temp_identifier = tokens[tCounter++].Lexeme;
                        if (tokens[tCounter].Type == TokenType.EQUALS)
                        {
                            tCounter++;
                            if (map.ContainsKey(temp_identifier))
                            {
                                if (tokens[tCounter].Type == TokenType.INT_LIT)
                                {
                                    temp = (int)tokens[tCounter].Literal;
                                    map[temp_identifier] = temp;
                                }
                                else if (tokens[tCounter].Type == TokenType.FLOAT_LIT)
                                {
                                    temp = (float)tokens[tCounter].Literal;
                                    map[temp_identifier] = temp;
                                }
                                else
                                {
                                    //error statement
                                }
                            }
                            else
                            {
                                Console.WriteLine("indentifier does not exist");
                                //error statement
                                //identifier does not exist in hashmap
                            }
                            
                        }
                        else
                        {
                            //next token is not = meaning unused identifier
                        }
                        //error statement (token is not =, INT_LIT, nor FLOAT_LIT)
                        break;
                    case TokenType.START:
                        hasStart = true;
                        tCounter++;
                        break;
                    case TokenType.STOP:
                        hasStop = true;
                        tCounter++;
                        break;
                    case TokenType.INPUT:

                        break;
                    default:
                        break;
                }
            }
            if (hasStart == true && hasStop == true && varAfterStart ==false)
            {
                return 0;
            }
            else if (hasStart == false)
            {
                return 13; //just error handling
            }
            else if (hasStart == true && hasStop == false)
            {
                return 14;
            }
            else if (hasStart == false && hasStop == false)
            {
                return -13;
            }
            else if (varAfterStart == true)
            {
                return -5;
            }
            else
                return 1;
        }
        public void NextToken()
        {
            tCounter++;
        }
        public void Error()
        {

        }
        public Dictionary<string, object> Map
        {
            get
            {
                return map;
            }
        }
    }
}
