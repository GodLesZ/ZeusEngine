using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Zeus.Library.Extensions {

    public static class YamlExtensions {

        public static dynamic ToExpando(this IDictionary<YamlNode, YamlNode> dict) {
            dynamic expando = new ExpandoObject();
            var expandoMap = (IDictionary<string, object>)expando;

            foreach (var entry in dict) {
                var node = entry.Value;

                var dictKey = entry.Key.ToString();
                dynamic dictValue;

                if (node is YamlScalarNode) {
                    // @TODO: How to detect string/int more safe ?
                    var scalarNode = node as YamlScalarNode;
                    if (scalarNode.Style == ScalarStyle.Plain) {
                        int intScalar;
                        float floatScalar;
                        if (int.TryParse(scalarNode.Value, out intScalar)) {
                            dictValue = intScalar;
                        } else if (float.TryParse(scalarNode.Value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out floatScalar)) {
                            dictValue = floatScalar;
                        } else {
                            // Fallback to string
                            dictValue = node.ToString();
                        }
                    } else {
                        dictValue = node.ToString();
                    }
                } else if (node is YamlSequenceNode) {
                    throw new NotImplementedException("Sub-nodes of type \"YamlSequenceNode\" are not implemented yet");
                } else if (node is YamlMappingNode) {
                    dictValue = ((YamlMappingNode)node).Children.ToExpando();
                } else {
                    throw new NotImplementedException("Unknown sub-node of type \"" + node.GetType() + "\"");
                }

                expandoMap[dictKey] = dictValue;
            }

            return expando;
        }

    }

}