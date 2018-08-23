namespace ProcessCaddy
{
	public interface INotificationReceiver
	{
		void OnProcessStarted(ProcessManager.ProcessEntry entry);
		void OnProcessStopped(ProcessManager.ProcessEntry entry);
		void OnProcessExited(ProcessManager.ProcessEntry entry);
		void OnProcessRestarted(ProcessManager.ProcessEntry entry);
	}
}