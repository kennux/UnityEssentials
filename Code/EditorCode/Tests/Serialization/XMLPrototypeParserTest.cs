﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityTK.Serialization.Prototypes;
using UnityTK.Serialization;

namespace UnityTK.Test.Serialization
{
	public class TestPrototypeSpec : TestPrototype
	{
		public int testField;
	}
	
	public class TestPrototype : IPrototype
	{
		public struct TestStruct
		{
			public int test;
			public int test2;
		}
		
		public class TestBase
		{
			public string baseStr;
		}
		
		public class SpecializedClass : TestBase
		{
			public int lul;
		}

		public string name;
		public float someRate;
		public int someInt;
		public Vector2 vec2;
		public Vector3 vec3;
		public Vector4 vec4;
		public Quaternion quat;
		public TestPrototype someOtherPrototype = null;
		public Type type = null;
		public TestStruct _struct;
		public TestBase testBase;
		public Color color;
		
		public TestBase[] array;
		public List<TestBase> list;
		public HashSet<TestBase> hashSet;
		public TestPrototype[] arrayRefs;

		string ISerializableRoot.identifier
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

	public class XMLPrototypeParserTest
	{
		protected virtual ISerializer CreateSerializer()
		{
			return PrototypeParser.CreateXMLSerializer("UnityTK.Test.Serialization");
		}

		[Test]
		public void TestCustomPrototypeClass()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototypeSpec Id=\"Test\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"		<testField>500</testField>\n" +
				"	</TestPrototypeSpec>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();
			
			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
			Assert.AreEqual(500, (prototypes[0] as TestPrototypeSpec).testField);
		}

		[Test]
		public void TestValueTypes()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"		<vec2>5.1,2.5</vec2>\n" +
				"		<vec3>2.5,5.1,9</vec3>\n" +
				"		<vec4>9,2.5,5,1.25</vec4>\n" +
				"		<quat>9,2.5,5,1.25</quat>\n" +
				"		<color>0.25,1,0.5,1</color>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
			Assert.AreEqual(new Vector2(5.1f, 2.5f), (prototypes[0] as TestPrototype).vec2);
			Assert.AreEqual(new Vector3(2.5f, 5.1f, 9), (prototypes[0] as TestPrototype).vec3);
			Assert.AreEqual(new Vector4(9, 2.5f, 5, 1.25f), (prototypes[0] as TestPrototype).vec4);
			Assert.AreEqual(new Quaternion(9, 2.5f, 5, 1.25f), (prototypes[0] as TestPrototype).quat);
			Assert.AreEqual(new Color(0.25f, 1f, 0.5f, 1f), (prototypes[0] as TestPrototype).color);
		}

