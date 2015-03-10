using ClassLibrary1;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace WebRole1
{
    /// <summary>
    /// Summary description for Admin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]

    public class Admin : System.Web.Services.WebService
    {
        private PerformanceCounter cpuUtil = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter ramAvail = new PerformanceCounter("Memory", "Available MBytes");

        [WebMethod]
        public string Start()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudQueueClient urlQueueClient = storageAccount.CreateCloudQueueClient();
            CloudTable webTable2 = tableClient.GetTableReference("indextable2");
            webTable2.CreateIfNotExists();
            CloudQueue queue2 = queueClient.GetQueueReference("queue2");
            queue2.CreateIfNotExists();
            CloudQueue urlQueue2 = urlQueueClient.GetQueueReference("urlqueue2");
            urlQueue2.CreateIfNotExists();
            urlQueue2.Clear();
            urlQueue2.AddMessage(new CloudQueueMessage("http://www.bleacherreport.com/robots.txt"));
            urlQueue2.AddMessage(new CloudQueueMessage("http://www.cnn.com/robots.txt"));
            string message = "Started";
            CloudQueueMessage msg = new CloudQueueMessage(message);
            queue2.AddMessage(msg);
            return message;

        }

        [WebMethod]
        public string Stop()
        { 
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue2 = queueClient.GetQueueReference("queue2");
            queue2.CreateIfNotExists();
            CloudQueue urlQueue2 = queueClient.GetQueueReference("urlqueue2");
            urlQueue2.CreateIfNotExists();
            queue2.Clear();
            queue2.AddMessage(new CloudQueueMessage("Stopped"));
            urlQueue2.Clear();
            return "Stopped";
        }

        [WebMethod]
        public void DeleteTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table2 = tableClient.GetTableReference("indextable2");
            table2.CreateIfNotExists();
            table2.Delete();
        }

        [WebMethod]
        public float cpuUsage()
        {
            float cpuU = this.cpuUtil.NextValue();
            Thread.Sleep(1000);
            cpuU = this.cpuUtil.NextValue();
            return cpuU;
        }

        [WebMethod]
        public float ramUsage()
        {
            float ram = this.ramAvail.NextValue();
            Thread.Sleep(1000);
            ram = this.ramAvail.NextValue();
            return ram;
        }

        [WebMethod]
        public int? getTogo()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient urlQueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue urlQueue2 = urlQueueClient.GetQueueReference("urlqueue2");
            urlQueue2.CreateIfNotExists();
            urlQueue2.FetchAttributes();
            int? cachedMessageCount = urlQueue2.ApproximateMessageCount;
            return cachedMessageCount;
        }

        [WebMethod]
        public int getIndex()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table2 = tableClient.GetTableReference("indextable2");
            table2.CreateIfNotExists();
            int count = 0;
            TableQuery<website> cquery = new TableQuery<website>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, ""));
            foreach (website entity in table2.ExecuteQuery(cquery))
            {
                count++;
            }
            //TableQuery<website> bquery = new TableQuery<website>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "BleacherReport"));
            //foreach (website entity in table2.ExecuteQuery(bquery))
            //{
            //    br++;
            //}
            return count;
        }

        //[WebMethod]
        //public string getTitle(string url)
        //{
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("StorageConnectionString"));
        //    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        //    CloudTable table2 = tableClient.GetTableReference("indextable2");
        //    table2.CreateIfNotExists();
        //    string domain = "";
        //    if(url.ToLower().Contains("cnn"))
        //    {
        //        domain = "CNN";
        //    }
        //    else if (url.ToLower().Contains("bleacherreport"))
        //    {
        //        domain = "BleacherReport";
        //    }
        //    string enUrl = url;
        //    var keyBytes = System.Text.Encoding.UTF8.GetBytes(enUrl);
        //    var base64 = System.Convert.ToBase64String(keyBytes);
        //    enUrl = base64.Replace('/', '_');
        //    TableOperation retrieveOperation = TableOperation.Retrieve<website>(domain, enUrl);
        //    TableResult retrievedResult = table2.Execute(retrieveOperation);
        //    if (retrievedResult.Result != null)
        //    {
        //        return (((website)retrievedResult.Result).RowKey);
        //    }
        //    else
        //    {
        //        return "The title could not be retrieved.";
        //    }
        //}

        //Query Suggestiong stuff

        public static List<String> dictionary = new List<String>();
        private PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available Mbytes");

        [WebMethod]
        public float GetAvailableBytes()
        {
            float memUsage = memProcess.NextValue();
            return memUsage;
        }

        [WebMethod]
        public string getWords() 
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("helloblob3");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("output.txt");
            try
            {
                using (var file = blockBlob.OpenRead())
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        string line = sr.ReadLine();
                        while (GetAvailableBytes() > 50 || line != null)
                        {
                            dictionary.Add(line);
                            line = sr.ReadLine();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            //StringBuilder builder = new StringBuilder();
            //foreach (string word in dictionary)
            //{
            //    builder.Append(word).Append("|");
            //}
            //string result = builder.ToString();    --section can be uncommented confirm available words
            return "success" + dictionary.Count(); //+ result;
        }

        [WebMethod]
        public string returnLastTitle()
        {
            if (dictionary.Count > 0)
            {
                return dictionary.Last();
            }
            else return "No titles indexed.";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<string> getSuggestions(string search)
        {
            List<string> suggestionsList = new List<string>();
            int count = 0;
            foreach (string element in dictionary)
            {
                if (element.ToLower().StartsWith(search.ToLower()))
                {
                    suggestionsList.Add(element);
                    count++;
                    if (count == 10)
                    {
                        return suggestionsList;
                    }    
                }
            }
            if (count == 0)
            {
                suggestionsList.Add("No results found.");
            }
            return suggestionsList;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public int dictionaryCount()
        {
            return dictionary.Count;
        }

        private static Dictionary<string, List<string>> cache = new Dictionary<string,List<string>>();

        // Search results
        [WebMethod]
        public List<string> getSearchResults(string search)
        {
            if(cache.ContainsKey(search))
            {
                return cache[search];
            }
            else
            {
                string[] searchWordArray = Crawler.getTitleWords(search);
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table2 = tableClient.GetTableReference("indextable2");
                table2.CreateIfNotExists();
                TableQuery<website> rangeQuery = new TableQuery<website>();
                List<website> websiteList = new List<website>();
                foreach (string s in searchWordArray)
                {
                    string lowS = s.ToLower();
                    rangeQuery.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, lowS));
                    foreach (website entity in table2.ExecuteQuery(rangeQuery))
                    {
                        websiteList.Add(entity);
                    }
                }
                List<string> results = new List<string>();
                var rankings = websiteList
                    .Where(x => x.RowKey != "")
                    .GroupBy(x => x.RowKey)
                    .Select(x => new Tuple<string, string, int>(x.First().Title, x.First().RowKey, x.ToList().Count))
                    .OrderByDescending(x => x.Item3);
                Tuple<string, string, int>[] rankedList = new Tuple<string, string, int>[] { };
                rankedList = rankings.ToArray();
                var list = rankedList.Take(10);
                foreach (Tuple<string, string, int> w in list)
                {
                    try
                    {
                        results.Add("<br/>" + w.Item1 + "<br/>");
                        results.Add("<a href=&quot;" + Crawler.DecodeUrlInKey(w.Item2) + "&quot&;>" + Crawler.DecodeUrlInKey(w.Item2) + "</a> <br/>");
                    }
                    catch { }
                }
                if (results.Count() < 1)
                {
                    results.Add("No results found for this search term");
                }
                cache.Add(search, results);
                return results;
            }
        }

        [WebMethod]
        public List<string> lastCrawled()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue lastCrawled = queueClient.GetQueueReference("lastcrawled");
            lastCrawled.CreateIfNotExists();
            lastCrawled.FetchAttributes();
            int? approx = lastCrawled.ApproximateMessageCount;
            List<string> lastTen = new List<string>();
            if (approx > 10)
            {
                IEnumerable<CloudQueueMessage> messages = lastCrawled.GetMessages(10);                
                foreach (CloudQueueMessage m in messages)
                {
                    lastTen.Add(m.AsString);
                }
            }
            else
            {
                lastTen.Add("None added");
            }
            return lastTen;        
        }

        [WebMethod]
        public string HelloWorld()  
        {
            return "Hello World";
        }
    }
}
