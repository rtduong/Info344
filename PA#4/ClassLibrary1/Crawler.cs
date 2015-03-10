using HtmlAgilityPack;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ClassLibrary1
{
    public class Crawler
    {
        public Crawler() { }

        public void processRobots(string url, CloudQueue urlQueue2, HashSet<string> disallows, HashSet<string> checkedUrls, CloudQueueMessage currentUrlMessage)
        {
            WebClient wClient = new WebClient();
            Stream data = wClient.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string content = reader.ReadToEnd();
            string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            int urlLength = url.Length;
            string domain = url.Remove(urlLength - 11); // removes '/robots.txt' from end of url
            foreach (string line in lines)
            {
                string[] tokens = line.Split(' ');
                if (tokens[0].Equals("Sitemap:"))
                {
                    if (!tokens[1].Equals("http://bleacherreport.com/sitemap.xml") &&
                        !tokens[1].Equals("http://bleacherreport.com/sitemap/auto-racing.xml") &&
                        !tokens[1].Equals("http://bleacherreport.com/sitemap/college-basketball.xml") &&
                        !tokens[1].Equals("http://bleacherreport.com/sitemap/college-football.xml") &&
                        !tokens[1].Equals("http://bleacherreport.com/sitemap/mlb.xml") &&
                        !tokens[1].Equals("http://bleacherreport.com/sitemap/nfl.xml") &&
                        !tokens[1].Equals("http://bleacherreport.com/sitemap/nhl.xml") &&
                        !tokens[1].Equals("http://bleacherreport.com/sitemap/world-football.xml"))
                    {
                        urlQueue2.AddMessage(new CloudQueueMessage(tokens[1].ToString()));
                    }
                }
                else if (tokens[0].Equals("Disallow:"))
                {
                    disallows.Add(domain + tokens[1].ToString());
                }
            }
            checkedUrls.Add(url);
            urlQueue2.DeleteMessage(currentUrlMessage);
        }


        public void processXml(string url, CloudQueue urlQueue2, HashSet<string> checkedUrls, CloudQueueMessage currentUrlMessage)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(url);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
            if (url.ToString().Contains("bleacherreport"))
            {
                nsmgr.AddNamespace("breport", "http://www.google.com/schemas/sitemap/0.9");
                XmlNodeList xnlist = xml.SelectNodes("//breport:loc", nsmgr);
                foreach (XmlNode xn in xnlist)
                {
                    urlQueue2.AddMessage(new CloudQueueMessage(xn.InnerText));
                }
            }
            else if (url.ToString().Contains("cnn.com"))
            {
                nsmgr.AddNamespace("breport", "http://www.sitemaps.org/schemas/sitemap/0.9");
                XmlNodeList xnlist = xml.SelectNodes("//breport:loc", nsmgr);
                foreach (XmlNode xn in xnlist)
                {
                    if (xn.InnerText.Contains("cnn.com/2015"))
                    {
                        urlQueue2.AddMessage(new CloudQueueMessage(xn.InnerText));
                    }
                }
            }
            checkedUrls.Add(url);
            urlQueue2.DeleteMessage(currentUrlMessage);
        }


        public void processHtml(string url, CloudQueue urlQueue2, HashSet<string> checkedUrls, CloudTable webTable2, CloudQueueMessage currentUrlMessage, CloudQueue lastCrawled)
        {
            var webGet = new HtmlWeb();
            var document = webGet.Load(url.ToString());
            var linksOnPage = from lnks in document.DocumentNode.Descendants()
                              where lnks.Name == "a" &&
                              lnks.Attributes["href"] != null &&
                              lnks.InnerText.Trim().Length > 0
                              select new
                              {
                                  links = lnks.Attributes["href"].Value,
                                  text = lnks.InnerText
                              };
            foreach (var link in linksOnPage)
            {
                if (link.ToString().Contains("http") &&
                    (link.ToString().ToLower().Contains("cnn.com") || link.ToString().ToLower().Contains("http://bleacherreport.com")) && !checkedUrls.Contains(link.links))
                {
                    urlQueue2.AddMessage(new CloudQueueMessage(link.links));
                    checkedUrls.Add(link.links);
                }
            }
            string websiteTitle = document.DocumentNode.SelectSingleNode("//title").InnerText;
            string[] titleWords = getTitleWords(websiteTitle);
            string encodedUrl = EncodeUrlInKey(url);
            foreach (string w in titleWords)
            {
                string lowerW = w.ToLower();
                TableOperation retrieveOperation = TableOperation.Retrieve<website>(lowerW, encodedUrl);
                TableResult retrieveResult = webTable2.Execute(retrieveOperation);
                if (retrieveResult.Result == null)
                {
                    TableOperation insert = TableOperation.Insert(new website(lowerW, encodedUrl, websiteTitle));
                    webTable2.Execute(insert);
                }
            }
            checkedUrls.Add(url);
            int tenCount = 0;
            lastCrawled.AddMessage(new CloudQueueMessage(url));
            tenCount++;
            if(tenCount > 10)
            {
                CloudQueueMessage retrievedMessage = lastCrawled.GetMessage();
                lastCrawled.DeleteMessage(retrievedMessage);
                tenCount--;
            }
            urlQueue2.DeleteMessage(currentUrlMessage);
        }

        public static string EncodeUrlInKey(string url) //http://stackoverflow.com/questions/21144694/how-can-i-encode-azure-storage-table-row-keys-and-partition-keys
        {
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(url);
            var base64 = System.Convert.ToBase64String(keyBytes);
            return base64.Replace('/', '_');
        }

        public static String DecodeUrlInKey(string encodedKey)
        {
            var base64 = encodedKey.Replace('_', '/');
            byte[] bytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static string[] getTitleWords(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\b[\w']*\b");
            var words = from m in matches.Cast<Match>()
                        where !string.IsNullOrEmpty(m.Value)
                        select trim(m.Value);

            return words.ToArray();
        }
        public static string trim(string word)
        {
            int apostropheLocation = word.IndexOf('\'');
            if (apostropheLocation != -1)
            {
                word = word.Substring(0, apostropheLocation);
            }

            return word;
        }
    }
}
