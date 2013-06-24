using System;
using System.Threading;

namespace UTask
{
    public class Task
    {
		
        private bool _exceptionThrown = false;
		
        private Exception _exception;
        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }
		
        private TaskState _state = TaskState.NotStarted;
        public TaskState State
        {
            get
            {
                return _state;
            }
        }
		
        private Action _action;
	
		private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);

		public Task<T1> ContinueWith<T1>(Func<Task, T1> continuation)
		{
			return Task<T1>.StartNew(() => {
				this.Wait();
				return continuation(this);
			});
		}

		public Task ContinueWith(Action<Task> continuation)
		{
			return Task.StartNew(() => {
				this.Wait();
				continuation(this);
			});
		}

        public void Wait()
        {
            WaitHandle.WaitAny(new WaitHandle[]{_waitHandle});
            if ( _exception != null && !_exceptionThrown)
            {
                _exceptionThrown = true;
                throw _exception;
            }
        }
		
        public Task(Action action)
        {
            //_waitHandle = new AutoResetEvent(false);
            _action = action;
        }
			
        public static Task StartNew(Action action)
        {
            Task t = new Task(action);
            t.Run();
            return t;
        }
		
        public void Run()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(_processAction), null);
        }
		
        private void _processAction(object actionObject)
        {
            _state = TaskState.Running;
            try
            {
                _action();
                _state = TaskState.Completed;
            }
            catch ( Exception ex)
            {
                _exception = ex;
                _state = TaskState.Faulted;
            }
            finally
            {
                _waitHandle.Set();
            }
        }
    }

    public class Task<T>
    {
        ~Task()
        {	
            if ( _exception != null && !_exceptionThrown)
            {
                _exceptionThrown = true;
                throw _exception;
            }
        }
		
        private T _result;
        public T Result
        {
            get
            {
                this.Wait ();
                return _result;
            }
        }
		
        private bool _exceptionThrown = false;
        private Exception _exception;
        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }
		
        private TaskState _state = TaskState.NotStarted;
        public TaskState State
        {
            get
            {
                return _state;
            }
        }
		
        private Func<T> _action;
	
		private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);

		public Task<T1> ContinueWith<T1>(Func<Task<T>, T1> continuation)
		{
			return Task<T1>.StartNew(() => {
				this.Wait();
				return continuation(this);
			});
		}

		public Task ContinueWith(Action<Task<T>> continuation)
		{
			return Task.StartNew(() => {
				this.Wait();
				continuation(this);
			});
		}

        public void Wait()
        {
			if (this.State == TaskState.Completed)
			{
				return;
			}
			else if (this.State == TaskState.Faulted)
			{
				if ( _exception != null && !_exceptionThrown)
				{
					_exceptionThrown = true;
					throw _exception;
				}
			}

            WaitHandle.WaitAny(new WaitHandle[]{_waitHandle});
            if ( _exception != null && !_exceptionThrown)
            {
                _exceptionThrown = true;
                throw _exception;
            }
        }
		
        public Task(Func<T> action)
        {
            //_waitHandle = new AutoResetEvent(false);
            _action = action;
        }
			
        public static Task<T> StartNew(Func<T> action)
        {
            Task<T> t = new Task<T>(action);
            t.Run();
            return t;
        }
		
        public void Run()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(_processAction), null);
        }
		
        private void _processAction(object actionObject)
        {
            _state = TaskState.Running;
            try
            {
                _result = _action();
                _state = TaskState.Completed;
            }
            catch ( Exception ex)
            {
                _exception = ex;
                _state = TaskState.Faulted;
            }
            finally
            {
                _waitHandle.Set();
            }
        }
    }
}