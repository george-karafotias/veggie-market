using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeggieMarketDataReader
{
    public class ExcelDataReader
    {
        public static DataTable ReadFile(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var encoding = Encoding.GetEncoding("UTF-8");
                    using (var reader = ExcelReaderFactory.CreateReader(stream,
                      new ExcelReaderConfiguration() { FallbackEncoding = encoding }))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = false }
                        });

                        if (result.Tables.Count > 0)
                        {
                            return result.Tables[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
