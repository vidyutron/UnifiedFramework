using System;

namespace OpenQA.Selenium.Remote
{
	public class SessionId
	{
		private string sessionOpaqueKey;

		public SessionId(string opaqueKey)
		{
			this.sessionOpaqueKey = opaqueKey;
		}

		public override string ToString()
		{
			return this.sessionOpaqueKey;
		}

		public override int GetHashCode()
		{
			return this.sessionOpaqueKey.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			SessionId sessionId = obj as SessionId;
			if (sessionId != null)
			{
				result = this.sessionOpaqueKey.Equals(sessionId.sessionOpaqueKey);
			}
			return result;
		}
	}
}
