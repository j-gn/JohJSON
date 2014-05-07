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
	public class Tokenizer
	{
		const int BUFFER_SIZE = 4096;
		public char[] buffer = new char[BUFFER_SIZE];
		StringBuilder currentToken = new StringBuilder();
		bool insideQuotes = false;
		bool escape = false;
		readonly List<string> result = new List<string>();

		public List<string> ParseText(TextReader pReader)
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
							default  :
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
							case ',':
								if (insideQuotes)
								{
									AddLetter(c);
								}
								else
								{
									PushWord();
									AddLetter(c);
									PushWord();
								}
							break;
							case '"':
								if (!insideQuotes)
								{
									PushWord();
									AddLetter(c);
									PushWord();
									insideQuotes = true;
								}
								else
								{
									if (escape)
									{
										AddLetter(c);
									}
									else
									{
										PushWord();
										AddLetter(c);
										PushWord();
										insideQuotes = false;
									}
								}
							break;
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
			PushWord();
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

		void PushWord()
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



