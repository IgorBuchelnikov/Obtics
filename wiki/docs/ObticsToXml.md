# ObticsToXml

This is a library with method mappings for the Obtics ExpressionObserver. These mappings allow the ExpressionObserver to create fully live expressions from plain Linq-to-Xml statements. 

These mappings consist of a set of static methods in a single class named Obtics.Xml.ObservableExtensions. These methods are directly related to properties, operators and methods on the System.Xml.Linq.XObject derived classed and the System.Xml.Linq.Extensions class. The methods can be used directly but it is more convenient to do this via the ExpressionObserver.

The methods need to be registered with an ExpressionObserverMaster before the associated ExpressionObserverObject can translate plain Linq-to-Xml expression to live expressions. THis can be done with the Obtics.Xml.ObservableExtensions.RegisterMappings() method.

{{
var master = new ExpressionObserverMaster( ExpressionObserver.Default );

Obtics.Xml.ObservableExtensions.RegisterMappings( master );

XDocument document = XDocument.Load("MyXml.xml");

var personsNamedVicky = 
    master.ExpressionObserver.Execute( 
        document, 
        d => d.Descendants("Person").Where(p => p.Attribute("Name").Value == "Vicky") 
    ).Cascade() ;
}}

**limitations**
[Reactivity](Reactivity) is based on the limited functionality of the System.Xml.Linq.XDocument Changed and Changing events. This puts a number of limitiations on ObticsToXml. Only those LinqToXml entities can be translated to a live form that are supported by the Changed and Changing events. These entities involve the Xml tree structure, names of elements and Values of Elements, Attributes, Comments, Text and CData objects. 

Annotations for example are not observable and can not be traced. They can be used in translated expressions but the result will never be reactive to any changes in annotations.

Only those System.Xml.Linq objects can be observed that are actually part of a complete Xml document tree. Though each XObject supports its own Changing and Changed events only those of the top XDocument are regarded. The reason is that the events for lower objects are incomplete and it would not be possible to trace changed properly. The result of the following will not be reactive to changes to the attributes:
{{
//element is not a descendant of an XDocument.
var element = new XElement();
var elementNameAttribute = master.ExpressionObserver.Execute(element, e => e.Attribute("Name").Value );
}}
