using System.Collections.Generic;
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

        static List<PocoMember> ParseMembers(XmlElement node, List<PocoEnum> enums)
        {
            return node.SelectNodes("members/member")
                .Cast<XmlElement>()
                .Select(memberNode => ParseMember(memberNode, enums))
                .ToList();
        }

        static PocoMember ParseMember(XmlElement node, List<PocoEnum> enums)
        {
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type");

            foreach (var enume in enums)
                if (type.Equals(enume.Name))
                    return new EnumPocoMember(name, enume);

            switch (type)
            {
                case "bool":
                    return new BoolPocoMember(name);
                case "byte":
                    return new BytePocoMember(name);
                case "short":
                    return new Int16PocoMember(name);
                case "int":
                    return new Int32PocoMember(name);
                case "long":
                    return new Int64PocoMember(name);
                case "sbyte":
                    return new SBytePocoMember(name);
                case "ushort":
                    return new UInt16PocoMember(name);
                case "uint":
                    return new UInt32PocoMember(name);
                case "ulong":
                    return new UInt64PocoMember(name);
                case "string":
                    return new StringPocoMember(name);
                default:
                    throw new InvalidDataException("Unrecognized member type: " + type);
            }
        }
    }
}
