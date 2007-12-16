using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Xe.Tools.IO
{
	public class XeFile : IDisposable
	{
		private string m_fileName = "";
		private FileMode m_fileMode = FileMode.OpenOrCreate; 
		private FileAccess m_fileAccess = FileAccess.ReadWrite;
		private FileShare m_fileShare = FileShare.None; 
		
		private FileStream m_fileStream = null;			
		private StreamWriter m_streamWriter = null;
		private StreamReader m_streamReader = null;

		private bool m_autoFlush = true;

		public string FileName
		{
			get
			{
				return m_fileName;
			}
		}

		private FileStream FileStream
		{
			get
			{
				try
				{
					if (m_fileStream == null)
						m_fileStream = new FileStream(m_fileName, m_fileMode, m_fileAccess, m_fileShare);
					return m_fileStream;
				}
				catch (Exception ex)
				{
					throw ;
				}
			}
		}

		public StreamWriter StreamWriter
		{
			get 
			{
				try
				{
					if (m_streamWriter == null)
					{
						m_streamWriter = new StreamWriter(FileStream);
					}

					return m_streamWriter;
				}
				catch (Exception ex)
				{
					throw;
				}
			}
		}


		public StreamReader StreamReader
		{
			get 
			{
				try
				{
					if (m_streamReader == null)
						m_streamReader = new StreamReader(FileStream);

					return m_streamReader;
				}
				catch (Exception ex)
				{
					throw;
				}
			}
		}

		public XeFile(string fileName)
		{
			m_fileName = fileName;
		}

		public XeFile(string fileName, FileMode mode)
		{
			m_fileName = fileName;
			m_fileMode = mode;
		}

		public XeFile(string fileName, FileMode mode, FileAccess access)
		{
			m_fileName = fileName;
			m_fileMode = mode;
			m_fileAccess = access;
		}

		public XeFile(string fileName, FileMode mode, FileAccess access, FileShare share)
		{
			m_fileName = fileName;
			m_fileMode = mode;
			m_fileAccess = access;
			m_fileShare = share;
		}

		public XeFile(string fileName, FileMode mode, FileAccess access, FileShare share, bool autoFlush)
		{
			m_fileName = fileName;
			m_fileMode = mode;
			m_fileAccess = access;
			m_fileShare = share;
			m_autoFlush = autoFlush;
		}

		~XeFile()
		{
			Close();
		}

		/*
		public void Open()
		{
			throw new Exception("Unused Function XeFile.Open()");
		}
		*/

		public void Close()
		{
			if (m_fileStream != null)
			{
				m_streamWriter.Close();
				m_streamReader.Close();
				
				FileStream.Flush();
				FileStream.Close();
			}
		}

		public void Write(string data)
		{
			try
			{
				StreamWriter.Write(data);
				if (m_autoFlush)
					StreamWriter.Flush();
			}
			catch (Exception ex)
			{
				throw;
			}
		}


		public void WriteLine(string data)
		{
			try
			{
				StreamWriter.WriteLine(data);
				if (m_autoFlush)
					StreamWriter.Flush();
			}
			catch (Exception)
			{
				throw;
			}
		}


		public void Seek(SeekOrigin origin, long offset)
		{
			try
			{
				m_fileStream.Seek(offset, origin);
			}
			catch (Exception)
			{
				throw;
			}
		}


		public int Read()
		{
			try
			{
				using (m_streamReader = new StreamReader(this.m_fileStream))
				{
					return m_streamReader.Read();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public string ReadLine()
		{
			try
			{
				using (m_streamReader = new StreamReader(this.m_fileStream))
				{
					return m_streamReader.ReadLine();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (m_streamWriter != null)
			{
				m_streamWriter.Dispose();
				m_streamWriter = null;
			}

			if (m_streamReader != null)
			{
				m_streamReader.Dispose();
				m_streamReader = null;
			}

			if (m_fileStream != null)
			{
				m_fileStream.Dispose();
				m_fileStream = null;
			}
		}

		#endregion
	}
}
