using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SharpMath.Expressions
{
    public class ExpressionTree
    {
        public ExpressionNode Root { get; set; }

        //public ExpressionTree Build(Stack<Token> postfixTokens)
        //{
        //    var operandStack = new Stack<Token>();
        //    var nodeStack = new Stack<ExpressionNode>();
        //    while (postfixTokens.Any())
        //    {
        //        switch (postfixTokens.Peek().Type)
        //        {
        //            case TokenType.Number:
        //                operandStack.Push(postfixTokens.Pop());
        //                break;
        //            case TokenType.Operator:
        //                var currentToken = postfixTokens.Pop();
        //                var expressionNode = new ExpressionNode(currentToken.ToString())
        //                {
        //                    RightDescendant = new ExpressionNode(operandStack.Pop().ToString()),
        //                    LeftDescendant = new ExpressionNode(operandStack.Pop().ToString())
        //                };
        //                break;
        //        }
        //    }
        //}
    }
}