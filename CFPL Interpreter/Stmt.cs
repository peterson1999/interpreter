using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFPL_Interpreter
{
    abstract class Statement
    {
        //List<Statement> statements;
        interface Visitor<R>
        {
            R visitBlockStatement(Block stmt);
            //R visitClassStatement(Class stmt);
            R visitExpressionStatement(Expression stmt);
            //R visitFunctionStatement(Function stmt);
            R visitIfStatement(If stmt);
            R visitOutputStatement(Output stmt);
            //R visitReturnStatement(Return stmt);
            R visitVarStatement(Var stmt);
            R visitWhileStatement(While stmt);
        }

        class Block : Statement
        {
            Block( List<Statement> statements) {
              this.statements = statements;
            }

            R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitBlockStatement(this);
            }

            List<Statement> statements;
        }

        class If : Statement
        {
            If(Expr condition, Statement thenBranch, Statement elseBranch) {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitIfStatement(this);
            }

             Expr condition;
             Statement thenBranch;
             Statement elseBranch;
         }

        class Var : Statement
        {
            Var(Tokens name, Expr initializer) {
              this.name = name;
              this.initializer = initializer;
            }

            R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitVarStatement(this);
            }

            Tokens name;
            Expr initializer;
        }

        class Output : Statement
        {
            Output(Expr expression) {
              this.expression = expression;
            }

            R accept<R>(Visitor<R> visitor)
            {
                return visitor.visitOutputStatement(this);
            }

            Expr expression;
        }

        class Expression : Statement
        {
            Expression(Expr expression) {
                this.expression = expression;
            }

            R accept<R>(Visitor<R> visitor) {
                return visitor.visitExpressionStatement(this);
            }

            Expr expression;
        }

        class While : Statement
        {
            While(Expr condition, Statement body) {
            this.condition = condition;
            this.body = body;
        }

        R accept<R>(Visitor<R> visitor)
        {
            return visitor.visitWhileStatement(this);
        }

        Expr condition;
        Statement body;
        }
    }
}
