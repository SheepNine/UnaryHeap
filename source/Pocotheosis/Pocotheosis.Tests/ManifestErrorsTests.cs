using NUnit.Framework;
using System;
using System.IO;

namespace Pocotheosis.Tests
{
    [TestFixture]
    class ManifestErrorsTests
    {
        [Test]
        public void MissingNamespaceName()
        {
            CheckErrorCondition("Missing namespace name",
                @"<namespace />");
        }

        [Test]
        public void MissingEnumName()
        {
            CheckErrorCondition("Missing enum name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum>
      <enumerator name=""True"" value=""0""/>
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void EmptyEnum()
        {
            CheckErrorCondition("Enum 'Ralph' has no enumerators",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""Ralph"">
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void MissingEnumeratorName()
        {
            CheckErrorCondition("Missing enumerator name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""E"">
      <enumerator value=""0""/>
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void MissingEnumeratorValue()
        {
            CheckErrorCondition("Enumerator Key missing value",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""E"">
      <enumerator name=""Key"" />
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void ReservedEnumName()
        {
            CheckErrorCondition("Enum 'while' has reserved keyword for a name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""while"">
      <enumerator name=""Key"" value=""6"" />
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void InvalidEnumName()
        {
            CheckErrorCondition("Enum '*/' has invalid identifier for a name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""*/"">
      <enumerator name=""Key"" value=""6"" />
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void ReservedEnumeratorName()
        {
            CheckErrorCondition("Enumerator 'while' has reserved keyword for a name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""Doorframe"">
      <enumerator name=""while"" value=""6"" />
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void InvalidEnumeratorName()
        {
            CheckErrorCondition("Enumerator '6112' has invalid identifier for a name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""Doorframe"">
      <enumerator name=""6112"" value=""6"" />
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void DuplicateEnum()
        {
            CheckErrorCondition("Enum 'Duplicate' appears multiple times",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""Duplicate"">
      <enumerator name=""Alpha"" value=""1"" />
    </enum>
    <enum name=""Duplicate"">
      <enumerator name=""Beta"" value=""2"" />
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void DuplicateEnumeratorName()
        {
            CheckErrorCondition("Enum 'E' enumerator 'Foo' appears multiple times",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""E"">
      <enumerator name=""Foo"" value=""0""/>
      <enumerator name=""Foo"" value=""1""/>
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void DuplicateEnumeratorValue()
        {
            CheckErrorCondition("Enum 'E' value '7' appears multiple times",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <enums>
    <enum name=""E"">
      <enumerator name=""Foo"" value=""7""/>
      <enumerator name=""Bar"" value=""7""/>
    </enum>
  </enums>
</namespace>");
        }

        [Test]
        public void MissingClassName()
        {
            CheckErrorCondition("Missing class name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class id=""0"">
      <members />
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void MissingClassId()
        {
            CheckErrorCondition("Class EmptyPoco missing identifier",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""EmptyPoco"">
      <members />
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void ReservedClassName()
        {
            CheckErrorCondition("Class 'volatile' has reserved keyword for a name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""volatile"" id=""7"">
      <members />
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void InvalidClassName()
        {
            CheckErrorCondition("Class 'Two Words' has invalid identifier for a name",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""Two Words"" id=""7"">
      <members />
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void DuplicateClassName()
        {
            CheckErrorCondition("Class 'Duplicate' appears multiple times",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""Duplicate"" id=""0"">
      <members />
    </class>
    <class name=""Duplicate"" id=""1"">
      <members />
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void DuplicateId()
        {
            CheckErrorCondition("Streaming ID '3' appears multiple times",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""Unique"" id=""3"">
      <members />
    </class>
    <class name=""Uneeque"" id=""3"">
      <members />
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void DuplicateMemberName()
        {
            CheckErrorCondition("Class 'Unique' has duplicate member name 'Duped'",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""Unique"" id=""3"">
      <members>
        <member name=""Duped"" type=""int""/>
        <member name=""Duped"" type=""int""/>
      </members>
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void FloatTypeNotSupported()
        {
            CheckErrorCondition("Floating-point types (float and double) are not supported",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""Tempo"" id=""3"">
      <members>
        <member name=""Borken"" type=""float"" />
      </members>
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void DoubleTypeNotSupported()
        {
            CheckErrorCondition("Floating-point types (float and double) are not supported",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""Tempo"" id=""3"">
      <members>
        <member name=""Borken"" type=""double"" />
      </members>
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void UnrecognizedPoco()
        {
            CheckErrorCondition("No definition given for Poco type(s): Hempo, Myst, Uru, Zempo",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""Happy"" id=""2"">
      <members />
    </class>
    <class name=""Tempo"" id=""3"">
      <members>
        <member name=""Borken"" type=""Hempo"" />
        <member name=""Fine"" type=""Happy"" />
        <member name=""AlsoBorked"" type=""Zempo"" />
        <member name=""ArrayBorked"" type=""Uru[]"" />
        <member name=""DictionaryBorked"" type=""int->Myst"" />
      </members>
    </class>
  </classes>
</namespace>");
        }

        [Test]
        public void NonPrimitiveDictionaryKey()
        {
            CheckErrorCondition(
                "Point cannot be used as a dictionary key as it is not comparable",
@"<namespace name=""Pocotheosis.Tests.Pocos"">
  <classes>
    <class name=""Point"" id=""1"">
      <members>
        <member name=""X"" type=""int"" />
        <member name=""Y"" type=""int"" />
      </members>
    </class>
    <class name=""Fudge"" id=""3"">
      <members>
        <member name=""Borken"" type=""Point->string"" />
      </members>
    </class>
  </classes>
</namespace>");
        }

        private static void CheckErrorCondition(string expectedExceptionMessage,
            string manifestXml)
        {
            try
            {
                ManifestParser.Parse(new StringReader(manifestXml), DateTime.MinValue);
                Assert.Fail("No exception thrown!");
            }
            catch (InvalidDataException ex)
            {
                Assert.AreEqual(ex.Message, expectedExceptionMessage);
            }
        }
    }
}
