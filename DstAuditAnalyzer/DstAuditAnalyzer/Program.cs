using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DstAuditAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var auditStoragePath = @"C:\Users\hinte_000\Downloads\RawAuditSample";

            var result = new MapReduceResult();

            foreach (var documentFolderPath in Directory.EnumerateDirectories(auditStoragePath, "*", SearchOption.TopDirectoryOnly))
            {
                DocumentAudit documentAudit = LoadDocumentAudit(documentFolderPath).Result;
            }
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
