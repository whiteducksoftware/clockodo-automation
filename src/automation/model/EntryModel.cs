namespace automation.model

{
    public class EntryModel
    {
        public class Rootobject
        {
            public Paging paging { get; set; }
            public object filter { get; set; }
            public Entry[] entries { get; set; }
        }

        public class Paging
        {
            public int items_per_page { get; set; }
            public int current_page { get; set; }
            public int count_pages { get; set; }
            public int count_items { get; set; }
        }

        public class Entry
        {
            public int id { get; set; }
            public int customers_id { get; set; }
            public int? projects_id { get; set; }
            public int users_id { get; set; }
            public int billable { get; set; }
            public int? texts_id { get; set; }
            public string text { get; set; }
            public string time_since { get; set; }
            public string time_until { get; set; }
            public string time_insert { get; set; }
            public string time_last_change { get; set; }
            public int type { get; set; }
            public int services_id { get; set; }
            public int duration { get; set; }
            public string time_last_change_work_time { get; set; }
            public string time_clocked_since { get; set; }
            public bool clocked { get; set; }
            public bool clocked_offline { get; set; }
            public int hourly_rate { get; set; }
            public int lumpsum_services_id { get; set; }
            public int lumpsum_services_amount { get; set; }
        }
    }
}