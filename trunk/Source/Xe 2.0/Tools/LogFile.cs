using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Xe.Tools.IO
{
	public enum Verbosity
	{
		None = 0,		// No chat (0)
		Quiet = 1,		// Quiet chat (1)
		Normal = 2,		// Normal Output (2)
		Verbose = 3,	// Detailed Output (3)
		Debug = 4		// Debug Detailed Output (4)
	}

	public class LogFile : IDisposable
	{
		Verbosity m_logOption;
		int m_maxSize;

		XeFile m_file;

		public LogFile(string filepath, Verbosity options, int maxSize)
		{
			m_logOption = options;
			m_maxSize = maxSize;
			m_file = new XeFile(filepath, FileMode.Append, FileAccess.Write, FileShare.Read, true);
		}

		public void CheckMaxSize()
		{
			try
			{
				FileInfo thisFileInfo = new FileInfo(m_file.FileName);
				if (thisFileInfo.Exists && thisFileInfo.Length >= m_maxSize)
					thisFileInfo.Delete();
			}
			catch (System.Exception e)
			{
				throw (e);
			}
		}

		public void WriteException(System.Exception e)
		{
			this.CheckMaxSize();

			if (e.InnerException != null)
			{
				if (this.m_logOption >= Verbosity.Verbose) WriteException(e.InnerException);
				m_file.WriteLine("<- <- <- <- <- <- <- <- <- <- <- <- <- <- <- <- <- <- <-");
			}

			m_file.WriteLine("********************************************************");
			if (this.m_logOption >= Verbosity.None)
			{
				this.WriteTime();
				m_file.WriteLine("Error in " + e.TargetSite);
			}
			if (this.m_logOption >= Verbosity.Quiet)
			{
				this.WriteTime();
				m_file.WriteLine("Messsage =" + e.Message);
			}

			if (this.m_logOption >= Verbosity.Normal)
			{
				this.WriteTime();
				m_file.WriteLine("CallStack :");
				m_file.WriteLine(e.StackTrace);
			}

			this.m_file.Close();
		}

		private void WriteTime()
		{
			m_file.Write("[" + DateTime.Now.ToString("G") + "]");
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (m_file != null)
			{
				m_file.Dispose();
				m_file = null;
			}
		}

		#endregion
	}
}
