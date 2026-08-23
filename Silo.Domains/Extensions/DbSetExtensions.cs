using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore;
public static class DbSetExtensions
{
    public static int ExecuteUpdateMulti<T>(this DbSet<T> set
        , Expression<Func<T, bool>> where
        , params Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>[] exprs) where T : class
    {
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> expr = sett => sett;

        foreach (var expr2 in exprs)
        {
            var call = (MethodCallExpression)expr2.Body;
            expr = Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(
                Expression.Call(expr.Body, call.Method, call.Arguments),
                expr2.Parameters
            );
        }

        var a =  set.Where(where).ExecuteUpdate(expr);

        return a;
    }
}
