using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProcessCaddy
{
	public class HeartbeatMonitor
	{
		IProcessManager m_processManager;
		UdpClient m_Peer;
		List<int> m_pidsToValidate = new List<int>();
		static object s_lockObject = new object();
		Dictionary<int, MonitoredProcess> m_processes = new Dictionary<int, MonitoredProcess>();

		struct MonitoredProcess
		{
			public MonitoredProcess( int pid )
			{
				m_pid = pid;
				m_startTime = DateTime.Now;
			}

			public int m_pid;
			public DateTime m_startTime;

		}

		public HeartbeatMonitor(IProcessManager mgr)
		{
			m_processManager = mgr;

			//start server
			CreateSocket(25002);
		}

		public void Update()
		{
			//handle any incoming commands...
			lock (s_lockObject)
			{
				foreach (int pid in m_pidsToValidate)
				{
					//
					if (m_processManager.FindProcessById(pid))
					{
						MonitoredProcess proc;
						if ( m_processes.TryGetValue( pid, out proc ) )
						{
							m_processes[pid] = new MonitoredProcess(pid);
						} else
						{
							m_processes.Add( pid, new MonitoredProcess(pid));
						}
					}
				}
				m_pidsToValidate.Clear();

				//check for missing pids
				
				foreach( var pair in m_processes )
				{
					if ( !m_processManager.FindProcessById( pair.Key ) )
					{
						m_pidsToValidate.Add(pair.Key);
					}
				}

				foreach( int pid in m_pidsToValidate )
				{
					m_processes.Remove(pid);
				}
				m_pidsToValidate.Clear();

				//check for pid timeouts
				foreach (var pair in m_processes)
				{
					int secondsSinceLastHeartbeat = (DateTime.Now - pair.Value.m_startTime).Seconds;
					if ( secondsSinceLastHeartbeat >= 30 )
					{
						Console.WriteLine("Restart " + pair.Key );
						m_processManager.RestartProcessById(pair.Key);
					}
				}
			}
		}

		private bool CreateSocket(int port)
		{
			//int orgPort = port;
			m_Peer = new UdpClient();
			m_Peer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

			try
			{
				IPEndPoint localpt = new IPEndPoint(IPAddress.Any, port);
				m_Peer.Client.Bind(localpt);
				m_Peer.EnableBroadcast = false;
				m_Peer.BeginReceive(new AsyncCallback(recv), null);
				return true;
			}
			catch (System.Exception e )
			{
				Console.WriteLine(  e.ToString() );
			}

			return false;
		}

		void recv(IAsyncResult res)
		{
			try
			{
				IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
				byte[] bytes = m_Peer.EndReceive(res, ref remote);

				string msg = Decode(bytes);
				if (int.TryParse(msg, out int pid))
				{
					lock (s_lockObject)
					{
						m_pidsToValidate.Add(pid);
					}
				}

			}
			catch (System.Exception e)
			{
				Console.WriteLine(e.ToString());
			}

			// Wait for the next packet
			m_Peer.BeginReceive(recv, null);
		}

		protected virtual string Decode(byte[] bytes)
		{
			return ASCIIEncoding.ASCII.GetString(bytes);
		}

		protected virtual byte[] Encode(string msg)
		{
			return Encoding.ASCII.GetBytes(msg);
		}

		// Serialize to bytes (BinaryFormatter)
		public static byte[] SerializeToBytes<T>(T source)
		{
			using (var stream = new MemoryStream())
			{
				try
				{
					var formatter = new BinaryFormatter();
					formatter.Serialize(stream, source);
					return stream.ToArray();
				}
				catch (System.Exception) { }

				return null;
			}
		}

		// Deerialize from bytes (BinaryFormatter)
		public static T DeserializeFromBytes<T>(byte[] source) where T : Packet
		{
			using (var stream = new MemoryStream(source))
			{
				try
				{
					var formatter = new BinaryFormatter();
					stream.Seek(0, SeekOrigin.Begin);
					return (T)formatter.Deserialize(stream);
				}
				catch (System.Exception e )
				{
					Console.WriteLine( e.ToString() );
				}
				return null;
			}
		}
	}
}