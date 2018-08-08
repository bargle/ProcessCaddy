namespace ProcessCaddy
{
	[System.Serializable]
	public class Packet
	{
		private const uint VERSION = 1;
		private const uint SPECIAL = 0xCAFECAFE;

		public Packet(byte[] _bytes)
		{
			m_version = VERSION;
			m_special = SPECIAL;
			m_bytes = _bytes;
		}

		public bool IsValid
		{
			get
			{
				return (m_version == VERSION) && (m_special == SPECIAL);
			}
		}

		private uint m_version;
		private uint m_special;

		//FIXME: this should be private
		public byte[] m_bytes;
	}
}