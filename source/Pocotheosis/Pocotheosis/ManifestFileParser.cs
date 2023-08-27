using Pocotheosis.MemberTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Pocotheosis
{
    public static class ManifestParser
    {
        public static PocoNamespace Parse(TextReader input, DateTime lastWriteTimeUtc)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(input);
            return Parse(doc, lastWriteTimeUtc);
        }

        static PocoNamespace Parse(XmlDocument input, DateTime lastWriteTimeUtc)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return ParseNamespace(input.SelectSingleNode("/namespace") as XmlElement,
                lastWriteTimeUtc);
        }

        static PocoNamespace ParseNamespace(XmlElement node, DateTime lastWriteTimeUtc)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing namespace name");

            var enums = ParseEnums(node);
            foreach (var enume in enums)
            {
                foreach (var i in Enumerable.Range(0, enume.Enumerators.Count))
                    foreach (var j in Enumerable.Range(i + 1, enume.Enumerators.Count - i - 1))
                    {
                        if (enume.Enumerators[i].Name.Equals(enume.Enumerators[j].Name,
                                StringComparison.Ordinal))
                            throw new InvalidDataException(string.Format(
                                CultureInfo.InvariantCulture,
                                "Enum '{0}' enumerator '{1}' appears multiple times",
                                enume.Name, enume.Enumerators[i].Name
                                ));
                        if (enume.Enumerators[i].Value == enume.Enumerators[j].Value)
                            throw new InvalidDataException(string.Format(
                                CultureInfo.InvariantCulture,
                                "Enum '{0}' value '{1}' appears multiple times",
                                enume.Name, enume.Enumerators[i].Value
                                ));
                    }
            }

            foreach (var i in Enumerable.Range(0, enums.Count))
                foreach (var j in Enumerable.Range(i + 1, enums.Count - i - 1))
                {
                    if (enums[i].Name.Equals(enums[j].Name, StringComparison.Ordinal))
                        throw new InvalidDataException(string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum '{0}' appears multiple times",
                            enums[i].Name));
                }


            var classes = ParseClasses(node, enums);
            foreach (var i in Enumerable.Range(0, classes.Count))
                foreach (var j in Enumerable.Range(i + 1, classes.Count - i - 1))
                {
                    if (classes[i].Name.Equals(classes[j].Name, StringComparison.Ordinal))
                        throw new InvalidDataException(string.Format(
                            CultureInfo.InvariantCulture,
                            "Class '{0}' appears multiple times",
                            classes[i].Name));
                    if (classes[i].StreamingId == classes[j].StreamingId)
                        throw new InvalidDataException(string.Format(
                            CultureInfo.InvariantCulture,
                            "Streaming ID '{0}' appears multiple times",
                            classes[i].StreamingId));
                }

            return new PocoNamespace(name, lastWriteTimeUtc, enums, classes);
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
            if (IsReserved(name))
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Enum '{0}' has reserved keyword for a name", name));
            if (IsInvalid(name))
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Enum '{0}' has invalid identifier for a name", name));


            var enumerators = node.SelectNodes("enumerator")
                .Cast<XmlElement>()
                .Select(valueNode => ParseEnumerator(valueNode))
                .ToList();

            if (enumerators.Count == 0)
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Enum '{0}' has no enumerators", name));

            bool isBitField = false;
            if (node.HasAttribute("bitField"))
                isBitField = bool.Parse(node.GetAttribute("bitField"));

            return new PocoEnumDefinition(name, isBitField, enumerators);
        }

        static PocoEnumerator ParseEnumerator(XmlElement node)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing enumerator name");
            if (IsReserved(name))
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Enumerator '{0}' has reserved keyword for a name", name));
            if (IsInvalid(name))
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Enumerator '{0}' has invalid identifier for a name", name));

            var valueText = node.GetAttribute("value");
            if (string.IsNullOrEmpty(valueText))
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Enumerator {0} missing value", name));
            return new PocoEnumerator(name,
                int.Parse(valueText, CultureInfo.InvariantCulture));
        }

        static List<PocoClass> ParseClasses(XmlElement node, List<PocoEnumDefinition> enums)
        {
            var classTypePocos = new SortedSet<string>();
            var result = node.SelectNodes("classes/class")
                .Cast<XmlElement>()
                .Select(classNode => ParseClass(classNode, enums, classTypePocos))
                .ToList();
            classTypePocos.ExceptWith(result.Select(r => r.Name));
            if (classTypePocos.Count != 0)
                throw new InvalidDataException("No definition given for Poco type(s): "
                    + string.Join(", ", classTypePocos));
            return result;
        }

        static PocoClass ParseClass(XmlElement node, List<PocoEnumDefinition> enums,
            SortedSet<string> classTypePocos)
        {
            var name = node.GetAttribute("name");
            if (string.IsNullOrEmpty(name))
                throw new InvalidDataException("Missing class name");
            if (IsReserved(name))
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Class '{0}' has reserved keyword for a name", name));
            if (IsInvalid(name))
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                    "Class '{0}' has invalid identifier for a name", name));

            var idText = node.GetAttribute("id");
            if (string.IsNullOrEmpty(idText))
                throw new InvalidDataException(
                    string.Format(CultureInfo.InvariantCulture,
                    "Class {0} missing identifier", name));
            var members = ParseMembers(node, enums, classTypePocos);
            return new PocoClass(name, int.Parse(idText, CultureInfo.InvariantCulture),
                members);
        }

        static List<IPocoMember> ParseMembers(XmlElement node, List<PocoEnumDefinition> enums,
            SortedSet<string> classTypePocos)
        {
            var result = node.SelectNodes("members/member")
                .Cast<XmlElement>()
                .Select((memberNode, index) =>
                        ParseMember(memberNode, index, enums, classTypePocos))
                .ToList();

            for (int i = 0; i < result.Count; i++)
                for (int j = i + 1; j < result.Count; j++)
                    if (result[i].PublicMemberName.Equals(result[j].PublicMemberName,
                        StringComparison.Ordinal))
                    {
                        throw new InvalidDataException(
                            string.Format(CultureInfo.InvariantCulture,
                                "Class '{0}' has duplicate member name '{1}'",
                                node.GetAttribute("name"),
                                result[i].PublicMemberName));
                    }

            return result;
        }

        static IPocoMember ParseMember(XmlElement node, int index,
            List<PocoEnumDefinition> enums, SortedSet<string> classTypePocos)
        {
            var name = node.GetAttribute("name");
            var singularName = node.HasAttribute("singular") ?
                node.GetAttribute("singular") : name;
            var type = node.GetAttribute("type");
            var secretName = $"field{index}";
            return new PocoMember(name, singularName, secretName,
                ParseType(type, enums, classTypePocos));
        }

        static IPocoType ParseType(string typeName, List<PocoEnumDefinition> enums,
            SortedSet<string> classTypePocos)
        {
            if (typeName.EndsWith("[]", StringComparison.Ordinal))
            {
                return new ArrayType(ParsePrimitiveType(
                    typeName.Substring(0, typeName.Length - 2), enums, classTypePocos));
            }
            else if (typeName.Contains("->"))
            {
                var tokens = typeName.Split(new[] { "->" },
                    StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 2)
                {
                    var keyType = ParsePrimitiveType(tokens[0], enums, classTypePocos);
                    var valueType = ParsePrimitiveType(tokens[1], enums, classTypePocos);
                    if (keyType.IsNullable)
                        throw new InvalidDataException("Dictionary keys cannot be nullable");
                    if (keyType.IsComparable)
                        return new DictionaryType(keyType, valueType);
                    else
                        throw new InvalidDataException(tokens[0] +
                            " cannot be used as a dictionary key as it is not comparable");
                }
                else
                    throw new ArgumentException("Invalid dictionary declaration");
            }
            else
            {
                return ParsePrimitiveType(typeName, enums, classTypePocos);
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
                { "string?", NullableStringType.Instance },
        };

        static PrimitiveType ParsePrimitiveType(string baseTypeName,
            List<PocoEnumDefinition> enums, SortedSet<string> classTypePocos)
        {
            var nullable = false;
            var typeName = baseTypeName;
            if (typeName.EndsWith('?'))
            {
                typeName = typeName.Substring(0, typeName.Length - 1);
                nullable = true;
            }

            var enumType = enums.FirstOrDefault(e =>
                typeName.Equals(e.Name, StringComparison.Ordinal));
            if (enumType != null)
            {
                if (nullable)
                    throw new InvalidDataException("Nullable enums are not supported");
                return new EnumType(enumType);
            }

            if (baseTypes.ContainsKey(typeName))
            {
                if (nullable && typeName != "string")
                    throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
                        "Nullable {0}s are not supported", typeName));
                return baseTypes[baseTypeName];
            }

            if (typeName.Equals("float", StringComparison.Ordinal)
                    || typeName.Equals("double", StringComparison.Ordinal))
                throw new InvalidDataException(
                    "Floating-point types (float and double) are not supported");

            classTypePocos.Add(typeName);
            return new ClassType(typeName, nullable);
        }

        static SortedSet<string> reservedWords = new SortedSet<string>()
        {
            "abstract",  "as",         "base",      "bool",
            "break",     "byte",       "case",      "catch",
            "char",      "checked",    "class",     "const",
            "continue",  "decimal",    "default",   "delegate",
            "do",        "double",     "else",      "enum",
            "event",     "explicit",   "extern",    "false",
            "finally",   "fixed",      "float",     "for",
            "foreach",   "goto",       "if",        "implicit",
            "in",        "int",        "interface", "internal",
            "is",        "lock",       "long",      "namespace",
            "new",       "null",       "object",    "operator",
            "out",       "override",   "params",    "private",
            "protected", "public",     "readonly",  "ref",
            "return",    "sbyte",      "sealed",    "short",
            "sizeof",    "stackalloc", "static",    "string",
            "struct",    "switch",     "this",      "throw",
            "true",      "try",        "typeof",    "uint",
            "ulong",     "unchecked",  "unsafe",    "ushort",
            "using",     "virtual",    "void",      "volatile",
            "while",
        };

        private static bool IsReserved(string name)
        {
            return reservedWords.Contains(name);
        }

        private static bool IsInvalid(string name)
        {
            return !Regex.IsMatch(name, "^[_\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}]"
                + "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}]*$");
        }
    }
}
