using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelJobScheduler
{
    internal class Job
    {
        public string Name { get; set; }
        public bool IsCompleted { get; set; }

        public List<Job> ChildrenJobs { get; set; }
        
        public Job(string name)
        {
            Name = name;
            ChildrenJobs = new();
        }
    }
}
