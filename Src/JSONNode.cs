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

namespace JohJSON
{
	public enum NodeType
	{
		LIST,
		DICTIONARY,
		VALUE,
		NUMBER,
		TEXT,
		BOOL,
	}

	public class JSONNode
	{
		public NodeType nodeType = NodeType.VALUE;
		internal string textVal;

		public string asText {
			get {
				return textVal;
			}
			set {
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

		public int length { 
			get {
				if (next == null)
					return 1;
				else
					return next.length + 1;
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

		public JSONNode this [int pKey] {
			get {
				MakeList(pKey);
				if (Math.Round(numberVal) == pKey)
				{
					if (data == null)
						data = new JSONNode();

					return data;
				}
				else
				{
					if (next == null)
					{
						next = new JSONNode
						{
							numberVal = pKey
						};
					}
					return next[pKey];
				}
			}
			set {
				MakeList(pKey);
				if (numberVal == pKey)
				{

					data = value;
				}
				else
				{
					if (next == null)
					{
						next = new JSONNode
						{
							numberVal = pKey
						};
					}
					next[pKey] = value;
				}
			}
		}

		void MakeDictionary(string pKey)
		{
			if (nodeType != NodeType.DICTIONARY)
			{
				textVal = pKey;
			}
			nodeType = NodeType.DICTIONARY;
		}

		public JSONNode this [string pKey] {
			get {

				MakeDictionary(pKey);
				if (textVal == pKey)
				{
					if (data == null)
						data = new JSONNode();
					return data;
				}
				else
				{
					if (next == null)
					{
						next = new JSONNode
						{
							textVal = pKey
						};
					}
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
					{
						next = new JSONNode
						{
							textVal = pKey
						};
					}
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

		public override string ToString()
		{
			var w = new JSONNodeWriter();
			return w.WriteToString(this);
		}
	}
}