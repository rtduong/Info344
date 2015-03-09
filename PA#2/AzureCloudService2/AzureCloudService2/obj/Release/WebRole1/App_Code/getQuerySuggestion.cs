using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
﻿using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.IO;
using System.Web.Script.Services;
using System.Text;
using System.Diagnostics;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class getQuerySuggestion : System.Web.Services.WebService {

    public static List<String> dictionary = new List<String>();
    private PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available Mbytes");

    public getQuerySuggestion () {
        // getWords();
    }

    [WebMethod]
    public float GetAvailableBytes()
    {
        float memUsage = memProcess.NextValue();
        return memUsage;
    }

    [WebMethod]
    public string getWords() 
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference("helloblob2");
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

}
