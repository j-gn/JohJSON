//
//  Tests.cs
//
//  Author:
//       Johannes Gotlen <johannesgotlen.se>
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
using System.Collections.Generic;

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
			//foreach (var s in result)
			//	Console.WriteLine(s);
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

		[Test()]
		public void HeroTestA()
		{
	
			string data = "{\"playerInfo\":{\"cash\":10,\"diamonds\":10,\"selectedAttributes\":[\"StandardSubmarine 2\"],\"attributes\":[\"StandardSubmarine 2\",\"StandardSubmarine\"]}}";
			var loaded = JSONNode.CreateFromString(data).ToString();
			Assert.AreEqual(data, loaded);
		}

		[Test()]
		public void HeroTestB()
		{

			JSONNode n = new JSONNode();
			n["playerInfo"]["cash"].asInt = 10;
			n["playerInfo"]["diamonds"].asInt = 10;
			n["playerInfo"]["selectedAttributes"][0].asText = "StandardSubmarine 2";
			n["playerInfo"]["attributes"][0].asText = "StandardSubmarine";
			n["playerInfo"]["attributes"][1].asText = "StandardSubmarine 2";

			string data = "{\"playerInfo\":{\"cash\":10,\"diamonds\":10,\"selectedAttributes\":[\"StandardSubmarine 2\"],\"attributes\":[\"StandardSubmarine\",\"StandardSubmarine 2\"]}}";

			Assert.AreEqual(n.ToString(), data);
		}

		[Test()]
		public void SaveLoad()
		{
			List<string> s = new List<string>
			{
				"one", "two", "three", null, "five"
			};

			JSONNode n = new JSONNode()["root"];

			n["cash"].asInt = 10;
			n["diamonds"].asInt = 100;
			SaveArray(n["saved numbers"], s);
			SaveArray(n["saved"][0], s);

			var json = n.ToString();
			JSONNode loaded = JSONNode.CreateFromString(json);

			Assert.AreEqual(loaded.ToString(), json);
			Console.WriteLine(json);
			List<string> tmpList = new List<string>();

			LoadArray(loaded["saved numbers"], tmpList);
			for (int i = 0; i < s.Count; ++i)
			{
				Assert.AreEqual(s[i], tmpList[i]);
			}
			tmpList.Clear();
		
			LoadArray(loaded["saved"][0], tmpList);
			for (int i = 0; i < s.Count; ++i)
			{
				Assert.AreEqual(s[i], tmpList[i]);
			}

			tmpList.Clear();
			Console.Write(json);
		
		}

		void SaveArray(JSONNode pOutput, List<string> input)
		{
			for (int i = 0; i < input.Count; ++i)
			{
				pOutput[i].asText = input[i];
			}
		}

		void LoadArray(JSONNode pInput, List<string> output)
		{
			for (int i = 0; i < pInput.length; i++)
			{
				var str = pInput[i].asText;
				output.Add(str);
			}
		}

		[Test()]
		public void NullTest()
		{
			
			JSONNode n = new JSONNode();

			n["A"] = null;
			Assert.AreEqual(NodeType.NULL, n["A"].nodeType);

			n["A"].asText = null;
			Assert.AreEqual(NodeType.NULL, n["A"].nodeType);

			n["A"].asInt = 10;
			Assert.AreEqual(NodeType.NUMBER, n["A"].nodeType);

			string json = "{\"list with null\":[0,1,2,3,null],\"nullfield\":null,\"nested\":{\"stuff\":null}}";
			var node = JSONNode.CreateFromString(json);
			Assert.AreEqual(node.ToString(), json);

			json = "null";
			node = JSONNode.CreateFromString(json);
			Assert.AreEqual(node.ToString(), json);
		}

		[Test()]
		public void BooleanTest()
		{
			JSONNode n;

			n = JSONNode.CreateFromString("false");
			Assert.IsFalse(n.asBool);

			n = JSONNode.CreateFromString("true");
			Assert.IsTrue(n.asBool);

			n = JSONNode.CreateFromString("{ \"field\" : false }");
			Assert.IsFalse(n["field"].asBool);

			n = JSONNode.CreateFromString("{ \"field\" : true }");
			Assert.IsTrue(n["field"].asBool);
		}

		[Test()]
		public void CorrectCommaSeparationException()
		{
			JSONNode n;

			bool exeptionThrown = false;
			try{
			n = JSONNode.CreateFromString("{ \"field\" : false, \"second field\": true \"third field\":false }");
			}catch(Exception e){
				exeptionThrown = true;
			}
			Assert.IsTrue(exeptionThrown);

		}
	}
}

