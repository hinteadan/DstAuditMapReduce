using System;
using System.Collections.Generic;

namespace DstAuditAnalyzer
{
    public class DocumentAuditEntry
    {
        public class TokenSnapshot
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public int Page { get; set; }
            public string Text { get; set; }
            public int IndexInFullText { get; set; }
        }
        public class ReferenceSnapshot
        {
            public int StartIndex { get; set; }
            public int Length { get; set; }
            public TokenSnapshot[] Tokens { get; set; }
        }

        public class CellSnapshot
        {
            public string Value { get; set; }
            public ReferenceSnapshot Reference { get; set; }
            public string[] SortedKeywords { get; set; }
        }

        public class TableSnapshot
        {
            public CellSnapshot[] Header { get; set; }
            public CellSnapshot[][] Body { get; set; }
        }

        public class DocumentSnapshot
        {
            public Guid Id { get; set; }
            public TableSnapshot[] Tables { get; set; }
        }

        public DateTime Timestamp { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public DocumentSnapshot Document { get; set; }
    }
}