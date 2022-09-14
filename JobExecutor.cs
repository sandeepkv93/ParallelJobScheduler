namespace ParallelJobScheduler
{
    internal class JobExecutor
    {
        public async Task SubmitJob(Job job)
        {
            Console.WriteLine($"Started the job: {job.Name}");

            double randomDelayInSeconds = new Random().Next(1, 5);
            await Task.Delay(TimeSpan.FromSeconds(randomDelayInSeconds));

            job.IsCompleted = true;
            Console.WriteLine($"Completed the job: {job.Name}");
        }
    }
}
