# Clockodo API

🚩 Ressources:

- [Customers](#Customers)

  - GET /api/customers
  - GET /api/customers/[ID]

- [Projects](#Projects)

  - GET /api/projects/[ID]

- [Services](#Services)

  - GET /api/services
  - GET /api/services/[ID]

- [Entries](#Entries)

  - GET /api/entries
  - GET /api/entries/[ID]

- [Tasks](#Tasks)
  - GET /api/tasks

## Customers

---

| Parameter        |    Type    | Description                                               |
| :--------------- | :--------: | :-------------------------------------------------------- |
| id               | `integer`  | ID of the customer                                        |
| name             |  `string`  | Name of the customer                                      |
| number           |  `string`  | Customer number                                           |
| active           | `boolean`` | Is the customer active?                                   |
| billable_default | `boolean`` | Is the customer billable by default? (1 or 0)             |
| note             |  `string`  | Note for the customer                                     |
| [projects]       |  `array`   | Will not be delivered when a single customer gets queried |

---

📡 Request

```basic
   GET /api/customers
```

💡 Response

```csharp
    public class Rootobject
            {
                public Customer[] customers { get; set; }
            }

            public class Customer
            {
                public int id { get; set; }
                public string name { get; set; }
                public object number { get; set; }
                public bool active { get; set; }
                public bool billable_default { get; set; }
                public object note { get; set; }
                public object projects { get; set; }
            }
```

📡 Request

```basic
    GET /api/customers/[ID]
```

💡 Response

```csharp
    public class Rootobject
            {
                public class Customer
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public object number { get; set; }
                    public bool active { get; set; }
                    public bool billable_default { get; set; }
                    public object note { get; set; }
                }
            }
```

## Projects

---

| Parameter            |   Type    | Description                                                       |
| :------------------- | :-------: | :---------------------------------------------------------------- |
| id                   | `integer` | ID of the project                                                 |
| customers_id         | `integer` | ID of the corresponding customer                                  |
| name                 | `string`  | Name of the project                                               |
| number               | `string`  | Project number                                                    |
| active               | `boolean` | Is the project active?                                            |
| billable_default     | `boolean` | Is the project billable by default? (1 or 0)                      |
| note                 | `string`  | Note for the project                                              |
| budget_money         |  `float`  | Budget for the project                                            |
| budget_is_hours      | `boolean` | Is the budget based on hours?                                     |
| budget_is_not_strict | `boolean` | Is the budget not strict?                                         |
| completed            | `boolean` | Ist the project completed?                                        |
| billed_money         |  `float`  | Billed amount of money                                            |
| billed_completely    | `boolean` | Is the project billed completely?                                 |
| revenue_factor       |  `float`  | Factor with which revenues and hourly rates have to multiplicated |

---

### Revenue_factor

> Factor with which revenues and hourly rates have to multiplicated in order to get the effective values In case of a project which has a hard budget and has been completed with a budget usage of 400%, the factor is "0.25".
>
> > "0" if a project with hard budget hasn't been completed yet.
>
> > "1" for projects without or with soft budget.

📡 Request

```basic
    GET /api/projects/[ID]
```

💡 Response

```csharp
    public class Rootobject
            {
                public Project project { get; set; }
            }

            public class Project
            {
                public int id { get; set; }
                public int customers_id { get; set; }
                public string name { get; set; }
                public object number { get; set; }
                public bool active { get; set; }
                public bool billable_default { get; set; }
                public object note { get; set; }
                public int budget_money { get; set; }
                public bool budget_is_hours { get; set; }
                public bool budget_is_not_strict { get; set; }
                public object billed_money { get; set; }
                public bool billed_completely { get; set; }
                public bool completed { get; set; }
                public object revenue_factor { get; set; }
            }
```

## Services

---

| Parameter | Type    | Description            |
| --------- | ------- | ---------------------- |
| id        | integer | ID of the service      |
| name      | string  | Name of the service    |
| number    | string  | Service number         |
| active    | boolean | Is the service active? |
| note      | string  | Note for the service   |
|           |

---

📡 Request

```basic
  GET /api/services
```

💡 Response

```csharp
  public class Rootobject
          {
              public Service[] services { get; set; }
          }

          public class Service
          {
              public int id { get; set; }
              public string name { get; set; }
              public object number { get; set; }
              public bool active { get; set; }
              public object note { get; set; }
          }
```

📡 Request

```basic
  GET /api/services/[ID]
```

💡 Response

```csharp
  public class Rootobject
          {
              public Service service { get; set; }
          }

          public class Service
          {
              public int id { get; set; }
              public string name { get; set; }
              public object number { get; set; }
              public bool active { get; set; }
              public object note { get; set; }
          }
```

## Entries

---

| Parameter                 | Type    | Description                                                                                                                              |
| ------------------------- | ------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| id                        | integer | ID of the entry                                                                                                                          |
| customers_id              | integer | ID of the corresponding customer                                                                                                         |
| projects_id               | integer | ID of the corresponding project                                                                                                          |
| users_id                  | integer | ID of the corresponding co-worker                                                                                                        |
| services_id               | integer | ID of the corresponding service                                                                                                          |
| lumpSums_id               | integer | ID of the corresponding lump sum                                                                                                         |
| billable                  | integer | Is the entry billable? (1 or 0)                                                                                                          |
| billed                    | boolean | Is the entry billable and already billed?In order to set an entry to billed, you have to set "billable = 2" in the edit request.         |
| texts_id                  | integer | ID of the description text                                                                                                               |
| text                      | string  | description text                                                                                                                         |
| duration                  | integer | Duration of the entry in seconds                                                                                                         |
| duration_time             | string  | Duration of the entry in HH:MM:SS format                                                                                                 |
| offset                    | integer | The time correction of the entry in seconds. Is set if the duration differs from the period between start and end.                       |
| offset_time               | string  | The time correction of the entry in HH:MM:SS format. Is set if the duration differs from the period between start and end.               |
| time_since                | string  | Starting time(e.g. in format “YYYY-MM-DD HH:MM:SS” see section “Localisation”)                                                           |
| time_until                | string  | End time, NULL if entry is running(e.g. in format “YYYY-MM-DD HH:MM:SS” see section “Localisation”)                                      |
| time_insert               | string  | Insert time(e.g. in format “YYYY-MM-DD HH:MM:SS” see section “Localisation”)                                                             |
| time_last_change          | string  | Time at which the entry has been changed the last time(e.g. in format “YYYY-MM-DD HH:MM:SS” see section “Localisation”)                  |
| time_last_change_worktime | string  | Time at which worktime relevant details have been changed the last time(e.g. in format “YYYY-MM-DD HH:MM:SS” see section “Localisation”) |
| clocked                   | boolean | Entry was stopped with the clock                                                                                                         |
| is_clocking               | boolean | Entry is running                                                                                                                         |
| lumpSum                   | float   | Returns the lump sum if the entry is a lump sum entry with no amount allocated to a lump sum                                             |
| lumpSums_amount           | float   | The lump sum amount, if the entry is a entry with allocation to a lump sum                                                               |
| [hourly_rate]             | float   | Hourly rateOnly with necessary access rights and only in list function                                                                   |
| [revenue]                 | float   | Revenue of the entryOnly with necessary access rights and only in list function                                                          |
| [budget]                  | float   | Corr. projects budgetOnly with necessary access rights and only in list function                                                         |
| [budget_is_hours]         | boolean | Corr. projects budget is a time budget, not a money budgetOnly with necessary access rights and only in list function                    |
| [budget_is_not_strict]    | boolean | Corr. projects is not a strict budget, but a should-be budgetOnly with necessary access rights and only in list function                 |
| [customers_name]          | string  | Name of the corr. customersOnly in list function                                                                                         |
| [projects_name]           | string  | Name of the corr. projectOnly in list function                                                                                           |
| [services_name]           | string  | Name of the corr. serviceOnly in list function                                                                                           |
| [users_name]              | string  | Name of the corr. co-workerOnly in list function                                                                                         |

---

📡 Request

```basic
  GET /api/entries
```

💡 Response

```csharp
  public class Rootobject
          {
              public Paging paging { get; set; }
              public object filter { get; set; }
              public object[] entries { get; set; }
          }

          public class Paging
          {
              public int items_per_page { get; set; }
              public int current_page { get; set; }
              public int count_pages { get; set; }
              public int count_items { get; set; }
          }
```

---

| Flag     | parameters                      | Type             | Format                                                                                                       |
| -------- | ------------------------------- | ---------------- | ------------------------------------------------------------------------------------------------------------ |
| Required | time_since                      | string           | (YYYY-MM-DD HH:MM:SS)                                                                                        |
| Required | time_until                      | string           | (YYYY-MM-DD HH:MM:SS)                                                                                        |
|          | filter[customers_id]            | integer          |                                                                                                              |
|          | filter[projects_id]             | integer          |                                                                                                              |
|          | filter[services_id]             | integer          |                                                                                                              |
|          | filter[lumpSums_id]             | integer          |                                                                                                              |
|          | filter[billable]                | integer          | 0, 1 or 2 With the request filter[billable]=2 you only receive entries which are billable and already billed |
|          | filter[text] / filter[texts_id] | string / integer |                                                                                                              |
|          | filter[budget_type]             | string           | strict, strict-completed, strict-incomplete, soft, soft-completed, soft-incomplete, without, without-strict  |

---

### Example curl request

```bash
curl -v
  -X POST \
  -H 'X-ClockodoApiUser: [Email adress]' \
  -H 'X-ClockodoApiKey: [API key]' \
  "https://my.clockodo.com/api/entries?time_since=2017-01-01%2000:00:00&time_until=2017-02-01%2000:00:00"
```

📡 Request

```basic
  GET /api/entries/[ID]
```

💡 Response

```csharp
  public class Rootobject
          {
              public Paging paging { get; set; }
              public object filter { get; set; }
              public object[] entries { get; set; }
          }

          public class Paging
          {
              public int items_per_page { get; set; }
              public int current_page { get; set; }
              public int count_pages { get; set; }
              public int count_items { get; set; }
          }
```

## Tasks

---

| Parameter         | Type    | Description                                                                                         |
| ----------------- | ------- | --------------------------------------------------------------------------------------------------- |
| day               | string  | Day on which the task has been executed in YYYY-MM-DD format                                        |
| customers_id      | integer | ID of the corresponding customer                                                                    |
| customers_name    | string  | Name of the corresponding customer                                                                  |
| projects_id       | integer | ID of the corresponding project                                                                     |
| projects_name     | string  | Name of the corresponding project                                                                   |
| services_id       | integer | ID of the corresponding service                                                                     |
| services_name     | string  | Name of the corresponding service                                                                   |
| lumpSums_id       | integer | ID of the corresponding lump sum                                                                    |
| lumpSums_amount   | float   | Amount of the corresponding lump sum                                                                |
| lumpSums_name     | string  | Name of the corresponding lump sum                                                                  |
| lumpSums_price    | float   | Price per unit of the corresponding lump sum                                                        |
| lumpSums_unit     | string  | Unit of the corresponding lump sum                                                                  |
| users_id          | integer | ID of the corresponding co-worker                                                                   |
| billable          | integer | Is the task billable? (1 or 0)                                                                      |
| texts_id          | integer | ID of the text description                                                                          |
| text_id           | integer | ID of the text description                                                                          |
| text              | string  | Text description                                                                                    |
| time_since        | string  | Starting time(e.g. in format “YYYY-MM-DD HH:MM:SS” see section “Localisation”)                      |
| time_until        | string  | End time, NULL if entry is running(e.g. in format “YYYY-MM-DD HH:MM:SS” see section “Localisation”) |
| duration          | integer | Duration in seconds                                                                                 |
| duration_time     | string  | Duration in HH:MM:SS format                                                                         |
| duration_text     | string  | Localized duration                                                                                  |
| is_clocking       | boolean | Task is currently running                                                                           |
| has_just_lumpSums | boolean | Has the task just lump sums?                                                                        |
| [revenue]         | float   | Revenue of the task(only with necessary access rights)                                              |

---

📡 Request

```basic
  GET /api/tasks
```

💡 Response

```csharp
  public class Rootobject
        {
            public Day[] days { get; set; }
        }

        public class Day
        {
            public string date { get; set; }
            public string date_text { get; set; }
            public int duration { get; set; }
            public string duration_text { get; set; }
            public Task[] tasks { get; set; }
        }

        public class Task
        {
            public string day { get; set; }
            public int customers_id { get; set; }
            public string customers_name { get; set; }
            public int projects_id { get; set; }
            public object projects_name { get; set; }
            public int services_id { get; set; }
            public string services_name { get; set; }
            public object lumpSums_id { get; set; }
            public object lumpSums_amount { get; set; }
            public object lumpSums_name { get; set; }
            public object lumpSums_price { get; set; }
            public object lumpSums_unit { get; set; }
            public int users_id { get; set; }
            public int billable { get; set; }
            public int texts_id { get; set; }
            public int text_id { get; set; }
            public string text { get; set; }
            public string time_since { get; set; }
            public string time_until { get; set; }
            public int duration { get; set; }
            public string duration_time { get; set; }
            public string duration_text { get; set; }
            public bool is_clocking { get; set; }
            public bool has_just_lumpSums { get; set; }
            public int revenue { get; set; }
        }
```
---

| Flag     | parameters |  Type   | Description                                                          |
| :------- | :--------- | :-----: | -------------------------------------------------------------------- |
| Optional | count      | integer | Count of days for which the tasks will be listed, default 8, max. 30 |

---

📡 Request

```basic
  GET /api/tasks/duration
```

💡 Response

```csharp
  public class Rootobject
        {
            public Task task { get; set; }
        }

        public class Task
        {
            public object duration { get; set; }
        }
```

---

| Flag     | parameters                            |  Type   | Description                                                    |
| :------- | :------------------------------------ | :-----: | -------------------------------------------------------------- |
| Required | task[customers_id]                    | integer |                                                                |
| Required | task[projects_id]                     | integer |                                                                |
| Required | task[services_id] or task[lumpSum_id] | integer |                                                                |
| Required | task[text]                            | integer |                                                                |
| Required | task[billable]                        | integer |                                                                |
| Optional | excludeIds                            | integer | list of entry-Ids which should not be included in the duration |

---

