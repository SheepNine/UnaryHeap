using System;
using System.IO;

namespace Pocotheosis
{
    public interface IPocoMember
    {
        string PublicMemberName();
        string BackingStoreName();
        string TempVarName();
        string BuilderReifier();
        void WritePublicMemberDeclaration(TextWriter output);
        void WriteBackingStoreDeclaration(TextWriter output);
        void WriteFormalParameter(TextWriter output);
        void WriteAssignment(TextWriter output);
        string EqualityTester();
        string Hasher();
        string Deserializer();
        string Serializer();
        string ToStringer();
        void WriteConstructorCheck(TextWriter output);
        void WriteBuilderDeclaration(TextWriter output);
        void WriteBuilderAssignment(TextWriter output);
        void WriteBuilderPlumbing(TextWriter output);
    }

    class PocoMember : IPocoMember
    {
        string name;
        string singularName;
        IPocoType type;

        public PocoMember(string variableName, string singularName, IPocoType type)
        {
            this.name = variableName;
            this.singularName = singularName;
            this.type = type;
        }

        public string PublicMemberName()
        {
            return type.PublicMemberName(name);
        }

        public string BackingStoreName()
        {
            return type.BackingStoreName(name);
        }

        public string TempVarName()
        {
            return type.TempVarName(name);
        }

        public string BuilderReifier()
        {
            return type.BuilderReifier(type.BackingStoreName(name));
        }

        public void WriteAssignment(TextWriter output)
        {
            type.WriteAssignment(name, output);
        }

        public void WritePublicMemberDeclaration(TextWriter output)
        {
            type.WritePublicMemberDeclaration(name, output);
        }

        public void WriteBackingStoreDeclaration(TextWriter output)
        {
            type.WriteBackingStoreDeclaration(name, output);
        }

        public string Deserializer()
        {
            return type.GetDeserializer(name);
        }

        public string EqualityTester()
        {
            return type.GetEqualityTester(name);
        }

        public void WriteFormalParameter(TextWriter output)
        {
            type.WriteFormalParameter(name, output);
        }

        public string Hasher()
        {
            return type.GetHasher(name);
        }

        public string Serializer()
        {
            return type.GetSerializer(name);
        }

        public string ToStringer()
        {
            return type.ToStringOutput(type.BackingStoreName(name));
        }

        public void WriteConstructorCheck(TextWriter output)
        {
            type.WriteConstructorCheck(name, output);
        }

        public void WriteBuilderDeclaration(TextWriter output)
        {
            type.WriteBuilderDeclaration(name, output);
        }

        public void WriteBuilderAssignment(TextWriter output)
        {
            type.WriteBuilderAssignment(name, output);
        }

        public void WriteBuilderPlumbing(TextWriter output)
        {
            type.WriteBuilderPlumbing(name, singularName, output);
        }
    }

    interface IPocoType
    {
        string PublicMemberName(string variableName);
        string BackingStoreName(string variableName);
        string TempVarName(string variableName);
        string BuilderReifier(string variableName);
        void WritePublicMemberDeclaration(string variableName, TextWriter output);
        void WriteBackingStoreDeclaration(string variableName, TextWriter output);
        void WriteFormalParameter(string variableName, TextWriter output);
        void WriteAssignment(string variableName, TextWriter output);
        string GetEqualityTester(string variableName);
        string GetHasher(string variableName);
        string GetDeserializer(string variableName);
        string GetSerializer(string variableName);
        string ToStringOutput(string variableName);
        void WriteConstructorCheck(string variableName, TextWriter output);
        void WriteBuilderDeclaration(string variableName, TextWriter output);
        void WriteBuilderAssignment(string variableName, TextWriter output);
        void WriteBuilderPlumbing(string variableName, string singularName, TextWriter output);
    }
}
