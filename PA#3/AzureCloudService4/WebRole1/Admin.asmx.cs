using ClassLibrary1;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
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
            CloudTable table = tableClient.GetTableReference("indextable");
            table.CreateIfNotExists();
            CloudQueue queue = queueClient.GetQueueReference("queue");
            queue.CreateIfNotExists();
            CloudQueue urlQueue = urlQueueClient.GetQueueReference("urlqueue");
            urlQueue.CreateIfNotExists();
            urlQueue.Clear();
            urlQueue.AddMessage(new CloudQueueMessage("http://www.bleacherreport.com/robots.txt"));
            urlQueue.AddMessage(new CloudQueueMessage("http://www.cnn.com/robots.txt"));
            string message = "Started";
            CloudQueueMessage msg = new CloudQueueMessage(message);
            queue.AddMessage(msg);
            return message;

        }

        [WebMethod]
        public string Stop()
        { 
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("queue");
            queue.CreateIfNotExists();
            CloudQueue urlQueue = queueClient.GetQueueReference("urlqueue");
            urlQueue.CreateIfNotExists();
            queue.Clear();
            queue.AddMessage(new CloudQueueMessage("Stopped"));
            
            urlQueue.Clear();
            return "Stopped";
        }

        [WebMethod]
        public void DeleteTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("indextable");
            table.CreateIfNotExists();
            table.Delete();
        }

        [WebMethod]
        public string cpuUsage()
        {
            float cpuU = cpuUtil.NextValue();
            Thread.Sleep(1000);
            return cpuU.ToString();
        }

        [WebMethod]
        public float ramUsage()
        {
            float ram = ramAvail.NextValue();
            Thread.Sleep(1000);
            return ram;
        }

        [WebMethod]
        public int? getTogo()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient urlQueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue urlQueue = urlQueueClient.GetQueueReference("urlqueue");
            urlQueue.CreateIfNotExists();
            urlQueue.FetchAttributes();
            int? cachedMessageCount = urlQueue.ApproximateMessageCount;
            return cachedMessageCount;
        }

        [WebMethod]
        public int getIndex()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("indextable");
            table.CreateIfNotExists();
            int cnn = 0;
            int br = 0;
            TableQuery<website> cquery = new TableQuery<website>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "CNN"));
            foreach (website entity in table.ExecuteQuery(cquery))
            {
                cnn++;
            }
            TableQuery<website> bquery = new TableQuery<website>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "BleacherReport"));
            foreach (website entity in table.ExecuteQuery(bquery))
            {
                br++;
            }
            return cnn + br;
        }

        [WebMethod]
        public string getTitle(string url)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("indextable");
            table.CreateIfNotExists();
            string domain = "";
            if(url.ToLower().Contains("cnn"))
            {
                domain = "CNN";
            }
            else if (url.ToLower().Contains("bleacherreport"))
            {
                domain = "BleacherReport";
            }
            string enUrl = url;
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(enUrl);
            var base64 = System.Convert.ToBase64String(keyBytes);
            enUrl = base64.Replace('/', '_');
            TableOperation retrieveOperation = TableOperation.Retrieve<website>(domain, enUrl);
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                return (((website)retrievedResult.Result).Title);
            }
            else
            {
                return "The title could not be retrieved.";
            }
        }

        [WebMethod]
        public string HelloWorld()  
        {
            return "Hello World";
        }
    }
}
