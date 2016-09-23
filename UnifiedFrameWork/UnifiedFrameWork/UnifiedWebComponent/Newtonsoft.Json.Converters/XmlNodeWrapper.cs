using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal class XmlNodeWrapper : IXmlNode
	{
		private readonly XmlNode _node;

		public object WrappedNode
		{
			get
			{
				return this._node;
			}
		}

		public XmlNodeType NodeType
		{
			get
			{
				return this._node.NodeType;
			}
		}

		public string LocalName
		{
			get
			{
				return this._node.LocalName;
			}
		}

		public IList<IXmlNode> ChildNodes
		{
			get
			{
				return (from XmlNode n in this._node.ChildNodes
				select this.WrapNode(n)).ToList<IXmlNode>();
			}
		}

		public IList<IXmlNode> Attributes
		{
			get
			{
				if (this._node.Attributes == null)
				{
					return null;
				}
				return (from XmlAttribute a in this._node.Attributes
				select this.WrapNode(a)).ToList<IXmlNode>();
			}
		}

		public IXmlNode ParentNode
		{
			get
			{
				XmlNode xmlNode = (this._node is XmlAttribute) ? ((XmlAttribute)this._node).OwnerElement : this._node.ParentNode;
				if (xmlNode == null)
				{
					return null;
				}
				return this.WrapNode(xmlNode);
			}
		}

		public string Value
		{
			get
			{
				return this._node.Value;
			}
			set
			{
				this._node.Value = value;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return this._node.NamespaceURI;
			}
		}

		public XmlNodeWrapper(XmlNode node)
		{
			this._node = node;
		}

		private IXmlNode WrapNode(XmlNode node)
		{
			XmlNodeType nodeType = node.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				return new XmlElementWrapper((XmlElement)node);
			}
			if (nodeType != XmlNodeType.XmlDeclaration)
			{
				return new XmlNodeWrapper(node);
			}
			return new XmlDeclarationWrapper((XmlDeclaration)node);
		}

		public IXmlNode AppendChild(IXmlNode newChild)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
			this._node.AppendChild(xmlNodeWrapper._node);
			return newChild;
		}
	}
}
