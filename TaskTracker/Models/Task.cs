using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTracker.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string TaskText { get; set; }
        public DateTime TillDate { get; set; }
        public TaskState State { get; set; }
    }
}