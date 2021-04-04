using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if PARALLEL

using System.Threading.Tasks;
using System.Threading;

namespace Obtics
{
    internal static class Tasks
    {
        //Number of tasks waiting to run
        static int _TasksInQueue ;

        //Number of tasks running
        static int _RunningTasks;

        //Obtics operation is quite 'Amorph' meaning there is no clear calling hierarchy. Invocations can come from many sources. From source to
        //target (change events) or from targets to sources (value requests). This means that it is hard to determine a good spot for
        //parallelization. We don't want to just parallelize everywhere since that will become quite expensive and is ineffective anyway.
        //Limmit the number of scheduled tasks to roughly 4 * the number of running tasks.

        public static bool SuggestParallelization
        { get { return _TasksInQueue <= 4 + _RunningTasks * 2 ; } }

        public static Task CreateTask<TPrm>(Action<TPrm> action, TPrm arg)
        {
            Interlocked.Increment(ref _TasksInQueue);
            var task = new Task(
                () => 
                {
                    Interlocked.Decrement(ref _TasksInQueue);
                    Interlocked.Increment(ref _RunningTasks);

                    try { action(arg); }
                    finally { Interlocked.Decrement(ref _RunningTasks); }
                }, 
                TaskCreationOptions.AttachedToParent
            );
            task.Start(TaskScheduler.Current);
            return task;
        }


        public class Future<T>
        {
            internal bool _Cancel;
            internal Task<T> _Task;
            internal Func<T> _Func;
        }

        public static Future<T> CreateFuture<T>(Func<T> func)
        {
            Interlocked.Increment(ref _TasksInQueue);

            var future = new Future<T> { _Func = func };

            var task = 
                new Task<T>(
                    () =>
                    {
                        Interlocked.Decrement(ref _TasksInQueue);

                        if (future._Cancel)
                            return default(T);

                        Interlocked.Increment(ref _RunningTasks);

                        try { return future._Func(); }
                        finally { Interlocked.Decrement(ref _RunningTasks); }
                    },
                    TaskCreationOptions.AttachedToParent
                )
            ;
            future._Task = task;
            task.Start(TaskScheduler.Current);
            return future;
        }

        public static T GetResult<T>(Future<T> future)
        {
            switch (future._Task.Status)
            {
                case TaskStatus.WaitingForActivation:
                case TaskStatus.WaitingToRun:
                    future._Cancel = true;
                    return future._Func();

                default:
                    return future._Task.Result;
            }
        }
    }
}

#endif
