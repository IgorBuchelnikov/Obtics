# Rules for mappings

This section describes what the rules are for mapping a non-live method, property or field to live methods. ExpressionObserver can use these mappings to create live expressions from statndard expressions that contain your non-live method, property or field.

**Mapping target are always static methods of a non-generic class**
Wether mapping an instance or static scoped method, property or field; The targets of your mapping are always static methods of a non-generic class.

**Type parameters**
If the mapped entity is a non-generic member of a non-generic type then the target methods have no type parameters.

if the mapped entity is a generic method definition of a non-generic type then the target method has the same type parameters in the same order.

if the mapped entity is a non-generic member of a generic type definition then the target method has the same type parameters as the generic type definition and in the same order.

if the mapped entity is a generic method definition of a generic type definition then the target method has the type parameters of the type definition and the method definition combined. First parameters are the same as those of the type definition in the same order. These are followed by the same type parameters as those of the method definition in the same order.

{{
class C<X>
{
    public X Method<Y>( Y v );
}
}}
maps to
{{
static class CMap
{
    public static X Method<X,Y>( Y v );
}
}}

**Method parameters**
If the mapped entity is a static property or field then the target method has no parameters

If the mapped entity is a static method then the target method has matching parameters for each original parameter in the same order. Matching parameters is explained later.

If the mapped entity is an instance field or property without indexers then the target method has a single parameter matching the defining type. Matching parameters is explained later.

If the mapped entity is an instance method or property with indexers then the target method has a parameter matching the defining type as first parameter, followed by a mathcing parameter for each of the original parameters of the mapped entity.

{{
class C 
{
    public int Method( int v );
}
}}
maps to:
{{
static class CMap
{
    public static int Method( C c, int v );
}
}}

**Result value**
A mapped method must return a result value.

The target methods must return a result value. The type of this value must match the type of the value of the mapped property or field or result of the mapped method in the following way:
The type returned by the target method must be the same as or must be a derivation of the type given by the original entity.
Or the target method must return an IValueProvider<T> where T is the exact same type as the type given by the original entity.
IValueProvider can give a live result. If IValueProvider as result is not used then the original type needs to be made implicitly observable. IEnumerable can be made imlicitly observable by letting the returned instance implement INotifyCollectionChanged. The means by which a returned type can be made implicitly observable is not defined and is up to the implementor.

{{
class C
{
    public static C Method( C v );
}
}}

Can map to either one of:
{{
class CMap
{
    public static C Method1( C v);
    public static IValueProvider<C> Method2( C v);
}
}}

Multiple target methods can be given in a mapping but all need to return the same result type.

**Matching parameters**
Parameters can be matched in the following ways:
* not-mapped; The parameter is of the exact same type as the original parameter.
* mapped; A parameter of any type T can be mapped to a parameter of type IValueProvider<T>. A lambda parameter of a type Func<..,T> can be mapped to Func<..,IValueProvider<T>>. A mapped parameter indicates to the ExpressionObserver that the target method knows how to deal with observable parameters. It assumes that the target method indeed does know that.

Multiple target methods can have a different configuration of not-mapped and mapped parameters. Each target method must have a unique configuration. There must be one target method with a configuration such that its mapped parameters cover the mapped parameters of all other target methods. This last method is the default or fallback method. It is not necessairy that there is a target method for every mapping configuration possible.

When the ExpressionObserver choses a target method to use it tries to find the method with the least number of mapped parameters needed. Assuming that the more live the target method is the more expensive it will be.

for
{{
class C
{
    public int Method( int a, int b )
}
}}

a valid target method set would be:
{{
static class CMap
{
    public static IValueProvider<int> Method( C c, int a, int b );
    public static IValueProvider<int> Method( C c, IValueProvider<int> a, int b );
    public static IValueProvider<int> Method( C c, int a, IValueProvider<int> b );
    public static IValueProvider<int> Method( C c, IValueProvider<int> a, IValueProvider<int> b );
}
}}