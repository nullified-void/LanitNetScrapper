
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NetScrapper
{
    class Program
    {
        public static List<string> linksList = new List<string>();
        static void Main(string[] args)
        {
            
            Console.Write("Url: ");
            string startingUrl = Console.ReadLine();
            Console.Write("Depth: ");
            int maxDepth = int.Parse(Console.ReadLine());
            GetAllLinks(startingUrl, 0, maxDepth, startingUrl);
            foreach (string link in linksList)
            {
                Console.WriteLine(link);
            }
            Console.WriteLine("Links" + linksList.Count);
        }
        public static void  GetAllLinks(string link, int currentDepth, int maxDepth, string startingUrl)
        {
            if (currentDepth > maxDepth)
            {
                return;
            }
            WebClient wb = new WebClient();
            var parser = new HtmlParser();
            string Html;
            try
            {
                Html = wb.DownloadString(link);

            }
            catch (System.Net.WebException)
            {
                return;
            }
            var document = parser.ParseDocument(Html);
            Parallel.ForEach(document.QuerySelectorAll("a"), new ParallelOptions { MaxDegreeOfParallelism = 8 }, element =>
            {
                if (element.GetAttribute("href") == null)
                {
                    return;
                }
                if (element.GetAttribute("href").StartsWith('/'))
                {
                    string url = startingUrl + element.GetAttribute("href");
                    linksList.Add(url);
                    GetAllLinks(url, currentDepth + 1, maxDepth, startingUrl);
                }
                else
                {
                    linksList.Add(element.GetAttribute("href"));
                    GetAllLinks(element.GetAttribute("href"), currentDepth + 1, maxDepth, startingUrl);
                }
            });
        }
    }
}
