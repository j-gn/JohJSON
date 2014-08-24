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

namespace JohJSON
{
	public enum NodeType
	{
		NULL,
		LIST,
		DICTIONARY,
		EMPTY_DICTIONARY,
		EMPTY_LIST,
		VALUE,
		NUMBER,
		TEXT,
		BOOL,
	}

	public class JSONNode
	{
		public NodeType nodeType { get; set; }

		/// <summary>
		/// Internal use only.
		/// </summary>
		internal string _textVal;

		public string asText {
			get {
				return _textVal;
			}
			set {

				if (value == null)
					nodeType = NodeType.NULL;
				else
					nodeType = NodeType.TEXT;

				_textVal = value;
			}
		}

		internal bool _boolVal;

		public bool asBool {
			get {
				return _boolVal;
			}
			set {
				nodeType = NodeType.BOOL;
				_boolVal = value;
			}
		}

		internal double _numberVal;

		public double asNumber {
			get {
				return _numberVal;
			}
			set {
				nodeType = NodeType.NUMBER;
				_numberVal = value;
			}
		}

		public int asInt {
			get { 
				return (int)asNumber;
			}

			set { 
				asNumber = value;
			}
		}

		public float asFloat {
			get { 
				return (float)asNumber;
			}

			set { 
				asNumber = value;
			}
		}

		public int length { 
			get {
				if (nodeType == NodeType.LIST || nodeType == NodeType.DICTIONARY)
				{
					if (next == null)
						return 1;
					else
						return next.length + 1;
				}
				else
				{
					return 0;
				}
			}
		}

		public JSONNode next = null;
		public JSONNode data = null;

		public JSONNode FindKeyByIndex(int pKey)
		{
			JSONNode self = this;
			TryNextLabel:
			;

			//make list
			if (self.nodeType != NodeType.LIST)
			{
				_numberVal = pKey;
			}
			self.nodeType = NodeType.LIST;

			if ((int)self._numberVal == pKey)
			{
				//create datanode
				if (self.data == null)
				{
					self.data = new JSONNode
					{ 
						nodeType = NodeType.NULL,
					};
				}
				return self.data;
			}
			else
			{
				//create index node
				if (self.next == null)
					self.next = new JSONNode();

				self = self.next;
				goto TryNextLabel;
			}
		}


		public JSONNode this [int pKey] {
			get {
				return FindNodeByIndex(pKey).data;
			}
			set {
				FindNodeByIndex(pKey).data = value;
			}
		}
		JSONNode FindNodeByIndex(int pKey)
		{
			JSONNode self = this;
			bool foundKey = false;

			while (!foundKey)
			{
				if (((int)self._numberVal) == pKey && self.nodeType == NodeType.LIST)
				{
					foundKey = true;
				}
				else if (self.nodeType != NodeType.LIST)
				{
					self._numberVal = pKey;
					self.nodeType = NodeType.LIST;
					foundKey = true;

				}
				if (!foundKey)
				{
					if(self.next == null)
						self.next = new JSONNode();
					self = self.next;
				}
			}
			if (self.data == null)
			{
				self.data = new JSONNode
				{
					nodeType = NodeType.NULL,
				};
			}
			return self;
		}

		JSONNode FindNodeByString(string pKey)
		{
			JSONNode self = this;
			bool foundKey = false;

			while (!foundKey)
			{
				if (self._textVal == pKey && self.nodeType == NodeType.DICTIONARY)
				{
					foundKey = true;
				}
				else if (self.nodeType != NodeType.DICTIONARY)
				{
					self._textVal = pKey;
					self.nodeType = NodeType.DICTIONARY;
					foundKey = true;
				
				}
				if (!foundKey)
				{
					if(self.next == null)
						self.next = new JSONNode();
					self = self.next;
				}
			}
			if (self.data == null)
			{
				self.data = new JSONNode
				{
					nodeType = NodeType.NULL,
				};
			}
			return self;
		}

		public JSONNode this [string pKey] {
			get {
				return FindNodeByString(pKey).data;
			}
			set {
				FindNodeByString(pKey).data = value;
			}
		}

		public bool PropertyExists(string pName)
		{
			JSONNode self = this;
			while (self.nodeType == NodeType.DICTIONARY)
			{
				if (self._textVal == pName)
					return true;
				if (self.next == null)
					return false;

				self = self.next;
			}
			return false;
		}

		public bool ElementExists(int pIndex)
		{
			JSONNode self = this;
			while (self.nodeType == NodeType.LIST)
			{
				if (self._numberVal == pIndex)
					return true;
				if (self.next == null)
					return false;
				self = self.next;
			}
			return false;
		}

		public DictionaryEnumerable dictionaryValues {
			get {
				return new DictionaryEnumerable(this);
			}
		}

		public class DictionaryEnumerable : IEnumerable<KeyValuePair<string, JSONNode>>
		{
			JSONNode node;

			public DictionaryEnumerable(JSONNode pNode)
			{
				node = pNode;
			}

			public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
			{
				if (node.nodeType != NodeType.DICTIONARY)
					yield break;
				var t = node;
				while (t != null)
				{
					yield return new KeyValuePair<string, JSONNode>(t._textVal, t.data);
					t = t.next;
				}
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public ListEnumerable listValues {
			get {
				return new ListEnumerable(this);
			}
		}

		public class ListEnumerable : IEnumerable<JSONNode>
		{
			JSONNode node;

			public ListEnumerable(JSONNode pNode)
			{
				node = pNode;
			}

			public IEnumerator<JSONNode> GetEnumerator()
			{
				if (node.nodeType != NodeType.LIST)
					yield break;

				var t = node;
				while (t != null)
				{
					yield return t.data;
					t = t.next;
				}
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public override string ToString()
		{
			var w = new JSONNodeWriter();
			return w.WriteToString(this);
		}

		public static JSONNode CreateFromString(string pString)
		{
			Generator g = new Generator();
			return g.Generate(pString);
		}

		public static JSONNode empty {
			get{ return new JSONNode{ nodeType = NodeType.NULL }; }
		}
	}
}