using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Models.Dashboard.Ip
{
    public class IpGrab
    {
        public class Root
        {
            public string Ip;
        }
    }

    public class IpInfo
    {
        public class Root
        {
            public string Ip;
            public string City;
            public string Region;
            public string Country;
            public string Loc;
            public string Postal;
            public string Timezone;
            public string Readme;
        }
    }
}
