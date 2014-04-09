//
//  JSONNode.cs
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
		public NodeType nodeType;
		internal string textVal;

		public string asText {
			get {
				return textVal;
			}
			set {

				if (value == null)
					nodeType = NodeType.NULL;
				else
					nodeType = NodeType.TEXT;

				textVal = value;
			}
		}

		internal bool boolVal;

		public bool asBool {
			get {
				return boolVal;
			}
			set {
				nodeType = NodeType.BOOL;
				boolVal = value;
			}
		}

		internal double numberVal;

		public double asNumber {
			get {
				return numberVal;
			}
			set {
				nodeType = NodeType.NUMBER;
				numberVal = value;
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

		void MakeList(int pKey)
		{
			if (nodeType != NodeType.LIST)
			{
				numberVal = pKey;
			}
			nodeType = NodeType.LIST;
		}

		void CreateNextNode()
		{
			next = new JSONNode();
		}

		void MakeDictionary(string pKey)
		{
			if (nodeType != NodeType.DICTIONARY)
			{
				textVal = pKey;
			}
			nodeType = NodeType.DICTIONARY;
		}

		public JSONNode this [int pKey] {
			get {
				MakeList(pKey);
				if ((int)numberVal == pKey)
				{
					if (data == null)
					{
						data = new JSONNode
						{ 
							nodeType = NodeType.NULL,
						};
					}

					return data;
				}
				else
				{
					if (next == null)
						CreateNextNode();

					return next[pKey];
				}
			}
			set {
				MakeList(pKey);
				if ((int)numberVal == pKey)
				{
					data = value;
				}
				else
				{
					if (next == null)
						CreateNextNode();

					next[pKey] = value;
				}
			}
		}

		public JSONNode this [string pKey] {
			get {

				MakeDictionary(pKey);
				if (textVal == pKey)
				{
					if (data == null)
					{
						data = new JSONNode
						{
							nodeType = NodeType.NULL,
						};
					}
					return data;
				}
				else
				{
					if (next == null)
						CreateNextNode();

					return next[pKey];
				}
			}
			set {
				MakeDictionary(pKey);
				if (textVal == pKey)
				{
					data = value;
				}
				else
				{
					if (next == null)
						CreateNextNode();

					next[pKey] = value;
				}
			}
		}

		public bool PropertyExists(string pName)
		{
			if (this.nodeType == NodeType.DICTIONARY)
			{
				if (textVal == pName)
					return true;
				if (next == null)
					return false;

				return next.PropertyExists(pName);
			}
			return false;
		}

		public bool ElementExists(int pIndex)
		{
			if (this.nodeType == NodeType.LIST)
			{
				if (numberVal == pIndex)
					return true;
				if (next == null)
					return false;
				return next.ElementExists(pIndex);
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
					yield return new KeyValuePair<string, JSONNode>(t.textVal, t.data);
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