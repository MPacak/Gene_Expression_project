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
            // rewind the stream before reading!
            memoryStream.Position = 0;

            var patientRecords = new List<PatientGeneExpression>();
            using var reader = new StreamReader(memoryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

            // header
            var headerLine = reader.ReadLine();
            if (headerLine == null) return patientRecords;

            var headers = headerLine.Split('\t');
            var patientIds = headers.Skip(1)
                                    .Where(h => h.StartsWith("TCGA-"))
                                    .ToList();

            // init dictionary
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
                    // force InvariantCulture so "1.1165..." actually parses
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

            // build the final list
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
        /*   private List<PatientGeneExpression> ParseTSVFromMemory(MemoryStream memoryStream, string cancerCohort)
      {
          List<PatientGeneExpression> patientRecords = new();
          using StreamReader reader = new(memoryStream, Encoding.UTF8);

          string? headerLine = reader.ReadLine();
          if (headerLine == null) return patientRecords;

          var headers = headerLine.Split('\t');
          var patientIds = headers.Skip(1).ToList();

          Dictionary<string, Dictionary<string, double>> patientData = new();

          foreach (var patientId in patientIds)
          {
              if (patientId.StartsWith("TCGA-")) 
              {
                  patientData[patientId] = new Dictionary<string, double>();
              }
          }

          while (!reader.EndOfStream)
          {
              var line = reader.ReadLine();
              var columns = line?.Split('\t');



              if (columns == null || columns.Length != headers.Length)
                  continue;

              string geneName = columns[0];
              if (!CGAS_STING_GENES.Contains(geneName)) 
                  continue;

              for (int i = 1; i < columns.Length; i++) 
              {
                  if (double.TryParse(columns[i], out double expressionValue) && patientData.ContainsKey(patientIds[i - 1]))
                  {
                      patientData[patientIds[i - 1]][geneName] = expressionValue;
                  }
              }


          }
          foreach (var patientId in patientData.Keys)
          {
              patientRecords.Add(new PatientGeneExpression
              {
                  PatientId = patientId,
                  CancerCohort = cancerCohort,
                  GeneExpressions = patientData[patientId]
              });
          }

          Console.WriteLine($"Parsed {patientRecords.Count} valid patient records.");
          return patientRecords;
      }*/
    }

}
