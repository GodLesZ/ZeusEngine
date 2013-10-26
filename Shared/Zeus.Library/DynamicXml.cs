using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace Zeus.Library {

    /// <summary>
    ///     Expose a xml document as a <see cref="DynamicObject" />
    /// </summary>
    public class DynamicXml {

        protected List<string> _knownLists;


        public DynamicXml() {
            _knownLists = new List<string>();
        }


        /// <summary>
        /// Creates and returns a new <see cref="DynamicObject"/> from the given xml <paramref name="node"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Dynamic object</returns>
        public static dynamic Parse(XElement node) {
            var xml = new DynamicXml();
            dynamic obj = new ExpandoObject();
            xml.Parse(obj, node);
            return obj;
        }


        protected void Parse(dynamic obj, XElement node) {

            IEnumerable<XElement> sorted = 
                from XElement elt in node.Elements() 
                orderby node.Elements(elt.Name.LocalName).Count() descending
                select elt;

            if (node.HasElements) {
                int nodeCount = node.Elements(sorted.First().Name.LocalName).Count();
                bool foundNode = false;
                if (_knownLists.Count > 0) {
                    foundNode = 
                        (from XElement el in node.Elements() 
                         where _knownLists.Contains(el.Name.LocalName) 
                         select el
                         ).Any();
                }

                // At least one of the child elements is a list
                if (nodeCount > 1 || foundNode) {
                    var item = new ExpandoObject();
                    List<dynamic> list = null;
                    string elementName = string.Empty;
                    foreach (var element in sorted) {
                        if (element.Name.LocalName != elementName) {
                            list = new List<dynamic>();
                            elementName = element.Name.LocalName;
                        }

                        if (element.HasElements || _knownLists.Contains(element.Name.LocalName)) {
                            Parse(list, element);
                            AddProperty(item, element.Name.LocalName, list);
                        } else {
                            Parse(item, element);
                        }
                    }

                    foreach (var attribute in node.Attributes()) {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    AddProperty(obj, node.Name.ToString(), item);
                } else {
                    var item = new ExpandoObject();

                    foreach (var attribute in node.Attributes()) {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    // Element
                    foreach (var element in sorted) {
                        Parse(item, element);
                    }

                    AddProperty(obj, node.Name.ToString(), item);
                }
            } else {
                AddProperty(obj, node.Name.ToString(), node.Value.Trim());
            }
        }

        protected static void AddProperty(dynamic parent, string name, object value) {
            if (parent is List<dynamic>) {
                (parent as List<dynamic>).Add(value);
            } else {
                ((IDictionary<string, object>) parent)[name] = value;
            }
        }

    }

}