using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace System.Threading.Tasks
{
    static class YieldAwaitableExtensions
    {
        public static YieldAwaitableThreadPool ConfigureAwait(this YieldAwaitable _, bool LockContext)
        {
            return new YieldAwaitableThreadPool();
        }
    }

    public readonly ref struct YieldAwaitableThreadPool
    {
        private readonly bool _LockContext;

        public YieldAwaitableThreadPool(in bool LockContext) => _LockContext = LockContext;

        public Awaiter GetAwaiter() => new Awaiter(_LockContext);

        public readonly struct Awaiter : ICriticalNotifyCompletion, IEquatable<Awaiter>
        {
            private readonly bool _LockContext;
            private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
            private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;


            public Awaiter(in bool LockContext) => _LockContext = LockContext;

            public bool IsCompleted => false;

            public void GetResult() { }

            private static void RunAction(object? State) => ((Action)State!).Invoke();

            public void OnCompleted(Action Continuation) => QueueContinuation(Continuation, true, _LockContext);


            public void UnsafeOnCompleted(Action Continuation) => QueueContinuation(Continuation, false, _LockContext);

            private static void QueueContinuation(Action Continuation, bool FlowContext, bool LockContext)
            {
                if(Continuation is null) throw new ArgumentNullException(nameof(Continuation));

                var context = SynchronizationContext.Current;
                if (LockContext && context != null && context.GetType() != typeof(SynchronizationContext))
                    context.Post(__SendOrPostCallbackRunAction, Continuation);
                else
                {
                    var scheduler = TaskScheduler.Current;
                    if (!LockContext || scheduler == TaskScheduler.Default)
                    {
                        if (FlowContext)
                            ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                        else
                            ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                    else
                        Task.Factory.StartNew(Continuation, default, TaskCreationOptions.PreferFairness, scheduler);
                }
            }

            public override bool Equals(object? obj) => obj is Awaiter awaiter && awaiter._LockContext == _LockContext;

            public override int GetHashCode() => _LockContext.GetHashCode();

            public static bool operator ==(Awaiter left, Awaiter right) => left.Equals(right);

            public static bool operator !=(Awaiter left, Awaiter right) => !(left == right);

            public bool Equals(Awaiter other) => other._LockContext == _LockContext;

        }
    }

    //internal struct YieldAsyncAwaitable
    //{
    //    //private readonly bool _LockContext;        

    //    //public YieldAsyncAwaitable(bool LockContext) => _LockContext = LockContext;

    //    public YieldAsyncAwaiter GetAwaiter() => new YieldAsyncAwaiter();

    //    [StructLayout(LayoutKind.Sequential, Size = 1)]

    //    public struct YieldAsyncAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    //    {
    //        [NotNull] private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
    //        [NotNull] private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;
                        

    //        public bool IsCompleted => false;

    //        private static void RunAction([NotNull] object action) => ((Action)action)();

    //        [SecurityCritical]

    //        private static void QueueContinuation([NotNull] Action continuation, bool FlowContext)
    //        {
    //            if (continuation is null) throw new ArgumentNullException(nameof(continuation));

    //            if (FlowContext)
    //                ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, continuation);
    //            else
    //                ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, continuation);
    //        }

    //        [SecuritySafeCritical]
    //        public void OnCompleted([NotNull] Action continuation) => QueueContinuation(continuation, true);

    //        //[SecuritySafeCritical]
    //        //public YieldAsyncAwaiter(in bool LockContext) { _LockContext = LockContext; }
                           
    //         [SecuritySafeCritical]
    //        public void UnsafeOnCompleted([NotNull] Action continuation) => QueueContinuation(continuation, false);

    //        public void GetResult() { }
    //    }
    //}
}
