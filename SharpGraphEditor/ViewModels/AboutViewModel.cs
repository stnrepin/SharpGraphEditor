using System;
using System.Reflection;

using Caliburn.Micro;

namespace SharpGraphEditor.ViewModels
{
    public class AboutViewModel : PropertyChangedBase
    {
        private string _title;
        private string _projectName;
        private string _author;
        private string _email;
        private string _creationDate;
        private string _projectSite;

        public AboutViewModel()
        {
            ProjectName = "#GraphEditor";

            Title = "About " + ProjectName;
            Author = "Stepan Repin";
            Email = "stepan.repin@inbox.ru";
            CreationDate = GetPackageBuildingDate().ToShortDateString();
            ProjectSite = "https://github.com/AceSkiffer/SharpGraphEditor";
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public string ProjectName
        {
            get { return _projectName; }
            set
            {
                _projectName = value;
                NotifyOfPropertyChange(() => ProjectName);
            }
        }

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                NotifyOfPropertyChange(() => Author);
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        public string CreationDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value;
                NotifyOfPropertyChange(() => CreationDate);
            }
        }

        public string ProjectSite
        {
            get { return _projectSite; }
            set
            {
                _projectSite = value;
                NotifyOfPropertyChange(() => ProjectSite);
            }
        }

        private DateTime GetPackageBuildingDate()
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;

            var filePath = Assembly.GetCallingAssembly().Location;
            var b = new byte[2048];

            using (var s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                s.Read(b, 0, 2048);
            }

            var i = BitConverter.ToInt32(b, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(b, i + linkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }
    }
}
