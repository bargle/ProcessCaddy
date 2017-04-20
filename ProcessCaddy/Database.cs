using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProcessCaddy
{
	public class Database
	{
		public struct Entry
		{
			public string name;
			public string exec;
			public string args;
		}

		List<Entry> m_entries = new List<Entry>();

		public List<Entry> Entries
		{
			get
			{
				return m_entries;
			}
		}

		public bool Load( string path )
		{
			using (StreamReader reader = new StreamReader( path ))
			{
				if ( reader == null )
				{
					Console.WriteLine( "Failed to load " + path );
					return false;
				}

				string textJson = reader.ReadToEnd();

				List<Entry> entries = JsonConvert.DeserializeObject< List<Entry> >( textJson );
				m_entries = entries;

				return true;
			}
		}

		public bool Save( string path )
		{
			using ( StreamWriter writer = new StreamWriter( path, false ) )
			{
				if ( writer == null )
				{
					Console.WriteLine( "Failed to save " + path );
					return false;
				}

				string json = JsonConvert.SerializeObject( m_entries );
				writer.Write( json );
				return true;
			}
		}
	}
}