using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SourceCode.Clay.Javascript.Ast
{
    public class JSBinaryExpression : JSExpression, IEnumerable
    {
        public override JSNodeType Type => JSNodeType.BinaryExpression;

        public JSBinaryOperator Operator { get; set; }

        public JSExpression Left { get; set; }

        public List<JSExpression> Right { get; }

        public JSBinaryExpression()
        {
            Right = new List<JSExpression>();
        }

        public JSBinaryExpression(JSExpression left, JSBinaryOperator @operator)
        {
            Left = left;
            Operator = @operator;
            Right = new List<JSExpression>();
        }

        public JSBinaryExpression(JSExpression left, JSBinaryOperator @operator, JSExpression right)
        {
            Left = left;
            Operator = @operator;
            Right = new List<JSExpression>()
            {
                right
            };
        }

        public JSBinaryExpression Add(JSExpression right)
        {
            if (right is JSBinaryExpression binary && binary.Operator == Operator)
            {
                Right.Add(binary.Left);
                Right.AddRange(binary.Right);
                return this;
            }
            Right.Add(right);
            return this;
        }

        public JSBinaryExpression Add(IEnumerable<JSExpression> right)
        {
            foreach (var item in right)
                Add(item);
            return this;
        }

        public JSBinaryExpression Add(params JSExpression[] right)
            => Add((IEnumerable<JSExpression>)right);

        IEnumerator IEnumerable.GetEnumerator() => Enumerable.Empty<object>().GetEnumerator();
    }
}
