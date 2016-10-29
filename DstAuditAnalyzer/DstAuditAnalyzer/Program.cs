using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DstAuditAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var auditStoragePath = @"C:\Users\hinte_000\Downloads\RawAuditSample";
            var outputPath = @"C:\Users\hinte_000\Downloads\DstIpAddresses.txt";

            var ipAddresses = new ConcurrentDictionary<string, int>();

            int docCounter = 0;

            Directory.EnumerateDirectories(auditStoragePath, "*", SearchOption.TopDirectoryOnly)
                .AsParallel()
                .ForAll(p =>
                {
                    var docAudit = LoadDocumentAudit(p).Result;
                    foreach (string ip in docAudit.IpAddresses.Concat(docAudit.IpForwardedForAddresses))
                    {
                        ipAddresses.AddOrUpdate(ip, 1, (key, count) => count + 1);
                    }
                    Interlocked.Increment(ref docCounter);
                    Console.WriteLine($"{docCounter} - Done reducing {docAudit.DocumentId} @ {DateTime.Now}");
                });

            File.WriteAllLines(outputPath, ipAddresses.OrderByDescending(x => x.Value).Select(a => a.Key));
        }

        private static async Task<DocumentAudit> LoadDocumentAudit(string documentFolderPath)
        {
            return await Task.WhenAll(
                Directory.EnumerateFiles(documentFolderPath)
                    .Select(auditEntryFilePath => Task.Run(() => LoadDocumentAuditEntry(auditEntryFilePath)))
                )
                .ContinueWith(t => new DocumentAudit(t.Result));
        }

        private static DocumentAuditEntry LoadDocumentAuditEntry(string auditEntryFilePath)
        {
            return JsonConvert.DeserializeObject<DocumentAuditEntry>(File.ReadAllText(auditEntryFilePath));
        }
    }
}
