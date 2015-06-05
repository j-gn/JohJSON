//  Copyright 2014 Johannes Gotlen <johannes@gotlen.se>
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;

namespace JohJSON.Tests
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
			Assert.AreEqual(new StringDelimiter(), list[1]);
			Assert.AreEqual("root", list[2]);
			Assert.AreEqual(new StringDelimiter(), list[3]);
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
			w.ParseText(new StringReader(json));
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
			bool exeptionThrown = false;
			try
			{
				JSONNode.CreateFromString("{ \"field\" : false, \"second field\": true \"third field\":false }");
			}
			catch
			{
				exeptionThrown = true;
			}
			Assert.IsTrue(exeptionThrown);

		}

		[Test()]
		public void EmptyArrays()
		{
			string s = "{\"field\":[],\"second field\":{}}";
			JSONNode n = JSONNode.CreateFromString(s);
			Assert.AreEqual(s, n.ToString());
			Assert.AreEqual(0, n["field"].length);
			Assert.AreEqual(0, n["second field"].length);
		}

		[Test()]
		public void Iterators()
		{
			JSONNode n = new JSONNode();
			string[] values = { "a", "b", "c", "d", "e" };
			n["root"]["D"][0].asText = values[0];
			n["root"]["D"][1].asText = values[1];
			n["root"]["D"][2].asText = values[2];
			n["root"]["D"][3].asText = values[3];
			n["root"]["D"][4].asText = values[4];
			var e = values.GetEnumerator();
			foreach (var c in n["root"]["D"].listValues)
			{
				e.MoveNext();
				Assert.AreEqual(e.Current, c.asText);
			}

			foreach (var x in n["root"].dictionaryValues)
			{
				Assert.AreEqual(x.Key, "D");
			}
		}

		[Test()]
		public void EmptyStrings()
		{
			JSONNode root = new JSONNode();
			root["root"]["X"].asText = string.Empty;
			root["root"]["Y"].asText = "";
			root["root"]["Z"].asText = null;

			Assert.AreEqual("", root["root"]["X"].asText);
			Assert.AreEqual("", root["root"]["Y"].asText);
			Assert.AreEqual(null, root["root"]["Z"].asText);
		
			string jsonString = "";
			jsonString = root.ToString();
			Console.WriteLine(jsonString);

			var n = JSONNode.CreateFromString(jsonString);
			Assert.AreEqual(string.Empty, n["root"]["Y"].asText);
			Assert.AreEqual("", n["root"]["Y"].asText);
			Assert.AreEqual(null, n["root"]["Z"].asText);
		}

		[Test()]
		public void FileIO()
		{
			JSONNode original = new JSONNode();
			string[] values = { "a", "b", "c", "d", "e" };
			original["root"]["D"][0].asText = values[0];
			original["root"]["D"][1].asText = values[1];
			original["root"]["D"][2].asText = values[2];
			original["root"]["D"][3].asText = values[3];
			original["root"]["D"][4].asText = values[4];

			const string FILENAME = "tempFile.json";
			File.Delete(FILENAME);
			//write to file
			JSONNodeWriter w = new JSONNodeWriter();
			var fWrite = File.Open(FILENAME, FileMode.Create);
			w.WriteToStream(original, fWrite);
			fWrite.Close();


			//read from file
			Generator g = new Generator();
			var fRead = File.Open(FILENAME, FileMode.Open);
			JSONNode output = g.Generate(fRead);
			fRead.Close();

			var enumrtr = values.GetEnumerator();
			for (int i = 0; i < 5; i++)
			{
				enumrtr.MoveNext();
				Assert.AreEqual(enumrtr.Current, output["root"]["D"][i].asText);
			}
		}

		[Test()]
		public void AnonymousRoot()
		{
			var node = new JohJSON.JSONNode();
			node["text"].asText = "This is text";

			Assert.AreEqual("{\"text\":\"This is text\"}", node.ToString());
		}

		void TestEscapeChar(char c){

			var x = new JSONNode();
			x[c.ToString()].asText = c.ToString();
			string testString = x.ToString();
			Console.WriteLine(testString);
			var y = JSONNode.CreateFromString(testString);
			Assert.AreEqual(x.ToString(), y.ToString());
			Assert.AreEqual(y[c.ToString()].asText[0], c);
			Assert.AreEqual(x[c.ToString()].asText[0], c);

		}

		[Test()]
		public void EscapeCharacters()
		{
		
			TestEscapeChar('\"');
			TestEscapeChar('/');
			TestEscapeChar('"');
			TestEscapeChar('\\');
			TestEscapeChar('\n');
			TestEscapeChar('\b');
			TestEscapeChar('\r');
			TestEscapeChar('\t');
			TestEscapeChar('\v');
			TestEscapeChar('\f');
			TestEscapeChar('\'');
			TestEscapeChar('\a');

			//Not really sure how to handle this
			TestEscapeChar('\u0012');

		}

//		[Test()]
//		public void StackBustingTest()
//		{
//			var head = new JohJSON.JSONNode();
//			head.nodeType = NodeType.LIST;
//			var node = head;
//			const int testval = 100000;
//			for (int i = 0; i < testval; i++)
//			{
//				node.next = new JSONNode();
//				node.next.asText = "::i" + i.ToString();
//				node = node.next;
//			}
//			//Console.Write(head);
//			Assert.AreEqual("::i" + (testval-1).ToString(), head[testval-1].asText);
//
//		}
	}
}

