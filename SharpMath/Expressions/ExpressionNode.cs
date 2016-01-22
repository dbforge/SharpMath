namespace SharpMath.Expressions
{
    public class ExpressionNode
    {
        public ExpressionNode(string content)
        {
            Content = content;
        }

        public string Content { get; set; }
        public ExpressionNode LeftDescendant { get; set; }
        public ExpressionNode RightDescendant { get; set; }
    }
}