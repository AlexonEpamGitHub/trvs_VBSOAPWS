using System;
using System.Runtime.Serialization;

namespace SOAPCoreServices.Models
{
    /// <summary>
    /// Input parameters for report generation
    /// </summary>
    [DataContract(Namespace = "http://tempuri.org/")]
    public class ReportInput
    {
        /// <summary>
        /// Gets or sets the name of the report to generate
        /// </summary>
        [DataMember(IsRequired = true, Order = 1)]
        public string ReportName { get; set; } = string.Empty;
    }
}