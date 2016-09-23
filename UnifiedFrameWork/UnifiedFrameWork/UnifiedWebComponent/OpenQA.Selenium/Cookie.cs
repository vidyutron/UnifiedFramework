using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using OpenQA.Selenium.Internal;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Represents a cookie in the browser.
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Cookie
    {
        private string cookieName;
        private string cookieValue;
        private string cookiePath;
        private string cookieDomain;
        private DateTime? cookieExpiry;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cookie"/> class with a specific name,
        /// value, domain, path and expiration date.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="domain">The domain of the cookie.</param>
        /// <param name="path">The path of the cookie.</param>
        /// <param name="expiry">The expiration date of the cookie.</param>
        /// <exception cref="ArgumentException">If the name is <see langword="null"/> or an empty string,
        /// or if it contains a semi-colon.</exception>
        /// <exception cref="ArgumentNullException">If the value is <see langword="null"/>.</exception>
        public Cookie(string name, string value, string domain, string path, DateTime? expiry)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Cookie name cannot be null or empty string", "name");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value", "Cookie value cannot be null");
            }

            if (name.IndexOf(';') != -1)
            {
                throw new ArgumentException("Cookie names cannot contain a ';': " + name, "name");
            }

            this.cookieName = name;
            this.cookieValue = value;
            if (!string.IsNullOrEmpty(path))
            {
                this.cookiePath = path;
            }

            this.cookieDomain = StripPort(domain);

            if (expiry != null)
            {
                this.cookieExpiry = expiry;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cookie"/> class with a specific name,
        /// value, path and expiration date.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="path">The path of the cookie.</param>
        /// <param name="expiry">The expiration date of the cookie.</param>
        /// <exception cref="ArgumentException">If the name is <see langword="null"/> or an empty string,
        /// or if it contains a semi-colon.</exception>
        /// <exception cref="ArgumentNullException">If the value is <see langword="null"/>.</exception>
        public Cookie(string name, string value, string path, DateTime? expiry)
            : this(name, value, null, path, expiry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cookie"/> class with a specific name,
        /// value, and path.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="path">The path of the cookie.</param>
        /// <exception cref="ArgumentException">If the name is <see langword="null"/> or an empty string,
        /// or if it contains a semi-colon.</exception>
        /// <exception cref="ArgumentNullException">If the value is <see langword="null"/>.</exception>
        public Cookie(string name, string value, string path)
            : this(name, value, path, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cookie"/> class with a specific name and value.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <exception cref="ArgumentException">If the name is <see langword="null"/> or an empty string,
        /// or if it contains a semi-colon.</exception>
        /// <exception cref="ArgumentNullException">If the value is <see langword="null"/>.</exception>
        public Cookie(string name, string value)
            : this(name, value, null, null)
        {
        }

        /// <summary>
        /// Gets the name of the cookie.
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get { return this.cookieName; }
        }

        /// <summary>
        /// Gets the value of the cookie.
        /// </summary>
        [JsonProperty("value")]
        public string Value
        {
            get { return this.cookieValue; }
        }

        /// <summary>
        /// Gets the domain of the cookie.
        /// </summary>
        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain
        {
            get { return this.cookieDomain; }
        }

        /// <summary>
        /// Gets the path of the cookie.
        /// </summary>
        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Path
        {
            get { return this.cookiePath; }
        }

        /// <summary>
        /// Gets a value indicating whether the cookie is secure.
        /// </summary>
        [JsonProperty("secure")]
        public virtual bool Secure
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the cookie is an HTTP-only cookie.
        /// </summary>
        [JsonProperty("httpOnly")]
        public virtual bool IsHttpOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the expiration date of the cookie.
        /// </summary>
        public DateTime? Expiry
        {
            get { return this.cookieExpiry; }
        }

        /// <summary>
        /// Gets the cookie expiration date in seconds from the defined zero date (01 January 1970 00:00:00 UTC).
        /// </summary>
        /// <remarks>This property only exists so that the JSON serializer can serialize a
        /// cookie without resorting to a custom converter.</remarks>
        [JsonProperty("expiry", NullValueHandling = NullValueHandling.Ignore)]
        internal long? ExpirySeconds
        {
            get
            {
                if (this.cookieExpiry == null)
                {
                    return null;
                }

                DateTime zeroDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan span = this.cookieExpiry.Value.ToUniversalTime().Subtract(zeroDate);
                long totalSeconds = Convert.ToInt64(span.TotalSeconds);
                return totalSeconds;
            }
        }

        /// <summary>
        /// Converts a Dictionary to a Cookie.
        /// </summary>
        /// <param name="rawCookie">The Dictionary object containing the cookie parameters.</param>
        /// <returns>A <see cref="Cookie"/> object with the proper parameters set.</returns>
        public static Cookie FromDictionary(Dictionary<string, object> rawCookie)
        {
            if (rawCookie == null)
            {
                throw new ArgumentNullException("rawCookie", "Dictionary cannot be null");
            }

            string name = rawCookie["name"].ToString();
            string value = rawCookie["value"].ToString();

            string path = "/";
            if (rawCookie.ContainsKey("path") && rawCookie["path"] != null)
            {
                path = rawCookie["path"].ToString();
            }

            string domain = string.Empty;
            if (rawCookie.ContainsKey("domain") && rawCookie["domain"] != null)
            {
                domain = rawCookie["domain"].ToString();
            }

            DateTime? expires = null;
            if (rawCookie.ContainsKey("expiry") && rawCookie["expiry"] != null)
            {
                double seconds = 0;
                if (double.TryParse(rawCookie["expiry"].ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out seconds))
                {
                    try
                    {
                        expires = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).ToLocalTime();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        expires = DateTime.MaxValue.ToLocalTime();
                    }
                }
            }

            bool secure = false;
            if (rawCookie.ContainsKey("secure") && rawCookie["secure"] != null)
            {
                secure = bool.Parse(rawCookie["secure"].ToString());
            }

            bool isHttpOnly = false;
            if (rawCookie.ContainsKey("httpOnly") && rawCookie["httpOnly"] != null)
            {
                isHttpOnly = bool.Parse(rawCookie["httpOnly"].ToString());
            }

            return new ReturnedCookie(name, value, domain, path, expires, secure, isHttpOnly);
        }

        /// <summary>
        /// Creates and returns a string representation of the cookie.
        /// </summary>
        /// <returns>A string representation of the cookie.</returns>
        public override string ToString()
        {
            return this.cookieName + "=" + this.cookieValue
                + (this.cookieExpiry == null ? string.Empty : "; expires=" + this.cookieExpiry.Value.ToUniversalTime().ToString("ddd MM dd yyyy hh:mm:ss UTC", CultureInfo.InvariantCulture))
                    + (string.IsNullOrEmpty(this.cookiePath) ? string.Empty : "; path=" + this.cookiePath)
                    + (string.IsNullOrEmpty(this.cookieDomain) ? string.Empty : "; domain=" + this.cookieDomain);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object">Object</see> is equal
        /// to the current <see cref="object">Object</see>.
        /// </summary>
        /// <param name="obj">The <see cref="object">Object</see> to compare with the
        /// current <see cref="object">Object</see>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="object">Object</see>
        /// is equal to the current <see cref="object">Object</see>; otherwise,
        /// <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            // Two cookies are equal if the name and value match
            Cookie cookie = obj as Cookie;

            if (this == obj)
            {
                return true;
            }

            if (cookie == null)
            {
                return false;
            }

            if (!this.cookieName.Equals(cookie.cookieName))
            {
                return false;
            }

            return !(this.cookieValue != null ? !this.cookieValue.Equals(cookie.cookieValue) : cookie.Value != null);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="object">Object</see>.</returns>
        public override int GetHashCode()
        {
            return this.cookieName.GetHashCode();
        }

        private static string StripPort(string domain)
        {
            return string.IsNullOrEmpty(domain) ? null : domain.Split(':')[0];
        }
    }
}