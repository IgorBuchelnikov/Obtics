# ExpressionEqualityComparer

It is a little strange that two similar Expressions trees are not considered equal even though they are completely interchangeable. Like:

{{
int a = 1, b = 2

Expression<Func<bool, int>> exp1 = x => x ? a : b ;
Expression<Func<bool, int>> exp2 = x => x ? a : b ;

Console.Out.WriteLine(exp1.Equals(exp2));
}}

This will write _False_ to the console, even though they will always give the same result. The only thing compared here is the instance id.

{{
int a = 1, b = 2

Expression<Func<bool, int>> exp1 = x => x ? a : b ;
Expression<Func<bool, int>> exp2 = x => x ? a : b ;

Console.Out.WriteLine(  (new ExpressionEqualityComparer()).Equals(exp1,exp2) );
}}

This code will return _True_ in this case. If two Expression trees are equal ExpressionEqualityComparer will return true.
More specifically: if ExpressionEqualityComparer.Equals returns true for two expressions trees than the compiled versions of those
trees are completely interchangeable. Note that the reverse is not true! If ExpressionEqualityComparer.Equals returns false for two expression trees then it does not mean that they are not interchangeable. For example:

{{
Expression<Func<int, int, int>> exp1 = (x,y) => x + y ;
Expression<Func<int, int, int>> exp2 = (x,y) => y + x ;

Console.Out.WriteLine(  (new ExpressionEqualityComparer()).Equals(exp1,exp2) );
}}

Will return _False_ but the two expressions are interchangeable.
