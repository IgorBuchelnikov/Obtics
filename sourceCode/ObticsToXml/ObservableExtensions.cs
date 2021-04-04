using System;
using System.Collections.Generic;
using SL = System.Linq.Enumerable;
using System.Text;
using System.Xml.Linq;
using Obtics.Values;
using Obtics.Collections;
using System.Reflection;
using System.Globalization;
using System.Xml;
using System.Runtime.CompilerServices;

namespace Obtics.Xml
{
    /// <summary>
    /// class with ExpressionObserver mappings for various Linq-to-Xml methods.
    /// </summary>
    /// <remarks>
    /// Not all properties and methods of XObject and derived classes and System.Xml.Linq.Extensions methods have mappings here.
    /// XObject classes offer limited observability of the Xml model. Only those entities that are observable have live mappings here.
    /// 
    /// There entities involve the Xml tree structure, names of elements and Values of Elements, Attributes, Comments, Text and CData objects.
    /// Annotations for example are not observable and can not be traced. A different model would need to be used than the System.Xml.Linq model.
    /// 
    /// Live results work only for those objects that are actually part of a full Xml tree. That means only those objects that have a not null Document property. Objects
    /// that are not part of a full Xml tree can not be completely traced.
    /// 
    /// These methods can be used directly but that is not it's intended purpose.
    /// 
    /// The methods need to be registered with an <see cref="ExpressionObserverMaster"/> before use. This can be done with the RegisterMappings method.
    /// The master.ExpressionObserver object can then be used to create live Linq-to-Xml expressions.
    /// </remarks>
    public static class ObservableExtensions
    {
        static void TryGetValue<TKey, TValue>(this ConditionalWeakTable<TKey, TValue> table, TKey key, Action<TKey, TValue> action)
            where TKey : class
            where TValue : class
        {
            TValue item;

            if (table.TryGetValue(key, out item))
                action(key, item);
        }

        class XEventDispatcher
        {
            internal XEventDispatcher(XDocument document)
            {
                document.Changed += ChangedHandler;
                document.Changing += ChangingHandler;
            }

            XElement _Parent;
            XObject _Next;
            XObject _Previous;
            XDocument _Document;

            void ChangingHandler(object sender, XObjectChangeEventArgs args)
            {
                var obj = (XObject)sender ;

                _Parent = obj.Parent;
                _Document = obj.Document;

                switch (NodeTypeFromXmlNodeType(obj.NodeType))
                {
                    case NodeType.XAttribute:
                        var attr = (XAttribute)sender;
                        _Next = attr.NextAttribute;
                        _Previous = attr.PreviousAttribute;
                        break;
                    case NodeType.XCData:
                    case NodeType.XComment:
                    case NodeType.XDocument:
                    case NodeType.XDocumentType:
                    case NodeType.XElement:
                    case NodeType.XProcessingInstruction:
                    case NodeType.XText:
                        var node = (XNode)sender;
                        _Next = node.NextNode;
                        _Previous = node.PreviousNode;
                        break;
                }                                
            }


            void ChangedHandler(object sender, XObjectChangeEventArgs args)
            {
                var obj = (XObject)sender ;

                switch (args.ObjectChange)
                {
                    case XObjectChange.Remove:
                    case XObjectChange.Add:
                        switch (NodeTypeFromXmlNodeType(obj.NodeType))
                        {
                            case NodeType.XAttribute:
                                {
                                    var addedAttr = (XAttribute)sender;

                                    var parent = addedAttr.Parent ?? _Parent;

                                    if (parent != null)
                                        _ElementAttributesProviderMap.TryGetValue(parent, (prt, attrProvider) => attrProvider.Value = SL.ToArray(prt.Attributes()));

                                    var next = addedAttr.NextAttribute ?? (XAttribute)_Next;
                                    var previous = addedAttr.PreviousAttribute ?? (XAttribute)_Previous;

                                    if (next != null)
                                        _AttributePreviousProviderMap.TryGetValue(next, (_, previousProvider) => previousProvider.Value = args.ObjectChange == XObjectChange.Add ? addedAttr : previous);

                                    if (previous != null)
                                        _AttributeNextProviderMap.TryGetValue(previous, (_, nextProvider) => nextProvider.Value = args.ObjectChange == XObjectChange.Add ? addedAttr : next);
                                }
                                break;
                            case NodeType.XText:
                            case NodeType.XCData:
                            case NodeType.XElement:
                                {
                                    var owningElement = obj.Parent ?? _Parent;

                                    if (owningElement != null)
                                        _ElementValueProviderMap.TryGetValue(owningElement, (oe, valueProvider) => valueProvider.Value = oe.IsEmpty ? null : oe.Value);
                                }
                                goto case NodeType.XComment;

                            case NodeType.XDocument:
                                _DocumentRootProviderMap.TryGetValue((XDocument)sender, (document, rootProvider) => rootProvider.Value = document.Root);
                                goto case NodeType.XComment;

                            case NodeType.XComment:
                            case NodeType.XDocumentType:
                            case NodeType.XProcessingInstruction:
                                {
                                    var addedNode = (XNode)sender;

                                    var parent = addedNode.Parent ?? _Parent;

                                    if (parent != null)
                                        _ContainerNodeProviderMap.TryGetValue(parent, (prt, nodeProvider) => nodeProvider.Value = SL.ToArray(prt.Nodes()));
                                    else
                                    {
                                        //root element?
                                        var document = addedNode.Document ?? _Document;

                                        if (document != null)
                                            _DocumentRootProviderMap.TryGetValue(document, (d, rootProvider) => rootProvider.Value = d.Root);
                                    }

                                    var next = addedNode.NextNode ?? (XNode)_Next;
                                    var previous = addedNode.PreviousNode ?? (XNode)_Previous;

                                    if (next != null)
                                        _NodePreviousProviderMap.TryGetValue(next, (_, previousProvider) => previousProvider.Value = args.ObjectChange == XObjectChange.Add ? addedNode : previous);

                                    if (previous != null)
                                        _NodeNextProviderMap.TryGetValue(previous, (_, nextProvider) => nextProvider.Value = args.ObjectChange == XObjectChange.Add ? addedNode : next);
                                }
                                break;
                            default:
                                throw new Exception("Unexpected NodeType value.");
                        }
                        break;
                    case XObjectChange.Name:
                        {
                            if (NodeTypeFromXmlNodeType(obj.NodeType) == NodeType.XElement)
                                _ElementNameProviderMap.TryGetValue((XElement)sender, (element, nameProvider) => nameProvider.Value = element.Name);
                        }
                        break;
                    case XObjectChange.Value:
                        {
                            switch (NodeTypeFromXmlNodeType(obj.NodeType))
                            {
                                case NodeType.XAttribute:
                                    _AttributeValueProviderMap.TryGetValue((XAttribute)sender, (attr, valueProvider) => valueProvider.Value = attr.Value);
                                    break;

                                case NodeType.XElement:
                                    _ElementValueProviderMap.TryGetValue((XElement)sender, (element, valueProvider) => valueProvider.Value = element.IsEmpty ? null : element.Value);
                                    break;

                                case NodeType.XComment:
                                    _CommentValueProviderMap.TryGetValue((XComment)sender, (comment, valueProvider) => valueProvider.Value = comment.Value);
                                    break;

                                case NodeType.XText:
                                case NodeType.XCData:
                                    _TextValueProviderMap.TryGetValue((XText)sender, (text, valueProvider) => valueProvider.Value = text.Value);
                                    break;

                                case NodeType.XDocument :
                                case NodeType.XDocumentType :
                                case NodeType.XProcessingInstruction :
                                    break;

                                default:
                                    throw new Exception("Unexpected NodeType value.");
                            }
                        }
                        break;
                    default :
                        throw new Exception("Unexpected XObjectChange value.");
                }
            }
        }

