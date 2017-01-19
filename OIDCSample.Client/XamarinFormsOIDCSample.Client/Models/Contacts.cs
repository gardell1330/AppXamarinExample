using System;
using System.Collections.Generic;


namespace XamarinFormsOIDCSample.Client.Views
{
    

    public class GroupContacts : List<Profile>
    {
        public string Type { get; set; }
        public GroupContacts(string _type)
        {
            Type = _type;
        }
    }

    public class GroupedContacts
    {
        public List<int> ungrouped { get; set; }
        public List<int> ignored { get; set; }
        public List<int> muted { get; set; }
        public object groups { get; set; }
        public List<int> blocked { get; set; }
    }

    public class Private
    {
        public GroupedContacts grouped_contacts { get; set; }
    }

    public class Profile
    {
        public int last_battle_time { get; set; }
        public int account_id { get; set; }
        public Private @private { get; set; }
        public int? clan_id { get; set; }
        public string nickname { get; set; }
        public object ban_info { get; set; }
        public double logout_at { get; set; }
        public string status { get; set; }        
    }

    public class Contacts : Profile
    {
    }
}