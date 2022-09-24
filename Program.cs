using ParallelJobScheduler;

JobScheduler jobScheduler = new();

Job jobA = new("A");
Job jobC = new("C");
jobA.ChildrenJobs.Add(jobC);

Job jobB = new("B");
Job jobD = new("D");
jobB.ChildrenJobs.Add(jobD);

Job jobE = new("E");
jobC.ChildrenJobs.Add(jobE);
jobD.ChildrenJobs.Add(jobE);

Job jobF = new("F");
Job jobG = new("G");
Job jobH = new("H");
jobE.ChildrenJobs.AddRange(new List<Job> { jobF, jobG, jobH });

Job jobI = new("I");
jobF.ChildrenJobs.Add(jobI);
jobG.ChildrenJobs.Add(jobI);
jobH.ChildrenJobs.Add(jobI);

await jobScheduler.ScheduleAllJobs(startingJobs: new List<Job> { jobA, jobB });