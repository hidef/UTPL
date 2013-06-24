using System;
using UTask;
using System.Threading;

namespace UTPL.Demo
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Task<DateTime> t = Task<DateTime>.StartNew(worker);
			for ( int i = 0; i < 10; i++ )
			{
				Thread.Sleep(500);
				Console.WriteLine("{0} - Doing sync work: {1}", Thread.CurrentThread.ManagedThreadId, i);
			}
			Console.WriteLine("{0} - Synchronous work done, waiting for async task", Thread.CurrentThread.ManagedThreadId);
			t.Wait();
			Console.WriteLine("{0} - Waiting for async work complete. Result: {1}", Thread.CurrentThread.ManagedThreadId, t.Result);
			Console.ReadLine();
		}

		private static DateTime worker()
		{
			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(1000);
				Console.WriteLine("{0} - Doing async work: {1}", Thread.CurrentThread.ManagedThreadId, i);
			}

			return DateTime.Now;
		}
	}
}
