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

                file.WriteLine("public static partial class JsonSerializationHelpers {");

                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    WriteClassJsonSerializationDeclaration(pocoClass, file);
                }

                file.WriteLine("}");

                WriteNamespaceFooter(file);
            }
        }

        private static void WriteClassJsonSerializationDeclaration(PocoClass clasz,
            TextWriter output)
        {
            output.WriteLine(
                "\t\tpublic static void Serialize(this {0} @this, global::Newtonsoft.Json.JsonWriter output)", clasz.Name);
            output.WriteLine("\t\t{");
            output.WriteLine("\t\t\toutput.WriteStartObject();");
            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\toutput.WritePropertyName(\"{0}\");",
                    member.PublicMemberName());
                output.WriteLine("\t\t\t{0}", member.JsonSerializer());
            }
            output.WriteLine("\t\t\toutput.WriteEndObject();");
            output.WriteLine("\t\t}");

            output.WriteLine();

            output.WriteLine(
                "\t\tpublic static {0} Deserialize{0}(global::Newtonsoft.Json.JsonReader input)",
                clasz.Name);
            output.WriteLine("\t\t{");

            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\t{0} = default;", member.FormalParameter());
            }
            output.WriteLine();
            output.WriteLine("\t\t\tWarmReader(input);");
            output.WriteLine("\t\t\tIterateObject(input, () =>");
            output.WriteLine("\t\t\t{");
            output.WriteLine("\t\t\t\tvar propertyName = GetPropertyName(input);");
            output.WriteLine("\t\t\t\tswitch (propertyName)");
            output.WriteLine("\t\t\t\t{");
            foreach (var member in clasz.Members)
            {
                output.WriteLine("\t\t\t\t\tcase \"{0}\":", member.PublicMemberName());
                output.WriteLine("\t\t\t\t\t\t{0}", member.JsonDeserializer());
                output.WriteLine("\t\t\t\t\t\tbreak;");
            }
            output.WriteLine("\t\t\t\t\tdefault:");
            output.WriteLine("\t\t\t\t\t\tthrow new global::System.Exception("
                + "\"Unexpected property \" + input.Value);");
            output.WriteLine("\t\t\t\t}");
            output.WriteLine("\t\t\t});");

            output.WriteLine("\t\t\treturn new {0}({1});", clasz.Name,
                string.Join(", ", clasz.Members.Select(member => member.TempVarName())));

            output.WriteLine("\t\t}");
        }
    }
}
