using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Models.Dashboard.Intranet
{
    public class Notifications
    {
        public class User
        {
            public string Picture;
            public string Title;
            public string Url;
        }

        public class Notification
        {
            public string Id;
            public string Title;
            public string Class;
            public User User;
            public string Content;
            public string Date;
        }
    }
}
