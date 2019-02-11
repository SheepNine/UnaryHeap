using System.Globalization;
using System.IO;

namespace Pocotheosis
{
    static partial class Generator
    {
        public static void WriteDefinitionFile(PocoNamespace dataModel,
            string outputFileName)
        {
            using (var file = File.CreateText(outputFileName))
            {
                WriteNamespaceHeader(dataModel, file);
                WriteConstructorHelperClass(file, dataModel);

                file.WriteLine("\tpublic abstract partial class Poco");
                file.WriteLine("\t{");
                file.WriteLine("\t}");

                foreach (var pocoClass in dataModel.Classes)
                {
                    file.WriteLine();
                    WriteClassDeclaration(pocoClass, file);
                }

                foreach (var pocoEnum in dataModel.Enums)
                {
                    file.WriteLine();
                    WriteEnumDeclaration(pocoEnum, file);
                }
                WriteNamespaceFooter(file);
            }
        }

        static void WriteClassDeclaration(PocoClass clasz, TextWriter output)
        {
            output.Write("\tpublic partial class ");
            output.Write(clasz.Name);
            output.WriteLine(" : Poco");
            output.WriteLine("\t{");
            foreach (var member in clasz.Members)
            {
                output.Write("\t\t");
                member.WritePublicMemberDeclaration(output);
                output.WriteLine();
                output.Write("\t\t");
                member.WriteBackingStoreDeclaration(output);
                output.WriteLine();
            }
            output.WriteLine();
            output.Write("\t\tpublic " + clasz.Name + "(");
            var first = true;
            foreach (var member in clasz.Members)
            {
                if (!first)
                {
                    output.Write(", ");
                }
                first = false;

                member.WriteFormalParameter(output);
            }
            output.WriteLine(")");
            output.WriteLine("\t\t{");

            foreach (var member in clasz.Members)
                member.WriteConstructorCheck(output);

            foreach (var member in clasz.Members)
            {
                output.Write("\t\t\t");
                member.WriteAssignment(output);
                output.WriteLine();
            }
            output.WriteLine("\t\t}");
            output.WriteLine("\t}");
        }

        static void WriteEnumDeclaration(PocoEnumDefinition enume, StreamWriter file)
        {
            file.WriteLine("\tpublic enum " + enume.Name);
            file.WriteLine("\t{");
            foreach (var enumerator in enume.Enumerators)
                file.WriteLine(string.Format(CultureInfo.InvariantCulture, 
                    "\t\t{0} = {1},",
                    enumerator.Name, enumerator.Value));
            file.WriteLine("\t}");
        }

        static void WriteConstructorHelperClass(TextWriter output,
    PocoNamespace dataModel)
        {
            output.WriteLine(@"
    static class ConstructorHelper
    {
        public static bool CheckValue(bool value)
        {
            return true;
        }
        public static bool CheckValue(string value)
        {
            return value != null;
        }
        public static bool CheckValue(byte value)
        {
            return true;
        }
        public static bool CheckValue(ushort value)
        {
            return true;
        }
        public static bool CheckValue(uint value)
        {
            return true;
        }
        public static bool CheckValue(ulong value)
        {
            return true;
        }
        public static bool CheckValue(sbyte value)
        {
            return true;
        }
        public static bool CheckValue(short value)
        {
            return true;
        }
        public static bool CheckValue(int value)
        {
            return true;
        }
        public static bool CheckValue(long value)
        {
            return true;
        }");

            foreach (var enume in dataModel.Enums)
            {
                output.WriteLine(string.Format(CultureInfo.InvariantCulture, 
                    "        public static bool CheckValue("
                    + "{0} value) "
                    + "{{ return true; }}", enume.Name));
            }

            foreach (var classe in dataModel.Classes)
            {
                output.WriteLine(string.Format(CultureInfo.InvariantCulture, 
                    "        public static bool CheckValue("
                    + "{0} value) "
                    + "{{ return value != null; }}", classe.Name));
            }

            output.WriteLine(@"        public static bool CheckArrayValue<T>(
            global::System.Collections.Generic.IEnumerable<T> memberValues,
            global::System.Func<T, bool> memberChecker)
        {
            if (memberValues == null)
                return false;
            foreach (var memberValue in memberValues)
                if (!memberChecker(memberValue))
                    return false;
            return true;
        }

        public static bool CheckDictionaryValue<TKey, TValue>(
            global::System.Collections.Generic.IDictionary<TKey, TValue> memberValues,
            global::System.Func<TKey, bool> keyChecker,
            global::System.Func<TValue, bool> valueChecker)
        {
            if (memberValues == null)
                return false;
            foreach (var memberValue in memberValues)
                if (!keyChecker(memberValue.Key) || !valueChecker(memberValue.Value))
                    return false;
            return true;
        }
    }

    class ListWrapper<T> : global::System.Collections.Generic.IReadOnlyList<T>
    {
        private global::System.Collections.Generic.IList<T> wrappedObject;

        public ListWrapper(global::System.Collections.Generic.IList<T> wrappedObject)
        {
            this.wrappedObject = wrappedObject;
        }

        public T this[int index]
        {
            get { return wrappedObject[index]; }
        }

        public int Count
        {
            get { return wrappedObject.Count; }
        }

        public global::System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return wrappedObject.GetEnumerator();
        }

        global::System.Collections.IEnumerator
            global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DictionaryWrapper<TKey, TValue>
        : global::System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>
    {
        private global::System.Collections.Generic.IDictionary<TKey, TValue> wrappedObject;

        public DictionaryWrapper(
            global::System.Collections.Generic.IDictionary<TKey, TValue> wrappedObject)
        {
            this.wrappedObject = wrappedObject;
        }

        public TValue this[TKey key]
        {
            get { return wrappedObject[key]; }
        }

        public int Count
        {
            get { return wrappedObject.Count; }
        }

        public global::System.Collections.Generic.IEnumerable<TKey> Keys
        {
            get { return wrappedObject.Keys; }
        }

        public global::System.Collections.Generic.IEnumerable<TValue> Values
        {
            get { return wrappedObject.Values; }
        }

        public bool ContainsKey(TKey key)
        {
            return wrappedObject.ContainsKey(key);
        }

        public global::System.Collections.Generic.IEnumerator<
            global::System.Collections.Generic.KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return wrappedObject.GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return wrappedObject.TryGetValue(key, out value);
        }

        global::System.Collections.IEnumerator
            global::System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
");
        }
    }
}
