//
//  Tokenizer.cs
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



