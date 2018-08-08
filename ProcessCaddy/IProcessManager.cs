namespace ProcessCaddy
{
	public interface IProcessManager
	{
		bool FindProcessById( int pid );
		void RestartProcessById( int pid );
	}
}