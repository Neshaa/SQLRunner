using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptsRunner
{
    public class Settings
    { 
        public List<Details> Connections { get; set; }
    }

    public class Details
    {
        public bool Enabled { get; set; }
        public ConnectionString AdmConnectionString { get; set; }
        public ConnectionString ConnectionString { get; set; }

    }
    public class ConnectionString
    {
        public string Host { get; set; }
        public string DbName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
