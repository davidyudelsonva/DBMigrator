﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetMigrations.Core;
using DotNetMigrations.Repositories;
using System.Windows.Forms;
using DbMigratorGUI;
using DotNetMigrations.Loggers;

namespace DotNetMigrations
{
	internal class Program
	{
		private static Program _current = null;
		public static Program Current
		{
			get
			{
				if (_current == null)
					_current = new Program();
				return _current;
			}
		}

		#region int Main

		/// <summary>
		/// Application entry point
		/// </summary>
		/// <param name="args"></param>
		[STAThread]
		private static int Main(string[] args)
		{
			return Current.Run(args);
		}

		#endregion

		private readonly CommandRepository _commandRepo;
		private readonly ScriptPreprocessorRepository _scriptPreprocessorRepo;

		private readonly LogRepository _logger;
		private FileLog _fileLog;
		private readonly bool _logFullErrors;

		private readonly bool _keepConsoleOpen;

		public CommandRepository CommandRepository { get { return _commandRepo; } }

		public ScriptPreprocessorRepository ScriptPreprocessorRepository { get { return _scriptPreprocessorRepo; } }

		private Program()
				: this(new ConfigurationManagerWrapper())
		{
		}

		/// <summary>
		/// Program constructor, instantiates primary repository objects.
		/// </summary>
		private Program(IConfigurationManager configManager)
		{
			_commandRepo = new CommandRepository();
			_scriptPreprocessorRepo = new ScriptPreprocessorRepository();
			_logger = new LogRepository();
			_fileLog = new FileLog();
			_logger.Logs.Add(_fileLog);

			string logFullErrorsSetting = configManager.AppSettings[AppSettingKeys.LogFullErrors];
			bool.TryParse(logFullErrorsSetting, out _logFullErrors);

			_keepConsoleOpen = ProgramLaunchedInSeparateConsoleWindow();
		}

		/// <summary>
		/// Primary Program Execution
		/// </summary>
		private int Run(string[] args)
		{
			if (!args.Any())
			{

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new DBMigratorMainForm(_logger));
				_fileLog.Dispose();
				return 0;
			}

			string executableName = Process.GetCurrentProcess().ProcessName + ".exe";
			ArgumentSet allArguments = ArgumentSet.Parse(args);
			var failure = false;
			var helpWriter = new CommandHelpWriter(_logger);

			bool showHelp = allArguments.ContainsName("help");

			string commandName = showHelp
															 ? allArguments.GetByName("help")
															 : allArguments.AnonymousArgs.FirstOrDefault();

			ICommand command = null;

			if (commandName != null)
			{
				command = _commandRepo.GetCommand(commandName);
			}

			if (command == null)
			{
				if (showHelp)
				{
					//  no command name was found, show the list of available commands
					WriteAppUsageHelp(executableName);
					helpWriter.WriteCommandList(_commandRepo.Commands);
				}
				else
				{
					//  invalid command name was given
					_logger.WriteError("'{0}' is not a DotNetMigrations command.", commandName);
					_logger.WriteLine(string.Empty);
					_logger.WriteError("See '{0} -help' for a list of available commands.", executableName);
					failure = true;
				}
			}

			if (showHelp && command != null)
			{
				//  show help for the given command
				helpWriter.WriteCommandHelp(command, executableName);
			}
			else if (command != null)
			{
				command.Log = _logger;

				var commandArgumentSet = ArgumentSet.Parse(args.Skip(1).ToArray());
				IArguments commandArgs = command.CreateArguments();
				commandArgs.Parse(commandArgumentSet);

				if (commandArgs.IsValid)
				{
					var timer = new Stopwatch();
					timer.Start();

					try
					{
						command.Run(commandArgs);
					}
					catch (Exception ex)
					{
						//_logger.WriteLine(string.Empty);
						failure = true;
						if (_logFullErrors)
						{
							_logger.WriteError(ex.ToString());
						}
						else
						{
							WriteShortErrorMessages(ex);
						}

						if (Debugger.IsAttached)
							throw;
					}
					finally
					{
						timer.Stop();

						_logger.WriteLine(string.Empty);
						_logger.WriteLine(string.Format("Command duration was {0}.",
																									 decimal.Divide(timer.ElapsedMilliseconds, 1000).ToString(
																											 "0.0000s")));
					}
				}
				else
				{
					//  argument validation failed, show errors
					WriteValidationErrors(command.CommandName, commandArgs.Errors);
					_logger.WriteLine(string.Empty);
					helpWriter.WriteCommandHelp(command, executableName);
					failure = true;
				}
			}

			if (_keepConsoleOpen)
			{
				Console.WriteLine();
				Console.WriteLine("  > Uh-oh. It looks like you didn't run me from a console.");
				Console.WriteLine("  > Did you double-click me?");
				Console.WriteLine("  > I don't like to be clicked.");
				Console.WriteLine("  > Please open a command prompt and run me by typing " + executableName + ".");
				Console.WriteLine();
				Console.WriteLine("Press any key to exit...");
				Console.Read();
			}
			_fileLog.Dispose();
			return failure ? 1 : 0;
		}

		/// <summary>
		/// Determines if the program was launched in a separate console window
		/// or whether it was run from within an existing console.
		/// i.e. launched by click the EXE in Windows Explorer or by typing db.exe at the command line
		/// </summary>
		/// <remarks>
		/// This method isn't full proof.
		/// http://support.microsoft.com/kb/99115
		/// and
		/// http://stackoverflow.com/questions/510805/can-a-win32-console-application-detect-if-it-has-been-run-from-the-explorer-or-no
		/// </remarks>
		private static bool ProgramLaunchedInSeparateConsoleWindow()
		{
			try
			{
				//  if no debugger is attached
				if (!Debugger.IsAttached && TestConsole())
					//  and the cursor position appears untouched
					if (Console.CursorLeft == 0 && Console.CursorTop == 1)
						//  and there are no arguments
						//  (we allow for 1 arg because the first arg appears
						//   to always be the path to the executable being run)
						if (Environment.GetCommandLineArgs().Length <= 1)
							//  then assume we were launched into a separate console window
							return true;
			}
			catch (IOException)
			{
				// if launched from inside some build runners (e.g. TeamCity) Console.CursorLeft will fail because of how the handles are attached
				return false;
			}

			//  looks like we were launched from command line, good!
			return false;
		}

		/// <summary>
		/// Returns true/false whether a console window is available.
		/// </summary>
		private static bool TestConsole()
		{
			try
			{
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		private void WriteShortErrorMessages(Exception ex)
		{
			_logger.WriteError(ex.Message);
			Exception innerEx = ex.InnerException;
			while (innerEx != null)
			{
				_logger.WriteLine(string.Empty);
				_logger.WriteError(innerEx.Message);

				innerEx = innerEx.InnerException;
			}
		}

		/// <summary>
		/// Writes usage help for the app to the logger.
		/// </summary>
		private void WriteAppUsageHelp(string executableName)
		{
			//_logger.WriteLine(string.Empty);
			_logger.Write("Usage: ");
			_logger.Write(executableName);
			_logger.WriteLine(" [-help] command [args]");
		}

		/// <summary>
		/// Writes out all validation errors to the logger.
		/// </summary>
		private void WriteValidationErrors(string commandName, IEnumerable<string> errors)
		{
			_logger.WriteError("Invalid arguments for the {0} command", commandName);
			_logger.WriteLine(string.Empty);
			errors.ForEach(x => _logger.WriteError("\t* " + x));
		}
	}
}