        static ConditionalWeakTable<XDocument, XEventDispatcher> _DispatchersMap = new ConditionalWeakTable<XDocument, XEventDispatcher>();

        private static void EnsureDispatcher(XObject container)
        {
            var document = container.Document;
            XEventDispatcher dummy;

            if(!_DispatchersMap.TryGetValue(document, out dummy))
                lock(_DispatchersMap)
                    _DispatchersMap.GetValue(document, d => new XEventDispatcher(d));
        }

        static ConditionalWeakTable<XContainer, IValueProvider<IEnumerable<XNode>>> _ContainerNodeProviderMap = new ConditionalWeakTable<XContainer, IValueProvider<IEnumerable<XNode>>>();

        static IValueProvider<IEnumerable<XNode>> GetNodeProvider(XContainer container)
        {
            return 
                _ContainerNodeProviderMap.GetValue(
                    container,
                    c =>
                    {
                        EnsureDispatcher(c);
                        return ValueProvider.Dynamic(c.Nodes());
                    }
                )
            ;
        }

        static ConditionalWeakTable<XElement, IValueProvider<IEnumerable<XAttribute>>> _ElementAttributesProviderMap = new ConditionalWeakTable<XElement, IValueProvider<IEnumerable<XAttribute>>>();

        static IValueProvider<IEnumerable<XAttribute>> GetAttributesProvider(XElement element)
        {
            return
                _ElementAttributesProviderMap.GetValue(
                    element,
                    e =>
                    {
                        EnsureDispatcher(e);
                        return ValueProvider.Dynamic(e.Attributes());
                    }
                )
            ;
        }

        static ConditionalWeakTable<XAttribute, IValueProvider<string>> _AttributeValueProviderMap = new ConditionalWeakTable<XAttribute, IValueProvider<string>>();

        static IValueProvider<string> GetAttributeValueProvider(XAttribute attr)
        {
            return
                _AttributeValueProviderMap.GetValue(
                    attr,
                    a =>
                    {
                        EnsureDispatcher(a);
                        return ValueProvider.Dynamic(a.Value);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XElement, IValueProvider<string>> _ElementValueProviderMap = new ConditionalWeakTable<XElement, IValueProvider<string>>();

        static IValueProvider<string> GetElementValueProvider(XElement element)
        {
            return
                _ElementValueProviderMap.GetValue(
                    element,
                    e =>
                    {
                        EnsureDispatcher(e);
                        return ValueProvider.Dynamic(e.IsEmpty ? null : e.Value);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XComment, IValueProvider<string>> _CommentValueProviderMap = new ConditionalWeakTable<XComment, IValueProvider<string>>();

        static IValueProvider<string> GetCommentValueProvider(XComment comment)
        {
            return
                _CommentValueProviderMap.GetValue(
                    comment,
                    c =>
                    {
                        EnsureDispatcher(c);
                        return ValueProvider.Dynamic(c.Value);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XText, IValueProvider<string>> _TextValueProviderMap = new ConditionalWeakTable<XText, IValueProvider<string>>();

        static IValueProvider<string> GetTextValueProvider(XText comment)
        {
            return
                _TextValueProviderMap.GetValue(
                    comment,
                    c =>
                    {
                        EnsureDispatcher(c);
                        return ValueProvider.Dynamic(c.Value);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XNode, IValueProvider<XNode>> _NodeNextProviderMap = new ConditionalWeakTable<XNode, IValueProvider<XNode>>();

        static IValueProvider<XNode> GetNextNodeProvider(XNode node)
        {
            return
                _NodeNextProviderMap.GetValue(
                    node,
                    n =>
                    {
                        EnsureDispatcher(n);
                        return ValueProvider.Dynamic(n.NextNode);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XNode, IValueProvider<XNode>> _NodePreviousProviderMap = new ConditionalWeakTable<XNode, IValueProvider<XNode>>();

        static IValueProvider<XNode> GetPreviousNodeProvider(XNode node)
        {
            return
                _NodePreviousProviderMap.GetValue(
                    node,
                    n =>
                    {
                        EnsureDispatcher(n);
                        return ValueProvider.Dynamic(n.PreviousNode);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XAttribute, IValueProvider<XAttribute>> _AttributeNextProviderMap = new ConditionalWeakTable<XAttribute, IValueProvider<XAttribute>>();

        static IValueProvider<XAttribute> GetNextAttributeProvider(XAttribute node)
        {
            return
                _AttributeNextProviderMap.GetValue(
                    node,
                    n =>
                    {
                        EnsureDispatcher(n);
                        return ValueProvider.Dynamic(n.NextAttribute);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XAttribute, IValueProvider<XAttribute>> _AttributePreviousProviderMap = new ConditionalWeakTable<XAttribute, IValueProvider<XAttribute>>();

        static IValueProvider<XAttribute> GetPreviousAttributeProvider(XAttribute node)
        {
            return
                _AttributePreviousProviderMap.GetValue(
                    node,
                    n =>
                    {
                        EnsureDispatcher(n);
                        return ValueProvider.Dynamic(n.PreviousAttribute);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XElement, IValueProvider<XName>> _ElementNameProviderMap = new ConditionalWeakTable<XElement, IValueProvider<XName>>();

        static IValueProvider<XName> GetNameProvider(XElement element)
        {
            return
                _ElementNameProviderMap.GetValue(
                    element,
                    e =>
                    {
                        EnsureDispatcher(e);
                        return ValueProvider.Dynamic(e.Name);
                    }
                )
            ;
        }

        static ConditionalWeakTable<XDocument, IValueProvider<XElement>> _DocumentRootProviderMap = new ConditionalWeakTable<XDocument, IValueProvider<XElement>>();

        static IValueProvider<XElement> GetDocumentRootProvider(XDocument document)
        {
            return
                _DocumentRootProviderMap.GetValue(
                    document,
                    d =>
                    {
                        EnsureDispatcher(d);
                        return ValueProvider.Dynamic(d.Root);
                    }
                )
            ;
        }


        //
        static IValueProvider<T> As<T>(this IValueProvider<string> valueProvider, Func<string, T> converter)
        {
            return
                valueProvider.Select(
                    ValueProvider.Static(converter),
                    (v, c) =>
                    {
                        try { return String.IsNullOrEmpty(v) ? default(T) : c(v); }
                        catch (FormatException) { return default(T); }
                    }
                );
        }

        static IValueProvider<T?> AsNullable<T>(this IValueProvider<string> valueProvider, Func<string, T> converter) where T : struct
        {
            return
                valueProvider.Select(
                    ValueProvider.Static(converter),
                    (v, c) =>
                    {
                        try { return String.IsNullOrEmpty(v) ? default(T?) : c(v); }
                        catch (FormatException) { return default(T?); }
                    }
                );
        }


        static IValueProvider<int?> AsNullableInt(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, Int32>)XmlConvert.ToInt32); }

        static IValueProvider<int> AsInt(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string,Int32>)XmlConvert.ToInt32); }

        static IValueProvider<uint?> AsNullableUnsignedInt(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, UInt32>)XmlConvert.ToUInt32); }

        static IValueProvider<uint> AsUnsignedInt(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, UInt32>)XmlConvert.ToUInt32); }

        static IValueProvider<long?> AsNullableLong(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, Int64>)XmlConvert.ToInt64); }

        static IValueProvider<long> AsLong(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, Int64>)XmlConvert.ToInt64); }

        static IValueProvider<ulong?> AsNullableUnsignedLong(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, UInt64>)XmlConvert.ToUInt64); }

        static IValueProvider<ulong> AsUnsignedLong(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, UInt64>)XmlConvert.ToUInt64); }

        static IValueProvider<bool?> AsNullableBoolean(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, Boolean>)XmlConvert.ToBoolean); }

        static IValueProvider<bool> AsBoolean(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, Boolean>)XmlConvert.ToBoolean); }

        static IValueProvider<Guid?> AsNullableGuid(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, Guid>)XmlConvert.ToGuid); }

        static IValueProvider<Guid> AsGuid(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, Guid>)XmlConvert.ToGuid); }

