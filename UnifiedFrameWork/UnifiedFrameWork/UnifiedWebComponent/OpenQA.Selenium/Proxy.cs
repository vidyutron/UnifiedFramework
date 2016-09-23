using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Describes the kind of proxy.
    /// </summary>
    /// <remarks>
    /// Keep these in sync with the Firefox preferences numbers:
    /// http://kb.mozillazine.org/Network.proxy.type
    /// </remarks>
    public enum ProxyKind
    {
        /// <summary>
        ///  Direct connection, no proxy (default on Windows).
        /// </summary>
        Direct = 0,

        /// <summary>
        /// Manual proxy settings (e.g., for httpProxy).
        /// </summary>
        Manual,

        /// <summary>
        /// Proxy automatic configuration from URL.
        /// </summary>
        ProxyAutoConfigure,

        /// <summary>
        /// Use proxy automatic detection.
        /// </summary>
        AutoDetect = 4,

        /// <summary>
        /// Use the system values for proxy settings (default on Linux).
        /// </summary>
        System,

        /// <summary>
        /// No proxy type is specified.
        /// </summary>
        Unspecified
    }

    /// <summary>
    /// Describes proxy settings to be used with a driver instance.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Proxy
    {
        private ProxyKind proxyKind = ProxyKind.Unspecified;
        private bool isAutoDetect;
        private string ftpProxyLocation;
        private string httpProxyLocation;
        private string noProxy;
        private string proxyAutoConfigUrl;
        private string sslProxyLocation;
        private string socksProxyLocation;
        private string socksUserName;
        private string socksPassword;

        /// <summary>
        /// Initializes a new instance of the <see cref="Proxy"/> class.
        /// </summary>
        public Proxy()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Proxy"/> class with the given proxy settings.
        /// </summary>
        /// <param name="settings">A dictionary of settings to use with the proxy.</param>
        public Proxy(Dictionary<string, object> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings", "settings dictionary cannot be null");
            }

            if (settings.ContainsKey("proxyType"))
            {
                ProxyKind rawType = (ProxyKind)Enum.Parse(typeof(ProxyKind), settings["proxyType"].ToString(), true);
                this.Kind = rawType;
            }

            if (settings.ContainsKey("ftpProxy"))
            {
                this.FtpProxy = settings["ftpProxy"].ToString();
            }

            if (settings.ContainsKey("httpProxy"))
            {
                this.HttpProxy = settings["httpProxy"].ToString();
            }

            if (settings.ContainsKey("noProxy"))
            {
                this.NoProxy = settings["noProxy"].ToString();
            }

            if (settings.ContainsKey("proxyAutoconfigUrl"))
            {
                this.ProxyAutoConfigUrl = settings["proxyAutoconfigUrl"].ToString();
            }

            if (settings.ContainsKey("sslProxy"))
            {
                this.SslProxy = settings["sslProxy"].ToString();
            }

            if (settings.ContainsKey("socksProxy"))
            {
                this.SocksProxy = settings["socksProxy"].ToString();
            }

            if (settings.ContainsKey("socksUsername"))
            {
                this.SocksUserName = settings["socksUsername"].ToString();
            }

            if (settings.ContainsKey("socksPassword"))
            {
                this.SocksPassword = settings["socksPassword"].ToString();
            }

            if (settings.ContainsKey("autodetect"))
            {
                this.IsAutoDetect = (bool)settings["autodetect"];
            }
        }

        /// <summary>
        /// Gets or sets the type of proxy.
        /// </summary>
        [JsonIgnore]
        public ProxyKind Kind
        {
            get
            {
                return this.proxyKind;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(value);
                this.proxyKind = value;
            }
        }

        /// <summary>
        /// Gets the type of proxy as a string for JSON serialization.
        /// </summary>
        [JsonProperty("proxyType")]
        public string SerializableProxyKind
        {
            get
            {
                if (this.proxyKind == ProxyKind.ProxyAutoConfigure)
                {
                    return "PAC";
                }

                return this.proxyKind.ToString().ToUpperInvariant();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the proxy uses automatic detection.
        /// </summary>
        [JsonIgnore]
        public bool IsAutoDetect
        {
            get
            {
                return this.isAutoDetect;
            }

            set
            {
                if (this.isAutoDetect == value)
                {
                    return;
                }

                this.VerifyProxyTypeCompatilibily(ProxyKind.AutoDetect);
                this.proxyKind = ProxyKind.AutoDetect;
                this.isAutoDetect = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the proxy for the FTP protocol.
        /// </summary>
        [JsonProperty("ftpProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string FtpProxy
        {
            get
            {
                return this.ftpProxyLocation;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                this.proxyKind = ProxyKind.Manual;
                this.ftpProxyLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the proxy for the HTTP protocol.
        /// </summary>
        [JsonProperty("httpProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string HttpProxy
        {
            get
            {
                return this.httpProxyLocation;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                this.proxyKind = ProxyKind.Manual;
                this.httpProxyLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the value for bypass proxy addresses.
        /// </summary>
        [JsonProperty("noProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string NoProxy
        {
            get
            {
                return this.noProxy;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                this.proxyKind = ProxyKind.Manual;
                this.noProxy = value;
            }
        }

        /// <summary>
        /// Gets or sets the URL used for proxy automatic configuration.
        /// </summary>
        [JsonProperty("proxyAutoconfigUrl", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string ProxyAutoConfigUrl
        {
            get
            {
                return this.proxyAutoConfigUrl;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(ProxyKind.ProxyAutoConfigure);
                this.proxyKind = ProxyKind.ProxyAutoConfigure;
                this.proxyAutoConfigUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the proxy for the SSL protocol.
        /// </summary>
        [JsonProperty("sslProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string SslProxy
        {
            get
            {
                return this.sslProxyLocation;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                this.proxyKind = ProxyKind.Manual;
                this.sslProxyLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the proxy for the SOCKS protocol.
        /// </summary>
        [JsonProperty("socksProxy", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string SocksProxy
        {
            get
            {
                return this.socksProxyLocation;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                this.proxyKind = ProxyKind.Manual;
                this.socksProxyLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of username for the SOCKS proxy.
        /// </summary>
        [JsonProperty("socksUsername", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string SocksUserName
        {
            get
            {
                return this.socksUserName;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                this.proxyKind = ProxyKind.Manual;
                this.socksUserName = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of password for the SOCKS proxy.
        /// </summary>
        [JsonProperty("socksPassword", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string SocksPassword
        {
            get
            {
                return this.socksPassword;
            }

            set
            {
                this.VerifyProxyTypeCompatilibily(ProxyKind.Manual);
                this.proxyKind = ProxyKind.Manual;
                this.socksPassword = value;
            }
        }

        private void VerifyProxyTypeCompatilibily(ProxyKind compatibleProxy)
        {
            if (this.proxyKind != ProxyKind.Unspecified && this.proxyKind != compatibleProxy)
            {
                string errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "Specified proxy type {0} is not compatible with current setting {1}",
                    compatibleProxy.ToString().ToUpperInvariant(),
                    this.proxyKind.ToString().ToUpperInvariant());

                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}