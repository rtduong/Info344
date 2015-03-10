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
using System.Text.RegularExpressions;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private static HashSet<string> disallows = new HashSet<string>();
        private static HashSet<string> checkedUrls = new HashSet<string>();
        private static CloudTable webTable2;

        public override void Run()
        {
            Thread.Sleep(50);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue2 = queueClient.GetQueueReference("queue2");
            queue2.CreateIfNotExists();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            webTable2 = tableClient.GetTableReference("indextable2");
            webTable2.CreateIfNotExists();
            Debug.WriteLine("made it to the run");
            
            if (queue2.PeekMessage() != null && queue2.PeekMessage().AsString.Equals("Started"))
            {
                CloudQueueMessage nextMessage = queue2.PeekMessage();
                CloudQueue urlQueue2 = queueClient.GetQueueReference("urlqueue2");
                CloudQueue lastCrawled = queueClient.GetQueueReference("lastcrawled");
                Debug.WriteLine("Made it here");
                bool urlQueueHasNext = false;
                if (urlQueue2.PeekMessage() != null)
                {
                    urlQueueHasNext = true;
                }
                bool notStop = true;
                Crawler crawler = new Crawler();
                while (notStop && urlQueueHasNext)
                {
                    Debug.WriteLine("made it to the while loop");
                    Thread.Sleep(50);
                    CloudQueueMessage currentUrlMessage = urlQueue2.GetMessage();
                    if (currentUrlMessage != null)
                    {
                        string url = currentUrlMessage.AsString;
                        if (!disallows.Contains(url) && !checkedUrls.Contains(url))
                        {
                            try
                            {
                                Debug.WriteLine("made it to the string check");
                                if (url.Contains("robots.txt"))
                                {
                                    crawler.processRobots(url, urlQueue2, disallows, checkedUrls, currentUrlMessage);
                                }
                                else if (url.ToString().Contains(".xml"))
                                {
                                    try
                                    {
                                        crawler.processXml(url, urlQueue2, checkedUrls, currentUrlMessage);
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        crawler.processHtml(url, urlQueue2, checkedUrls, webTable2, currentUrlMessage, lastCrawled);                                                                           
                                    }
                                    catch { }
                                }
                            }
                            catch { } 
                        }
                        try
                        {
                            nextMessage = queue2.PeekMessage();
                            if (urlQueue2.PeekMessage() == null)
                            {
                                urlQueueHasNext = false;
                            }
                            if (queue2.PeekMessage().AsString.Equals("Stopped"))
                            {
                                notStop = false;
                            }
                        }
                        catch { }
                    }
                }
                if (queue2.PeekMessage() != null && queue2.PeekMessage().AsString.Equals("Stopped"))
                {
                    queue2.Clear();
                }
            }
        }
    }
}
