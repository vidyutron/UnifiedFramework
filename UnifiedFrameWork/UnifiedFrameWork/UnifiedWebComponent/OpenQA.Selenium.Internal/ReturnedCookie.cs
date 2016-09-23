using System;
using System.Globalization;

namespace OpenQA.Selenium.Internal
{
	public class ReturnedCookie : Cookie
	{
		private bool isSecure;

		private bool isHttpOnly;

		public override bool Secure
		{
			get
			{
				return this.isSecure;
			}
		}

		public override bool IsHttpOnly
		{
			get
			{
				return this.isHttpOnly;
			}
		}

		public ReturnedCookie(string name, string value, string domain, string path, DateTime? expiry, bool isSecure, bool isHttpOnly) : base(name, value, domain, path, expiry)
		{
			this.isSecure = isSecure;
			this.isHttpOnly = isHttpOnly;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.Name,
				"=",
				base.Value,
				(!base.Expiry.HasValue) ? string.Empty : ("; expires=" + base.Expiry.Value.ToUniversalTime().ToString("ddd MM/dd/yyyy HH:mm:ss UTC", CultureInfo.InvariantCulture)),
				string.IsNullOrEmpty(this.Path) ? string.Empty : ("; path=" + this.Path),
				string.IsNullOrEmpty(base.Domain) ? string.Empty : ("; domain=" + base.Domain),
				this.isSecure ? "; secure" : string.Empty,
				this.isHttpOnly ? "; httpOnly" : string.Empty
			});
		}
	}
}
