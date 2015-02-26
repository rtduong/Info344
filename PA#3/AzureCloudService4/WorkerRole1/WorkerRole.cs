using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.IO;
using System.Xml;
using HtmlAgilityPack;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using ClassLibrary1;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private static HashSet<string> disallows = new HashSet<string>();
        private static HashSet<string> checkedUrls = new HashSet<string>();
        private static CloudTable webTable;

        public override void Run()
        {
            Thread.Sleep(50);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("queue");
            queue.CreateIfNotExists();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            webTable = tableClient.GetTableReference("indextable");
            webTable.CreateIfNotExists();
            Debug.WriteLine("made it to the run");
            
            if (queue.PeekMessage() != null && queue.PeekMessage().AsString.Equals("Started"))
            {
                CloudQueueMessage nextMessage = queue.PeekMessage();
                CloudQueue urlQueue = queueClient.GetQueueReference("urlqueue");
                Debug.WriteLine("Made it here");
                bool urlQueueHasNext = false;
                if (urlQueue.PeekMessage() != null)
                {
                    urlQueueHasNext = true;
                }
                bool notStop = true;
                while (notStop && urlQueueHasNext)
                {
                    Debug.WriteLine("made it to the while loop");
                    Thread.Sleep(50);
                    CloudQueueMessage currentUrlMessage = urlQueue.GetMessage();
                    if (currentUrlMessage != null)
                    {
                        string url = currentUrlMessage.AsString;
                        if (!disallows.Contains(url) && !checkedUrls.Contains(url))
                        {
                            Debug.WriteLine("made it to the string check");
                            if (url.Contains("robots.txt"))
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
                                            urlQueue.AddMessage(new CloudQueueMessage(tokens[1].ToString()));
                                        }
                                    }
                                    else if (tokens[0].Equals("Disallow:"))
                                    {
                                        disallows.Add(domain + tokens[1].ToString());
                                    }
                                }
                            }
                            else if (url.ToString().Contains(".xml"))
                            {
                                XmlDocument xml = new XmlDocument();
                                xml.Load(url);
                                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
                                if(url.ToString().Contains("bleacherreport"))
                                {
                                    nsmgr.AddNamespace("breport", "http://www.google.com/schemas/sitemap/0.9");
                                    XmlNodeList xnlist = xml.SelectNodes("//breport:loc", nsmgr);
                                    foreach (XmlNode xn in xnlist)
                                    {
                                        urlQueue.AddMessage(new CloudQueueMessage(xn.InnerText));
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
                                            urlQueue.AddMessage(new CloudQueueMessage(xn.InnerText));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                try
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
                                            urlQueue.AddMessage(new CloudQueueMessage(link.links));
                                            checkedUrls.Add(link.links);
                                        }
                                    }
                                    string domain = "";
                                    if (url.Contains("cnn"))
                                    {
                                        domain = "CNN";
                                    }
                                    else if (url.Contains("bleacherreport"))
                                    {
                                        domain = "BleacherReport";
                                    }
                                    string websiteTitle = document.DocumentNode.SelectSingleNode("//title").InnerText;
                                    string encodedUrl = EncodeUrlInKey(url);
                                    TableOperation retrieveOperation = TableOperation.Retrieve<website>(domain, encodedUrl);
                                    TableResult retrieveResult = webTable.Execute(retrieveOperation);
                                    if (retrieveResult.Result == null)
                                    {
                                        TableOperation insert = TableOperation.Insert(new website(domain, encodedUrl, websiteTitle));
                                        webTable.Execute(insert);
                                    }
                                }
                                catch { }
                            }
                            try
                            {
                                checkedUrls.Add(url);
                                urlQueue.DeleteMessage(currentUrlMessage);
                            }
                            catch
                            {
                                
                            }
                        }
                        try
                        {
                            nextMessage = queue.PeekMessage();
                            if (urlQueue.PeekMessage() == null)
                            {
                                urlQueueHasNext = false;
                            }
                            if (queue.PeekMessage().AsString.Equals("Stopped"))
                            {
                                notStop = false;
                            }
                        }
                        catch { }
                    }
                }
            }
            if (queue.PeekMessage() != null && queue.PeekMessage().AsString.Equals("Stopped"))
            {
                queue.Clear();
            }
        }

        private static string EncodeUrlInKey(string url)
        {
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(url);
            var base64 = System.Convert.ToBase64String(keyBytes);
            return base64.Replace('/', '_');
        }

        //public override bool OnStart()
        //{
        //    // Set the maximum number of concurrent connections
        //    ServicePointManager.DefaultConnectionLimit = 12;

        //    // For information on handling configuration changes
        //    // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

        //    bool result = base.OnStart();

        //    Trace.TraceInformation("WorkerRole1 has been started");

        //    return result;
        //}

        //public override void OnStop()
        //{
        //    //Trace.TraceInformation("WorkerRole1 is stopping");

        //    //this.cancellationTokenSource.Cancel();
        //    //this.runCompleteEvent.WaitOne();

        //    //base.OnStop();

        //    //Trace.TraceInformation("WorkerRole1 has stopped");
        //}

        //private async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Replace the following with your own logic.
        //    //while (!cancellationToken.IsCancellationRequested)
        //    //{
        //    //    Trace.TraceInformation("Working");
        //    //    await Task.Delay(1000);
        //    //}
        //}
    }
}
