using DotNetMigrations.Commands;
using DotNetMigrations.Core;
using DotNetMigrations.Core.Data;
using DotNetMigrations.Core.Migrations;
using DotNetMigrations.Loggers;
using DotNetMigrations.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace DbMigratorGUI
{
	public partial class DBMigratorMainForm : Form
	{
		private MigrationDirectory _rollbackDirectory;
		private MigrationDirectory _migrationDirectory;
		private ConnectionStringFactory _connectionStringFactory;
		private IConfigurationManager _configurationManager = new ConfigurationManagerWrapper();
		private ILogger Log;
          public string _Test;
          private string _dbMigratorVersionNumber;

		public DBMigratorMainForm(LogRepository logRepo)
		{
			InitializeComponent();
			Text = $"DB Migrator - Environment: {(_configurationManager.AppSettings[AppSettingKeys.Environment] ?? "Local")}";
			logRepo.Logs.Add(new RichTextLog(outputTextBox));
			Log = logRepo;

               _dbMigratorVersionNumber = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
          }

		private void DBMigratorMainForm_Load(object sender, EventArgs e)
		{
			_connectionStringFactory = new ConnectionStringFactory();
			InitializeListViews();
			_rollbackDirectory = new MigrationDirectory("rollback");
			_migrationDirectory = new MigrationDirectory("migrate");
			LoadRollbacksListView();
			LoadMigrationsListView();
			checkAllListViewItems(RollbacksListView, true);
			checkAllListViewItems(MigrationsListView, true);

			newMigrationPanel.Visible = bool.Parse(_configurationManager.AppSettings["enableNewScriptPanel"] ?? "false");


			LoadMigrationHistory();

			ScriptsTabs.Selected += ScriptsTabs_Selected;

			RollbacksListView.ItemChecked += ListView_ItemChecked;
			MigrationsListView.ItemChecked += ListView_ItemChecked;

			if (RollbacksListView.Items.Count == 0)
				ScriptsTabs.SelectTab(MigrationsTab);
			else
				ScriptsTabs.SelectTab(RollbacksTab);
			UpdateButtonState();
		}

		private void InitializeListViews()
		{
			var listViews = new List<ListView> { RollbacksListView, MigrationsListView, MigrationHistoryListView };

			var baseDirectory = _configurationManager.AppSettings[AppSettingKeys.MigrateFolder];
			var migrateDir = Path.Combine(baseDirectory, "migrate");
			var rollbackDir = Path.Combine(baseDirectory, "rollback");

			foreach (var dir in new List<string> { migrateDir, rollbackDir })
			{
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
			}

			var directories = Directory.EnumerateDirectories(migrateDir)
				.Select(d => new DirectoryInfo(d).Name).ToList();

			directories.AddRange(
					Directory.EnumerateDirectories(rollbackDir)
					.Select(d => new DirectoryInfo(d).Name));

			directories = directories.Distinct().ToList();

			NewMigrationDatabaseComboBox.Items.AddRange(directories.ToArray());
			NewMigrationDatabaseComboBox.SelectedIndex = 0;

			foreach (var lv in listViews)
			{
				lv.ShowItemToolTips = true;
				lv.Columns.AddRange(new ColumnHeader[]
				{
						new ColumnHeader() { Text = "Version", Width = 125},
						new ColumnHeader() { Text = "Name", Width = 170},
						new ColumnHeader() { Text = "Author", Width = 120},
						new ColumnHeader() { Text = "Ticket", Width = 60},
						new ColumnHeader() { Text = "Created Date", Width = 120},
						new ColumnHeader() { Text = "Applied Date", Width = 120},
						new ColumnHeader() { Text = "Applied By", Width = 120}
				});
				foreach (var dir in directories)
				{
					lv.Groups.Add(new ListViewGroup(dir, HorizontalAlignment.Left)
					{
						Name = dir,
						Header = dir
					});
				}
			}
		}
				
		private void ListView_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			UpdateButtonState();
		}

		private void ScriptsTabs_Selected(object sender, TabControlEventArgs e)
		{
			UpdateButtonState();
		}

		private void UpdateButtonState() { 
			

			switch (ScriptsTabs.SelectedTab.Name)
			{
				case "RollbacksTab":
					MigrateButton.Enabled = false;
					SwitchScriptsButton.Text = "Move selected scripts to migrations";
					if (RollbacksListView.CheckedItems.Count > 0)
					{
						RollbackButton.Enabled = true;
						SwitchScriptsButton.Enabled = true;
					}
					else
					{
						RollbackButton.Enabled = false;
						SwitchScriptsButton.Enabled = false;
					}
					break;
				case "MigrationsTab":
					RollbackButton.Enabled = false;
					SwitchScriptsButton.Text = "Move selected scripts to rollbacks";
					if (MigrationsListView.CheckedItems.Count > 0)
					{
						MigrateButton.Enabled = true;
						SwitchScriptsButton.Enabled = true;
					}
					else
					{
						MigrateButton.Enabled = false;
						SwitchScriptsButton.Enabled = false;
					}
					break;
				case "MigrationHistoryTab":
					MigrateButton.Enabled = false;
					RollbackButton.Enabled = false;
					SwitchScriptsButton.Enabled = false;
					break;
			}
		}

		private void LoadMigrationsListView()
		{
			LoadListView("Migration Scripts", _migrationDirectory, MigrationsListView, MigrationsTab);
		}

		private void LoadRollbacksListView()
		{
			LoadListView("Rollback Scripts", _rollbackDirectory, RollbacksListView, RollbacksTab);
		}

		private void LoadListView(string name, MigrationDirectory migrationDirectory, ListView listView, TabPage tabPage)
		{
			//Log.WriteLine($"Reading Directory for {name}");

			var _groups = new Dictionary<string, ListViewGroup>();

			listView.Items.Clear();
			listView.Groups.Clear();

			foreach (var script in migrationDirectory.GetScripts((string)null).OrderBy(s => s.Database))
			{
				script.Read();
				if (!_groups.ContainsKey(script.Database))
				{
					_groups[script.Database] = new ListViewGroup(script.Database, script.Database);
					listView.Groups.Add(_groups[script.Database]);
				}
				var item = new ListViewItem(
					new string[] {
						script.Version.ToString(),
						script.Name,
						script.Author,
						script.Ticket_Number,
						script.CreatedDate

					}, _groups[script.Database])
					{
						ToolTipText = script.Description,
					};
				item.Tag = script.FilePath;
				listView.Items.Add(item);
			}

			foreach (var group in listView.Groups.Cast<ListViewGroup>())
			{
				var dataAccess = GetDataAccess(group.Name);
				if (dataAccess == null)
					continue;

				var appliedMigrations = dataAccess.GetAppliedMigrations();
				foreach (var item in group.Items.Cast<ListViewItem>())
				{

					var matchingMigration = appliedMigrations.Where(a => long.Parse(item.Text) == a.Version).FirstOrDefault();
					if (matchingMigration != null)
					{
						item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = matchingMigration.AppliedDate?.ToShortDateString() });
						item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = matchingMigration.AppliedByUser });
					}
				}
			}
			//listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			tabPage.Text = name + $" ({listView.Items.Count})";
		}

		private void LoadMigrationHistory()
		{
			MigrationHistoryListView.Items.Clear();
			var showUndetailedMigrations = showUndetailedMigrationsCheckBox.Checked;

			foreach (var group in MigrationHistoryListView.Groups.Cast<ListViewGroup>())
			{
				var dataAccess = GetDataAccess(group.Name);
				if (dataAccess == null)
					continue;

				var appliedMigrations = dataAccess.GetAppliedMigrations();
				foreach (var migration in appliedMigrations.OrderByDescending(m => m.Version))
				{
					if (showUndetailedMigrations || !string.IsNullOrEmpty(migration.Name))
						MigrationHistoryListView.Items.Add(
							new ListViewItem(migration.ToStringArray(), group) { ToolTipText = migration.Description }
						);
				}

			}

			MigrationHistoryTab.Text = $"Migration History ({MigrationHistoryListView.Items.Count})";
		}

		private DataAccess GetDataAccess(string connectionStringName)
		{
			try
			{
				var connectionString = _connectionStringFactory.GetConnectionString(connectionStringName);

				var dataAccess = DataAccessFactory.Create(connectionString, Log);

				//  perform the database initialization
				dataAccess.OpenConnection();
				var dbInit = new DatabaseInitializer(dataAccess);
				dbInit.Initialize();

				return dataAccess;
			}
			catch (ArgumentException ex)
			{
				Log.WriteError(ex.Message);
				return null;
			}
			catch (SqlException ex)
			{
				Log.WriteError(ex.Message);
				return null;
			}
		}

		private void checkAllListViewItems(ListView scriptListView, bool check)
		{
			foreach (var item in scriptListView.Items.Cast<ListViewItem>())
			{
				item.Checked = check;
			}
		}

		private void rollbackSelectAllLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			checkAllListViewItems(RollbacksListView, true);
		}

		private void rollbackSelectNoneLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			checkAllListViewItems(RollbacksListView, false);
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			checkAllListViewItems(MigrationsListView, true);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			checkAllListViewItems(MigrationsListView, false);
		}

		private void showUndetailedMigrationsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			LoadMigrationHistory();
		}

		private void MigrateButton_Click(object sender, EventArgs e)
		{
               if (!PreserveLogCheckBox.Checked)
				outputTextBox.Text = "";

               Log.WriteLine(cbReleaseMode.Checked
                    ? "Beginning Migration in RELEASE mode."
                    : "Beginning Migration in default mode.");

               var command = new MigrateFolderCommand {Log = Log};

               var items = MigrationsListView.CheckedItems;
			var versions = new List<long>();
			foreach(var item in items.Cast<ListViewItem>())
			{
				versions.Add(long.Parse(item.Text));
			}


               command.Run(new MigrateFolderCommandArgs
			{
				Folder = "migrate",
				Versions = versions,
				Direction = MigrationDirection.Up,
                    IsReleaseMode =  cbReleaseMode.Checked,
                    DbMigratorVersionNumber = _dbMigratorVersionNumber
               });

			LoadMigrationsListView();
               var migrationMessage = "Migration Complete";
               if (!command.WasMigrationSuccessful)
                    migrationMessage = "Migration ABORTED";

               Log.WriteLine(migrationMessage);
          }

		private void RollbackButton_Click(object sender, EventArgs e)
		{
			if (!PreserveLogCheckBox.Checked)
				outputTextBox.Text = "";

			Log.WriteLine("Beginging Rollback");

			var command = new MigrateFolderCommand();
			command.Log = Log;

			var items = RollbacksListView.CheckedItems;
			var versions = new List<long>();
			foreach (var item in items.Cast<ListViewItem>())
			{
				versions.Add(long.Parse(item.Text));
			}

			command.Run(new MigrateFolderCommandArgs
			{
				Folder = "rollback",
				Versions = versions,
				Direction = MigrationDirection.Down,
                    IsReleaseMode = cbReleaseMode.Checked,
                    DbMigratorVersionNumber = _dbMigratorVersionNumber
               });
			LoadRollbacksListView();
			Log.WriteLine("Rollback Complete");
		}

		private void SwitchScriptsButton_Click(object sender, EventArgs e)
		{
			var selectedTab = ScriptsTabs.SelectedTab;

			string sourceDirectory = null;
			string destinationDirectory = null;

			var filePaths = new List<string>();

			if(selectedTab == RollbacksTab)
			{
				sourceDirectory = "\\rollback\\";
				destinationDirectory = "\\migrate\\";

				filePaths = RollbacksListView.CheckedItems.Cast<ListViewItem>()
										.Select(lvi => (string)lvi.Tag).ToList();
			}
			else
			{
				sourceDirectory = "\\migrate\\";
				destinationDirectory = "\\rollback\\";

				filePaths = MigrationsListView.CheckedItems.Cast<ListViewItem>()
										.Select(lvi => (string)lvi.Tag).ToList();
			}

			foreach(var filePath in filePaths)
			{
				var newFilePathInfo = new FileInfo(filePath.Replace(sourceDirectory, destinationDirectory));
				if (!Directory.Exists(newFilePathInfo.DirectoryName))
				{
					Directory.CreateDirectory(newFilePathInfo.DirectoryName);
				}

				Log.WriteLine($"Moving MigrationScript from {filePath} to {newFilePathInfo.FullName}");

				if (File.Exists(newFilePathInfo.FullName))
				{
					File.Delete(newFilePathInfo.FullName);
				}

				File.Move(filePath, newFilePathInfo.FullName);
			}

			LoadRollbacksListView();
			LoadMigrationsListView();
			Log.WriteLine("Moving Scripts Complete");
		}


		private void ListView_DoubleClick(object sender, System.EventArgs e)
		{
			var listView = sender as ListView;
			if (listView == null || listView.SelectedItems.Count == 0)
				return;

			var filePath = listView.SelectedItems[0].Tag.ToString();
			System.Diagnostics.Process.Start(filePath);
		}

		private void CreateScriptButton_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(NewMigrationDatabaseComboBox.SelectedItem.ToString()))
			{
				Log.WriteError("Must select Database for new script");
				return;
			}

			if (string.IsNullOrEmpty(NewMigrationNameTextBox.Text))
			{
				Log.WriteError("Must Enter name for new migration");
				return;
			}

			var command = new GenerateScriptCommand();
			command.Log = Log;

			command.Run(new GenerateScriptCommandArgs
			{
				DatabaseName = NewMigrationDatabaseComboBox.SelectedItem.ToString(),
				MigrationName = NewMigrationNameTextBox.Text
			});

			LoadMigrationsListView();
		}

		private void ClearLogButton_Click(object sender, EventArgs e)
		{
			outputTextBox.Text = "";
		}

		private void CopyLogButton_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.Clipboard.SetText(outputTextBox.Text);
		}
	}

     internal class SimpleAssemblyInfo
     {
          public string Name { get; set; }
          public string Version { get; set; }
     }
}
