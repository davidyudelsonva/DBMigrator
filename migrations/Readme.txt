USAGE:

Use DBMigrator to create and apply migrations to SQL databases.
Recommend process:
1) Generate Migration and move to DB folder
2) Create Migration SETUP and TEARDOWN scripts in generated file
3) Apply migration
4) Rollback migration
5) Apply migration
6) Push migration up for code review.


See DbMigrator.exe -help [command] for specifics.

--- Creating a new migration ---
	DbMigrator.exe generate YourMigrationName 
Creates a script file with a timestamp in the name and appropriate tags in the files.
Move the script to the folder for the database it targets.

--- Checking For Applied/Unapplied scripts ---
	DBMigrator.exe version [DatabaseName]

This will check the version stored in the database with the timestamps on the files to determine which files have been run.

--- Applying a Migration ---
	DBMigrator.exe migrate [DatabaseName]
	
This will run through all the scripts that need to be applied and execute them on the database.
Each script is run inside a transaction, so if errors occur, the whole script will be rolled back.
DO NOT USE TRANSACTIONS in these scripts, as that will interfere with the DBMigrator created transaction

--- Rolling Back a Migration ---
	DBMigrator.exe rollback [DatbaseName]

This executes the teardown of the highest timestamped script that was applied to the database.
A rollback should exactly undo all the changes applied in the script.

--- Multi-database commands
	DBMigrator.exe versionAll
	DBMigrator.exe migrateAll
	
These two commands will loop through all folders in the migrations directory and execute the respective command for each database.