		[Test]
		public void TestTypeSerializer()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<type>TestPrototype+TestBase</type>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreSame(typeof(TestPrototype.TestBase), (prototypes[0] as TestPrototype).type);
		}

		[Test]
		public void TestPrototypeRefs()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\">\n" +
				"		<someOtherPrototype>Test</someOtherPrototype>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
			Assert.AreSame((prototypes[0] as TestPrototype), (prototypes[1] as TestPrototype).someOtherPrototype);
		}

		[Test]
		public void TestSubData()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<testBase>\n" +
				"			<baseStr>teststr</baseStr>\n" +
				"		</testBase>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual("teststr", (prototypes[0] as TestPrototype).testBase.baseStr);
		}

		[Test]
		public void TestSubDataStruct()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<_struct>\n" +
				"			<test>1337</test>\n" +
				"		</_struct>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(1337, (prototypes[0] as TestPrototype)._struct.test);
		}

		[Test]
		public void TestSubDataCustomType()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<testBase Type=\"SpecializedClass\">\n" +
				"			<baseStr>teststr</baseStr>\n" +
				"			<lul>10</lul>\n" +
				"		</testBase>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual("teststr", (prototypes[0] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(10, ((prototypes[0] as TestPrototype).testBase as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestCollectionsArray()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<array>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);

			var collection = (prototypes[0] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestCollectionsList()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<list>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</list>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);

			var collection = (prototypes[0] as TestPrototype).list;
			Assert.AreEqual(2, collection.Count);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Inheritance tests
		/////////////////////////////////////////////////////////////////////////////////////////////
		
		[Test]
		public void TestInheritance()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
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
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<someOtherPrototype>Test</someOtherPrototype>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

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
		public void TestDeepInheritance()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
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
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<someOtherPrototype>Test</someOtherPrototype>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test3\" Inherits=\"Test2\">\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(3, prototypes.Count);
			
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

			Assert.AreEqual(2.5f, (prototypes[2] as TestPrototype).someRate);
			collection = (prototypes[2] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual("teststr", (prototypes[2] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(1337, (prototypes[2] as TestPrototype)._struct.test);
			Assert.AreSame((prototypes[0] as TestPrototype), (prototypes[2] as TestPrototype).someOtherPrototype);
		}
		
		[Test]
		public void TestInheritanceSplit()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
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
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
				string xml2 = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<someOtherPrototype>Test</someOtherPrototype>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			parser.Parse(xml2, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

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
		public void TestAbstractPrototypes()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\" Abstract=\"True\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Override tests
		/////////////////////////////////////////////////////////////////////////////////////////////

		[Test]
		public void TestOverrideCombineCollectionsArray()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<array>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<array CollectionOverrideAction=\"Combine\">\n" +
				"			<li>\n" +
				"				<baseStr>teststr3</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr4</baseStr>\n" +
				"				<lul>11</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			var collection = (prototypes[0] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			
			collection = (prototypes[1] as TestPrototype).array;
			Assert.AreEqual(4, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual("teststr3", collection[2].baseStr);
			Assert.AreEqual("teststr4", collection[3].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual(11, (collection[3] as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestOverrideCombineCollectionsList()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<list>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</list>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<list CollectionOverrideAction=\"Combine\">\n" +
				"			<li>\n" +
				"				<baseStr>teststr3</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr4</baseStr>\n" +
				"				<lul>11</lul>\n" +
				"			</li>\n" +
				"		</list>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			var collection = (prototypes[0] as TestPrototype).list;
			Assert.AreEqual(2, collection.Count);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			
			collection = (prototypes[1] as TestPrototype).list;
			Assert.AreEqual(4, collection.Count);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual("teststr3", collection[2].baseStr);
			Assert.AreEqual("teststr4", collection[3].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual(11, (collection[3] as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestImplicitCombineCollectionsArray()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<array>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<array>\n" +
				"			<li>\n" +
				"				<baseStr>teststr3</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr4</baseStr>\n" +
				"				<lul>11</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test3\">\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test4\" ParentName=\"Test3\">\n" +
				"		<array>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(4, prototypes.Count);
			
			var collection = (prototypes[0] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			
			collection = (prototypes[3] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			
			collection = (prototypes[1] as TestPrototype).array;
			Assert.AreEqual(4, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual("teststr3", collection[2].baseStr);
			Assert.AreEqual("teststr4", collection[3].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual(11, (collection[3] as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestImplicitCombineCollectionsList()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<list>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</list>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<list>\n" +
				"			<li>\n" +
				"				<baseStr>teststr3</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr4</baseStr>\n" +
				"				<lul>11</lul>\n" +
				"			</li>\n" +
				"		</list>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test3\">\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test4\" ParentName=\"Test3\">\n" +
				"		<list>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</list>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(4, prototypes.Count);
			
			var collection = (prototypes[0] as TestPrototype).list;
			Assert.AreEqual(2, collection.Count);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			
			collection = (prototypes[3] as TestPrototype).list;
			Assert.AreEqual(2, collection.Count);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			
			collection = (prototypes[1] as TestPrototype).list;
			Assert.AreEqual(4, collection.Count);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual("teststr3", collection[2].baseStr);
			Assert.AreEqual("teststr4", collection[3].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual(11, (collection[3] as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestOverrideReplaceCollectionsArray()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<array>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<array CollectionOverrideAction=\"Replace\">\n" +
				"			<li>\n" +
				"				<baseStr>teststr3</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr4</baseStr>\n" +
				"				<lul>11</lul>\n" +
				"			</li>\n" +
				"		</array>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			var collection = (prototypes[0] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			
			collection = (prototypes[1] as TestPrototype).array;
			Assert.AreEqual(2, collection.Length);
			Assert.AreEqual("teststr3", collection[0].baseStr);
			Assert.AreEqual("teststr4", collection[1].baseStr);
			Assert.AreEqual(11, (collection[1] as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestOverrideReplaceCollectionsList()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<list>\n" +
				"			<li>\n" +
				"				<baseStr>teststr1</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr2</baseStr>\n" +
				"				<lul>10</lul>\n" +
				"			</li>\n" +
				"		</list>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<list CollectionOverrideAction=\"Replace\">\n" +
				"			<li>\n" +
				"				<baseStr>teststr3</baseStr>\n" +
				"			</li>\n" +
				"			<li Type=\"SpecializedClass\">\n" +
				"				<baseStr>teststr4</baseStr>\n" +
				"				<lul>11</lul>\n" +
				"			</li>\n" +
				"		</list>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			var collection = (prototypes[0] as TestPrototype).list;
			Assert.AreEqual(2, collection.Count);
			Assert.AreEqual("teststr1", collection[0].baseStr);
			Assert.AreEqual("teststr2", collection[1].baseStr);
			Assert.AreEqual(10, (collection[1] as TestPrototype.SpecializedClass).lul);
			
			collection = (prototypes[1] as TestPrototype).list;
			Assert.AreEqual(2, collection.Count);
			Assert.AreEqual("teststr3", collection[0].baseStr);
			Assert.AreEqual("teststr4", collection[1].baseStr);
			Assert.AreEqual(11, (collection[1] as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestOverridePrototypeRefs()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\">\n" +
				"		<someOtherPrototype>Test</someOtherPrototype>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test3\" Inherits=\"Test2\">\n" +
				"		<someOtherPrototype>Test2</someOtherPrototype>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(3, prototypes.Count);
			
			Assert.AreEqual(2.5f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
			Assert.AreSame((prototypes[0] as TestPrototype), (prototypes[1] as TestPrototype).someOtherPrototype);
			Assert.AreSame((prototypes[1] as TestPrototype), (prototypes[2] as TestPrototype).someOtherPrototype);
		}

		[Test]
		public void TestOverrideSubData()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<testBase Type=\"SpecializedClass\">\n" +
				"			<baseStr>teststr</baseStr>\n" +
				"			<lul>1</lul>\n" +
				"		</testBase>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<testBase>\n" +
				"			<baseStr>teststr2</baseStr>\n" +
				"		</testBase>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			Assert.AreEqual("teststr", (prototypes[0] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(1, ((prototypes[0] as TestPrototype).testBase as TestPrototype.SpecializedClass).lul);
			Assert.AreEqual("teststr2", (prototypes[1] as TestPrototype).testBase.baseStr);
			Assert.AreEqual(1, ((prototypes[1] as TestPrototype).testBase as TestPrototype.SpecializedClass).lul);
		}

		[Test]
		public void TestOverrideSubDataStruct()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\">\n" +
				"		<_struct>\n" +
				"			<test>1337</test>\n" +
				"		</_struct>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<_struct>\n" +
				"			<test2>1338</test2>\n" +
				"		</_struct>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(2, prototypes.Count);
			
			Assert.AreEqual(1337, (prototypes[0] as TestPrototype)._struct.test);
			Assert.AreEqual(1338, (prototypes[1] as TestPrototype)._struct.test2);
		}

		[Test]
		public void TestOverrideValueTypes()
		{
			string xml = "<PrototypeContainer>\n" +
				"	<TestPrototype Id=\"Test\" Abstract=\"True\">\n" +
				"		<someRate>2.5</someRate>\n" +
				"		<someInt>5</someInt>\n" +
				"	</TestPrototype>\n" +
				"	<TestPrototype Id=\"Test2\" Inherits=\"Test\">\n" +
				"		<someRate>4</someRate>\n" +
				"	</TestPrototype>\n" +
				"</PrototypeContainer>";
			
			var parser = new PrototypeParser(CreateSerializer());
			parser.Parse(xml, "DIRECT PARSE");
			var prototypes = parser.GetPrototypes();
			var errors = parser.GetParsingErrors();

			foreach (var error in errors)
				throw new Exception(error.GetFullMessage());
			Assert.AreEqual(1, prototypes.Count);
			
			Assert.AreEqual(4f, (prototypes[0] as TestPrototype).someRate);
			Assert.AreEqual(5f, (prototypes[0] as TestPrototype).someInt);
		}
	}
}