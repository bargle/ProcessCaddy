using System;
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

		List<ProcessEntry> m_processList = new List<ProcessEntry>();

		public ProcessManager()
		{

		}

		public void AddListener( OnEventFn fn )
		{
			m_onEvent += fn;
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

		public bool Launch( int index )
		{
			
			return false;
		}

		public bool Stop( int index )
		{
			//
			return false;
		}
	}

}