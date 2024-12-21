using System.IO;

namespace Pocotheosis
{
    public interface IPocoMember
    {
        bool NeedsConstructorCheck { get; }
        string PublicMemberName { get; }
        string BackingStoreName { get; }
        string FormalParameterType { get; }

        string BuilderReifier();
        string PublicMemberDeclaration();
        string BackingStoreDeclaration();
        string[] Assignment();
        string EqualityTester();
        string Hasher();
        string Deserializer();
        string JsonDeserializer();
        string Serializer();
        string JsonSerializer();
        string ToStringOutput();
        string InputCheck();
        string BuilderDeclaration();
        string BuilderAssignment();
        void WriteBuilderPlumbing(TextWriter output);
    }

    sealed class PocoMember : IPocoMember
    {
        readonly string name;
        readonly string singularName;
        readonly IPocoType type;
        readonly string secretName;

        public PocoMember(string variableName, string singularName,
            string secretName, IPocoType type)
        {
            this.name = variableName;
            this.singularName = singularName;
            this.type = type;
            this.secretName = secretName;
        }

        public bool NeedsConstructorCheck
        {
            get { return type.NeedsConstructorCheck; }
        }

        public string PublicMemberName
        {
            get { return name; }
        }

        public string BackingStoreName
        {
            get { return secretName; }
        }

        public string FormalParameterType
        {
            get { return type.FormalParameterType; }
        }

        public string BuilderReifier()
        {
            return type.BuilderReifier(secretName);
        }

        public string[] Assignment()
        {
            return type.Assignment(name, secretName);
        }

        public string PublicMemberDeclaration()
        {
            return type.PublicMemberDeclaration(name, secretName);
        }

        public string BackingStoreDeclaration()
        {
            return type.BackingStoreDeclaration(name, secretName);
        }

        public string Deserializer()
        {
            return type.GetDeserializer(secretName);
        }

        public string JsonDeserializer()
        {
            return type.GetJsonDeserializer(name);
        }

        public string EqualityTester()
        {
            return type.GetEqualityTester(name, secretName);
        }

        public string Hasher()
        {
            return type.GetHasher(name, secretName);
        }

        public string Serializer()
        {
            return type.GetSerializer(name, secretName);
        }

        public string JsonSerializer()
        {
            return type.GetJsonSerializer(name);
        }

        public string ToStringOutput()
        {
            return type.ToStringOutput($"poco.{name}");
        }

        public string InputCheck()
        {
            return type.InputCheck(name);
        }

        public string BuilderDeclaration()
        {
            return type.BuilderDeclaration(name, secretName);
        }

        public string BuilderAssignment()
        {
            return type.BuilderAssignment(name, secretName);
        }

        public void WriteBuilderPlumbing(TextWriter output)
        {
            type.WriteBuilderPlumbing(name, singularName, secretName, output);
        }
    }

    interface IPocoType
    {
        bool IsComparable { get; }
        bool NeedsConstructorCheck { get; }
        string FormalParameterType { get; }


        string InputCheck(string variableName);
        string BuilderReifier(string privateName);
        string PublicMemberDeclaration(string variableName, string privateName);
        string BackingStoreDeclaration(string variableName, string privateName);
        string[] Assignment(string variableName, string privateName);
        string GetEqualityTester(string variableName, string privateName);
        string GetHasher(string variableName, string privateName);
        string GetDeserializer(string privateName);
        string GetJsonDeserializer(string variableName);
        string GetSerializer(string variableName, string privateName);
        string GetJsonSerializer(string variableName);
        string ToStringOutput(string variableName);
        string BuilderDeclaration(string variableName, string privateName);
        string BuilderAssignment(string variableName, string privateName);
        void WriteBuilderPlumbing(string variableName, string singularName,
                string privateName, TextWriter output);
    }
}
