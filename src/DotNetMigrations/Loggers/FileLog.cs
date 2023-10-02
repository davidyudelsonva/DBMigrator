using System;
using System.Linq;
using DotNetMigrations.Core;
using System.IO;

namespace DotNetMigrations.Loggers
{
	public class FileLog : LoggerBase, IDisposable
	{
		private string _logName = "FileLog";
		protected StreamWriter _streamWriter;


		public FileLog()
		{
			var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
			if (!Directory.Exists(".\\AppData"))
				Directory.CreateDirectory(".\\AppData");
			_streamWriter = new StreamWriter(new FileStream($".\\AppData\\Migration.{timestamp}.log", FileMode.Create));
		}

		/// <summary>
		/// The name of the log.
		/// </summary>
		public override string LogName
		{
			get { return _logName; }
			set { _logName = value; }
		}

		/// <summary>
		/// Writes output to the console. Intelligently wraps
		/// text to fit the console's remaining line buffer,
		/// aligning wrapped lines to the current cursor position.
		/// </summary>
		public override void Write(string message)
		{
			_streamWriter.Write(message);
			_streamWriter.Flush();
		}

		/// <summary>
		/// Writes a line of text to the console window.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public override void WriteLine(string message)
		{
			Write(message + Environment.NewLine);
		}

		/// <summary>
		/// Writes a line of yellow text to the console window.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public override void WriteWarning(string message)
		{
			WriteLine("[WARNING] " + message);
		}

		/// <summary>
		/// Writes a line of red text to the console window.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public override void WriteError(string message)
		{
			WriteLine("[ERROR] " + message);
		}

		public override void Dispose()
		{
			_streamWriter.Close();
		}
	}
}