        static IValueProvider<float?> AsNullableSingle(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, float>)XmlConvert.ToSingle); }

        static IValueProvider<float> AsSingle(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, float>)XmlConvert.ToSingle); }

        static IValueProvider<double?> AsNullableDouble(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, double>)XmlConvert.ToDouble); }

        static IValueProvider<double> AsDouble(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, double>)XmlConvert.ToDouble); }

        static IValueProvider<decimal?> AsNullableDecimal(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, decimal>)XmlConvert.ToDecimal); }

        static IValueProvider<decimal> AsDecimal(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, decimal>)XmlConvert.ToDecimal); }

        static IValueProvider<TimeSpan?> AsNullableTimeSpan(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, TimeSpan>)XmlConvert.ToTimeSpan); }

        static IValueProvider<TimeSpan> AsTimeSpan(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, TimeSpan>)XmlConvert.ToTimeSpan); }

        static IValueProvider<DateTime?> AsNullableDateTime(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, DateTime>)XmlConvert.ToDateTime); }

        static IValueProvider<DateTime> AsDateTime(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, DateTime>)XmlConvert.ToDateTime); }

        static IValueProvider<DateTimeOffset?> AsNullableDateTimeOffset(IValueProvider<string> valueProvider)
        { return valueProvider.AsNullable((Func<string, DateTimeOffset>)XmlConvert.ToDateTimeOffset); }

        static IValueProvider<DateTimeOffset> AsDateTimeOffset(IValueProvider<string> valueProvider)
        { return valueProvider.As((Func<string, DateTimeOffset>)XmlConvert.ToDateTimeOffset); }

        static IValueProvider<string> AsString(IValueProvider<string> valueProvider)
        { return valueProvider; }

        //


        //XObject

        //XAttribute

        /// <summary>
        /// Returns the live value of an attribute.
        /// </summary>
        /// <param name="attr">The attribute to return the live value of.</param>
        /// <returns>An <see cref="IValueProvider{Str}"/> of <see cref="String"/> that represents the live value of the attribute.</returns>
        public static IValueProvider<string> Value(XAttribute attr)
        { 
            return
                attr == null ? null :
                attr.Document == null ? ValueProvider.Static(attr.Value) : 
                GetAttributeValueProvider(attr).ReadOnly(); 
        }

        /// <summary>
        /// Returns the next attribute on the owning element live. 
        /// </summary>
        /// <param name="attr">The attribute who's next sibling needs to be returned.</param>
        /// <returns>An <see cref="IValueProvider{XAttribute}"/> of <see cref="XAttribute"/> that represents the next attribute live.</returns>
        public static IValueProvider<XAttribute> NextAttribute(XAttribute attr)
        {
            return 
                attr == null ? null :
                attr.Document == null ? ValueProvider.Static(attr.NextAttribute) :
                GetNextAttributeProvider(attr).ReadOnly();
        }


        /// <summary>
        /// Returns the previous attribute on the owning element live. 
        /// </summary>
        /// <param name="attr">The attribute who's previous sibling needs to be returned.</param>
        /// <returns>An <see cref="IValueProvider{XAttribute}"/> of <see cref="XAttribute"/> that represents the previous attribute live.</returns>
        public static IValueProvider<XAttribute> PreviousAttribute(XAttribute attr)
        {
            return 
                attr == null ? null :
                attr.Document == null ? ValueProvider.Static(attr.NextAttribute) :
                GetPreviousAttributeProvider(attr).ReadOnly();
        }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="Int32"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Int32"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<int?> AsNullableInt(XAttribute attr)
        { return AsNullableInt(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="Int32"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Int32"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<int> AsInt(XAttribute attr)
        { return AsInt(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="UInt32"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="UInt32"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<uint?> AsNullableUnsignedInt(XAttribute attr)
        { return AsNullableUnsignedInt(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="UInt32"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="UInt32"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<uint> AsUnsignedInt(XAttribute attr)
        { return AsUnsignedInt(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="Int64"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Int64"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<long?> AsNullableLong(XAttribute attr)
        { return AsNullableLong(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="Int64"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Int64"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<long> AsLong(XAttribute attr)
        { return AsLong(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="UInt64"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="UInt64"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<ulong?> AsNullableUnsignedLong(XAttribute attr)
        { return AsNullableUnsignedLong(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="UInt64"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="UInt64"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<ulong> AsUnsignedLong(XAttribute attr)
        { return AsUnsignedLong(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="Boolean"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Boolean"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<bool?> AsNullableBoolean(XAttribute attr)
        { return AsNullableBoolean(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="Boolean"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Boolean"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<bool> AsBoolean(XAttribute attr)
        { return AsBoolean(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="Guid"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Guid"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<Guid?> AsNullableGuid(XAttribute attr)
        { return AsNullableGuid(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="Guid"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Guid"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<Guid> AsGuid(XAttribute attr)
        { return AsGuid(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="Single"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Single"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<float?> AsNullableSingle(XAttribute attr)
        { return AsNullableSingle(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="Single"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Single"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<float> AsSingle(XAttribute attr)
        { return AsSingle(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="Double"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Double"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<double?> AsNullableDouble(XAttribute attr)
        { return AsNullableDouble(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="Double"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Double"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<double> AsDouble(XAttribute attr)
        { return AsDouble(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="Decimal"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Decimal"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<decimal?> AsNullableDecimal(XAttribute attr)
        { return AsNullableDecimal(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="Decimal"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Decimal"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<decimal> AsDecimal(XAttribute attr)
        { return AsDecimal(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="TimeSpan"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="TimeSpan"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<TimeSpan?> AsNullableTimeSpan(XAttribute attr)
        { return AsNullableTimeSpan(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="TimeSpan"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="TimeSpan"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<TimeSpan> AsTimeSpan(XAttribute attr)
        { return AsTimeSpan(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="DateTime"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="DateTime"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<DateTime?> AsNullableDateTime(XAttribute attr)
        { return AsNullableDateTime(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="DateTime"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="DateTime"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<DateTime> AsDateTime(XAttribute attr)
        { return AsDateTime(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to a nullable <see cref="DateTimeOffset"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="DateTimeOffset"/> whose Value property give the content of '<paramref name="attr"/>', or null if the cast fails.</returns>
        public static IValueProvider<DateTimeOffset?> AsNullableDateTimeOffset(XAttribute attr)
        { return AsNullableDateTimeOffset(Value(attr)); }

        /// <summary>
        /// Cast the value of a given <see cref="XAttribute"/> to an <see cref="DateTimeOffset"/> live.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="DateTimeOffset"/> whose Value property give the content of '<paramref name="attr"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<DateTimeOffset> AsDateTimeOffset(XAttribute attr)
        { return AsDateTimeOffset(Value(attr)); }

        /// <summary>
        /// Gives the live value of a given <see cref="XAttribute"/>.
        /// </summary>
        /// <param name="attr">The <see cref="XAttribute"/> whose value to return.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="DateTimeOffset"/> whose Value property give the content of '<paramref name="attr"/>'.</returns>
        public static IValueProvider<string> AsString(XAttribute attr)
        { return AsString(Value(attr)); }

        //XNode

        enum NodeType
        {
            XCData,
            XComment,
            XElement,
            XProcessingInstruction,
            XText,
            XDocument,
            XDocumentType,
            XAttribute
        }

        static NodeType NodeTypeFromXmlNodeType(System.Xml.XmlNodeType type)
        {
            switch (type)
            {
                case System.Xml.XmlNodeType.CDATA:
                    return NodeType.XCData;
                case System.Xml.XmlNodeType.Comment:
                    return NodeType.XComment;
                case System.Xml.XmlNodeType.Element:
                case System.Xml.XmlNodeType.EndElement:
                    return NodeType.XElement;
                case System.Xml.XmlNodeType.ProcessingInstruction:
                    return NodeType.XProcessingInstruction;
                case System.Xml.XmlNodeType.SignificantWhitespace:
                case System.Xml.XmlNodeType.Whitespace:
                case System.Xml.XmlNodeType.Text:
                    return NodeType.XText;
                case System.Xml.XmlNodeType.Document:
                    return NodeType.XDocument;
                case System.Xml.XmlNodeType.DocumentType:
                    return NodeType.XDocumentType;
                case System.Xml.XmlNodeType.Attribute:
                    return NodeType.XAttribute;
                case System.Xml.XmlNodeType.DocumentFragment:
                case System.Xml.XmlNodeType.EndEntity:
                case System.Xml.XmlNodeType.Entity:
                case System.Xml.XmlNodeType.EntityReference:
                case System.Xml.XmlNodeType.None:
                case System.Xml.XmlNodeType.Notation:
                case System.Xml.XmlNodeType.XmlDeclaration:
                    throw new Exception("Type?:" + type);
                default:
                    throw new Exception("Unexpected XmlNodeType value.");
            }
        }

        /// <summary>
        /// Compares the values of two nodes live. Including the values of all descendant nodes.
        /// </summary>
        /// <param name="n1">The first <see cref="XNode"/> to compare.</param>
        /// <param name="n2">The second <see cref="XNode"/> to compare.</param>
        /// <returns>An <see cref="IValueProvider{B}"/> of <see cref="bool"/> whose Value is true if the nodes are equal and false othwrwise.</returns>
        public static IValueProvider<bool> DeepEquals(XNode n1, XNode n2)
        {
            if (n1 == null)
                return ValueProvider.Static(n2 == null);

            if (n2 == null || n1.NodeType != n2.NodeType)
                return ValueProvider.Static(false);

            var nodeType = NodeTypeFromXmlNodeType(n1.NodeType);

            switch (nodeType)
            {                
                case NodeType.XComment :
                    return _DeepEqualsComment((XComment)n1,(XComment)n2); ;
                case NodeType.XDocument :
                    return _DeepEqualsDocument((XDocument)n1, (XDocument)n2);
                case NodeType.XProcessingInstruction:
                case NodeType.XDocumentType:
                    return ValueProvider.Static(XNode.DeepEquals(n1,n2));
                case NodeType.XElement :
                    return _DeepEqualsElement((XElement)n1, (XElement)n2);
                case NodeType.XCData :
                case NodeType.XText :
                    return _DeepEqualsText((XText)n1, (XText)n2);
                default :
                    throw new Exception("Unexpected node type:" + nodeType);
            }
        }

        private static IValueProvider<bool> _DeepEqualsComment(XComment xComment, XComment xComment_2)
        { return Value(xComment).Select(Value(xComment_2), (v1, v2) => v1 == v2); }

        private static IValueProvider<bool> _DeepEqualsText(XText xText, XText xText_2)
        { return Value(xText).Select( Value(xText_2), (t1, t2) => t1 == t2); }

        private static IValueProvider<bool> _DeepEqualsNodeArray(XNode[] na1, XNode[] na2)
        {
            if (na1.Length != na2.Length)
                return ValueProvider.Static(false);

            var retVals = new IValueProvider<bool>[na1.Length];

            for (int i = 0, end = na1.Length; i != end; ++i)
                retVals[i] = DeepEquals(na1[i], na2[i]);

            return retVals.All(vp => vp);
        }

        static IEnumerable<KeyValuePair<string, string>> SortedAttributes(XElement xElement)
        { return Attributes(xElement).Select(attr => Value(attr).Select(ValueProvider.Static(attr), (v,a) => new KeyValuePair<string, string>(a.Name.ToString(), v))).OrderBy(kvp => kvp.Key).ThenBy(kvp => kvp.Value); }

        static IValueProvider<XNode[]> SignificantNodes(XElement xElement)
        { return Nodes(xElement).Where(n => n.NodeType != System.Xml.XmlNodeType.ProcessingInstruction && n.NodeType != System.Xml.XmlNodeType.Comment).ToArray(); }

        static IValueProvider<bool> _DeepEqualsElement(XElement xElement, XElement xElement_2)
        {
            return
                Name(xElement).Select(Name(xElement_2), (n1, n2) => n1 == n2)
                .And(SortedAttributes(xElement).SequenceEqual( SortedAttributes(xElement_2) ) )
                .And(SignificantNodes(xElement).Select(SignificantNodes(xElement_2), (Func<XNode[], XNode[], IValueProvider<bool>>)_DeepEqualsNodeArray)); 
        }

        static IValueProvider<bool> _DeepEqualsDocument(XDocument xDocument, XDocument xDocument_2)
        { return Root(xDocument).Select(Root(xDocument_2), (Func<XElement,XElement,IValueProvider<bool>>)_DeepEqualsElement); }

        /// <summary>
        /// Returns a live collection of the sibling elements after this node, in document order.
        /// </summary>
        /// <param name="node">The <see cref="XNode"/> to return the siblings of.</param>
        /// <returns>An <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> of the sibling elements after this node, in document order. The returned object also implements <see cref="INotifyCollectionChanged"/>.</returns>
        public static IEnumerable<XElement> ElementsAfterSelf(XNode node)
        {
            return 
                node == null ? null :
                node.Document == null || node.Parent == null ? node.ElementsAfterSelf() :
                Elements(node.Parent).SkipWhile(sib => !object.ReferenceEquals(sib, node)).Skip(1);
        }

        /// <summary>
        /// Returns a filtered live collection of the sibling elements after this node, in
        /// document order. Only elements that have a matching <see cref="XName"/> are included in the collection.
        /// </summary>
        /// <param name="node">The <see cref="XNode"/> to return the siblings of.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>An <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> of the sibling elements after this node, in document order. 
        /// Only elements that have a matching <see cref="XName"/> are included in the collection.
        /// The returned object also implements <see cref="INotifyCollectionChanged"/>.</returns>
        public static IEnumerable<XElement> ElementsAfterSelf(XNode node, XName name)
        { return ElementsAfterSelf(node, ValueProvider.Static(name)); }

        /// <summary>
        /// Returns a filtered live collection of the sibling elements after this node, in
        /// document order. Only elements that have a matching <see cref="XName"/> are included in the collection.
        /// </summary>
        /// <param name="node">The <see cref="XNode"/> to return the siblings of.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> that represents the live name to match.</param>
        /// <returns>An <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> of the sibling elements after this node, in document order. 
        /// Only elements that have a matching <see cref="XName"/> are included in the collection.
        /// The returned object also implements <see cref="INotifyCollectionChanged"/>.</returns>
        public static IEnumerable<XElement> ElementsAfterSelf(XNode node, IValueProvider<XName> name)
        {
            return
                node == null ? null :
                node.Document == null || node.Parent == null ? node.ElementsAfterSelf(name.Value) :
                Elements(node.Parent, name).SkipWhile(sib => !object.ReferenceEquals(sib, node)).Skip(1);
        }

        /// <summary>
        /// Returns a live collection of the sibling elements before this node, in document order.
        /// </summary>
        /// <param name="node">The <see cref="XNode"/> to return the siblings of.</param>
        /// <returns>An <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> of the sibling elements before this node, in document order. The returned object also implements <see cref="INotifyCollectionChanged"/>.</returns>
        public static IEnumerable<XElement> ElementsBeforeSelf(XNode node)
        {
            return 
                node == null ? null :
                node.Document == null || node.Parent == null ? node.ElementsBeforeSelf() :
                Elements(node.Parent).TakeWhile(sib => !object.ReferenceEquals(sib, node));
        }

        /// <summary>
        /// Returns a filtered live collection of the sibling elements before this node, in
        /// document order. Only elements that have a matching <see cref="XName"/> are included in the collection.
        /// </summary>
        /// <param name="node">The <see cref="XNode"/> to return the siblings of.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>An <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> of the sibling elements before this node, in document order. 
        /// Only elements that have a matching <see cref="XName"/> are included in the collection.
        /// The returned object also implements <see cref="INotifyCollectionChanged"/>.</returns>
        public static IEnumerable<XElement> ElementsBeforeSelf(XNode node, XName name)
        { return ElementsBeforeSelf( node, ValueProvider.Static(name)); }

        /// <summary>
        /// Returns a filtered live collection of the sibling elements before this node, in
        /// document order. Only elements that have a matching <see cref="XName"/> are included in the collection.
        /// </summary>
        /// <param name="node">The <see cref="XNode"/> to return the siblings of.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> that represents the live name to match.</param>
        /// <returns>An <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> of the sibling elements before this node, in document order. 
        /// Only elements that have a matching <see cref="XName"/> are included in the collection.
        /// The returned object also implements <see cref="INotifyCollectionChanged"/>.</returns>
        public static IEnumerable<XElement> ElementsBeforeSelf(XNode node, IValueProvider<XName> name)
        {
            return
                node == null ? null :
                node.Document == null || node.Parent == null ? node.ElementsBeforeSelf(name.Value) :
                Elements(node.Parent, name).TakeWhile(sib => !object.ReferenceEquals(sib, node));
        }

        /// <summary>
        /// Returns a live sequence of nodes that contains all nodes in the source sequence, sorted in document order.
        /// </summary>
        /// <typeparam name="T">The type of the objects in source, constrained to <see cref="XNode"/></typeparam>
        /// <param name="nodes">The sequence as <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> to sort.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that contains all nodes in the source sequence, sorted in document order.</returns>
        public static IEnumerable<T> InDocumentOrder<T>(IEnumerable<T> nodes) where T : XNode
        { return nodes.OrderBy(n => n, XNode.DocumentOrderComparer); }

        /// <summary>
        /// Returns a live sequence of the sibling nodes after this node, in document order.
        /// </summary>
        /// <param name="node">The node whose siblings need to be returned.</param>
        /// <returns>An <see cref="IEnumerable{XNode}"/> of <see cref="XNode"/> of the sibling nodes after <paramref name="node"/>, in document order.</returns>
        public static IEnumerable<XNode> NodesAfterSelf(XNode node)
        {
            return 
                node == null ? null :
                node.Document == null || node.Parent == null ? node.NodesAfterSelf() :
                Nodes(node.Parent).SkipWhile(sib => !object.ReferenceEquals(sib, node)).Skip(1);
        }

        /// <summary>
        /// Returns a live sequence of the sibling nodes before this node, in document order.
        /// </summary>
        /// <param name="node">The node whose siblings need to be returned.</param>
        /// <returns>An <see cref="IEnumerable{XNode}"/> of <see cref="XNode"/> of the sibling nodes before <paramref name="node"/>, in document order.</returns>
        public static IEnumerable<XNode> NodesBeforeSelf(XNode node)
        {
            return 
                node == null ? null :
                node.Document == null || node.Parent == null ? node.NodesAfterSelf() :
                Nodes(node.Parent).TakeWhile(sib => !object.ReferenceEquals(sib, node));
        }

        /// <summary>
        /// Gets the next sibling node of this node live.
        /// </summary>
        /// <param name="node">The <see cref="XNode"/> to take the next sibling of.</param>
        /// <returns>An <see cref="IValueProvider{XNode}"/> of <see cref="XNode"/> whose Value property gives the next sibling node.</returns>
        public static IValueProvider<XNode> NextNode(XNode node)
        {
            return
                node == null ? null :
                node.Document == null ? ValueProvider.Static( node.NextNode ) :
                GetNextNodeProvider(node).ReadOnly();
        }

        /// <summary>
        /// Gets the previous sibling node of this node live.
        /// </summary>
        /// <param name="node">The <see cref="XNode"/> to take the previous sibling of.</param>
        /// <returns>An <see cref="IValueProvider{XNode}"/> of <see cref="XNode"/> whose Value property gives the previous sibling node.</returns>
        public static IValueProvider<XNode> PreviousNode(XNode node)
        {
            return
                node == null ? null :
                node.Document == null ? ValueProvider.Static( node.PreviousNode ) :
                GetPreviousNodeProvider(node).ReadOnly();
        }

        //XContainer

        static IEnumerable<XNode> _DescendantNodesAndSelf(XNode node)
        {
            var container = node as XContainer;
            var stat = ObservableEnumerable.Static(node);
            return container == null ? stat : stat.Concat( DescendantNodes(container) );
        }

        /// <summary>
        /// Returns a live sequence of the descendant nodes for this document or element, in document order.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> to take the descendant nodes of.</param>
        /// <returns>A live <see cref="IEnumerable{XNode}"/> of <see cref="XNode"/> containing the descendant nodes of <paramref name="container"/>, in document order.</returns>
        public static IEnumerable<XNode> DescendantNodes(XContainer container)
        { return Nodes(container).SelectMany( (Func<XNode,IEnumerable<XNode>>)_DescendantNodesAndSelf ) ; }

        /// <summary>
        /// Returns a live sequence of the descendant nodes of every document and element in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the objects in <paramref name="source"/>, constrained to <see cref="XContainer"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that contains the source sequence.</param>
        /// <returns>A live <see cref="IEnumerable{XNode}"/> of <see cref="XNode"/> of
        /// the descendant nodes of every document and element in the source sequence.</returns>
        public static IEnumerable<XNode> DescendantNodes<T>(IEnumerable<T> source) where T : XContainer
        { return source.SelectMany( c => DescendantNodes(c)); }

        /// <summary>
        /// Returns a live sequence of the descendant elements for this document or element, in document order.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> to take the descendant elements of.</param>
        /// <returns>An <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing the descendant elements of <paramref name="container"/>, in document order.</returns>
        public static IEnumerable<XElement> Descendants(XContainer container)
        { return DescendantNodes(container).OfType<XElement>(); }

        /// <summary>
        /// Returns a live sequence of elements that contains the descendant elements of every element and document in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the objects in <paramref name="source"/>, constrained to <see cref="XContainer"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that contains the source sequence.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// containing the descendant elements of every element and document in the source collection.</returns>
        public static IEnumerable<XElement> Descendants<T>(IEnumerable<T> source) where T : XContainer
        { return source.SelectMany(c => Descendants(c)); }

        /// <summary>
        /// Returns a live filtered sequence of the descendant elements for this document or element, in document order. 
        /// Only elements that have a matching <see cref="XName"/> are included in the sequence.
        /// </summary>
        /// <param name="container">The document or element to take the descendant element of.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing the descendant elements of '<paramref name="container"/>' that
        /// match '<paramref name="name"/>'.</returns>
        public static IEnumerable<XElement> Descendants(XContainer container, XName name)
        { return Descendants(container, ValueProvider.Static(name)); }

        /// <summary>
        /// Returns a live filtered sequence of the descendant elements for this document or element, in document order. 
        /// Only elements that have a matching <see cref="XName"/> are included in the sequence.
        /// </summary>
        /// <param name="container">The document or element to take the descendant element of.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> whose Value property gives the name to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing the descendant elements of '<paramref name="container"/>' that
        /// match '<paramref name="name"/>'.</returns>
        public static IEnumerable<XElement> Descendants(XContainer container, IValueProvider<XName> name)
        { return ElementFilter( Descendants(container), name ); }

        /// <summary>
        /// Returns a live sequence of elements that contains the descendant elements of every element and document in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the objects in '<paramref name="source"/>', constrained to System.Xml.Linq.XContainer.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that represents the source sequence.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// that contains the descendant elements of every element and document in '<paramref name="source"/>'. </returns>
        public static IEnumerable<XElement> Descendants<T>(IEnumerable<T> source, XName name) where T : XContainer
        { return Descendants(source, ValueProvider.Static(name)); }

        /// <summary>
        /// Returns a live sequence of elements that contains the descendant elements of every element and document in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the objects in '<paramref name="source"/>', constrained to System.Xml.Linq.XContainer.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that represents the source sequence.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> whose Value property gives the name to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// that contains the descendant elements of every element and document in '<paramref name="source"/>'. </returns>
        public static IEnumerable<XElement> Descendants<T>(IEnumerable<T> source, IValueProvider<XName> name) where T : XContainer
        { return ElementFilter(Descendants(source), name); }

        /// <summary>
        /// Gets the first (in document order) child element live of a given document or element with the specified <see cref="XName"/>.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> to search for the child element.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>An <see cref="IValueProvider{XElement}"/> of <see cref="XElement"/> whose Value property gives the element that matches '<paramref name="name"/>', or null</returns>
        public static IValueProvider<XElement> Element(XContainer container, XName name)
        { return Element(container, ValueProvider.Static(name)); }

        /// <summary>
        /// Gets the first (in document order) child element live of a given document or element with the specified <see cref="XName"/>.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> to search for the child element.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> whose Value property gives the name to match.</param>
        /// <returns>An <see cref="IValueProvider{XElement}"/> of <see cref="XElement"/> whose Value property gives the element that matches '<paramref name="name"/>', or null</returns>
        public static IValueProvider<XElement> Element(XContainer container, IValueProvider<XName> name)
        { return Elements(container).FirstOrDefault(elt => Name(elt).Select(name, (eltN, cN) => eltN == cN)); }

        /// <summary>
        /// Returns a live sequence of the child elements of this element or document, in document order.
        /// </summary>
        /// <param name="container">The element or document to take the child elements of.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing the child elements of '<paramref name="container"/>', in document order.</returns>
        public static IEnumerable<XElement> Elements(XContainer container)
        { return Nodes<XElement>(container); }

        /// <summary>
        /// Returns a live sequence of elements that contains the child elements of every element and document in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the objects in <paramref name="source"/>, constrained to <see cref="XContainer"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that contains the source sequence.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// containing the child elements of every element and document in the source collection.</returns>
        public static IEnumerable<XElement> Elements<T>(IEnumerable<T> source) where T : XContainer
        { return source.SelectMany( c => Elements(c) ); }


        static IEnumerable<XElement> ElementFilter(IEnumerable<XElement> elements, IValueProvider<XName> name)
        { return name == null ? null : elements.Where(elt => Name(elt).Select(name, (eltName, cName) => eltName == cName)); }



        /// <summary>
        /// Returns a live filtered sequence of the child elements for this document or element, in document order. 
        /// Only elements that have a matching <see cref="XName"/> are included in the sequence.
        /// </summary>
        /// <param name="container">The document or element to take the child elements of.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing the child elements of '<paramref name="container"/>' that
        /// match '<paramref name="name"/>'.</returns>
        public static IEnumerable<XElement> Elements(XContainer container, XName name)
        { return Elements(container, ValueProvider.Static(name)); }

        /// <summary>
        /// Returns a live filtered sequence of the child elements for this document or element, in document order. 
        /// Only elements that have a matching <see cref="XName"/> are included in the sequence.
        /// </summary>
        /// <param name="container">The document or element to take the child elements of.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> whose Value property gives the name to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing the child elements of '<paramref name="container"/>' that
        /// match '<paramref name="name"/>'.</returns>
        public static IEnumerable<XElement> Elements(XContainer container, IValueProvider<XName> name)
        { return ElementFilter( Elements(container), name ); }

        /// <summary>
        /// Returns a live sequence of elements that contains the child elements of every element and document in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the objects in '<paramref name="source"/>', constrained to System.Xml.Linq.XContainer.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that represents the source sequence.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// that contains the child elements of every element and document in '<paramref name="source"/>'. </returns>
        public static IEnumerable<XElement> Elements<T>(IEnumerable<T> source, XName name) where T : XContainer
        { return Elements(source, ValueProvider.Static(name)); }

        /// <summary>
        /// Returns a live sequence of elements that contains the child elements of every element and document in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the objects in '<paramref name="source"/>', constrained to System.Xml.Linq.XContainer.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that represents the source sequence.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> whose Value property gives the name to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// that contains the child elements of every element and document in '<paramref name="source"/>'. </returns>
        public static IEnumerable<XElement> Elements<T>(IEnumerable<T> source, IValueProvider<XName> name) where T : XContainer
        { return ElementFilter(Elements(source), name); }

        /// <summary>
        /// Returns a live sequence of the child nodes of this element or document, in document order.
        /// </summary>
        /// <param name="container">The element or document to take the child nodes of.</param>
        /// <returns>A live <see cref="IEnumerable{XNode}"/> of <see cref="XNode"/> containing the child nodes of '<paramref name="container"/>', in document order.</returns>
        public static IEnumerable<XNode> Nodes(XContainer source)
        {
            return
                source == null ? null :
                source.Document == null ? source.Nodes() :
                GetNodeProvider(source).Cascade();
        }

        static IEnumerable<TNode> Nodes<TNode>(XContainer container) 
        {
            return
                container == null ? null :
                container.Document == null ? SL.OfType<TNode>(container.Nodes()) :
                GetNodeProvider(container).Cascade().OfType<TNode>();
        }

        /// <summary>
        /// Returns a live sequence of elements that contains the child nodes of every element and document in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the objects in <paramref name="source"/>, constrained to <see cref="XContainer"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> that contains the source sequence.</param>
        /// <returns>A live <see cref="IEnumerable{XNode}"/> of <see cref="XNode"/>
        /// containing the child nodes of every element and document in the source collection.</returns>
        public static IEnumerable<XNode> Nodes<T>(IEnumerable<T> source) where T : XContainer
        { return source.SelectMany(c => Nodes(c)); }

        /// <summary>
        /// Get the first child node live of the give element or document.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> to get the first child node of.</param>
        /// <returns>An <see cref="IValueProvider{XNode}"/> of <see cref="XNode"/> whose Value property gives the first child node of '<paramref name="container"/>', or null.</returns>
        public static IValueProvider<XNode> FirstNode(XContainer container)
        { return Nodes(container).FirstOrDefault(); }

        /// <summary>
        /// Get the last child node live of the give element or document.
        /// </summary>
        /// <param name="container">The <see cref="XContainer"/> to get the last child node of.</param>
        /// <returns>An <see cref="IValueProvider{XNode}"/> of <see cref="XNode"/> whose Value property gives the last child node of '<paramref name="container"/>', or null.</returns>
        public static IValueProvider<XNode> LastNode(XContainer container)
        { return Nodes(container).LastOrDefault(); }

        //XElement

        /// <summary>
        /// Returns the <see cref="XAttribute"/> of the given <see cref="XElement"/> that has the specified <see cref="XName"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to return the attribute of.</param>
        /// <param name="name">THe <see cref="XName"/> to match.</param>
        /// <returns>An <see cref="IValueProvider{XAttribute}"/> of <see cref="XAttribute"/> whose Value property gives the attribute or null if it can not be found.</returns>
        public static IValueProvider<XAttribute> Attribute(XElement element, XName name)
        {
            return
                name == null ? null : Attributes(element).FirstOrDefault(att => att.Name == name);
        }

        /// <summary>
        /// Returns a live sequence of the attributes of the given element.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to return the attributes of.</param>
        /// <returns>
        /// An <see cref="IEnumerable{XAttribute}"/> of <see cref="XAttribute"/> that represents the live sequence of attributes from '<paramref name="element"/>'.</returns>
        public static IEnumerable<XAttribute> Attributes(XElement element)
        {
            return
                element == null ? null :
                element.Document == null ? element.Attributes() :
                GetAttributesProvider(element).Cascade();
        }

        /// <summary>
        /// Returns a live sequence of all attributes from all elements in a given sequence of elements.
        /// </summary>
        /// <param name="source">The sequence of elements whose attributes need to be returned.</param>
        /// <returns>An <see cref="IEnumerable{XAttribute}"/> of <see cref="XAttribute"/> that represents the live sequence of attributes from the elements of '<paramref name="source"/>'.</returns>
        public static IEnumerable<XAttribute> Attributes(IEnumerable<XElement> source)
        { return source.SelectMany(elt => Attributes(elt)); }

        /// <summary>
        /// Returns a live sequence of the attributes of the given element, filtered by a given <see cref="XName"/>.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to return the attributes of.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>
        /// An <see cref="IEnumerable{XAttribute}"/> of <see cref="XAttribute"/> that represents the live sequence of attributes from '<paramref name="element"/>', filtered by '<paramref name="name"/>'.</returns>
        public static IEnumerable<XAttribute> Attributes(XElement element, XName name)
        { return name == null ? null : Attributes(element).Where(att => att.Name == name); }

        /// <summary>
        /// Returns a live sequence of all attributes from all elements in a given sequence of elements, filtered by a given <see cref="XName"/>.
        /// </summary>
        /// <param name="source">The sequence of elements whose attributes need to be returned.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>An <see cref="IEnumerable{XAttribute}"/> of <see cref="XAttribute"/> that represents the live sequence of attributes from the elements of '<paramref name="source"/>', filtered by '<paramref name="name"/>'.</returns>
        public static IEnumerable<XAttribute> Attributes(IEnumerable<XElement> elements, XName name)
        { return name == null ? null : Attributes(elements).Where(att => att.Name == name); }

        /// <summary>
        /// Returns a live sequence containing the given element and the descendant nodes of the given element, in document order.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to take the descendant nodes of.</param>
        /// <returns>A live <see cref="IEnumerable{XNode}"/> of <see cref="XNode"/> containing <paramref name="element"/> and the descendant nodes of <paramref name="element"/>, in document order.</returns>
        public static IEnumerable<XNode> DescendantNodesAndSelf(XElement element)
        { return _DescendantNodesAndSelf(element); }

        /// <summary>
        /// Returns a live sequence of nodes that contains every element in the source sequence and the descendant nodes of every element in the source sequence, in document order.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <see cref="XElement"/> that contains the source sequence.</param>
        /// <returns>A live <see cref="IEnumerable{XNode}"/> of <see cref="XNode"/>
        /// containing every element and document in the source sequence and the descendant nodes of every element in the source sequence.</returns>
        public static IEnumerable<XNode> DescendantNodesAndSelf(IEnumerable<XElement> elements)
        { return elements.SelectMany( elt => DescendantNodesAndSelf(elt) ); }

        /// <summary>
        /// Returns a live sequence containing the given element and the descendant elements of the given element, in document order.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to take the descendant elements of.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing <paramref name="element"/> and the descendant nodes of <paramref name="element"/>, in document order.</returns>
        public static IEnumerable<XElement> DescendantsAndSelf(XElement element)
        { return ObservableEnumerable.Static(element).Concat(Descendants(element)); }

        /// <summary>
        /// Returns a live sequence of elements that contains every element in the source sequence and the descendant elements of every element in the source sequence, in document order.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <see cref="XElement"/> that contains the source sequence.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// containing every element and document in the source sequence and the descendant elements of every element in the source sequence.</returns>
        public static IEnumerable<XElement> DescendantsAndSelf(IEnumerable<XElement> source)
        { return source.SelectMany( elt => DescendantsAndSelf( elt ) ); }

        /// <summary>
        /// Returns a live sequence containing the given element and the descendant elements of the given element, in document order.
        /// The elements are filtered by <see cref="XName"/>.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to take the descendant elements of.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing <paramref name="element"/> and the descendant nodes of <paramref name="element"/>, in document order and filtered by <see cref="XName"/>.</returns>
        public static IEnumerable<XElement> DescendantsAndSelf(XElement element, XName name)
        { return DescendantsAndSelf(element, ValueProvider.Static(name)); }

        /// <summary>
        /// Returns a live sequence containing the given element and the descendant elements of the given element, in document order.
        /// The elements are filtered by <see cref="XName"/>.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to take the descendant elements of.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> whose Value property gives the name to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/> containing <paramref name="element"/> and the descendant nodes of <paramref name="element"/>, in document order and filtered by <see cref="XName"/>.</returns>
        public static IEnumerable<XElement> DescendantsAndSelf(XElement element, IValueProvider<XName> name)
        { return ElementFilter( DescendantsAndSelf(element), name ); }

        /// <summary>
        /// Returns a live sequence of elements that contains every element in the source sequence and the descendant elements of every element in the source sequence, in document order.
        /// The elements are filtered by <see cref="XName"/>.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <see cref="XElement"/> that contains the source sequence.</param>
        /// <param name="name">The <see cref="XName"/> to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// containing every element and document in the source sequence and the descendant elements of every element in the source sequence, filtered by <see cref="XName"/>.</returns>
        public static IEnumerable<XElement> DescendantsAndSelf(IEnumerable<XElement> source, XName name)
        { return DescendantsAndSelf(source, ValueProvider.Static(name)); }

        /// <summary>
        /// Returns a live sequence of elements that contains every element in the source sequence and the descendant elements of every element in the source sequence, in document order.
        /// The elements are filtered by <see cref="XName"/>.
        /// </summary>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of <see cref="XElement"/> that contains the source sequence.</param>
        /// <param name="name">An <see cref="IValueProvider{XName}"/> of <see cref="XName"/> whose Value property gives the name to match.</param>
        /// <returns>A live <see cref="IEnumerable{XElement}"/> of <see cref="XElement"/>
        /// containing every element and document in the source sequence and the descendant elements of every element in the source sequence, filtered by <see cref="XName"/>.</returns>
        public static IEnumerable<XElement> DescendantsAndSelf(IEnumerable<XElement> elements, IValueProvider<XName> name)
        { return ElementFilter( DescendantsAndSelf(elements), name ); }

        /// <summary>
        /// Gets the first attribute of the given element live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to get the first attribute of.</param>
        /// <returns>An <see cref="IValueProvider{XAttribute}"/> of <see cref="XAttribute"/> whose Value property gives the first attribute of this element.</returns>
        public static IValueProvider<XAttribute> FirstAttribute(XElement element)
        { return Attributes(element).FirstOrDefault(); }

        /// <summary>
        /// Gets a live value indicating if the given element has any attributes.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to check for attributes.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/> of <see cref="Boolean"/> whose Value property indicates if '<paramref name="element"/>' has any attributes.</returns>
        public static IValueProvider<bool> HasAttributes(XElement element)
        { return Attributes(element).Any(); }

        /// <summary>
        /// Gets a live value indicating if the given element has any child elements.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to check for child elements.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/> of <see cref="Boolean"/> whose Value property indicates if '<paramref name="element"/>' has any child elements.</returns>
        public static IValueProvider<bool> HasElements(XElement element)
        { return Elements(element).Any(); }

        /// <summary>
        /// Gets a live value indicating whether the given element contains no content.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to check for content.</param>
        /// <returns>An <see cref="IValueProvider{Boolean}"/> of <see cref="Boolean"/> whose Value property indicates if '<paramref name="element"/>' has any content.</returns>
        public static IValueProvider<bool> IsEmpty(XElement element)
        {
            return
                element == null ? null :
                element.Document == null ? ValueProvider.Static(element.IsEmpty) :
                GetElementValueProvider(element).Select(v => v == null);
        }

        /// <summary>
        /// Gets the last attribute of the given element live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to get the last attribute of.</param>
        /// <returns>An <see cref="IValueProvider{XAttribute}"/> of <see cref="XAttribute"/> whose Value property gives the last attribute of this element.</returns>
        public static IValueProvider<XAttribute> LastAttribute(XElement element)
        { return Attributes(element).LastOrDefault(); }

        /// <summary>
        /// Gets the live name of the given element.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to get the name of.</param>
        /// <returns>An <see cref="IValueProvider{XNeme}"/> of <see cref="XName"/> whose Value property gives the name of the element.</returns>
        public static IValueProvider<XName> Name(XElement element)
        {
            return
                element == null ? null :
                element.Document == null ? ValueProvider.Static(element.Name) :
                GetNameProvider(element).ReadOnly();
        }

        /// <summary>
        /// Gets the live value of the given element.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to get the value of.</param>
        /// <returns>An <see cref="IValueProvider{string}"/> of <see cref="string"/> whose Value property gives the value of the element.</returns>
        public static IValueProvider<string> Value(XElement element)
        {
            return
                element == null ? null :
                element.Document == null ? ValueProvider.Static(element.Value) :
                GetElementValueProvider(element).Coalesce(string.Empty);
        }


        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="Int32"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Int32"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<int?> AsNullableInt(XElement element)
        { return AsNullableInt(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="Int32"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Int32"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<int> AsInt(XElement element)
        { return AsInt(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="UInt32"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="UInt32"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<uint?> AsNullableUnsignedInt(XElement element)
        { return AsNullableUnsignedInt(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="UInt32"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="UInt32"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<uint> AsUnsignedInt(XElement element)
        { return AsUnsignedInt(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="Int64"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Int64"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<long?> AsNullableLong(XElement element)
        { return AsNullableLong(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="Int64"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Int64"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<long> AsLong(XElement element)
        { return AsLong(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="UInt64"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="UInt64"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<ulong?> AsNullableUnsignedLong(XElement element)
        { return AsNullableUnsignedLong(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="UInt64"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="UInt64"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<ulong> AsUnsignedLong(XElement element)
        { return AsUnsignedLong(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="Boolean"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Boolean"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<bool?> AsNullableBoolean(XElement element)
        { return AsNullableBoolean(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="Boolean"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Boolean"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<bool> AsBoolean(XElement element)
        { return AsBoolean(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="Guid"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Guid"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<Guid?> AsNullableGuid(XElement element)
        { return AsNullableGuid(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="Guid"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Guid"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<Guid> AsGuid(XElement element)
        { return AsGuid(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="Single"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Single"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<float?> AsNullableSingle(XElement element)
        { return AsNullableSingle(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="Single"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Single"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<float> AsSingle(XElement element)
        { return AsSingle(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="Double"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Double"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<double?> AsNullableDouble(XElement element)
        { return AsNullableDouble(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="Double"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Double"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<double> AsDouble(XElement element)
        { return AsDouble(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="Decimal"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="Decimal"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<decimal?> AsNullableDecimal(XElement element)
        { return AsNullableDecimal(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="Decimal"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="Decimal"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<decimal> AsDecimal(XElement element)
        { return AsDecimal(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="TimeSpan"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="TimeSpan"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<TimeSpan?> AsNullableTimeSpan(XElement element)
        { return AsNullableTimeSpan(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="TimeSpan"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="TimeSpan"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<TimeSpan> AsTimeSpan(XElement element)
        { return AsTimeSpan(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="DateTime"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of nullable <see cref="DateTime"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<DateTime?> AsNullableDateTime(XElement element)
        { return AsNullableDateTime(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="DateTime"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="DateTime"/> whose Value property give the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<DateTime> AsDateTime(XElement element)
        { return AsDateTime(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to a nullable <see cref="DateTimeOffset"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="DateTimeOffset"/> whose Value property give the content of '<paramref name="element"/>', or null if the cast fails.</returns>
        public static IValueProvider<DateTimeOffset?> AsNullableDateTimeOffset(XElement element)
        { return AsNullableDateTimeOffset(Value(element)); }

        /// <summary>
        /// Cast the value of a given <see cref="XElement"/> to an <see cref="DateTimeOffset"/> live.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to cast.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="DateTimeOffset"/> whose Value property gives the content of '<paramref name="element"/>', or the default value if the cast fails.</returns>
        public static IValueProvider<DateTimeOffset> AsDateTimeOffset(XElement element)
        { return AsDateTimeOffset(Value(element)); }

        /// <summary>
        /// Gives the live value of a given <see cref="XElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> whose value to return.</param>
        /// <returns>An <see cref="IValueProvider{T}"/> of <see cref="DateTimeOffset"/> whose Value property gives the content of '<paramref name="element"/>'.</returns>
        public static IValueProvider<string> AsString(XElement element)
        { return AsString(Value(element)); }


        //XDocument

        /// <summary>
        /// Gets the root element of the XML Tree for the given document live.
        /// </summary>
        /// <param name="document">The <see cref="XDocument"/> to get the root of.</param>
        /// <returns>An <see cref="IValueProvider{XElement}"/> of <see cref="XElement"/> whose Value property give the root of the document.</returns>
        public static IValueProvider<XElement> Root(XDocument document)
        {
            return
                document == null ? null :
                GetDocumentRootProvider(document).ReadOnly();
        }

        //XComment

        /// <summary>
        /// Gets the live value of the given comment node.
        /// </summary>
        /// <param name="comment">The <see cref="XComment"/> object to get the value of.</param>
        /// <returns>An <see cref="IValueProvider{string}"/> of <see cref="string"/> whose Value property gives the value of the comment node.</returns>
        public static IValueProvider<string> Value(XComment comment)
        {
            return
                comment == null ? null :
                comment.Document == null ? ValueProvider.Static(comment.Value) :
                GetCommentValueProvider(comment).ReadOnly();
        }

        //XDocumentType

        //XProcessingInstruction

        //XText

        /// <summary>
        /// Gets the live value of the given data or text node.
        /// </summary>
        /// <param name="comment">The <see cref="XText"/> object to get the value of.</param>
        /// <returns>An <see cref="IValueProvider{string}"/> of <see cref="string"/> whose Value property gives the value of the data or text node.</returns>
        public static IValueProvider<string> Value(XText text)
        {
            return
                text == null ? null :
                text.Document == null ? ValueProvider.Static(text.Value) :
                GetTextValueProvider(text).ReadOnly();
        }

        //XCData
    }
}
