using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Collections.Generic;

namespace ProcessCaddy
{
    public partial class Form1 : Form
    {
		ProcessManager m_processManager = new ProcessManager();
		Database m_database = new Database();
	//	static ListViewItem s_selectedItem;

        public Form1()
        {
            InitializeComponent();
        }

		private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			var state = e.State == ListViewItemStates.Selected ? VisualStyleElement.Header.Item.Hot : VisualStyleElement.Header.Item.Normal;
			var itemRenderer = new VisualStyleRenderer(state);
			var r = e.Bounds; 
			r.X += 1;
			itemRenderer.DrawBackground(e.Graphics, r);
			r.Inflate(-2, 0);
			var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;
			itemRenderer.DrawText(e.Graphics, r, e.Header.Text, false, flags);
		}

		private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void listView1_Resize(object sender, EventArgs e)
		{
			SizeLastColumn((ListView) sender);
		}

		private void SizeLastColumn(ListView lv)
		{
			if ( lv.Columns.Count < 2 )
			{
				return;
			}

			 int x = ( ( lv.Width / 3 ) == 0 ) ? 1 : ( lv.Width / 3 );
			 lv.Columns[1].Width = ( x * 3 );
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			listView1.Columns.Add( "Name", 300 );
			listView1.Columns.Add( "Status", 200 );

			m_processManager.AddListener( OnEvent );
			m_processManager.Init();

		}

		delegate void SetTextCallback( int index, ProcessManager.Status status );

		private void SetSubItemText( int index, ProcessManager.Status status )  
		{  
			if (this.listView1.InvokeRequired)  
			{     
				SetTextCallback d = new SetTextCallback( SetSubItemText );  
				this.Invoke( d, new object[] { index, status } ); 
			}  
			else  
			{  
				listView1.Items[ index ].SubItems[ 1 ] = new ListViewItem.ListViewSubItem( listView1.Items[ index ], status.ToString()) ;
			}  
		} 
		private void OnEvent( string evt )
		{
			if ( evt.CompareTo( "ConfigLoaded" ) == 0 )
			{
				for ( int i = 0; i < m_processManager.Count; i++ )
				{
					Database.Entry entry = m_processManager.GetEntryAtIndex(i);
					ListViewItem listitem = new ListViewItem( entry.name );
					listitem.SubItems.Add( "Idle" );
					listView1.Items.Add( listitem );
				}

				SizeLastColumn(listView1);
			}

			if ( evt.CompareTo( "StatusUpdated" ) == 0 )
			{
				Console.WriteLine("Form1: StatusUpdated");

				for ( int i = 0; i < m_processManager.Count; i++ )
				{
					ProcessManager.Status status = m_processManager.GetProcessStatus( i );
					SetSubItemText( i, status );
				}
			}
		}

		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
			{	
				ListViewItem selection = listView1.GetItemAt( e.X, e.Y );
				if( selection != null )
				{
					if ( listView1.SelectedItems.Count <= 0 )
					{
						return;
					}

					MenuItem[] mi = new MenuItem[] {
						new MenuItem( selection.Text ), //+ " - " + selection.Index ),
						new MenuItem( "-" ),
						new MenuItem( "Start", OnStartClicked ),
						new MenuItem( "Stop", OnStopClicked )
						};
					listView1.ContextMenu = new ContextMenu( mi );
				}
			}
		}

		private void OnStartClicked( object sender, EventArgs e )
		{
			if ( listView1.SelectedItems.Count > 0 )
			{
				m_processManager.Launch( listView1.SelectedItems[0].Index );

				listView1.SelectedItems.Clear();
			}
		}

		private void OnStopClicked( object sender, EventArgs e )
		{
			if ( listView1.SelectedItems.Count > 0 )
			{
				m_processManager.Stop( listView1.SelectedItems[0].Index );
				listView1.SelectedItems.Clear();
			}
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Environment.ExitCode = 0;
			Application.Exit();
		}
	}
}
