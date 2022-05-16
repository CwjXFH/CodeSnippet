using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace EFCoreExtensions.Queryable;

public class NonQueryTagExpression : TableExpressionBase
{
    private readonly string _tag = "";
    private readonly TableExpressionBase _table;

    public NonQueryTagExpression(string tag)
        : base(tag)
    {
        _tag = tag;
    }

    public NonQueryTagExpression(string tag, TableExpressionBase table)
        : base(table.Alias)
    {
        _tag = tag;
        _table = table;
    }

    // public NonQueryTagExpression(string? alias) : base(alias)
    // {
    // }

    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        var tableExpressionBase = (TableExpressionBase) visitor.Visit(_table);
        if (_table == tableExpressionBase)
        {
            return this;
        }

        return new NonQueryTagExpression(_tag, tableExpressionBase);
    }

    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        expressionPrinter.Visit(_table);
        expressionPrinter.Append($"-- {_tag}");
    }
}