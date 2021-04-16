using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Models.Dashboard.Intranet
{
    public class Notes
    {
        public class Userinfo
        {
        }

        public class Rights
        {
        }

        public class Group
        {
            public string Title;
            public string Name;
            public int Count;
        }

        public class Event
        {
            public string IdEventFailed;
            public string IdUser;
            public string Begin;
            public string IdActiviteFailed;
        }

        public class GPA
        {
            public string Gpa;
            public string Cycle;
        }

        public class Root
        {
            public string Login;
            public string Title;
            public string InternalEmail;
            public string Lastname;
            public string Firstname;
            public Userinfo Userinfo;
            public bool ReferentUsed;
            public string Picture;
            public object PictureFun;
            public string Scolaryear;
            public int Promo;
            public int Semester;
            public string Location;
            public string Documents;
            public object Userdocs;
            public object Shell;
            public bool Close;
            public string Ctime;
            public string Mtime;
            public string IdPromo;
            public string IdHistory;
            public string CourseCode;
            public string SemesterCode;
            public string SchoolId;
            public string SchoolCode;
            public string SchoolTitle;
            public string OldIdPromo;
            public string OldIdLocation;
            public Rights Rights;
            public bool Invited;
            public int Studentyear;
            public bool Admin;
            public bool Editable;
            public bool Restrictprofiles;
            public List<Group> Groups;
            public List<Event> Events;
            public int Credits;
            public List<GPA> Gpa;
            public object Spice;
        }
    }
}
