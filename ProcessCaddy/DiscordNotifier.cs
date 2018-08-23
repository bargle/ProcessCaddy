using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace ProcessCaddy
{
	public class DiscordNotifier : INotificationReceiver
	{
		[Serializable]
		public struct Configuration
		{
			public bool enabled;
			public string URI;
		}

		Configuration m_config;

		IProcessManager m_manager;
		public DiscordNotifier( IProcessManager manager )
		{
			m_manager = manager;
			LoadConfig();

			if ( m_config.enabled )
			{
				m_manager.RegisterForNotifications(this);
			}
		}

		bool LoadConfig()
		{
			if ( !File.Exists( "discord.json" ) )
			{
				Console.WriteLine( "discord.json not found..." );
				m_config = new Configuration();
				return false;
			}

			try
			{
				using (StreamReader reader = new StreamReader("discord.json"))
				{
					if (reader == null)
					{
						Console.WriteLine("Failed to load discord.json");
						return false;
					}

					string textJson = reader.ReadToEnd();
					m_config = JsonConvert.DeserializeObject<Configuration>(textJson);

					return true;
				}
			}
			catch( Exception e )
			{
				Console.WriteLine( e.ToString() );
			}

			return false;
		}

		public void OnProcessStarted(ProcessManager.ProcessEntry entry)
		{
			SendMessage( "Starting " + entry.name );
		}
		public void OnProcessStopped(ProcessManager.ProcessEntry entry)
		{
			SendMessage("Stopping " + entry.name);
		}
		public void OnProcessExited(ProcessManager.ProcessEntry entry)
		{
			SendMessage("Process " + entry.name + " exited." );
		}
		public void OnProcessRestarted(ProcessManager.ProcessEntry entry)
		{
		}

		void SendMessage( string msg )
		{
			string message = "{ \"content\":\"" + msg + "\" }";

			HttpWebRequest req = null;
			HttpWebResponse res = null;

			try
			{
				string webhookURL = m_config.URI;
				req = (HttpWebRequest)WebRequest.Create(webhookURL);
				req.Method = "POST";
				req.ContentType = "application/json";

				req.ContentLength = message.Length;
				var sw = new StreamWriter(req.GetRequestStream());
				sw.Write(message);
				sw.Close();

				res = (HttpWebResponse)req.GetResponse();
				Stream responseStream = res.GetResponseStream();
				var streamReader = new StreamReader(responseStream);
				streamReader.ReadToEnd();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}
}