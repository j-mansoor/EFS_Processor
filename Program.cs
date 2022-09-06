using NLog;


namespace EFS_FileProcessor
{
    internal class Program
    {
        private static readonly Logger Logger = Nlog_Configuration.NlogConfiguration.Config();
        static void Main(string[] args)
        {
            //pull settings from SSMS
             using JamesDBContext context = new JamesDBContext(); 
            string inputDir =
                (from pa in context.PropertyApplication
                 join p in context.Property on pa.PropertyID equals p.PropertyID
                 join a in context.Application on pa.ApplicationID equals a.ApplicationID
                 where a.ApplicationID == 352 //EFS application ID
                 where p.PropertyTypeID == 10 //directory 1
                 select p.Property1).SingleOrDefault();
            string outputDir =
                (from pa in context.PropertyApplication
                join p in context.Property on pa.PropertyID equals p.PropertyID
                join a in context.Application on pa.ApplicationID equals a.ApplicationID
                where a.ApplicationID == 352 //EFS application ID
                where p.PropertyTypeID == 11 //directory 2
                select p.Property1).SingleOrDefault();
            
            
            string outputFile;
            //iterate through files and process
            try
            {
            string[] filePaths = Directory.GetFiles(inputDir,$"*.csv");
                if (filePaths.Length==0)
                {
                    Logger.Error("No file found in the input directory");
                    return;
                }
                else
                {
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        outputFile = $"{outputDir}\\T-{DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)}({i + 1}).txt";
                        FileProcessor processor = new FileProcessor(filePaths[i], outputFile);
                        Logger.Info($"File {filePaths[i]} Sent for Processing");
                        processor.Process();
                    }
                }
            } 
            catch (DirectoryNotFoundException e)
            {
                Logger.Error("Directory does not exist");
                Logger.Error(e);
            }
            catch(ArgumentNullException e)
            {
                Logger.Error("Path is null");
                Logger.Error(e);
            }
            
        }
    }
}