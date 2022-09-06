using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using NLog;

namespace EFS_FileProcessor
{
    internal class FileProcessor
    {
    private static readonly Logger Logger = Nlog_Configuration.NlogConfiguration.Config();
        private string InputFilePath { get; }
        private string OutputFilePath { get; } 
       

        internal FileProcessor(string inputPath, string outputPath)
        {
            InputFilePath = inputPath;
            OutputFilePath = outputPath;
        } 

        internal void Process()
        {
            
            
                using StreamReader inputReader = new StreamReader(InputFilePath);
                using StreamWriter outputWriter = new StreamWriter(OutputFilePath);

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                TrimOptions = TrimOptions.Trim
                };
                using CsvReader csvReader = new CsvReader(inputReader, csvConfiguration);
                using CsvWriter csvWriter = new CsvWriter(outputWriter, csvConfiguration);

                IEnumerable<dynamic> records = csvReader.GetRecords<dynamic>();
                int cardNumCount = 0;
                foreach (dynamic record in records)
                {
                    if (record.Field1 == "TA")
                    {
                        record.Field5 = record.Field5.Substring(0, 4);
                        cardNumCount++;
                    }
                        try
                        {
                        csvWriter.WriteRecord(record);
                        csvWriter.NextRecord();
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"Writer unable to write to {Path.GetFileName(OutputFilePath)}");
                            Logger.Error(e);
                        }

                }
                Logger.Info($"{Path.GetFileName(InputFilePath)} Processed to {Path.GetFileName(OutputFilePath)} with {cardNumCount} card numbers truncated");
            
            
        }

    }
}
