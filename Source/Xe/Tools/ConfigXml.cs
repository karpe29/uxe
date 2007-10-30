using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Xe.Tools.IO
{
	public static class ConfigXml
	{
		#region Load/Save code
		/// <summary>
		/// Saves the current public members of data
		/// </summary>
		/// <param name="filename">The filename to save to</param>
		/// <param name="data">The data to save</param>
		public static void Save(string fileName, object data)
		{
			Stream stream = File.Create(fileName);

			XmlSerializer serializer = new XmlSerializer(data.GetType());
			serializer.Serialize(stream, data);
			stream.Close();
		}

		/// <summary>
		/// Loads settings from a file
		/// </summary>
		/// <param name="filename">The filename to load</param>
		public static T Load<T>(string fileName)
		{
			Stream stream = File.OpenRead(fileName);
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(stream);
		}
		#endregion
	}
}
