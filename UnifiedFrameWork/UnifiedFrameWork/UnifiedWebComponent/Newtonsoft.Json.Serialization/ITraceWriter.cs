using System;
using System.Diagnostics;

namespace Newtonsoft.Json.Serialization
{
	internal interface ITraceWriter
	{
		TraceLevel LevelFilter
		{
			get;
		}

		void Trace(TraceLevel level, string message, Exception ex);
	}
}
