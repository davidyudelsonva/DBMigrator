DbMigrator Documentation

Table of Contents

1. Purpose
2. Configuration
   A. migrateFolder
   B. logFullErrors
   C. enableNewScriptPanel
   D. ConnectionStrings
	 E. Version Strategy
	 F. OutputSQL
   G. Folders
3. Database Schema & Operation
4. GUI Operation
   A. Migration Scripts Tab
      a. List View
      b. Selection
      c. Move Scripts to Rollback
      d. Migrate
      e. Create Script
   B. Rollback Scripts Tab
      a. List View
      b. Selection
      c. Move Scripts To Migrations
      d. Rollback
   C. Schema History Tab
      a. List View
      b. Show undetailed migrations
   D. Output
      a. Text Area
      b. Clear Log
      c. Copy Log
      d. Preserve Log
5. Deploying Hotfixes
6. Command Line Operation
   A. Version
   B. VersionAll
   C. Generate
   D. Apply
7. Logging

   

----- 1. Purpose ------

DB Migrator is a tool to aid in development and deployment of features dependent on MS SQL Servers
It is very important to deliver new code, and apply changes to the database at the same time.
Code is a matter of replacing existing applications.
Database changes must be done to a database while it is running, and must only be applied one.

Db Migrator operates on the concept of a migration file.  
The file contains meta-data, SQL to apply the changes, and SQL to rollback the changes.

Each database has its own dbo.schema_migrations table that tracks which scripts have been applied to it.
This makes the database itself the authority on which migrations have been applied, and works even with
database backups and restores.

DB Migrator shows information on scripts, and will apply them on the given databases.


----- 2. Configuration ------
----- 2.A. migrateFolder ------

This is that DB Migrator will read Scripts from.  Defaults to .\migrations  No need to change this in regular use.

----- 2.B. logFullErrors ------

When false, only the Message of any SQL errors are logged.
When true, a full stack trace will also be logged.

----- 2.C. enableNewScriptPanel -----

The GUI has a conveinence feature for developers.
On the migrations tab, there is a panel for creating a new migration from a template.
During deployment, new scripts shouldn't be created, as they will not have gone through the 
development and testing processes.
In the dev environment, setting this to true will show the panel.
Setting it to false or omitting it, will hide the panel.

----- 2.D. ConnectionStrings -----

The Connection Strings are standard MS SQL connection strings.
They should be customized for each environment.
The name="" element of each connection string should be the same for all environments.
The Name element is also used in the folder structure to make sure scripts are targeting the correct database.


----- 2.E. Version Strategy -----

Determines how the unique identifier is determined for each script.  Leave at the default value of utc_time.

----- 2.F. OutputSQL -----

If the value is set to true, the exact SQL executing is logged.

----- 2.G. Folders -----

This is the Folder Structure of DB Migrator:

