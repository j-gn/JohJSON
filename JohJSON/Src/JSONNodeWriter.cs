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
				return textWriter.ToString();
			}
		}
		public void WriteToStream(JSONNode pNode, Stream s)
		{
			using (textWriter = new StreamWriter(s))
			{
				WriteToStream(pNode, textWriter);
			}
		}
		public void WriteToStream(JSONNode pNode, TextWriter pTextWriter)
		{
			textWriter = pTextWriter;
			WriteData(pNode);
			textWriter.Flush();
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
					case '/':
						result += "\\/";
					break;
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
					default:
						result += c;
					break;
				}
			}
			return result;
		}
	}
}