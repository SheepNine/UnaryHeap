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
        void WriteEqualityComparison(TextWriter output);
        void WriteHash(TextWriter output);
        void WriteDeserialization(TextWriter output);
        void WriteSerialization(TextWriter output);
        void WriteToStringOutput(TextWriter output);
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

        public void WriteDeserialization(TextWriter output)
        {
            type.WriteDeserialization(name, output);
        }

        public void WriteEqualityComparison(TextWriter output)
        {
            type.WriteEqualityComparison(name, output);
        }

        public void WriteFormalParameter(TextWriter output)
        {
            type.WriteFormalParameter(name, output);
        }

        public void WriteHash(TextWriter output)
        {
            type.WriteHash(name, output);
        }

        public void WriteSerialization(TextWriter output)
        {
            type.WriteSerialization(name, output);
        }

        public void WriteToStringOutput(TextWriter output)
        {
            type.WriteToStringOutput(type.BackingStoreName(name), output);
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
        void WriteEqualityComparison(string variableName, TextWriter output);
        void WriteHash(string variableName, TextWriter output);
        void WriteDeserialization(string variableName, TextWriter output);
        void WriteSerialization(string variableName, TextWriter output);
        void WriteToStringOutput(string variableName, TextWriter output);
        void WriteConstructorCheck(string variableName, TextWriter output);
        void WriteBuilderDeclaration(string variableName, TextWriter output);
        void WriteBuilderAssignment(string variableName, TextWriter output);
        void WriteBuilderPlumbing(string variableName, string singularName, TextWriter output);
    }
}
