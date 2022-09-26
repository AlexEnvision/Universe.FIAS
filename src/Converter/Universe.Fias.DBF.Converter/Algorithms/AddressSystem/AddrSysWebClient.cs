using System;
using System.Net;

namespace Universe.Fias.DBF.Converter.x64.Algorithms.AddressSystem
{
    /// <summary>
    /// Address system <seealso cref="System.Net.WebClient"/>.
    /// </summary>
    /// <seealso cref="System.Net.WebClient"/>
    public class AddrSysWebClient : WebClient
    {
        /// <summary>
        /// Gets or sets the resume file range.
        /// </summary>
        /// <value>
        /// The resume file range.
        /// </value>
        public int? ResumeFileRange { get; set; }

        /// <summary>Returns a <see cref="T:System.Net.WebRequest"/> object for the specified resource.</summary>
        /// <returns>A new <see cref="T:System.Net.WebRequest"/> object for the specified resource.</returns>
        /// <param name="address">A <see cref="T:System.Uri"/> that identifies the resource to request.</param>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);

            var httpWebRequest = webRequest as HttpWebRequest;
            if (httpWebRequest != null && ResumeFileRange != null)
                httpWebRequest.AddRange(ResumeFileRange.Value);

            return webRequest;
        }
    }
}