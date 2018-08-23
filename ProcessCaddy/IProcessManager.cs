namespace ProcessCaddy
{
	public interface IProcessManager
	{
		bool FindProcessById( int pid );
		void RestartProcessById( int pid );
		void RegisterForNotifications( INotificationReceiver receiver );
		void UnregisterFromNotifications( INotificationReceiver receiver );
	}
}