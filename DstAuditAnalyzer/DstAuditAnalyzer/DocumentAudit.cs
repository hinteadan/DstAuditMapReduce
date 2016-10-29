using System;
using System.Collections.Generic;
using System.Linq;

namespace DstAuditAnalyzer
{
    public class DocumentAudit
    {
        public readonly Guid DocumentId;
        public readonly DateTime[] Timestamps;
        public readonly string[] IpAddresses;
        public readonly string[] IpForwardedForAddresses;
        public readonly string[] OriginalFileNames;
        public readonly string[] Extractors;
        public readonly string[] Validators;

        public DocumentAudit(DocumentAuditEntry[] auditEntries)
        {
            var sortedEntries = auditEntries.OrderBy(a => a.Timestamp);

            this.DocumentId = auditEntries[0].Document.Id;
            this.Timestamps = sortedEntries.Select(a => a.Timestamp).ToArray();
            this.IpAddresses = sortedEntries
                .SelectMany(a => new string[] {
                    NullSafeValueFor(a.Metadata, "ClientIpAddress")
                })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();
            this.IpForwardedForAddresses = sortedEntries
                .SelectMany(a => new string[] {
                    NullSafeValueFor(a.Metadata, "X-Forwarded-For")
                })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();

            this.OriginalFileNames = sortedEntries
                .Select(a => a.Metadata["FilingOriginalFileName"])
                .Distinct()
                .ToArray();

            this.Extractors = sortedEntries
                .Select(a => a.Metadata["FilingExtractedBy"])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();

            this.Validators = sortedEntries
                .Select(a => a.Metadata["FilingValidatedBy"])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();
        }

        private static string NullSafeValueFor(Dictionary<string, string> dictionary, string key)
        {
            string value;
            if (!dictionary.TryGetValue(key, out value))
            {
                return null;
            }
            return value;
        }
    }
}