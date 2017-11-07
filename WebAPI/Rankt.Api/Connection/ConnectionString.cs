using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrakkerApp.Api.Connection
{
    public class ConnectionString
    {
        
            public static string CONNECTION_STRING = "Data Source=(localdb)\\ProjectsV13;Initial Catalog=movietvdb;" +
                                                     "Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;" +
                                                     "ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

    }


}
