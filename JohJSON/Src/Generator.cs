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
		StringDelimiter DELIMITER = new StringDelimiter();
		Tokenizer tokenizer = new Tokenizer();
		Queue<object> tokens;
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
		public JSONNode Generate(IEnumerable<object> pTokens){
			tokens = new Queue<object>(pTokens);
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
			var token = tokens.Peek();
			var tokenStr = token as string;
			if (token is StringDelimiter)
			{
				return new JSONNode
				{
					nodeType = NodeType.TEXT,
					_textVal = ParseString(),
				};
			}
			else
			{
				switch (tokenStr)
				{
					case "{":
						return ParseDictionary();
					case "[":
						return ParseList();
					case "null":
						tokens.Dequeue();
						return new JSONNode
						{
							nodeType = NodeType.NULL
						};
					default:
						bool boolVal;
						if (bool.TryParse(tokenStr, out boolVal))
						{
							tokens.Dequeue();
							return new JSONNode
							{
								_boolVal = boolVal,
								nodeType = NodeType.BOOL,
							};
						}
						double doubleVal;
						if (double.TryParse(tokenStr, NumberStyles.Number, CultureInfo.InvariantCulture, out doubleVal))
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
		}

		void ExpectToken(object pExpected, object pActual)
		{
			if (!pExpected.Equals(pActual))
			{
				throw new Exception("Unexpected token: " + pActual + " expected: " + pExpected + "");
			}
		}

		JSONNode ParseDictionary()
		{
			JSONNode first = null;
			JSONNode current = null;
			ExpectToken(tokens.Dequeue(), "{");
			if (tokens.Peek().ToString() == "}")
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
				while (tokens.Peek().ToString() != "}")
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
					//Console.WriteLine("-> " + newNode._textVal + " || " + newNode.data);

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
			ExpectToken(DELIMITER, tokens.Dequeue());

			if (!tokens.Peek().Equals(DELIMITER)) //watch out for empty strings
			{
				str = tokens.Dequeue() as string;
			}
			ExpectToken(DELIMITER, tokens.Dequeue());
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
			if (tokens.Peek().ToString() == "]")
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
				while (tokens.Peek().ToString() != "]")
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