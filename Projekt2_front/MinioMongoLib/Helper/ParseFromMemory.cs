using MinioMongoService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioMongoLib.Helper
{
    public static class ParseFromMemory
    {
        private static readonly HashSet<string> CGAS_STING_GENES = new()
        {
            "C6orf150", "CCL5", "CXCL10", "TMEM173", "CXCL9",
            "CXCL11", "NFKB1", "IKBKE", "IRF3", "TREX1",
            "ATM", "IL6", "IL8"
        };
        public static List<PatientGeneExpression> ParseTSVFromMemory(MemoryStream memoryStream, string cancerCohort)
        {

            memoryStream.Position = 0;

            var patientRecords = new List<PatientGeneExpression>();
            using var reader = new StreamReader(memoryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);


            var headerLine = reader.ReadLine();
            if (headerLine == null) return patientRecords;

            var headers = headerLine.Split('\t');
            var patientIds = headers.Skip(1)
                                    .Where(h => h.StartsWith("TCGA-"))
                                    .ToList();

            var patientData = patientIds.ToDictionary(
                id => id,
                id => new Dictionary<string, double>());

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = line.Split('\t');
                if (cols.Length != headers.Length) continue;

                var gene = cols[0];
                if (!CGAS_STING_GENES.Contains(gene)) continue;

                for (int i = 1; i < cols.Length; i++)
                {
                    var raw = cols[i].Trim();
                    if (double.TryParse(raw,
                                        NumberStyles.Float | NumberStyles.AllowLeadingSign,
                                        CultureInfo.InvariantCulture,
                                        out var val)
                        && patientData.ContainsKey(patientIds[i - 1]))
                    {
                        patientData[patientIds[i - 1]][gene] = val;
                    }
                }
            }

            foreach (var kv in patientData)
            {
                patientRecords.Add(new PatientGeneExpression
                {
                    PatientId = kv.Key,
                    CancerCohort = cancerCohort,
                    GeneExpressions = kv.Value
                });
            }

            Console.WriteLine($"Parsed {patientRecords.Count} records with decimal gene-expressions.");
            return patientRecords;
        }
      
    }

}
