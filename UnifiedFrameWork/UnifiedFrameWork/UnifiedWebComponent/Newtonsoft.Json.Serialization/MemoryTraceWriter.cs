using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Newtonsoft.Json.Serialization
{
	internal class MemoryTraceWriter : ITraceWriter
	{
		private readonly Queue<string> _traceMessages;

		public TraceLevel LevelFilter
		{
			get;
			set;
		}

		public MemoryTraceWriter()
		{
			this.LevelFilter = TraceLevel.Verbose;
			this._traceMessages = new Queue<string>();
		}

		public void Trace(TraceLevel level, string message, Exception ex)
		{
			string item = string.Concat(new string[]
			{
				DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff", CultureInfo.InvariantCulture),
				" ",
				level.ToString("g"),
				" ",
				message
			});
			if (this._traceMessages.Count >= 1000)
			{
				this._traceMessages.Dequeue();
			}
			this._traceMessages.Enqueue(item);
		}

		public IEnumerable<string> GetTraceMessages()
		{
			return this._traceMessages;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string current in this._traceMessages)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(current);
			}
			return stringBuilder.ToString();
		}
	}
}
