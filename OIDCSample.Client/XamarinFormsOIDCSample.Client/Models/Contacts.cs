using System;
using System.Collections.Generic;


namespace XamarinFormsOIDCSample.Client.Views
{
    

    public class Groups
    {
    }

    public class GroupedContacts
    {
        public List<int> ungrouped { get; set; }
        public List<int> ignored { get; set; }
        public List<int> muted { get; set; }
        public Groups groups { get; set; }
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
        public object clan_id { get; set; }
        public string nickname { get; set; }
        public object ban_info { get; set; }
        public int logout_at { get; set; }        
    }

    public class Contacts : Profile
    {
    }
}