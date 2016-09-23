using System;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	internal class XProcessingInstructionWrapper : XObjectWrapper
	{
		private XProcessingInstruction ProcessingInstruction
		{
			get
			{
				return (XProcessingInstruction)base.WrappedNode;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.ProcessingInstruction.Target;
			}
		}

		public override string Value
		{
			get
			{
				return this.ProcessingInstruction.Data;
			}
			set
			{
				this.ProcessingInstruction.Data = value;
			}
		}

		public XProcessingInstructionWrapper(XProcessingInstruction processingInstruction) : base(processingInstruction)
		{
		}
	}
}
