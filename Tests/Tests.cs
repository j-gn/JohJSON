//
//  Tests.cs
//
//  Author:
//       Johannes Gotlen <johannes.gotlen@hellothere.se>
//
//  Copyright (c) 2014 
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using NUnit.Framework;
using System;
using System.IO;

namespace JohJSON
{
	[TestFixture()]
	public class Tests
	{
		const string BASIC_OBJECT = "{\"root\":10}";
		const string BASIC_ARRAY = "[\"name\",10, false]";

		[Test()]
		public void TokenizerBasicTests()
		{
			Tokenizer t = new Tokenizer();
			var list = t.ParseText(new System.IO.StringReader(BASIC_OBJECT));
			Assert.AreEqual("{", list[0]);
			Assert.AreEqual("\"", list[1]);
			Assert.AreEqual("root", list[2]);
			Assert.AreEqual("\"", list[3]);
			Assert.AreEqual(":", list[4]);
			Assert.AreEqual("10", list[5]);
			Assert.AreEqual("}", list[6]);
			Assert.AreEqual(7, list.Count);
		}

		[Test()] 
		public void GeneratorBasicTest()
		{
			Generator g = new Generator();

			var node = g.Generate(BASIC_OBJECT);
			Assert.AreEqual(NodeType.DICTIONARY, node.nodeType);
			Assert.AreEqual(10, node.data.asNumber);
		}

		[Test()]
		public void SpecialTest()
		{
			JSONNode n = new JSONNode();
			n["root"]["D"][0].asText = "1";
			n["root"]["D"][1].asText = "1";
			n["root"]["D"][2].asText = "1";
			n["root"]["D"][3].asText = "1";
			n["root"]["D"][4].asText = "1";
			n["root"]["D"][5]["qwert"].asText = "hundred";
			n["root"]["E"].asNumber = 1;
			n["root"]["G"].asNumber = 2;
			n["root"]["F"].asNumber = 3;
			n["root"]["myObjecy"]["test Array"][0].asText = "text";

			//Console.WriteLine(n.data);
			string json = n.ToString();
			Console.WriteLine(json);
			Tokenizer w = new Tokenizer();
			var result = w.ParseText(new StringReader(json));
			foreach (var s in result)
				Console.WriteLine(s);
			Generator g = new Generator();
			var newNode = g.Generate(json);
			Console.WriteLine("OUT" + newNode["root"]["D"][5]["qwert"].asText);
		}

		[Test()]
		public void PropertyExists()
		{
			JSONNode n = new JSONNode();
			n["root"]["D"][0].asText = "1";
			n["root"]["D"][1].asText = "1";
			n["root"]["D"][2].asText = "1";
			n["root"]["D"][3].asText = "1";
			n["root"]["D"][4].asText = "1";
			n["root"]["D"][5]["qwert"].asText = "hundred";
			n["root"]["E"].asNumber = 1;
			n["root"]["G"].asNumber = 2;
			n["root"]["F"].asNumber = 3;
			n["root"]["myObjecy"]["test Array"][0].asText = "text";
			Assert.IsTrue(n.PropertyExists("root"));
			Assert.IsTrue(n["root"].PropertyExists("F"));
			Assert.IsFalse(n["root"].PropertyExists("Q"));
			Assert.IsFalse(n["root"]["D"][0].PropertyExists("Q"));
		}
	}
}

