using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public class TaskPool
    {
        private readonly HashSet<IInternalTask> workingTasks = new HashSet<IInternalTask>();
        private readonly ConcurrentQueue<IInternalTask> queue = new ConcurrentQueue<IInternalTask>();
        private readonly object tasksMutex = new object();
        private readonly object checkMutex = new object();
        private int threadsMaxCount;

        private interface IInternalTask
        {
            Task Execute();
            string Id { get; set; }
        }

        private class InternalTaskHolder : IInternalTask
        {
            public Func<Task> Task { get; set; }
            public TaskCompletionSource<IDisposable> Waiter { get; set; }
            public string Id { get; set; }

            public async Task Execute()
            {
                await Task();
                Waiter.SetResult(null);
            }
        }

        private class InternalTaskHolderGeneric<T> : IInternalTask
        {
            public Func<Task<T>> Task { get; set; }
            public TaskCompletionSource<T> Waiter { get; set; }
            public string Id { get; set; }

            public async Task Execute()
            {
                var result = await Task();
                Waiter.SetResult(result);
            }
        }


        public event EventHandler Completed;

        public TaskPool(int threadsMaxCount)
        {
            this.threadsMaxCount = threadsMaxCount;
        }

        public TaskPool(int threadsMaxCount, Func<Task>[] tasks) : this(threadsMaxCount)
        {
            foreach (var task in tasks)
            {
                queue.Enqueue(new InternalTaskHolder { Task = task });
            }
        }


        public Task Enqueue(Func<Task> task, string id = null)
        {
            lock (tasksMutex)
            {
                var holder = new InternalTaskHolder { Task = task, Waiter = new TaskCompletionSource<IDisposable>(), Id = id };
                if (workingTasks.Count >= threadsMaxCount)
                    queue.Enqueue(holder);
                else
                    StartTask(holder);

                return holder.Waiter.Task;
            }
        }

        public Task<T> Enqueue<T>(Func<Task<T>> task, string id = null)
        {
            lock (tasksMutex)
            {
                var holder = new InternalTaskHolderGeneric<T> { Task = task, Waiter = new TaskCompletionSource<T>(), Id = id };

                if (workingTasks.Count >= threadsMaxCount)
                    queue.Enqueue(holder);
                else
                    StartTask(holder);

                return holder.Waiter.Task;
            }
        }

        public bool IsTaskExists(string id) => queue.Any(q => q.Id == id);

        private async void StartTask(IInternalTask task)
        {
            await task.Execute();
            TaskCompleted(task);
        }

        private void TaskCompleted(IInternalTask task)
        {
            lock (tasksMutex)
            {
                workingTasks.Remove(task);
                CheckQueue();

                if (queue.Count == 0 && workingTasks.Count == 0)
                    OnCompleted();
            }
        }

        private void CheckQueue()
        {
            lock (checkMutex)
                while (queue.Count > 0 && workingTasks.Count < threadsMaxCount)
                    if (queue.TryDequeue(out IInternalTask task))
                        StartTask(task);
        }

        /// <summary>
        /// Raises the Completed event.
        /// </summary>
        protected void OnCompleted()
        {
            Completed?.Invoke(this, null);
        }
    }
}
