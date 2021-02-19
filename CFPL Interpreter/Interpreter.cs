using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

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

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public Interpreter(List<Tokens> t)
        {
            tokens = new List<Tokens>(t);
            map = new Dictionary<string, object>();
            tCounter = error  =0;
            hasStart = hasStop = false;
            AllocConsole();
        }
        public int Parse()
        {
            List<string> varList = new List<string>();
            object temp;
            string temp_identifier = "";
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
                                //error statement
                                //identifier does not exist meaning variable not initialized
                            }
                            
                        }
                        else
                        {
                            error = -2;//next token is not = meaning unused identifier
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
                            int notIden=1;
                            tCounter++;
                            while (tokens[tCounter].Type == TokenType.IDENTIFIER)
                            {
                                notIden = 0;
                                temp_identifier = tokens[tCounter].Lexeme;
                                if (map.ContainsKey(temp_identifier))
                                {
                                    string s = Console.ReadLine();
                                    Type t = map[temp_identifier].GetType();
                                    if (t == typeof(Int32))
                                    {
                                        temp = Convert.ToInt32(s);
                                        map[temp_identifier] = temp;
                                    }
                                    else if (t == typeof(float))
                                    {
                                        temp = Convert.ToSingle(s);
                                        map[temp_identifier] = temp;
                                    }
                                }
                                else
                                {
                                    error = -6; //Variable not initialized
                                    break;
                                }
                                tCounter++;
                                if (tokens[tCounter].Type == TokenType.COMMA)
                                {
                                    tCounter++;
                                    continue;
                                }
                                else if(tokens[tCounter].Type==TokenType.IDENTIFIER)
                                {
                                    error = -8; //Syntax error put comma after a variable to have more than 1 INPUTS
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (notIden == 1)
                            {
                                error = -7;
                            }
                        }
                        break;
                    default:
                        break;
                }
                temp_identifier = "";
                temp = null;
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
