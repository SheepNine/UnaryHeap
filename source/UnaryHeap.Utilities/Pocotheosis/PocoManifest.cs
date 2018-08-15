using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System;

namespace Pocotheosis
{
    public class PocoManifest
    {
        public static PocoNamespace Parse(TextReader input)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(input);
            return Parse(doc);
        }

        public static PocoNamespace Parse(XmlDocument input)
        {
            return ParseNamespace(input.SelectSingleNode("/namespace") as XmlElement);
        }

        static PocoNamespace ParseNamespace(XmlElement node)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing namespace name");

            var enums = ParseEnums(node);
            var classes = ParseClasses(node, enums);
            return new PocoNamespace(name, enums, classes);
        }

        static List<PocoEnum> ParseEnums(XmlElement node)
        {
            return node.SelectNodes("enums/enum")
                .Cast<XmlElement>()
                .Select(enumNode => ParseEnum(enumNode))
                .ToList();
        }

        static PocoEnum ParseEnum(XmlElement node)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing enum name");

            var enumerators = node.SelectNodes("enumerator")
                .Cast<XmlElement>()
                .Select(valueNode => ParseEnumerator(valueNode))
                .ToList();
            return new PocoEnum(name, enumerators);
        }

        static PocoEnumerator ParseEnumerator(XmlElement node)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing enumerator name");
            var valueText = node.GetAttribute("value");
            if (string.IsNullOrEmpty(valueText))
                throw new InvalidDataException(
                    string.Format("Enumerator {0} missing value", name));
            return new PocoEnumerator(name, int.Parse(valueText));
        }

        static List<PocoClass> ParseClasses(XmlElement node, List<PocoEnum> enums)
        {
            return node.SelectNodes("classes/class")
                .Cast<XmlElement>()
                .Select(classNode => ParseClass(classNode, enums))
                .ToList();
        }

        static PocoClass ParseClass(XmlElement node, List<PocoEnum> enums)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing class name");
            var idText = node.GetAttribute("id");
            if (string.IsNullOrEmpty(idText))
                throw new InvalidDataException(
                    string.Format("Class {0} missing identifier", name));
            var members = ParseMembers(node, enums);
            string[] routes = new string[0];
            if (node.HasAttribute("routes"))
            {
                routes = node.GetAttribute("routes")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return new PocoClass(name, int.Parse(idText), routes, members);
        }

        static List<IPocoMember> ParseMembers(XmlElement node, List<PocoEnum> enums)
        {
            return node.SelectNodes("members/member")
                .Cast<XmlElement>()
                .Select(memberNode => ParseMember(memberNode, enums))
                .ToList();
        }

        static IPocoMember ParseMember(XmlElement node, List<PocoEnum> enums)
        {
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type");
            return new PocoMember(name, ParseType(type, enums));
        }

        static IPocoType ParseType(string typeName, List<PocoEnum> enums)
        {
            if (typeName.EndsWith("[]"))
            {
                return new ArrayType(ParsePrimitiveType(
                    typeName.Substring(0, typeName.Length - 2), enums));
            }
            else if (typeName.Contains("->"))
            {
                var tokens = typeName.Split(new[] { "->" },
                    StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 2)
                    return new DictionaryType(ParsePrimitiveType(tokens[0], enums),
                        ParsePrimitiveType(tokens[1], enums));
                else
                    throw new ArgumentException("Invalid dictionary declaration");
            }
            else
            {
                return ParsePrimitiveType(typeName, enums);
            }
        }

        static PrimitiveType ParsePrimitiveType(string typeName, List<PocoEnum> enums)
        {
            foreach (var enume in enums)
                if (typeName.Equals(enume.Name))
                    return new EnumType(enume);

            switch (typeName)
            {
                case "bool":
                    return BoolType.Instance;
                case "byte":
                    return UInt8Type.Instance;
                case "short":
                    return Int16Type.Instance;
                case "int":
                    return Int32Type.Instance;
                case "long":
                    return Int64Type.Instance;
                case "sbyte":
                    return Int8Type.Instance;
                case "ushort":
                    return UInt16Type.Instance;
                case "uint":
                    return UInt32Type.Instance;
                case "ulong":
                    return UInt64Type.Instance;
                case "string":
                    return StringType.Instance;
                default:
                    return new ClassType(typeName);
            }
        }
    }
}
