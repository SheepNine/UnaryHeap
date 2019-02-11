using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System;
using Pocotheosis.MemberTypes;
using System.Globalization;

namespace Pocotheosis
{
    public static class ManifestParser
    {
        public static PocoNamespace Parse(TextReader input)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(input);
            return Parse(doc);
        }

        static PocoNamespace Parse(XmlDocument input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

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

        static List<PocoEnumDefinition> ParseEnums(XmlElement node)
        {
            return node.SelectNodes("enums/enum")
                .Cast<XmlElement>()
                .Select(enumNode => ParseEnum(enumNode))
                .ToList();
        }

        static PocoEnumDefinition ParseEnum(XmlElement node)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing enum name");

            var enumerators = node.SelectNodes("enumerator")
                .Cast<XmlElement>()
                .Select(valueNode => ParseEnumerator(valueNode))
                .ToList();
            return new PocoEnumDefinition(name, enumerators);
        }

        static PocoEnumerator ParseEnumerator(XmlElement node)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing enumerator name");
            var valueText = node.GetAttribute("value");
            if (string.IsNullOrEmpty(valueText))
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Enumerator {0} missing value", name));
            return new PocoEnumerator(name,
                int.Parse(valueText, CultureInfo.InvariantCulture));
        }

        static List<PocoClass> ParseClasses(XmlElement node, List<PocoEnumDefinition> enums)
        {
            return node.SelectNodes("classes/class")
                .Cast<XmlElement>()
                .Select(classNode => ParseClass(classNode, enums))
                .ToList();
        }

        static PocoClass ParseClass(XmlElement node, List<PocoEnumDefinition> enums)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing class name");
            var idText = node.GetAttribute("id");
            if (string.IsNullOrEmpty(idText))
                throw new InvalidDataException(
                    string.Format(CultureInfo.InvariantCulture,
                    "Class {0} missing identifier", name));
            var members = ParseMembers(node, enums);
            return new PocoClass(name, int.Parse(idText, CultureInfo.InvariantCulture),
                members);
        }

        static List<IPocoMember> ParseMembers(XmlElement node, List<PocoEnumDefinition> enums)
        {
            return node.SelectNodes("members/member")
                .Cast<XmlElement>()
                .Select(memberNode => ParseMember(memberNode, enums))
                .ToList();
        }

        static IPocoMember ParseMember(XmlElement node, List<PocoEnumDefinition> enums)
        {
            var name = node.GetAttribute("name");
            var singularName = node.HasAttribute("singular") ?
                node.GetAttribute("singular") : name;
            var type = node.GetAttribute("type");
            return new PocoMember(name, singularName, ParseType(type, enums));
        }

        static IPocoType ParseType(string typeName, List<PocoEnumDefinition> enums)
        {
            if (typeName.EndsWith("[]", StringComparison.Ordinal))
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

        static Dictionary<string, PrimitiveType> baseTypes
            = new Dictionary<string, PrimitiveType>()
        {
                { "bool", BoolType.Instance },
                { "byte", UInt8Type.Instance },
                { "short", Int16Type.Instance },
                { "int", Int32Type.Instance },
                { "long", Int64Type.Instance },
                { "sbyte", Int8Type.Instance },
                { "ushort", UInt16Type.Instance },
                { "uint", UInt32Type.Instance },
                { "ulong", UInt64Type.Instance },
                { "string", StringType.Instance },
        };

        static PrimitiveType ParsePrimitiveType(string typeName, List<PocoEnumDefinition> enums)
        {
            var enumType = enums.FirstOrDefault(e => typeName.Equals(e.Name));
            if (enumType != null)
                return new EnumType(enumType);

            if (baseTypes.ContainsKey(typeName))
                return baseTypes[typeName];

            return new ClassType(typeName);
        }
    }
}
