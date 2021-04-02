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
            public int users_id { get; set; }
            public int projects_id { get; set; }
            public int customers_id { get; set; }
            public int? services_id { get; set; }
            public int hourly_rate { get; set; }
            public int billable { get; set; }
            public string time_insert { get; set; }
            public string time_since { get; set; }
            public string time_until { get; set; }
            public int offset { get; set; }
            public int duration { get; set; }
            public bool clocked { get; set; }
            public object lumpSum { get; set; }
            public string time_last_change { get; set; }
            public string time_last_change_work_time { get; set; }
            public object lumpSums_id { get; set; }
            public string time_clocked_since { get; set; }
            public bool offline { get; set; }
            public float revenue { get; set; }
            public bool budget_is_hours { get; set; }
            public bool budget_is_not_strict { get; set; }
            public string customers_name { get; set; }
            public object projects_name { get; set; }
            public string services_name { get; set; }
            public string users_name { get; set; }
            public object lumpSums_price { get; set; }
            public object lumpSums_unit { get; set; }
            public object lumpSums_name { get; set; }
            public object lumpSums_amount { get; set; }
            public bool billed { get; set; }
            public int? texts_id { get; set; }
            public string text { get; set; }
            public string duration_time { get; set; }
            public string offset_time { get; set; }
            public bool is_clocking { get; set; }
            public int budget { get; set; }
        }
    }
}
