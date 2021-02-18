using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFPL_Interpreter
{
    abstract class Expr
    {
        public Tokens name;
        public Expr value;
        interface Visitor<R>
        {
            R visitAssignExpr(Assign expr);
            R visitBinaryExpr(Binary expr);
            //R visitCallExpr(Call expr);
            //R visitGetExpr(Get expr);
            R visitGroupingExpr(Grouping expr);
            R visitLiteralExpr(Literal expr);
            R visitLogicalExpr(Logical expr);
            //R visitSetExpr(Set expr);
            //R visitSuperExpr(Super expr);
            //R visitThisExpr(This expr);
            R visitUnaryExpr(Unary expr);
            R visitVariableExpr(Variable expr);
        }

        // Nested Expr classes here...

        class Assign : Expr
        {
            Assign(Tokens name, Expr value) {
              this.name = name;
              this.value = value;
            }

     R accept<R>(Visitor<R> visitor)
    {
        return visitor.visitAssignExpr(this);
    }

    Tokens name;
    Expr value;
  }

 class Binary : Expr
{
    Binary(Expr left, Tokens op, Expr right) {
        this.left = left;
        this.op = op;
        this.right = right;
    }

     R accept<R>(Visitor<R> visitor) {
        return visitor.visitBinaryExpr(this);
    }

    Expr left, right;
    Tokens op;
}

class Grouping : Expr
{
    Grouping(Expr expression) {
        this.expression = expression;
    }

     R accept<R>(Visitor<R> visitor) {
        return visitor.visitGroupingExpr(this);
    }

    Expr expression;
}

 class Literal : Expr
{
    Literal(Object value) {
        this.value = value;
    }

    R accept<R>(Visitor<R> visitor) {
        return visitor.visitLiteralExpr(this);
    }

    Object value;
}

 class Logical : Expr
{
    Logical(Expr left, Tokens op, Expr right) {
        this.left = left;
        this.op = op;
        this.right = right;
    }

     R accept<R>(Visitor<R> visitor) {
        return visitor.visitLogicalExpr(this);
    }

     Expr left;
     Tokens op;
     Expr right;
}

 class Unary : Expr
{
    Unary(Tokens op, Expr right) {
        this.op = op;
        this.right = right;
    }

     R accept<R>(Visitor<R> visitor) {
        return visitor.visitUnaryExpr(this);
    }

    Tokens op;
    Expr right;
}

 class Variable : Expr
{
    Variable(Tokens name) {
        this.name = name;
    }

     R accept<R>(Visitor<R> visitor) {
        return visitor.visitVariableExpr(this);
    }

    Tokens name;
}


//abstract R accept<R>(Visitor<R> visitor);
}
}
