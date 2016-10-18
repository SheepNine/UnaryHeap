using System;
using NUnit.Framework;
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

        private void CheckErrorCondition(string expectedExceptionMessage, string manifestXml)
        {
            try
            {
                PocoManifest.Parse(new StringReader(manifestXml));
                Assert.Fail("No exception thrown!");
            }
            catch (InvalidDataException ex)
            {
                Assert.AreEqual(ex.Message, expectedExceptionMessage);
            }
        }
    }
}
