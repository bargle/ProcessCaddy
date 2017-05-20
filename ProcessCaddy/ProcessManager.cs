using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace ProcessCaddy
{ 
	public delegate void OnEventFn( string evt );
	public class ProcessManager
	{
		Database m_database = new Database();
		OnEventFn m_onEvent;
		public class ProcessEntry
		{
			public Process process;
			public string name;
			public string exec;
			public string args;
		}

		public enum Status
		{
			Idle = 0,
			Running = 1
		}

		List<ProcessEntry> m_processList = new List<ProcessEntry>();

		public ProcessManager()
		{

		}

		public void AddListener( OnEventFn fn )
		{
			m_onEvent += fn;
		}

		public Status GetProcessStatus( int index )
		{
			if ( index < 0 || index >= m_processList.Count )
			{
				return Status.Idle;
			}

			if ( m_processList[index].process == null )
			{
				return Status.Idle;
			}

			if ( m_processList[index].process.HasExited )
			{
				return Status.Idle;
			}

			return Status.Running;
		}

		public bool Init()
		{
			if ( !m_database.Load( "config.json" ) )
			{
				return false;
			}

			m_processList.Clear();

			foreach( Database.Entry entry in m_database.Entries )
			{
				AddProcess( entry.name, entry.exec, entry.args );
			}

			m_onEvent?.Invoke("ConfigLoaded");

			return true;
		}

		public int Count
		{
			get { return m_processList.Count; }
		}

		public Database.Entry GetEntryAtIndex( int index )
		{
			if ( index < 0 || index >= m_database.Entries.Count )
			{
				throw new ArgumentOutOfRangeException();
			}

			return m_database.Entries[index];
		}

		public int AddProcess( string name, string exec, string args )
		{
			ProcessEntry entry = new ProcessEntry();

			entry.name = name;
			entry.exec = exec;
			entry.args = args;

			m_processList.Add( entry );

			return m_processList.Count - 1; //this is the index now assigned to the process.
		}

		void OnProcessExit(object sender, EventArgs e)
		{
			Process proc = (Process)sender;
			Console.WriteLine("Process exited: " + proc.Id);

			for( int i = 0; i < m_processList.Count; i++ )
			{
				if ( m_processList[i].process == proc )
				{
					m_processList[i].process = null;
				}
			}

			m_onEvent?.Invoke("StatusUpdated");
		}

		public bool Launch( int index )
		{
			Console.WriteLine("Launch index: " + index);

			ProcessEntry entry = m_processList[ index ];

			if ( entry.process != null )
			{
				if ( !entry.process.HasExited )
				{ 
					Console.WriteLine("Error: process already started");
					return false;
				}
			}

			ProcessStartInfo pinfo = new ProcessStartInfo( entry.exec );
			string workingDirectory = Path.GetDirectoryName( entry.exec );
			pinfo.WorkingDirectory = workingDirectory;
			pinfo.Arguments = entry.args;

			try
			{ 
				entry.process = Process.Start( pinfo );
				entry.process.EnableRaisingEvents = true;
				entry.process.Exited += OnProcessExit;

				Console.WriteLine( "Launch process: " + entry.process.Id );
				m_onEvent?.Invoke("StatusUpdated");
				return true;
			} catch ( System.Exception )
			{
				//TODO: Display error dialog
			}

			return false;
		}

		public bool Stop( int index )
		{
			ProcessEntry entry = m_processList[ index ];

			if ( entry.process == null || entry.process.HasExited )
			{
				Console.WriteLine("Error: process already exited");
				return false;
			}

			try
			{
				entry.process.Kill();
				return true;
			}
			catch( System.Exception )
			{
				//TODO: Display error dialog
			}

			return false;
		}
	}

}