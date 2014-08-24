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

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace JohJSON
{
	public class Generator
	{
		Tokenizer tokenizer = new Tokenizer();
		Queue<string> tokens;
		public JSONNode Generate(Stream s)
		{
			var t = tokenizer.ParseText(new StreamReader(s));
			return Generate(t);
		}
		public JSONNode Generate(string s)
		{
			var t = tokenizer.ParseText(new StringReader(s));
			return Generate(t);
		}
		public JSONNode Generate(IEnumerable<string> pTokens){
			tokens = new Queue<string>(pTokens);
			if (tokens.Count > 0)
			{
				return ParseData();
			}
			else
			{
				return JSONNode.empty;
			}
		}
			
		JSONNode ParseData()
		{
			switch (tokens.Peek())
			{
				case "{":
					return ParseDictionary();
				case "[":
					return ParseList();
				case "null":
					tokens.Dequeue();
					return new JSONNode{
						nodeType = NodeType.NULL
					};
				case "\"":
					return new JSONNode
					{
						nodeType = NodeType.TEXT,
						_textVal = ParseString(),
					};
				default:
					bool boolVal;
					if (bool.TryParse(tokens.Peek(), out boolVal))
					{
						tokens.Dequeue();
						return new JSONNode
						{
							_boolVal = boolVal,
							nodeType = NodeType.BOOL,
						};
					}
					double doubleVal;
					if (double.TryParse(tokens.Peek(), NumberStyles.Number, CultureInfo.InvariantCulture, out doubleVal))
					{
						tokens.Dequeue();
						return new JSONNode
						{
							_numberVal = doubleVal,
							nodeType = NodeType.NUMBER,
						};
					}
					throw new Exception("Unexpected token: '" + tokens.Peek() + "'");
			}
		}

		void ExpectToken(string pExpected, string pActual)
		{
			if (pExpected != pActual)
			{
				throw new Exception("Unexpected token: " + pActual + " expected: " + pExpected + "");
			}
		}

		JSONNode ParseDictionary()
		{
			JSONNode first = null;
			JSONNode current = null;
			ExpectToken(tokens.Dequeue(), "{");
			if (tokens.Peek() == "}")
			{
				first = new JSONNode
				{
					nodeType = NodeType.EMPTY_DICTIONARY,
					_textVal = "",
					data = null
				};
			}
			else
			{
				while (tokens.Peek() != "}")
				{
					if (first != null)
					{
						ExpectToken(",", tokens.Dequeue());
					}

					var newNode = new JSONNode
					{
						nodeType = NodeType.DICTIONARY,
						_textVal = ParseKey(),
						data = ParseData(),
					};

					if (first == null)
						first = newNode;
					else
						current.next = newNode;

					current = newNode;
				}
			}

			ExpectToken("}", tokens.Dequeue()); //remove [
			return first;
		}

		string ParseString()
		{
			var str = string.Empty;
			ExpectToken("\"", tokens.Dequeue());
			if (tokens.Peek() != "\"") //watch out for empty strings
			{
				str = tokens.Dequeue();
			}
			ExpectToken("\"", tokens.Dequeue());
			return str;
		}
		string ParseKey()
		{
			var key = ParseString();
			ExpectToken(":", tokens.Dequeue());
			return key;
		}

		JSONNode ParseList()
		{
			JSONNode first = null;
			JSONNode current = null;
			ExpectToken("[", tokens.Dequeue()); //remove [
			if (tokens.Peek() == "]")
			{
				first = new JSONNode
				{
					nodeType = NodeType.EMPTY_LIST,
					_textVal = "",
					data = null
				};
			}
			else
			{
				int i = 0;
				while (tokens.Peek() != "]")
				{
					if (first != null)
					{
						ExpectToken(",", tokens.Dequeue());
					}

					var newListNode = new JSONNode
					{
						nodeType = NodeType.LIST,
						_numberVal = i++,
						data = ParseData()
					};
						
					if (first == null)
						first = newListNode;
					else
						current.next = newListNode;

					current = newListNode;
				}
			}
			ExpectToken("]", tokens.Dequeue()); //remove [
			return first;
		}
	}
}