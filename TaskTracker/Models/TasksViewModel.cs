using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTracker.Models
{
    public class TasksViewModel
    {
        public List<Task> Tasks { get; set; }
        public CheckList CheckList { get; set; }
    }
}