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

        public website(string domain, string url, string title)
        {
            this.PartitionKey = domain;
            this.RowKey = url;
            this.Title = title;
        }

        public string Title { get; set; }

        public string Date { get; set; }
    }
}
