using System;
using System.IO;
using System.Linq;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteJsonSerializationFile(PocoNamespace dataModel,
            string outputFileName)
        {
            if (OutputUpToDate(dataModel, outputFileName)) return;

            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);

                file.WriteLine("\tpublic abstract partial class Poco");
                file.WriteLine("\t{");
                file.WriteLine("\t\tpublic virtual void Serialize("
                    + "global::Newtonsoft.Json.JsonWriter output)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tthrow new global::System.NotImplementedException();");
                file.WriteLine("\t\t}");
                file.WriteLine("\t}");

                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    WriteClassJsonSerializationDeclaration(pocoClass, file);
                }
                WriteNamespaceFooter(file);
            }
        }

        private static void WriteClassJsonSerializationDeclaration(PocoClass clasz,
            TextWriter output)
        {
            output.WriteLine("\tpublic partial class {0}", clasz.Name);
            output.WriteLine("\t{");

            output.WriteLine(
                "\t\tpublic override void Serialize(global::Newtonsoft.Json.JsonWriter output)");
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\toutput.WriteStartObject();");
            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\t// Serialize {0}", member.PublicMemberName());
                output.WriteLine("\t\t\toutput.WritePropertyName(\"{0}\");",
                    member.PublicMemberName());
                output.WriteLine("\t\t\t{0}", member.JsonSerializer());
            }
            output.WriteLine("\t\t\toutput.WriteEndObject();");
            output.WriteLine("\t\t}");

            output.WriteLine();

            output.WriteLine(
                "\t\tpublic static {0} Deserialize(global::Newtonsoft.Json.JsonReader input)",
                clasz.Name);
            output.WriteLine("\t\t{");

            output.WriteLine("\t\t\tif (input.TokenType == Newtonsoft.Json.JsonToken.None)");
            output.WriteLine("\t\t\t\tif (!input.Read())");
            output.WriteLine("\t\t\t\t\tthrow new global::System.Exception("
                + "\"Unexpected end of stream\");");
            output.WriteLine();

            output.WriteLine("\t\t\tif (input.TokenType != "
                + "global::Newtonsoft.Json.JsonToken.StartObject)");
            output.WriteLine("\t\t\t\tthrow new global::System.Exception("
                + "\"Expected start of object\");");
            output.WriteLine();

            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\t{0} = default;", member.FormalParameter());
            }
            output.WriteLine();

            output.WriteLine("\t\t\twhile (input.Read())");
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tif (input.TokenType == "
                + "global::Newtonsoft.Json.JsonToken.EndObject)");
            output.WriteLine("\t\t\t\t\tbreak;");
            output.WriteLine("\t\t\t\telse if (input.TokenType == "
                + "global::Newtonsoft.Json.JsonToken.PropertyName)");
            output.WriteLine("\t\t\t\t{");
            output.WriteLine("\t\t\t\t\tvar propertyName = (string)input.Value;");
            output.WriteLine("\t\t\t\t\tif (!input.Read())");
            output.WriteLine("\t\t\t\t\t\tthrow new global::System.Exception("
                + "\"Unexpected end of stream\");");
            output.WriteLine("\t\t\t\t\tswitch (propertyName)");
            output.WriteLine("\t\t\t\t\t{");
            output.WriteLine("\t\t\t\t\t\t// PROPERTY READS");
            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\t\t\t\tcase \"{0}\":", member.PublicMemberName());
                output.WriteLine("\t\t\t\t\t\t\t{0}", member.JsonDeserializer());
                output.WriteLine("\t\t\t\t\t\t\tbreak;");
            }
            output.WriteLine("\t\t\t\t\t\tdefault:");
            output.WriteLine("\t\t\t\t\t\t\tthrow new global::System.Exception("
                + "\"Unexpected property \" + input.Value);");
            output.WriteLine("\t\t\t\t\t}");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t\telse");
            output.WriteLine("\t\t\t\t\tthrow new global::System.Exception("
                + "\"Expected property name\");");
            output.WriteLine("\t\t\t}");

            output.WriteLine("\t\t\treturn new {0}({1});", clasz.Name,
                string.Join(", ", clasz.Members.Select(member => member.TempVarName())));

            output.WriteLine("\t\t}");

            output.WriteLine("\t}");
        }
    }
}
