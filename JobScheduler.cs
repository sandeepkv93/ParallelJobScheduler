namespace ParallelJobScheduler
{
    internal class JobScheduler
    {
        JobExecutor jobExecutor;
        Dictionary<Job, List<Job>> jobToParentsMap;
        HashSet<Job> processedJobs;

        public JobScheduler()
        {
            jobExecutor = new();
            jobToParentsMap = new();
            processedJobs = new();
        }

        public async Task ScheduleAllJobs(IList<Job> startingJobs)
        {
            HashSet<Job> jobsWithoutChildren = PreProcessJobs(startingJobs);

            List<Task> jobTasks = new();
            foreach (Job job in jobsWithoutChildren)
            {
                jobTasks.Add(ProcessJob(job));
            }

            await Task.WhenAll(jobTasks);
        }

        private HashSet<Job> PreProcessJobs(IList<Job> startingJobs)
        {
            Dictionary<Job, List<Job>> jobToChildrenMap = new();
            HashSet<Job> allJobs = new();
            HashSet<Job> jobsWithoutChildren = new();
            Queue<Job> queue = new();

            foreach (Job job in startingJobs)
            {
                queue.Enqueue(job);
            }

            while (queue.Count > 0)
            {
                Job currentJob = queue.Dequeue();
                allJobs.Add(currentJob);
                List<Job> childrenJobs = currentJob.ChildrenJobs;

                if (childrenJobs.Count == 0)
                {
                    jobsWithoutChildren.Add(currentJob);
                }

                if (!jobToChildrenMap.ContainsKey(currentJob))
                {
                    jobToChildrenMap.Add(currentJob, childrenJobs);
                }

                foreach (Job childJob in childrenJobs)
                {
                    queue.Enqueue(childJob);
                }
            }

            foreach (KeyValuePair<Job, List<Job>> jobToChildrenMapEntry in jobToChildrenMap)
            {
                Job parentJob = jobToChildrenMapEntry.Key;
                List<Job> childrenJobs = jobToChildrenMapEntry.Value;

                foreach (Job childJob in childrenJobs)
                {
                    if (!jobToParentsMap.ContainsKey(childJob))
                    {
                        jobToParentsMap.Add(childJob, new());
                    }

                    jobToParentsMap[childJob].Add(parentJob);
                }
            }

            IEnumerable<Job> jobsWithoutParents = allJobs.Except(jobToParentsMap.Keys);
            foreach (Job jobWithoutParents in jobsWithoutParents)
            {
                jobToParentsMap.Add(jobWithoutParents, new());
            }

            return jobsWithoutChildren;
        }

        private async Task ProcessJob(Job job)
        {
            Console.WriteLine($"Finding parents of job: {job.Name}");
            IList<Job> parentsOfCurrentJob = jobToParentsMap[job];
            IList<Task> parentJobTasks = new List<Task>();
            foreach (var parentJob in parentsOfCurrentJob)
            {
                parentJobTasks.Add(ProcessJob(parentJob));
            }

            // Wait for all the parents to complete
            await Task.WhenAll(parentJobTasks);

            if (processedJobs.Add(job))
            {
                // Wait for the current job to complete
                await jobExecutor.SubmitJob(job);
            }
        }
    }
}
