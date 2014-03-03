//
//  Generator.cs
//
//  Author:
//       Johannes Gotlen <johannes@gotlen.se>
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

		public JSONNode Generate(string s)
		{
			var t = tokenizer.ParseText(new StringReader(s));
			tokens = new Queue<string>(t);
			if (tokens.Count > 0)
			{
				return ParseData();
			}
			else
			{
				throw new Exception("no tokens");
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
						textVal = ParseString(),
					};
				default:
					bool boolVal;
					if (bool.TryParse(tokens.Peek(), out boolVal))
					{
						tokens.Dequeue();
						return new JSONNode
						{
							boolVal = boolVal,
							nodeType = NodeType.BOOL,
						};
					}
					double doubleVal;
					if (double.TryParse(tokens.Peek(), NumberStyles.Number, CultureInfo.InvariantCulture, out doubleVal))
					{
						tokens.Dequeue();
						return new JSONNode
						{
							numberVal = doubleVal,
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
			while (tokens.Peek() != "}")
			{
				if (first != null)
				{
					ExpectToken(",", tokens.Dequeue());
				}

				var newNode = new JSONNode
				{
					nodeType = NodeType.DICTIONARY,
					textVal = ParseKey(),
					data = ParseData(),
				};

				if (first == null)
					first = newNode;
				else
					current.next = newNode;

				current = newNode;
			}
			ExpectToken("}", tokens.Dequeue()); //remove [

			return first;
		}

		string ParseString()
		{
			ExpectToken("\"", tokens.Dequeue());
			var str = tokens.Dequeue();
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
					numberVal = i++,
					data = ParseData()
				};
						
				if (first == null)
					first = newListNode;
				else
					current.next = newListNode;

				current = newListNode;
			}
			ExpectToken("]", tokens.Dequeue()); //remove [
			return first;
		}
	}
}