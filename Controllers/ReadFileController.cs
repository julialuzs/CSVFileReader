using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PocCSV.Controllers
{
    [ApiController]
    [Route("ReadFile")]
    public class ReadFileController : ControllerBase
    {

        private readonly ILogger<ReadFileController> _logger;

        public ReadFileController(ILogger<ReadFileController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Read([FromQuery] string link)
        {
            // https://support.staffbase.com/hc/en-us/article_attachments/360009197031/username.csv
            // https://file-examples-com.github.io/uploads/2017/02/file_example_XLS_10.xls

            string extension = Path.GetExtension(link);
            var path = DownloadFile(link, extension);

            if (extension.Equals(".csv"))
            {
                // return Ok(ReadXlxsFile(extension));
                return Ok(ReadCsvFile(path));
            }

            return Ok(ReadXlxsFile( extension));
        }

        public List<Excel> ReadXlxsFile(string extension)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Stream stream = System.IO.File.Open("file" + extension, FileMode.Open, FileAccess.Read);
            var reader = ExcelReaderFactory.CreateReader(stream);
            // var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
            // {
            //     AutodetectSeparators = new char[] { ',', ';', '|', '#' }
            // });

            var rowCount = 0;
            // var headers = new List<string>();

            List<Excel> results = new List<Excel>();
            List<Foo> csvResults = new List<Foo>();

            do
            {
                while (reader.Read())
                {

                    //map headers
                    // for (int column = 0; column < reader.FieldCount; column++)
                    // {
                    //     if (rowCount == 0) {
                    //         headers.Append(reader.GetValue(column));
                    //     }
                    // }

                    var hasValue = reader.GetValue(7) != null;

                    //csv
                    // var hasValue = reader.GetValue(1) != null;

                    if (rowCount > 0 && hasValue) {
                        // CSV
                        // var csv = new Foo {
                        //     Identifier = Int16.Parse(reader.GetValue(1).ToString()),
                        //     Username = reader.GetValue(0)?.ToString(),
                        //     FirstName = reader.GetValue(2)?.ToString(),
                        //     LastName = reader.GetValue(3)?.ToString()
                        // };
                        // csvResults.Add(csv);

                        var excel = new Excel {
                            FirstName = reader.GetValue(1)?.ToString(),
                            LastName = reader.GetValue(2)?.ToString(),
                            Gender = reader.GetValue(3)?.ToString(),
                            Country = reader.GetValue(4)?.ToString(),
                            Age = Int16.Parse(reader.GetValue(5).ToString()),
                            Date = Convert.ToDateTime(reader.GetValue(6).ToString()),
                            Id = Int16.Parse(reader.GetValue(7).ToString())
                        };
                        results.Add(excel);
                    }

                    rowCount++;
                }
            } while (reader.NextResult());

            return results;
        }


        public IEnumerable<Foo> ReadCsvFile(string path)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                MissingFieldFound = null
            };

            var reader = new StreamReader(path);
            var csv = new CsvReader(reader, config);

            csv.Read();
            csv.ReadHeader();

            var records = csv.GetRecords<Foo>().ToList().Select(r => new Foo {
                Identifier = r.Identifier,
                Username = r.Username,
                FirstName = r.FirstName,
                LastName = r.LastName
            });

            return records;
        }

        public string DownloadFile(string link, string extension) {

            string fileName = "file" + extension;

            WebClient myWebClient = new WebClient();

            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, link);
            myWebClient.DownloadFile(link, fileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, link);

            return fileName;
        }
    }
}
