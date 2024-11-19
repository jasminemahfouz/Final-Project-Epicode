using System;
using System.Collections.Generic;

namespace WomenActivity.Models
{
    public class HomePageViewModel
    {
        public DateTime CurrentDate { get; set; }
        public IEnumerable<TaskItem> TaskItems { get; set; }
        public bool ShowCalendar { get; set; }
        public CalendarViewModel Calendar { get; set; }
        public Dictionary<DateTime, int> TasksPerDay { get; set; } // Aggiunta la proprietà per il conteggio delle task
    }
}
