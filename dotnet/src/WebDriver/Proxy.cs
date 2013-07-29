﻿// <copyright file="Proxy.cs" company="WebDriver Committers">
// Copyright 2007-2011 WebDriver committers
// Copyright 2007-2011 Google Inc.
// Portions copyright 2011 Software Freedom Conservancy
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
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
                this.VerifyProxyTypeCompatilibily(ProxyKind.AutoDetect);
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
                    return "pac";
                }

                return this.proxyKind.ToString().ToLowerInvariant();
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
        /// Gets or sets the value for when no proxy is specified.
        /// </summary>
        [JsonIgnore]
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

        private void VerifyProxyTypeCompatilibily(ProxyKind compatibleProxy)
        {
            if (this.proxyKind != ProxyKind.Unspecified && this.proxyKind != compatibleProxy)
            {
                throw new InvalidOperationException("Proxy autodetect is incompatible with manual settings");
            }
        }
    }
}
