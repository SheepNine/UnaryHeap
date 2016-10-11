﻿using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;

namespace Pocotheosis
{
    class PocoManifest
    {
        public static PocoNamespace Parse(StreamReader input)
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
            var enumerators = node.SelectNodes("enumerator")
                .Cast<XmlElement>()
                .Select(valueNode => ParseEnumerator(valueNode))
                .ToList();
            return new PocoEnum(name, enumerators);
        }

        static PocoEnumerator ParseEnumerator(XmlElement node)
        {
            var name = node.GetAttribute("name");
            var value = int.Parse(node.GetAttribute("value"));
            return new PocoEnumerator(name, value);
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
            var id = int.Parse(node.GetAttribute("id"));
            var members = ParseMembers(node, enums);
            return new PocoClass(name, id, members);
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
                return new ArrayType(ParseType(typeName.Substring(0, typeName.Length - 2), enums));

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
                    throw new InvalidDataException("Unrecognized member type: " + typeName);
            }
        }
    }
}
