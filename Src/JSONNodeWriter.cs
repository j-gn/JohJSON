//
//  JSONNodeWriter.cs
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

using System.Globalization;
using System.IO;

namespace JohJSON
{
	public class JSONNodeWriter
	{
		TextWriter textWriter = null;

		public string WriteToString(JSONNode pNode)
		{
			using (textWriter = new StringWriter())
			{
				WriteToStream(pNode, textWriter);
			}
			return textWriter.ToString();
		}

		public void WriteToStream(JSONNode pNode, TextWriter pTextWriter)
		{
			textWriter = pTextWriter;
			WriteData(pNode);
		}

		void ToJSONString(JSONNode pNode)
		{
			if (pNode.nodeType == NodeType.DICTIONARY)
			{
				textWriter.Write('"');
				textWriter.Write(Escape(pNode.asText));
				textWriter.Write('"');
				textWriter.Write(":");
				WriteData(pNode.data);
			}
			if (pNode.nodeType == NodeType.LIST)
			{
				WriteData(pNode.data);
			}
			if (pNode.next != null)
			{
				textWriter.Write(",");
				ToJSONString(pNode.next);
			}
		}

		void WriteData(JSONNode pNode)
		{
			if (pNode == null || pNode.nodeType == NodeType.NULL)
			{
				textWriter.Write("null");
			}
			else
			{
				switch (pNode.nodeType)
				{
					case NodeType.EMPTY_DICTIONARY:
						textWriter.Write("{}");
					break;
					case NodeType.DICTIONARY:
						textWriter.Write("{");
						ToJSONString(pNode);
						textWriter.Write("}");
					break;
					case NodeType.LIST:
						textWriter.Write("[");
						ToJSONString(pNode);
						textWriter.Write("]");
					break;
					case NodeType.EMPTY_LIST:
						textWriter.Write("[]");
					break;
					case NodeType.BOOL:
						textWriter.Write(pNode.asBool.ToString());
					break;
					case NodeType.TEXT:
						textWriter.Write('"');
						textWriter.Write(Escape(pNode.asText));
						textWriter.Write('"');
					break;
					case NodeType.NUMBER:
						textWriter.Write(pNode.asNumber.ToString(CultureInfo.InvariantCulture));
					break;
					default:
					break;
				}
			}
		}

		string Escape(string pText)
		{
			string result = "";
			foreach (char c in pText)
			{
				switch (c)
				{
					case '\\':
						result += "\\\\";
					break;
					case '\"':
						result += "\\\"";
					break;
					case '\n':
						result += "\\n";
					break;
					case '\r':
						result += "\\r";
					break;
					case '\t':
						result += "\\t";
					break;
					case '\b':
						result += "\\b";
					break;
					case '\f':
						result += "\\f";
					break;
					default   :
						result += c;
					break;
				}
			}
			return result;
		}
	}
}