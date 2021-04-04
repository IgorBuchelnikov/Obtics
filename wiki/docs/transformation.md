# transformation

A transformation in the context of Obtics means an object that takes an input [source](source)(source) and from that [source](source)(source) calculates an output. A transformation therefore is much like a function. The difference is that a function gives a one-time result and is never [reactive](reactive) or [observable](observable) where a transformation can continualy update its output. A transformation object is usually the result of a function. 

Notably the transformation itself is a [static](static) object even though its output may be [volatile](volatile).

Multiple transformation objects may be chained to form a [transformation pipeline](transformation-pipeline).