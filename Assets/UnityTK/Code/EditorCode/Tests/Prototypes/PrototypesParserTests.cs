﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityTK.Prototypes;

namespace UnityTK.Test.Prototypes
{
	public class TestPrototypeSpec : TestPrototype
	{
		public int testField;
	}
	
	public class TestPrototype : IPrototype
	{
		[PrototypesTypeSerializableAttribute]
		public struct TestStruct
		{
			public int test;
			public int test2;
		}
		
		[PrototypesTypeSerializableAttribute]
		public class TestBase
		{
			public string baseStr;
		}
		
		[PrototypesTypeSerializableAttribute]
		public class SpecializedClass : TestBase
		{
			public int lul;
		}

		public string name;
		public float someRate;
		public int someInt;
		public TestPrototype someOtherPrototype = null;
		public Type type = null;
		public TestStruct _struct;
		public TestBase testBase;
		
		public TestBase[] array;
		public List<TestBase> list;
		public HashSet<TestBase> hashSet;
		public TestPrototype[] arrayRefs;

		string IPrototype.identifier
		{
			get
			{
				return name;
			}

			set
			{
				name = value;
			}
		}
	}

    public class PrototypesParserTest
    {
        [Test]
        public void ParserTestCustomPrototypeClass()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\" Type=\"TestPrototypeSpec\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"		<testField>500</testField>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);
			
			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
			Assert.AreEqual(500, (prototypes[0] as TestPrototypeSpec).testField);
        }

        [Test]
        public void ParserTestValueTypes()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
        }

        [Test]
        public void ParserTestPrototypeRefs()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"	</Prototype>\n" +
				"	<Prototype Id=\"Test2\">\n" +
				"		<someOtherPrototype>Test</someOtherPrototype>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
			Assert.AreSame((prototypes[0] as TestPrototype), (prototypes[1] as TestPrototype).someOtherPrototype);
        }

        [Test]
        public void ParserTestSubData()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<testBase>\n" +
				"			<baseStr>teststr</baseStr>\n" +
				"		</testBase>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual("teststr", (prototypes[0] as TestPrototype).testBase.baseStr);
        }

        [Test]
        public void ParserTestSubDataStruct()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<_struct>\n" +
				"			<test>1337</test>\n" +
				"		</_struct>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(1337, (prototypes[0] as TestPrototype)._struct.test);
        }

        [Test]
        public void ParserTestSubDataCustomType()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<testBase Type=\"SpecializedClass\">\n" +
				"			<baseStr>teststr</baseStr>\n" +
				"			<lul>10</lul>\n" +
				"		</testBase>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual("teststr", (prototypes[0] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(10, ((prototypes[0] as TestPrototype).testBase as TestPrototype.SpecializedClass).lul);
        }

        [Test]
        public void ParserTestCollectionsArray()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<array>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);

			var collection = (prototypes[0] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
        }

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Inheritance tests
		/////////////////////////////////////////////////////////////////////////////////////////////
		
        [Test]
        public void ParserTestInheritance()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<testBase Type=\"SpecializedClass\">\n" +
				"			<baseStr>teststr</baseStr>\n" +
				"			<lul>10</lul>\n" +
				"		</testBase>\n" +
				"		<_struct>\n" +
				"			<test>1337</test>\n" +
				"		</_struct>\n" +
				"		<array>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</Prototype>\n" +
				"	<Prototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<someOtherPrototype>Test</someOtherPrototype>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			var collection = (prototypes[0] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual("teststr", (prototypes[0] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(1337, (prototypes[0] as TestPrototype)._struct.test);

			Assert.AreEqual(2.5f, (prototypes[1] as TestPrototype).someRate);
			collection = (prototypes[1] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual("teststr", (prototypes[1] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(1337, (prototypes[1] as TestPrototype)._struct.test);
			Assert.AreSame((prototypes[0] as TestPrototype), (prototypes[1] as TestPrototype).someOtherPrototype);
        }

        [Test]
        public void ParserTestAbstractPrototypes()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\" Abstract=\"True\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"	</Prototype>\n" +
				"	<Prototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
        }

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Override tests
		/////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void ParserTestOverridePrototypeRefs()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"	</Prototype>\n" +
				"	<Prototype Id=\"Test2\">\n" +
				"		<someOtherPrototype>Test</someOtherPrototype>\n" +
				"	</Prototype>\n" +
				"	<Prototype Id=\"Test3\" Inherits=\"Test2\">\n" +
				"		<someOtherPrototype>Test2</someOtherPrototype>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(3, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
			Assert.AreSame((prototypes[0] as TestPrototype), (prototypes[1] as TestPrototype).someOtherPrototype);
			Assert.AreSame((prototypes[1] as TestPrototype), (prototypes[2] as TestPrototype).someOtherPrototype);
        }

        [Test]
        public void ParserTestOverrideSubData()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<testBase Type=\"SpecializedClass\">\n" +
				"			<baseStr>teststr</baseStr>\n" +
				"			<lul>1</lul>\n" +
				"		</testBase>\n" +
				"	</Prototype>\n" +
				"	<Prototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<testBase>\n" +
				"			<baseStr>teststr2</baseStr>\n" +
				"		</testBase>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			Assert.AreEqual("teststr", (prototypes[0] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(1, ((prototypes[0] as TestPrototype).testBase as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual("teststr2", (prototypes[1] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(1, ((prototypes[1] as TestPrototype).testBase as TestPrototype.SpecializedClass).lul);
        }

        [Test]
        public void ParserTestOverrideSubDataStruct()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\">\n" +
				"		<_struct>\n" +
				"			<test>1337</test>\n" +
				"		</_struct>\n" +
				"	</Prototype>\n" +
				"	<Prototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<_struct>\n" +
				"			<test2>1338</test2>\n" +
				"		</_struct>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			Assert.AreEqual(1337, (prototypes[0] as TestPrototype)._struct.test);
			Assert.AreEqual(1338, (prototypes[1] as TestPrototype)._struct.test2);
        }

        [Test]
        public void ParserTestOverrideValueTypes()
        {
			string xml = "<PrototypeContainer Type=\"TestPrototype\">\n" +
				"	<Prototype Id=\"Test\" Abstract=\"True\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"	</Prototype>\n" +
				"	<Prototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<someRate>4</someRate>\n" +
				"	</Prototype>\n" +
				"</PrototypeContainer>";

			List<ParsingError> errors = new List<ParsingError>();
			var prototypes = PrototypeParser.Parse(xml, new PrototypeParseParameters()
			{
				standardNamespace = "UnityTK.Test.Prototypes"
			}, ref errors);

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(4f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
        }
    }
}