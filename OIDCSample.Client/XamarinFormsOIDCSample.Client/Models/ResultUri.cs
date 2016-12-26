
using System.Collections.Generic;

namespace XamarinFormsOIDCSample.Client.Views
{
    public class ResultUri
    {
        public string status { get; set; }
        public meta meta { get; set; }
        public data data { get; set; }
    }

    public class data
    {
        public List<Profile> account { get; set; }        
    }

    public class meta
    {
        public int count { get; set; }
    }
}