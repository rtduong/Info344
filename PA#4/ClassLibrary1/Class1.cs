using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using Microsoft.WindowsAzure.Storage.Table;

namespace ClassLibrary1
{
    public class website : TableEntity
    {
        public website() { }

        public website(string word, string url, string title)
        {
            this.PartitionKey = word;
            this.RowKey = url;
            this.Title = title;
        }

        public string Title { get; set; }
        public string Date { get; set; }
        public string Url { get; set; }
    }
}
