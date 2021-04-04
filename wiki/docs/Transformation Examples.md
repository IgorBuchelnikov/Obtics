# Transformation Examples

There are a number of different styles of creating reactive and observable transformations.

* [Explicit Value Transformation Examples](Explicit-Value-Transformation-Examples)
* [Implicit Value Transformation Examples](Implicit-Value-Transformation-Examples)
* [Implicit Value Transformation With LINQ Examples](Implicit-Value-Transformation-With-LINQ-Examples)
* [Observable LINQ examples](Observable-LINQ-examples) (as from version 1.0.8.0 *)
* [Explicit Observable LINQ Examples](Explicit-Observable-LINQ-Examples) (deprecated since version 1.0.8.0 *)
* [Implicit Observable LINQ Examples](Implicit-Observable-LINQ-Examples) (deprecated since version 1.0.8.0 *)

Class Person in these examples is an observable class. That means an instance of this class sends change notifications whenever a property changes value. See [http://www.codeplex.com/Obtics/SourceControl/FileView.aspx?itemId=209671&changeSetId=14599](http://www.codeplex.com/Obtics/SourceControl/FileView.aspx?itemId=209671&changeSetId=14599) for a possible implementation.

*Before version 1.0.8.0 there was a difference between implicit and explicit observable (should be reactive) LINQ. Implicit observable methods would take a lambda expression whenever applicable and use ExpressionObserver to analyse it for dependencies. Since then LINQ capability has been added to ExpressionObserver itself and the resulting queries have a better performance than queries constructed with Implicit Observable LINQ. Implicit Observable LINQ has therefore been removed and Explicit Observable LINQ is the only remaining variation. Explicit Observable LINQ has been moved up in the namespace hierarchy and is now named just 'Observable LINQ'. It's methods are implemented in the static class Obtics.Collections.ObservableEnumerable.  