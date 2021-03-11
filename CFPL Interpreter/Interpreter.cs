using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace CFPL_Interpreter
{
    class Interpreter
    {
        private List<Tokens> tokens;
        private static bool hasStop;
        private static bool hasStart;
        private static int tCounter;
        private Dictionary<string, object> map;
        private static int error;

        public Interpreter(List<Tokens> t)
        {
            tokens = new List<Tokens>(t);
            map = new Dictionary<string, object>();
            tCounter = error  =0;
            hasStart = hasStop = false;
        }
        public int Parse()
        {
            List<string> varList = new List<string>();
            object temp="";
            int intTemp;
            float floatTemp;
            string temp_identifier = "";
            int unary = 0;
            TokenType lasttemp;
            Console.WriteLine(tokens.Count);
           while(tCounter < tokens.Count)
            {
                switch (tokens[tCounter].Type)
                {
                    case TokenType.VAR:
                        if (!hasStart)
                        {
                            tCounter++;
                            if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                            {
                                varList.Add(tokens[tCounter].Lexeme);
                                tCounter++;
                                while (tokens[tCounter].Type == TokenType.COMMA)
                                {
                                    tCounter++;
                                    if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                    {
                                        varList.Add(tokens[tCounter].Lexeme);
                                        tCounter++;
                                    }
                                }
                                if (tokens[tCounter].Type == TokenType.IDENTIFIER)
                                    error = -2; //Invalid variable declaration e.g VAR a b 
                            }
                            else
                            {
                                error = -3; //Invalid variable declaration token after VAR is not an Identifier.
                            }
                        }
                        else
                        {
                            error = -4; //variable declaration after start
                        }
                        break;
                    case TokenType.INT_LIT:
                        if (unary != 0)
                        {
                            
                            temp = ((int)tokens[tCounter].Literal)*unary;
                            Console.WriteLine(temp);
                            unary = 0;
                        }
                        else
                        {
                            temp = ((int)tokens[tCounter].Literal);
                        }
                        tCounter++;
                        break;
                    case TokenType.ADD:
                        //if prev token is int,float, or identifier, '+' is an arithemtic operator (left blank for now)
                        if (tCounter>0 && (tokens[tCounter-1].Type == TokenType.IDENTIFIER || tokens[tCounter - 1].Type == TokenType.INT_LIT || tokens[tCounter - 1].Type == TokenType.FLOAT_LIT))
                        {
                            tCounter++;
                            break;
                        }
                        
                        else
                        {
                            //if next token is int,float, or identifier, '+' is a unary operator (activates flag)
                            if (tokens[tCounter + 1].Type == TokenType.INT_LIT|| tokens[tCounter + 1].Type == TokenType.FLOAT_LIT || tokens[tCounter + 1].Type == TokenType.IDENTIFIER)
                            {
                                unary = 1;
                            }
                            tCounter++;
                            break;
                        }
                    case TokenType.SUBT:
                        //if prev token is int,float, or identifier, '+' is an arithemtic operator (left blank for now)
                        if (tCounter > 0 && (tokens[tCounter - 1].Type == TokenType.IDENTIFIER || tokens[tCounter - 1].Type == TokenType.INT_LIT || tokens[tCounter - 1].Type == TokenType.FLOAT_LIT))
                        {
                            tCounter++;
                            break;
                        }

                        else
                        {
                            
                            //if next token is int,float, or identifier, '+' is a unary operator (activates flag)
                            if (tokens[tCounter + 1].Type == TokenType.INT_LIT || tokens[tCounter + 1].Type == TokenType.FLOAT_LIT || tokens[tCounter + 1].Type == TokenType.IDENTIFIER)
                            {
                                
                                unary = -1;
                            }
                            tCounter++;
                            break;
                        }

                    case TokenType.FLOAT_LIT:
                        if (unary != 0)
                        {
                            temp = ((float)tokens[tCounter].Literal) * unary;
                            unary = 0;
                        }
                        else
                        {
                            temp = ((float)tokens[tCounter].Literal);
                        }
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
                        temp_identifier = tokens[tCounter++].Lexeme;
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
                            }
                            else
                            {
                                error = 21; //Identifier does not exist;
                                Console.WriteLine("indentifier does not exist");
                                //error statement
                                //identifier does not exist meaning variable not initialized
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
                        tCounter++;
                        if (tokens[tCounter].Type == TokenType.COLON)
                        {
                            tCounter++;
                            temp_identifier = tokens[tCounter].Lexeme;
                            if (map.ContainsKey(temp_identifier))
                            {
                                string s = Console.ReadLine();
                                Console.WriteLine(s);
                                //add here scanf method pls HAHA
                            }
                            else
                            {
                                error = -6; //Variable not initialized
                            }
                        }
                        break;
                    default:
                        break;
                }
                temp_identifier = "";
                if (error != 0) break;
            }
            if (error != 0)
            {
                return error;
            }
            else if (hasStart == true && hasStop == true)
            {
                return 0;
            }
            else if (hasStart == false)
            {
                return 13;
            }
            else if (hasStart == true && hasStop == false)
            {
                return 14;
            }
            else if (hasStart == false && hasStop == false)
            {
                return -13;
            }
            else
                return 0; //no problem
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
