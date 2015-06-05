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

using System.Collections.Generic;
using System.Text;
using System.IO;

namespace JohJSON
{
	public struct StringDelimiter {
		public override bool Equals(object obj)
		{
			if (obj is StringDelimiter)
				return true;
			else
				return false;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public override string ToString()
		{
			return string.Format("[StringDelimiter]");
		}
	}
	public class Tokenizer
	{
		const int BUFFER_SIZE = 4096;
		public char[] buffer = new char[BUFFER_SIZE];
		StringBuilder currentToken = new StringBuilder();
		bool insideQuotes = false;
		bool escape = false;
		readonly List<object> result = new List<object>();

		public List<object> ParseText(TextReader pReader)
		{
			result.Clear();
			int	count = 0;
			do
			{
				count = pReader.ReadBlock(buffer, 0, BUFFER_SIZE);
				for (int i = 0; i < count; i++)
				{
					char c = buffer[i];
					if (escape)
					{
						switch (c)
						{
							case 't':
								AddLetter('\t');
							break;
							case 'r':
								AddLetter('\r');
							break;
							case 'n':
								AddLetter('\n');
							break;
							case 'b':
								AddLetter('\b');
							break;
							case 'f':
								AddLetter('\f');
							break;
							case 'v':
								AddLetter('\v');
							break;
							default:
								AddLetter(c);
							break;
						}
						escape = false;
					}
					else
					{
						switch (c)
						{
							case '{':
							case '[':
							case '}':
							case ']':
							case ':':
							case ',': //Structural chars
								if (insideQuotes)
								{
									AddLetter(c);
								}
								else
								{
									PushStringToken();
									AddLetter(c);
									PushStringToken();
								}
							break;
							case '"': //Quotes, string start stop
								if (insideQuotes)
								{
									if (escape)
									{
										AddLetter(c);
									}
									else
									{
										PushStringDelimiter();
										insideQuotes = false;
									}
								}
								else
								{
									PushStringDelimiter();
									insideQuotes = true;
								}
							break;
							case '\'': //Whitespace and garbage
							case '\f':
							case '\v':
							case '\b':
							case '\r':
							case '\n':
							case ' ':
							case '\t':
								if (insideQuotes)
								{
									AddLetter(c);
								}
							break;
							case '\\':
								if (insideQuotes)
								{
									//don't add this letter, only escape next
									escape = true;
								}
							break;
							default:
								AddLetter(c);
							break;
						}
					}

				}

			} while(count == BUFFER_SIZE);
			PushStringToken();
			return result;
		}

		bool Contains(char[] pCollection, char pItem)
		{
			for (int i = 0; i < pCollection.Length; i++)
			{
				if (pItem == pCollection[i])
					return true;
			}
			return false;
		}



		bool Contains(string pCollection, char pItem)
		{
			for (int i = 0; i < pCollection.Length; i++)
			{
				if (pItem == pCollection[i])
					return true;
			}
			return false;
		}
		void PushStringDelimiter()
		{
			PushStringToken();
			result.Add(new StringDelimiter());
		}

		void PushStringToken()
		{
			if (currentToken.Length > 0)
			{
				result.Add(currentToken.ToString());
			}
			currentToken = new StringBuilder();
		}

		void AddLetter(char c)
		{
			currentToken.Append(c);
		}
	}
}



