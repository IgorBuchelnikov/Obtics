## Features before 1.0.8.0

Obtics allows you to create [reactive](reactive) and [observable](observable) [transformation](transformation)s in a number of ways, freeing you of the burden of having to update and knowing when to update your results in a dynamic application. Having less to think about, you should be able to create richer and more reliable applications sooner.

* Implicit Value transformation.s Using methods form the Obtics.Values.ExpressionObserver class. Simply write a lambda function yielding your desired result. ExpressionObserver will analyse it, extract any dependencies and build an expression that is fully [reactive](reactive)(reactive) to any [observable](observable)(observable) changes of those dependencies. It will also rewrite your existing standard object LINQ statements into a [reactive](reactive)(reactive) and [observable](observable)(observable) form giving the most convenient way of creating bindable object LINQ queries.
* Explicit Value transformations. Using methods from the Obtics.Values.ValueProvider class. You can build your value [transformation pipeline](transformation-pipeline) manually using these methods, trading ease uf use for control. It allows you to specify exactly where the relevant volatile dependencies are for you calculation, allowing you to prevent wasting of resources on stable or unobservable dependencies. This style may be usefull when working with larger amounts of data, when wasted resources add up.
* Implicit Collection transformations (LINQ). Using methods from the Obtics.Collections.ImplicitObservable.ObservableEnumerable class. Allows you to create fully [reactive](reactive) and [observable](observable) object LINQ queries using extension methods and inline query syntax. It adds extra methods, extending the power of the object LINQ query rewriting already offered by the ExpressionObserver class. To take full advantage of these methods though a System.Linq using statement in your source code needs to be replaced with a Obtics.Collections.ImplicitObservable using statement or ambigouity errors will occur. Implicit collection transformation methods can be used directly with implicit value transformations, offering some extra functionality over the standard object LINQ methods.    
* Explicit collection transformations (LINQ) . Using methods from the Obtics.Collections.ExplicitObservable.ObservableEnumerable class. Like with Explicit Value transformations this allows you to specify exactly how [reactive](reactive) your collection [transformation](transformation)(transformation)s should be. This goes at the expense of ease of use since you have to write out [transformation](transformation)(transformation)s manually and have to be aware of what dependencies need to be monitored for changes. But it does allow you to prevent wasting of resources on dependencies that never change anyway. This style may be usefull when working with larger collections and wasted resources add up. To take full advantage of these methods though a System.Linq using statement in your source code needs to be replaced with a Obtics.Collections.ExplicitObservable using statement or ambigouity errors will occur.

Obtics offers **full observable support for all object LINQ statements**. Even other LINQ versions (LINQ to SQL, LINQ to XML) can become [observable](observable) and [reactive](reactive) to local dependencies. 