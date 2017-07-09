using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckRedirects
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = ParseFilename(args);
            var culture = ParseCulture(args);
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);

            var results = new CheckResults();
            IEnumerable<RedirectUrls> redirectList = ParseExcelFile(filename);
          
            foreach (var redirectUrls in redirectList)
            {
                var request = new Request { RequestUrl = redirectUrls.Old };
                var client = new Client();
                var response = client.Get(request);
                var checkResult = Analyze(response, redirectUrls);
                results.Add(checkResult);
            }
            Output(results);
        }

        private static string ParseCulture(string[] args)
        {
            var culture = args.Length > 1 ? args[1] : null;
            culture = culture ?? ConfigurationManager.AppSettings["Culture"] ?? "en-US";
            return culture;
        }

        private static void Output(CheckResults results)
        {
            foreach (var result in results.Items.Where(x => x.IsOk==false))
            {
                Console.WriteLine(result);
            }
        }

        private static CheckResult Analyze(Response response, RedirectUrls urls)
        {
            var result = new CheckResult();

            if (response.StatusCode == 301)
            {
                var redirectResponse = (RedirectResponse)response;
                result.IsOk = (redirectResponse.RedirectUrl.Equals(urls.New, StringComparison.CurrentCultureIgnoreCase));
                result.Details = $"{response.StatusCode} - {redirectResponse.RedirectUrl}";
                
            }
            else
            {
                result.IsOk = false;
                result.Details = urls.Old;
            }
            return result;
            //var redirectResponse = response as RedirectResponse;

            //if (redirectResponse != null)
            //{ }
            //else { }
        }

        private static IEnumerable<RedirectUrls> ParseExcelFile(string filename)
        {
            var redirectsList = new List<RedirectUrls>();
            var excelPackage = new ExcelPackage(new System.IO.FileInfo(filename));
            var worksheet = excelPackage.Workbook.Worksheets.First();
            for (int i = 1; i <= 10000; i++)
            {
                var oldUrl = worksheet.Cells[$"A{i}"].Value?.ToString();
                var newUrl = worksheet.Cells[$"B{i}"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(oldUrl) || string.IsNullOrWhiteSpace(newUrl))
                { break; }
                else
                {
                    redirectsList.Add(new RedirectUrls { Old = oldUrl, New = newUrl});
                } 
            }
            return redirectsList;
        }

        private static string ParseFilename(string[] args)
        {
            var filename = args.FirstOrDefault() ?? ConfigurationManager.AppSettings["SourceFile"] ?? "redirects.xlsx";
            return filename;
        }
    }
}
