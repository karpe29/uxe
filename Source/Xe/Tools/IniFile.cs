using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.IO;

namespace Xe.Tools.IO
{
	public class IniFile : Hashtable, IDisposable
	{
		XeFile m_file = null;

		/// <summary>
		/// Constructeur qui charge un fichier de configuration lors de la création en appelant la fonction LoadConfig
		/// </summary>
		/// <param name="path">Chemin où se trouve le fichier</param>
		/// <param name="filename">Nom du fichier a charger</param>
		public IniFile(string filepath)
		{
			this.LoadConfig(filepath);
		}        

		/// <summary>
		/// Fonction qui charge un fichier de configuration
		/// </summary>
		/// <param name="filepath">Chemin où se trouve le fichier</param>
		public void LoadConfig(string filepath)
		{
			this.Clear();

			if (filepath == null)
			{
				this.LoadConfig("Xe.ini");
			}
			else
			{
				try
				{
					m_file = new XeFile(filepath,FileMode.Open);

					string thisString = m_file.ReadLine();
					string[] strArray;

					while (thisString != null)
					{
						strArray = thisString.Split('=');
						thisString = m_file.ReadLine();

						if (String.IsNullOrEmpty(strArray[0].Trim())) continue;
						if (strArray[0].Trim()[0] == ';') continue;
						if (strArray[0].Trim()[0] == '[') continue;

						if ((strArray[0].Trim()[0] == '/') && (strArray[0].Trim()[1] == '/')) continue;
						if (!this.Contains(strArray[0].Trim().ToUpper()))
							if (strArray.Length == 2)
								this.Add(strArray[0].Trim().ToUpper(), strArray[1].Trim());
							else
								this.Add(strArray[0].Trim(), "");
					}
				}
				catch (System.Exception ex)
				{
					throw new System.Exception("Error Loading Config File : " + filepath, ex);
				}
			}
		}

		public void SaveConfig(string filepath)
		{
			try
			{
				XeFile tmpFile = new XeFile(filepath, FileMode.Create);

				foreach (object key in base.Values)
				{
					string skey = (key as string);
					string line = skey + "=" + this[skey] + "\n\r";
					tmpFile.Write(line);
				}

				tmpFile.Close();
			}
			catch (System.Exception ex)
			{
				throw new System.Exception("Error Saving Config File: " + filepath, ex);
			}
		}

		/// <summary>
		/// Indexeur qui renvoie la valeur associé à une clef dans le fichier de configuration
		/// </summary>
		public String this[string index]
		{
			get
			{
				if (base.Contains(index.ToUpper()))
					return base[index.ToUpper()] as string;
				else
					return null;
			}
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
