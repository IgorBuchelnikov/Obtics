# Reactive

Reactive in obtics means that the output of an object depends on properties of some input sources and that if the properties of those input sources change value the output of the object will change accordingly.

The object here mentioned will usually be a [transformation pipeline](transformation-pipeline).

For example the Obtics.Collections.ExplicitObservable.ObservableEnumerable.Select( ) methods will take a source IEnumerable and return a different IEnumerable (object). Provided that the source IEnumerable is [observable](observable) whenever its sequence changes the sequence of the result IEnumerable will change as well, making the result reactive. 

[Transformation](Transformation)s generated by Obtics are [reactive](reactive) and [observable](observable)