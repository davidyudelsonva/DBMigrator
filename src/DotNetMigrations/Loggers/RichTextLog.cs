using System;
using System.Linq;
using DotNetMigrations.Core;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace DotNetMigrations.Loggers
{
	public class RichTextLog : LoggerBase
	{
		private string _logName = "RichTextLog";
		private RichTextBox _outputBox;
		//private const ConsoleColor WarningColor = ConsoleColor.Yellow;
		//private const ConsoleColor ErrorColor = ConsoleColor.Red;

		public RichTextLog(RichTextBox outputBox)
		{
			_outputBox = outputBox;
			//  pad the console with a blank line
			//Console.WriteLine();
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
			_outputBox.AppendText(message);
		}

		/// <summary>
		/// Writes a line of text to the console window.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public override void WriteLine(string message)
		{
			Write(message);
			_outputBox.AppendText(Environment.NewLine);
		}

		/// <summary>
		/// Writes a line of yellow text to the console window.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public override void WriteWarning(string message)
		{
			var index = _outputBox.TextLength;
			
			WriteLine(message);
			_outputBox.Select(index, _outputBox.TextLength - index);
			_outputBox.SelectionColor = Color.Yellow;
		}

		/// <summary>
		/// Writes a line of red text to the console window.
		/// </summary>
		/// <param name="message">The message to write.</param>
		public override void WriteError(string message)
		{
			var index = _outputBox.TextLength;

			WriteLine(message);
			_outputBox.Select(index, _outputBox.TextLength - index);
			_outputBox.SelectionColor = Color.Red;
		}
	}
}