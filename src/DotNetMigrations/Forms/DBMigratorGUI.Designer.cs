namespace DbMigratorGUI
{
	partial class DBMigratorMainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
               this.MigrationsListView = new System.Windows.Forms.ListView();
               this.ScriptsTabs = new System.Windows.Forms.TabControl();
               this.RollbacksTab = new System.Windows.Forms.TabPage();
               this.rollbackSelectNoneLink = new System.Windows.Forms.LinkLabel();
               this.rollbackSelectAllLink = new System.Windows.Forms.LinkLabel();
               this.label1 = new System.Windows.Forms.Label();
               this.RollbacksListView = new System.Windows.Forms.ListView();
               this.MigrationsTab = new System.Windows.Forms.TabPage();
               this.newMigrationPanel = new System.Windows.Forms.Panel();
               this.label3 = new System.Windows.Forms.Label();
               this.NewMigrationNameTextBox = new System.Windows.Forms.TextBox();
               this.NewMigrationDatabaseComboBox = new System.Windows.Forms.ComboBox();
               this.label4 = new System.Windows.Forms.Label();
               this.CreateScriptButton = new System.Windows.Forms.Button();
               this.linkLabel1 = new System.Windows.Forms.LinkLabel();
               this.linkLabel2 = new System.Windows.Forms.LinkLabel();
               this.label2 = new System.Windows.Forms.Label();
               this.MigrationHistoryTab = new System.Windows.Forms.TabPage();
               this.showUndetailedMigrationsCheckBox = new System.Windows.Forms.CheckBox();
               this.MigrationHistoryListView = new System.Windows.Forms.ListView();
               this.MigrateButton = new System.Windows.Forms.Button();
               this.RollbackButton = new System.Windows.Forms.Button();
               this.SwitchScriptsButton = new System.Windows.Forms.Button();
               this.outputTextBox = new System.Windows.Forms.RichTextBox();
               this.outputBox = new System.Windows.Forms.GroupBox();
               this.CopyLogButton = new System.Windows.Forms.Button();
               this.ClearLogButton = new System.Windows.Forms.Button();
               this.PreserveLogCheckBox = new System.Windows.Forms.CheckBox();
               this.cbReleaseMode = new System.Windows.Forms.CheckBox();
               this.ScriptsTabs.SuspendLayout();
               this.RollbacksTab.SuspendLayout();
               this.MigrationsTab.SuspendLayout();
               this.newMigrationPanel.SuspendLayout();
               this.MigrationHistoryTab.SuspendLayout();
               this.outputBox.SuspendLayout();
               this.SuspendLayout();
               // 
               // MigrationsListView
               // 
               this.MigrationsListView.CheckBoxes = true;
               this.MigrationsListView.Dock = System.Windows.Forms.DockStyle.Bottom;
               this.MigrationsListView.FullRowSelect = true;
               this.MigrationsListView.Location = new System.Drawing.Point(3, 36);
               this.MigrationsListView.Name = "MigrationsListView";
               this.MigrationsListView.Size = new System.Drawing.Size(1204, 375);
               this.MigrationsListView.TabIndex = 0;
               this.MigrationsListView.UseCompatibleStateImageBehavior = false;
               this.MigrationsListView.View = System.Windows.Forms.View.Details;
               this.MigrationsListView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
               // 
               // ScriptsTabs
               // 
               this.ScriptsTabs.Controls.Add(this.RollbacksTab);
               this.ScriptsTabs.Controls.Add(this.MigrationsTab);
               this.ScriptsTabs.Controls.Add(this.MigrationHistoryTab);
               this.ScriptsTabs.Dock = System.Windows.Forms.DockStyle.Top;
               this.ScriptsTabs.Location = new System.Drawing.Point(0, 0);
               this.ScriptsTabs.Name = "ScriptsTabs";
               this.ScriptsTabs.SelectedIndex = 0;
               this.ScriptsTabs.Size = new System.Drawing.Size(1218, 443);
               this.ScriptsTabs.TabIndex = 1;
               // 
               // RollbacksTab
               // 
               this.RollbacksTab.Controls.Add(this.rollbackSelectNoneLink);
               this.RollbacksTab.Controls.Add(this.rollbackSelectAllLink);
               this.RollbacksTab.Controls.Add(this.label1);
               this.RollbacksTab.Controls.Add(this.RollbacksListView);
               this.RollbacksTab.Location = new System.Drawing.Point(4, 25);
               this.RollbacksTab.Name = "RollbacksTab";
               this.RollbacksTab.Padding = new System.Windows.Forms.Padding(3);
               this.RollbacksTab.Size = new System.Drawing.Size(1210, 414);
               this.RollbacksTab.TabIndex = 0;
               this.RollbacksTab.Text = "Rollback Scripts (0)";
               this.RollbacksTab.UseVisualStyleBackColor = true;
               // 
               // rollbackSelectNoneLink
               // 
               this.rollbackSelectNoneLink.AutoSize = true;
               this.rollbackSelectNoneLink.Location = new System.Drawing.Point(91, 7);
               this.rollbackSelectNoneLink.Name = "rollbackSelectNoneLink";
               this.rollbackSelectNoneLink.Size = new System.Drawing.Size(42, 17);
               this.rollbackSelectNoneLink.TabIndex = 4;
               this.rollbackSelectNoneLink.TabStop = true;
               this.rollbackSelectNoneLink.Text = "None";
               this.rollbackSelectNoneLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.rollbackSelectNoneLink_LinkClicked);
               // 
               // rollbackSelectAllLink
               // 
               this.rollbackSelectAllLink.AutoSize = true;
               this.rollbackSelectAllLink.Location = new System.Drawing.Point(62, 7);
               this.rollbackSelectAllLink.Name = "rollbackSelectAllLink";
               this.rollbackSelectAllLink.Size = new System.Drawing.Size(23, 17);
               this.rollbackSelectAllLink.TabIndex = 3;
               this.rollbackSelectAllLink.TabStop = true;
               this.rollbackSelectAllLink.Text = "All";
               this.rollbackSelectAllLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.rollbackSelectAllLink_LinkClicked);
               // 
               // label1
               // 
               this.label1.AutoSize = true;
               this.label1.Location = new System.Drawing.Point(5, 7);
               this.label1.Name = "label1";
               this.label1.Size = new System.Drawing.Size(51, 17);
               this.label1.TabIndex = 2;
               this.label1.Text = "Select:";
               // 
               // RollbacksListView
               // 
               this.RollbacksListView.CheckBoxes = true;
               this.RollbacksListView.Dock = System.Windows.Forms.DockStyle.Bottom;
               this.RollbacksListView.FullRowSelect = true;
               this.RollbacksListView.Location = new System.Drawing.Point(3, 36);
               this.RollbacksListView.Name = "RollbacksListView";
               this.RollbacksListView.Size = new System.Drawing.Size(1204, 375);
               this.RollbacksListView.TabIndex = 1;
               this.RollbacksListView.UseCompatibleStateImageBehavior = false;
               this.RollbacksListView.View = System.Windows.Forms.View.Details;
               this.RollbacksListView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
               // 
               // MigrationsTab
               // 
               this.MigrationsTab.Controls.Add(this.newMigrationPanel);
               this.MigrationsTab.Controls.Add(this.linkLabel1);
               this.MigrationsTab.Controls.Add(this.linkLabel2);
               this.MigrationsTab.Controls.Add(this.label2);
               this.MigrationsTab.Controls.Add(this.MigrationsListView);
               this.MigrationsTab.Location = new System.Drawing.Point(4, 25);
               this.MigrationsTab.Name = "MigrationsTab";
               this.MigrationsTab.Padding = new System.Windows.Forms.Padding(3);
               this.MigrationsTab.Size = new System.Drawing.Size(1210, 414);
               this.MigrationsTab.TabIndex = 1;
               this.MigrationsTab.Text = "Migration Scripts (15)";
               this.MigrationsTab.UseVisualStyleBackColor = true;
               // 
               // newMigrationPanel
               // 
               this.newMigrationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
               this.newMigrationPanel.Controls.Add(this.label3);
               this.newMigrationPanel.Controls.Add(this.NewMigrationNameTextBox);
               this.newMigrationPanel.Controls.Add(this.NewMigrationDatabaseComboBox);
               this.newMigrationPanel.Controls.Add(this.label4);
               this.newMigrationPanel.Controls.Add(this.CreateScriptButton);
               this.newMigrationPanel.Location = new System.Drawing.Point(438, 0);
               this.newMigrationPanel.Name = "newMigrationPanel";
               this.newMigrationPanel.Size = new System.Drawing.Size(764, 30);
               this.newMigrationPanel.TabIndex = 13;
               // 
               // label3
               // 
               this.label3.AutoSize = true;
               this.label3.Location = new System.Drawing.Point(3, 7);
               this.label3.Name = "label3";
               this.label3.Size = new System.Drawing.Size(218, 17);
               this.label3.TabIndex = 8;
               this.label3.Text = "Generate Migration for database:";
               // 
               // NewMigrationNameTextBox
               // 
               this.NewMigrationNameTextBox.Location = new System.Drawing.Point(417, 4);
               this.NewMigrationNameTextBox.Name = "NewMigrationNameTextBox";
               this.NewMigrationNameTextBox.Size = new System.Drawing.Size(262, 22);
               this.NewMigrationNameTextBox.TabIndex = 12;
               // 
               // NewMigrationDatabaseComboBox
               // 
               this.NewMigrationDatabaseComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
               this.NewMigrationDatabaseComboBox.FormattingEnabled = true;
               this.NewMigrationDatabaseComboBox.Location = new System.Drawing.Point(227, 3);
               this.NewMigrationDatabaseComboBox.Name = "NewMigrationDatabaseComboBox";
               this.NewMigrationDatabaseComboBox.Size = new System.Drawing.Size(121, 24);
               this.NewMigrationDatabaseComboBox.TabIndex = 9;
               // 
               // label4
               // 
               this.label4.AutoSize = true;
               this.label4.Location = new System.Drawing.Point(354, 7);
               this.label4.Name = "label4";
               this.label4.Size = new System.Drawing.Size(57, 17);
               this.label4.TabIndex = 11;
               this.label4.Text = "Named:";
               // 
               // CreateScriptButton
               // 
               this.CreateScriptButton.Location = new System.Drawing.Point(686, 4);
               this.CreateScriptButton.Name = "CreateScriptButton";
               this.CreateScriptButton.Size = new System.Drawing.Size(75, 24);
               this.CreateScriptButton.TabIndex = 10;
               this.CreateScriptButton.Text = "Create Script";
               this.CreateScriptButton.UseVisualStyleBackColor = true;
               this.CreateScriptButton.Click += new System.EventHandler(this.CreateScriptButton_Click);
               // 
               // linkLabel1
               // 
               this.linkLabel1.AutoSize = true;
               this.linkLabel1.Location = new System.Drawing.Point(91, 7);
               this.linkLabel1.Name = "linkLabel1";
               this.linkLabel1.Size = new System.Drawing.Size(42, 17);
               this.linkLabel1.TabIndex = 7;
               this.linkLabel1.TabStop = true;
               this.linkLabel1.Text = "None";
               this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
               // 
               // linkLabel2
               // 
               this.linkLabel2.AutoSize = true;
               this.linkLabel2.Location = new System.Drawing.Point(62, 7);
               this.linkLabel2.Name = "linkLabel2";
               this.linkLabel2.Size = new System.Drawing.Size(23, 17);
               this.linkLabel2.TabIndex = 6;
               this.linkLabel2.TabStop = true;
               this.linkLabel2.Text = "All";
               this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
               // 
               // label2
               // 
               this.label2.AutoSize = true;
               this.label2.Location = new System.Drawing.Point(5, 7);
               this.label2.Name = "label2";
               this.label2.Size = new System.Drawing.Size(51, 17);
               this.label2.TabIndex = 5;
               this.label2.Text = "Select:";
               // 
               // MigrationHistoryTab
               // 
               this.MigrationHistoryTab.Controls.Add(this.showUndetailedMigrationsCheckBox);
               this.MigrationHistoryTab.Controls.Add(this.MigrationHistoryListView);
               this.MigrationHistoryTab.Location = new System.Drawing.Point(4, 25);
               this.MigrationHistoryTab.Name = "MigrationHistoryTab";
               this.MigrationHistoryTab.Padding = new System.Windows.Forms.Padding(3);
               this.MigrationHistoryTab.Size = new System.Drawing.Size(1210, 414);
               this.MigrationHistoryTab.TabIndex = 2;
               this.MigrationHistoryTab.Text = "Migration History (37)";
               this.MigrationHistoryTab.UseVisualStyleBackColor = true;
               // 
               // showUndetailedMigrationsCheckBox
               // 
               this.showUndetailedMigrationsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
               this.showUndetailedMigrationsCheckBox.AutoSize = true;
               this.showUndetailedMigrationsCheckBox.Location = new System.Drawing.Point(999, 9);
               this.showUndetailedMigrationsCheckBox.Name = "showUndetailedMigrationsCheckBox";
               this.showUndetailedMigrationsCheckBox.Size = new System.Drawing.Size(203, 21);
               this.showUndetailedMigrationsCheckBox.TabIndex = 2;
               this.showUndetailedMigrationsCheckBox.TabStop = false;
               this.showUndetailedMigrationsCheckBox.Text = "Show undetailed migrations";
               this.showUndetailedMigrationsCheckBox.UseVisualStyleBackColor = true;
               this.showUndetailedMigrationsCheckBox.CheckedChanged += new System.EventHandler(this.showUndetailedMigrationsCheckBox_CheckedChanged);
               // 
               // MigrationHistoryListView
               // 
               this.MigrationHistoryListView.Dock = System.Windows.Forms.DockStyle.Bottom;
               this.MigrationHistoryListView.Location = new System.Drawing.Point(3, 36);
               this.MigrationHistoryListView.Name = "MigrationHistoryListView";
               this.MigrationHistoryListView.Size = new System.Drawing.Size(1204, 375);
               this.MigrationHistoryListView.TabIndex = 1;
               this.MigrationHistoryListView.UseCompatibleStateImageBehavior = false;
               this.MigrationHistoryListView.View = System.Windows.Forms.View.Details;
               // 
               // MigrateButton
               // 
               this.MigrateButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
               this.MigrateButton.Location = new System.Drawing.Point(1074, 449);
               this.MigrateButton.Name = "MigrateButton";
               this.MigrateButton.Size = new System.Drawing.Size(132, 46);
               this.MigrateButton.TabIndex = 3;
               this.MigrateButton.Text = "Migrate";
               this.MigrateButton.UseVisualStyleBackColor = true;
               this.MigrateButton.Click += new System.EventHandler(this.MigrateButton_Click);
               // 
               // RollbackButton
               // 
               this.RollbackButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
               this.RollbackButton.Location = new System.Drawing.Point(12, 449);
               this.RollbackButton.Name = "RollbackButton";
               this.RollbackButton.Size = new System.Drawing.Size(132, 46);
               this.RollbackButton.TabIndex = 4;
               this.RollbackButton.Text = "Rollback";
               this.RollbackButton.UseVisualStyleBackColor = true;
               this.RollbackButton.Click += new System.EventHandler(this.RollbackButton_Click);
               // 
               // SwitchScriptsButton
               // 
               this.SwitchScriptsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
               this.SwitchScriptsButton.Location = new System.Drawing.Point(319, 449);
               this.SwitchScriptsButton.Name = "SwitchScriptsButton";
               this.SwitchScriptsButton.Size = new System.Drawing.Size(556, 46);
               this.SwitchScriptsButton.TabIndex = 5;
               this.SwitchScriptsButton.Text = "Move Scripts to Migrate|Rollback";
               this.SwitchScriptsButton.UseVisualStyleBackColor = true;
               this.SwitchScriptsButton.Click += new System.EventHandler(this.SwitchScriptsButton_Click);
               // 
               // outputTextBox
               // 
               this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
               this.outputTextBox.Location = new System.Drawing.Point(3, 56);
               this.outputTextBox.Name = "outputTextBox";
               this.outputTextBox.ReadOnly = true;
               this.outputTextBox.Size = new System.Drawing.Size(1212, 299);
               this.outputTextBox.TabIndex = 6;
               this.outputTextBox.Text = "";
               // 
               // outputBox
               // 
               this.outputBox.Controls.Add(this.cbReleaseMode);
               this.outputBox.Controls.Add(this.CopyLogButton);
               this.outputBox.Controls.Add(this.ClearLogButton);
               this.outputBox.Controls.Add(this.PreserveLogCheckBox);
               this.outputBox.Controls.Add(this.outputTextBox);
               this.outputBox.Dock = System.Windows.Forms.DockStyle.Bottom;
               this.outputBox.Location = new System.Drawing.Point(0, 514);
               this.outputBox.Name = "outputBox";
               this.outputBox.Size = new System.Drawing.Size(1218, 358);
               this.outputBox.TabIndex = 7;
               this.outputBox.TabStop = false;
               this.outputBox.Text = "Output";
               // 
               // CopyLogButton
               // 
               this.CopyLogButton.Location = new System.Drawing.Point(319, 19);
               this.CopyLogButton.Name = "CopyLogButton";
               this.CopyLogButton.Size = new System.Drawing.Size(556, 30);
               this.CopyLogButton.TabIndex = 9;
               this.CopyLogButton.Text = "Copy Log";
               this.CopyLogButton.UseVisualStyleBackColor = true;
               this.CopyLogButton.Click += new System.EventHandler(this.CopyLogButton_Click);
               // 
               // ClearLogButton
               // 
               this.ClearLogButton.Location = new System.Drawing.Point(12, 21);
               this.ClearLogButton.Name = "ClearLogButton";
               this.ClearLogButton.Size = new System.Drawing.Size(91, 30);
               this.ClearLogButton.TabIndex = 8;
               this.ClearLogButton.Text = "Clear Log";
               this.ClearLogButton.UseVisualStyleBackColor = true;
               this.ClearLogButton.Click += new System.EventHandler(this.ClearLogButton_Click);
               // 
               // PreserveLogCheckBox
               // 
               this.PreserveLogCheckBox.AutoSize = true;
               this.PreserveLogCheckBox.Location = new System.Drawing.Point(1100, 25);
               this.PreserveLogCheckBox.Name = "PreserveLogCheckBox";
               this.PreserveLogCheckBox.Size = new System.Drawing.Size(115, 21);
               this.PreserveLogCheckBox.TabIndex = 7;
               this.PreserveLogCheckBox.Text = "Preserve Log";
               this.PreserveLogCheckBox.UseVisualStyleBackColor = true;
               // 
               // cbReleaseMode
               // 
               this.cbReleaseMode.AutoSize = true;
               this.cbReleaseMode.Location = new System.Drawing.Point(949, 25);
               this.cbReleaseMode.Name = "cbReleaseMode";
               this.cbReleaseMode.Size = new System.Drawing.Size(121, 21);
               this.cbReleaseMode.TabIndex = 10;
               this.cbReleaseMode.Text = "Release Mode";
               this.cbReleaseMode.UseVisualStyleBackColor = true;
               // 
               // DBMigratorMainForm
               // 
               this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
               this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
               this.AutoSize = true;
               this.ClientSize = new System.Drawing.Size(1218, 872);
               this.Controls.Add(this.outputBox);
               this.Controls.Add(this.SwitchScriptsButton);
               this.Controls.Add(this.RollbackButton);
               this.Controls.Add(this.MigrateButton);
               this.Controls.Add(this.ScriptsTabs);
               this.Name = "DBMigratorMainForm";
               this.Text = "DB Migrator";
               this.Load += new System.EventHandler(this.DBMigratorMainForm_Load);
               this.ScriptsTabs.ResumeLayout(false);
               this.RollbacksTab.ResumeLayout(false);
               this.RollbacksTab.PerformLayout();
               this.MigrationsTab.ResumeLayout(false);
               this.MigrationsTab.PerformLayout();
               this.newMigrationPanel.ResumeLayout(false);
               this.newMigrationPanel.PerformLayout();
               this.MigrationHistoryTab.ResumeLayout(false);
               this.MigrationHistoryTab.PerformLayout();
               this.outputBox.ResumeLayout(false);
               this.outputBox.PerformLayout();
               this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView MigrationsListView;
		private System.Windows.Forms.TabControl ScriptsTabs;
		private System.Windows.Forms.TabPage RollbacksTab;
		private System.Windows.Forms.TabPage MigrationsTab;
		private System.Windows.Forms.TabPage MigrationHistoryTab;
		private System.Windows.Forms.ListView RollbacksListView;
		private System.Windows.Forms.ListView MigrationHistoryListView;
		private System.Windows.Forms.Button MigrateButton;
		private System.Windows.Forms.Button RollbackButton;
		private System.Windows.Forms.Button SwitchScriptsButton;
		private System.Windows.Forms.RichTextBox outputTextBox;
		private System.Windows.Forms.GroupBox outputBox;
		private System.Windows.Forms.LinkLabel rollbackSelectNoneLink;
		private System.Windows.Forms.LinkLabel rollbackSelectAllLink;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.LinkLabel linkLabel2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox showUndetailedMigrationsCheckBox;
		private System.Windows.Forms.Button CreateScriptButton;
		private System.Windows.Forms.ComboBox NewMigrationDatabaseComboBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox NewMigrationNameTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button CopyLogButton;
		private System.Windows.Forms.Button ClearLogButton;
		private System.Windows.Forms.CheckBox PreserveLogCheckBox;
		private System.Windows.Forms.Panel newMigrationPanel;
          private System.Windows.Forms.CheckBox cbReleaseMode;
     }
}

