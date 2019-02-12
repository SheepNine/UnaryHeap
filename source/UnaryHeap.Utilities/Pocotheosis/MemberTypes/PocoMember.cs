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
        string PublicMemberDeclaration();
        string BackingStoreDeclaration();
        string FormalParameter();
        string Assignment();
        string EqualityTester();
        string Hasher();
        string Deserializer();
        string Serializer();
        string ToStringer();
        string ConstructorCheck();
        string BuilderDeclaration();
        string BuilderAssignment();
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

        public string Assignment()
        {
            return type.Assignment(name);
        }

        public string PublicMemberDeclaration()
        {
            return type.PublicMemberDeclaration(name);
        }

        public string BackingStoreDeclaration()
        {
            return type.BackingStoreDeclaration(name);
        }

        public string Deserializer()
        {
            return type.GetDeserializer(name);
        }

        public string EqualityTester()
        {
            return type.GetEqualityTester(name);
        }

        public string FormalParameter()
        {
            return type.FormalParameter(name);
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

        public string ConstructorCheck()
        {
            return type.ConstructorCheck(name);
        }

        public string BuilderDeclaration()
        {
            return type.BuilderDeclaration(name);
        }

        public string BuilderAssignment()
        {
            return type.BuilderAssignment(name);
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
        string PublicMemberDeclaration(string variableName);
        string BackingStoreDeclaration(string variableName);
        string FormalParameter(string variableName);
        string Assignment(string variableName);
        string GetEqualityTester(string variableName);
        string GetHasher(string variableName);
        string GetDeserializer(string variableName);
        string GetSerializer(string variableName);
        string ToStringOutput(string variableName);
        string ConstructorCheck(string variableName);
        string BuilderDeclaration(string variableName);
        string BuilderAssignment(string variableName);
        void WriteBuilderPlumbing(string variableName, string singularName, TextWriter output);
    }
}