./                      | Root Folder
./DbMigrator.exe        | Main application executable
./DbMigrator.exe.config | The configuration file
./DbMigrator.pdb        | Symbols file for error logging
./*.dll		        | Supportin Code Libraries
./*.pdb                 | Symbols file for error logging
./Readme.Txt            | This File
--/AppData              | Logs Folder
----/*.log              | Individual Log files.  1 per program execution.  Timestamped
--/migrations           | Folder to hold all migration files.
----/migrate            | Folder for holding migrations to be applied
------/[DB Name]        | Folder Named after the Database.  Needs to match up with the connection string name element.
                        | Its OK for the folder to not be the exact DB name, as long as the connection string itself maps to the right database.
                        | IE the Folder and Connection string name can be "AAMS" while the initial catalog is AAMS_Dev
--------/*.sql          | One or more migration sql files.
----/rollback           | Folder for holding migrations to be rolled back
------/[DB Name]        | Folder Named after the database
--------/*.sql          | One or more migration sql files.



------ 3. Database Schema & Operation ------

Database Table: dbo.schema_migrations
Columns:
[id]               | integer Primary Key, auto-generated
[version]          | Script identifier, a compressed timestamp to make a unique ID
[name]             | Short name for script function
[author]           | Developer who authored the script
[ticket_number]    | TFS (or other) tracking number
[description]      | Long description of the purpose of the script
[created_date]     | Date the script was created
[applied_date]     | Date the script was applied to the database
[applied_by_user]  | Windows user who applied the script

Each Migration has a SETUP and TEARDOWN section.
When applying the script, only the SETUP section is executed.
When rolling back the script, only the TEARDOWN section is executed.

Each script is run inside its own transaction.
The SQL for the script is executed, and then an Insert or Delete from the schema_migrations table.
If any error occurs, the transaction is rolled back and the process halts.


----- 4. GUI Operation -----
----- 4.A. Migration Scripts Tab -----

The migrations tab shows scripts in the migre folder.
These scripts are ones either waiting to be applied to the database
or scripts that have been applied to the database.

----- 4.A.a. List View ------

Groups the scripts by Database.  Shows information from each script.
If the Applied Date and Applied By columns are empty, the script has not been applied to the database.
If the Applied Data and Applied By columns are populated, the script has been applied to the database.

Double clicking a script will open the script in the default SQL editor on the system.

Hovering over the version column will display a tool tip with the script description.

----- 4.A.b. Selection ------

Each script has a check box.  The check box determines if the "Move selected scripts ..." and "Migrate" button
will operate on the script.
There are links in the upper left that will select All or None of the scripts.

WARNING:  Scripts often build upon each other.  Migrating scripts out of order may lead to errors.
It is recommended to only migrate scripts in ascending order.

----- 4.A.c. Move Scripts to Rollback -----

Pressing this button will move all selected scripts to the rollback folder.
The Rollback Scripts and Migrate Scripts tabs will be refreshed.

----- 4.A.d. Migrate ------

Pressing this button will execute the SETUP portion of each migration to the database.
The scripts being executed will be messaged in the output box.
Any errors will be printed in the in the output box.

Migration scripts will only be executed against the database once.
If a script has an Applied Date and Applied By defined, it will not be run against the database again,
even if it is selected.

WARNING:  Scripts often build upon each other.  Migrating scripts out of order may lead to errors.
It is recommended to only migrate scripts in ascending order.

----- 4.A.e. Create Script ------

If the enableNewScriptPanel value is set to "true" some controls for creating a new Script 
will be visible in the upper right of the Migration Scripts tab.
A drop down allows you to select the database for the script, and a Name text box to name the script.
After clicking create, the script will be generated using a template.
It will then open up the script in the default SQL editor on the system.

----- 5.B. Rollback Scripts Tab -----

The rollback scripts tab shows scripts in the rollback folder.
These scripts are ones either waiting to be reversed on the database
or scripts that have been applied to the database.

----- 5.B.a. List View -----

Groups the scripts by Database.  Shows information from each script.
If the Applied Date and Applied By columns are empty, the script has not been applied to the database.
If the Applied Data and Applied By columns are populated, the script has been applied to the database.

Double clicking a script will open the script in the default SQL editor on the system.

Hovering over the version column will display a tool tip with the script description.

----- 5.B.b. Selection ------

Each script has a check box.  The check box determines if the "Move selected scripts ..." and "Rollback" button
will operate on the script.
There are links in the upper left that will select All or None of the scripts.

WARNING:  Scripts often build upon each other.  Rollingback scripts out of order may lead to errors.
It is recommended to only rollback scripts in descending order.

----- 5.B.c. Move Scripts To Migrations ------

Pressing this button will move all selected scripts to the migrate folder.
The Rollback Scripts and Migrate Scripts tabs will be refreshed.

------ 5.B.d. Rollback -----

Pressing this button will execute the TEARDOWN portion of each migration to the database.
The scripts being executed will be messaged in the output box.
Any errors will be printed in the in the output box.

Teardown scripts will only be executed against the database once.
If a script has no value for Applied Data and Applied by, the teardown will not be run against the database again,
even if it is selected.

WARNING:  Scripts often build upon each other.  Rolling back scripts out of order may lead to errors.
It is recommended to only rollback scripts in descending order.

----- 4.C. Schema History Tab -----

This view lists data directly from the the schema_migrations table from each database.
It lists applied migrations in descending order.

----- 4.C.a. List View ------

Displays information from the database.
Hovering over the Version column will display a tooltip of the description, if one exists for the script.

----- 4.C.b. Show undetailed migrations -----

Prior to tracking more information, the version number was the only value tracked in the databse.
By default these migrations are hidden due to conveying very little information.
Checking this box will display these limited detail migration records.

----- 4.D. Output -----

Displays messages to the user.

----- 4.D.a. Text Area -----

Contains messages to the user on the operations being performed.
Is read only.

----- 4.D.b. Clear Log -----

Pressing this button will erase the output.

----- 4.D.c. Copy Log -----

Pressing this button will copy the text box contents to the clipboard.

----- 4.D.d. Preserve Log -----

By Default, migrating or rolling back scripts will erase the output area before beginning.
Checking this box will stop it from erasing the output box before beginning an operation.

----- 5. Deploying Hotfixes -----

DbMigrator operates solely on the scripts placed in the migrations folder.
If new script needs to be applied to the database, it can be placed in the 
.\migrations\migrate\[Database Name]\ folder.

Execute DbMigrator and it will show the new script in the Migration Scripts tab
and use the migrate functionality as described above.

----- 6. Command Line Operation -----

DbMigrator also works on the command line.
A detailed list of these commands can be found by entering: DbMigrator.exe -help

----- 6.A. Version -----

Usage: DbMigrator.exe version [connectionName]

Will display the highest migration applied to the database,
the highest migration in the migrations folder,
and lists any scripts that are pending migration.

----- 6.B. VersionAll -----

Usage: DbMigrator.exe versionAll

For each database in order:
Will display the highest migration applied to the database,
the highest migration in the migrations folder,
and lists any scripts that are pending migration.

----- 6.C. Generate -----

Usage: DbMigrator.exe generate -n migration_name -d DatabaseName

Creates a new migration script from a template.
Migration Name is the name for the file.
DatabaseName is the name of the database folder to place the script in.

----- 6.D. Apply -----

Usage: DbMigrator.exe Apply

This will first run through all scripts in the rollback folder 
and execute their TEARDOWN section.
It will then run through all scripts in the migrate folder 
and execute their SETUP section.

This is targeted at usage for build servers, or headless deployments.

----- 7. Logging -----

DbMigrator Logs Information to the console, the Output text area in GUI mode,
and to log folders in the .\AppData folder.

Each time DbMigrator is executed it will begin a new log file.
The file will be named Migration.YYYY-MM-DD_HH-MM.log
All output statements will be logged to this file until the program closes.

