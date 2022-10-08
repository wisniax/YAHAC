using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YAHAC.Core
{
	//The stuff that happens here well dont ask me...
	public class BackgroundTask
	{
		public delegate void TaskToPerform();
		private Task TimerTask;
		private readonly PeriodicTimer timer;
		private readonly CancellationTokenSource _cts = new();

		public BackgroundTask(TimeSpan interval)
		{
			timer = new PeriodicTimer(interval);
		}

		private async Task DoWorkAsync(TaskToPerform del)
		{
			try
			{
				while (await timer.WaitForNextTickAsync(_cts.Token))
				{
					await Task.Run(() => del());
				}
			}
			catch (OperationCanceledException)
			{

			}
		}
		public void Start(TaskToPerform del)
		{
			TimerTask = DoWorkAsync(del);
		}

		public async Task StopAsync()
		{
			if (TimerTask is null) { return; }
			_cts.Cancel();
			await TimerTask;
			_cts.Dispose();
		}
	}
}
