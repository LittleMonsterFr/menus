using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Menus
{
    public static class Utils
    {
        public static async Task<List<Tuple<string, string, string>>> parseCsv(StorageFile file)
        {
            var parsedResult = new List<Tuple<string, string, string>>();

            string text = await Windows.Storage.FileIO.ReadTextAsync(file);

            var records = text.Split("\"\n\"");
            foreach (var record in records)
            {
                var fields = record.Split("\"|\"");
                string s1 = fields[0].TrimStart('"').TrimEnd('"').Trim().TrimEnd('\n');
                string s2 = fields[1];
                string s3 = null;
                if (!string.IsNullOrEmpty(fields[2]))
                    s3 = fields[2].TrimStart('"').TrimEnd('"').Trim().TrimEnd('\n');
                var recordItem = new Tuple<string, string, string>(s1, s2, s3);
                parsedResult.Add(recordItem);
            }

            return parsedResult;
        }
    }
}