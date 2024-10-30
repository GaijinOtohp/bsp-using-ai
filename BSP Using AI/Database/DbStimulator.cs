using BSP_Using_AI.Database;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace BSP_Using_AI
{
    class DbStimulator
    {
        DbStimulatorReportHolder _recordsDbStimulatorReportHolder;

        SQLiteConnection _cnn;

        public DbStimulator()
        {
            // Create database file if not exists
            if (!File.Exists("data.db")) SQLiteConnection.CreateFile("data.db");

            // String to open connection to database 
            string connetionString = "Data Source=data.db;Version=3;";

            // Insert credentials for opening database
            _cnn = new SQLiteConnection(connetionString);
            // Open database
            _cnn.Open();

            // Create tables if not exist
            createTable("models", new string[] { "_id integer PRIMARY KEY", "type_name text NOT NULL", "model_target text NOT NULL",
                                    "the_model blob", "dataset_size integer NOT NULL"}, _cnn);
            createTable("dataset", new string[] { "_id integer PRIMARY KEY", "sginal_name text NOT NULL", "starting_index integer NOT NULL",
                                    "signal blob NOT NULL", "sampling_rate integer NOT NULL", "quantisation_step integer NOT NULL",
                                    "features blob NOT NULL" }, _cnn);
            createTable("anno_ds", new string[] { "_id integer PRIMARY KEY", "sginal_name text NOT NULL", "starting_index integer NOT NULL",
                                    "signal_data blob NOT NULL", "sampling_rate integer NOT NULL", "quantisation_step integer NOT NULL",
                                    "anno_objective text NOT NULL", "anno_data blob NOT NULL" }, _cnn);
            createTable("cwd_rl_dataset", new string[] { "_id integer PRIMARY KEY", "sginal_data_key text NOT NULL", "training_data blob NOT NULL" }, _cnn);
        }

        /**
         * Initialize for querying records
         */
        ///<summary>
        ///For querying for records
        ///</summary>
        public DataTable Query(String table, String[] projection, String selection, Object[] selectionArgs, String sortOrder, String callingClassName)
        {
            // Qyery for data and insert it in dataTable
            DataTable dataTable = (DataTable)selectRecordByArgs(table, projection, selection, selectionArgs, sortOrder, false, _cnn);
            dataTable.TableName = table;
            // Return the dataTable to the querying class if it exists
            _recordsDbStimulatorReportHolder?.holdRecordReport(dataTable, callingClassName);

            return dataTable;
        }

        /**
         * Initialize for creating new record
         */
        ///<summary>
        ///For creating new record
        ///</summary>
        public long Insert(String table, String[] columns, Object[] values, String callingClassName)
        {
            // Insert new row and return its id
            return createRecord(table, columns, values, _cnn);
        }

        /**
         * Initialize for updating record
         */
        ///<summary>
        ///For updating a record
        ///</summary>
        public long Update(String table, String[] columns, Object[] values, long rowId, String callingClassName)
        {
            // Update the row and return its id
            return updateRecord(table, columns, values, rowId, _cnn);
        }

        /**
         * Initialize for updating records
         */
        ///<summary>
        ///For updating multiple records
        ///</summary>
        public long UpdateByArgs(String table, String[] columns, Object[] values, String selection, Object[] selectionArgs, String callingClassName)
        {
            // Update the row and return its id
            return updateMultipleRecords(table, columns, values, selection, selectionArgs, _cnn);
        }

        /**
         * Initialize for deleting record
         */
        ///<summary>
        ///For deleting a record
        ///</summary>
        public void Delete(String table, String selection, Object[] selectionArgs, String callingClassName)
        {
            deleteRecordByArgs(table, selection, selectionArgs, _cnn);
        }

        /**
         * Initialize for droping table
         */
        ///<summary>
        ///For droping a table
        ///</summary>
        public void DropTable(String table, String callingClassName)
        {
            dropTable(table, _cnn);
        }

        /**
         * Creating table
         */
        private void createTable(String tableName, String[] columnsNames, SQLiteConnection cnn)
        {
            String createTableSql = "CREATE TABLE IF NOT EXISTS " + tableName + " (";
            foreach (String column in columnsNames)
            {
                createTableSql += column + ",";
            }
            createTableSql = createTableSql.Remove(createTableSql.Length - 1, 1);
            createTableSql += ");";

            SQLiteCommand command = new SQLiteCommand(createTableSql, cnn);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /**
         * Insert column to existing table
         */
        private void insertColumn(String columnName, String dataType, String tableName, SQLiteConnection cnn)
        {
            String alterTableSql = "ALTER TABLE " + tableName + " ADD COLUMN " + columnName + " " + dataType;

            SQLiteCommand command = new SQLiteCommand(alterTableSql, cnn);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /**
         * Drop column from existing table
         */
        private void dropColumn(String columnName, String tableName, SQLiteConnection cnn)
        {
            String alterTableSql = "ALTER TABLE " + tableName + " DROP COLUMN " + columnName;

            SQLiteCommand command = new SQLiteCommand(alterTableSql, cnn);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /**
         * Selecting record by args
         * with either DataReader
         * or DataAdapter
         */
        private object selectRecordByArgs(String table, String[] projection, String selection, Object[] selectionArgs, String sortOrder, bool keepConnectionToDb, SQLiteConnection cnn)
        {
            SQLiteCommand command;

            // Create the command string
            String selectRecordSql = "SELECT ";
            selectRecordSql += projection[0];
            for (int i = 1; i < projection.Length; i++)
            {
                selectRecordSql += ", " + projection[i];
            }
            if (selection == null)
                selectRecordSql += " FROM " + table;
            else
                selectRecordSql += " FROM " + table + " WHERE ";
            int startingIndex = 0;
            if (selectionArgs != null)
            {
                for (int i = 0; i < selectionArgs.Length; i++)
                {
                    selectRecordSql += selection.Substring(startingIndex, selection.IndexOf("?", selection.IndexOf("?", startingIndex)) - startingIndex);
                    //selectRecordSql += "@" + (selectionArgs[i].ToString().Replace(" ", String.Empty));
                    selectRecordSql += "@" + i;
                    startingIndex = selection.IndexOf("?", startingIndex) + 1;
                }
                selectRecordSql += selection.Substring(startingIndex, selection.Length - startingIndex);
            }
            selectRecordSql += " " + sortOrder;

            // Create the command with its string
            command = new SQLiteCommand(selectRecordSql, cnn);

            // Insert values into their places
            if (selectionArgs != null)
                for (int i = 0; i < selectionArgs.Length; i++)
                {
                    //command.Parameters.AddWithValue("@" + (selectionArgs[i].ToString().Replace(" ", String.Empty)), selectionArgs[i]);
                    command.Parameters.AddWithValue("@" + i, selectionArgs[i]);
                }

            // Execute the command
            if (keepConnectionToDb)
                return selectRecordByArgs_Reader(command);
            else
                return selectRecordByArgs_Adapter(command);
        }

        /**
         * DataReader keeps the connection with database open.
         * The database will not be available until DataReader is closed.
         */
        private SQLiteDataReader selectRecordByArgs_Reader(SQLiteCommand command)
        {
            // Execute the command
            SQLiteDataReader dataReader = command.ExecuteReader();

            command.Dispose();

            // Return data reader
            return dataReader;
        }

        /**
         * DataAdapter fills a DataTable and closes the connection to the database
         * DataTable keeps the queried data while the database is free
         */
        private DataTable selectRecordByArgs_Adapter(SQLiteCommand command)
        {
            // Create DataAdapter
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
            // Create DataTable and fill it with data using DataAdapter
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            command.Dispose();

            // Return dataTable
            return dataTable;
        }

        /**
         * Creating record
         */
        private long createRecord(String table, String[] columns, Object[] values, SQLiteConnection cnn)
        {
            SQLiteCommand command;

            // Create the command string
            String createRecordSql = "INSERT INTO " + table + "(";
            createRecordSql += columns[0];
            for (int i = 1; i < columns.Length; i++)
            {
                createRecordSql += ", " + columns[i];
            }
            createRecordSql += ") VALUES(";
            //createRecordSql += "@" + (columns[0].ToString().Replace(" ", String.Empty));
            createRecordSql += "@" + 0;
            for (int i = 1; i < columns.Length; i++)
            {
                //createRecordSql += ", @" + (columns[i].ToString().Replace(" ", String.Empty));
                createRecordSql += ", @" + i;
            }
            createRecordSql += ")";

            // Create the command with its string
            command = new SQLiteCommand(createRecordSql, cnn);

            // Insert values into their places
            for (int i = 0; i < columns.Length; i++)
            {
                //command.Parameters.AddWithValue("@" + (columns[i].ToString().Replace(" ", String.Empty)), values[i]);
                command.Parameters.AddWithValue("@" + i, values[i]);
            }

            // Execute the command
            long lastRowAdded = -1;
            try
            {
                command.ExecuteNonQuery();
                lastRowAdded = command.Connection.LastInsertRowId;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }

            command.Dispose();

            // Return last added row
            return lastRowAdded;
        }

        /**
         * Updating record
         */
        private long updateRecord(String table, String[] columns, Object[] values, long rowId, SQLiteConnection cnn)
        {
            SQLiteCommand command;

            // Create the command string
            String updateRecordSql = "UPDATE " + table + " SET ";
            //updateRecordSql += columns[0] + " = @" + (columns[0].ToString().Replace(" ", String.Empty));
            updateRecordSql += columns[0] + " = @" + 0;
            for (int i = 1; i < columns.Length; i++)
            {
                //updateRecordSql += ", " + columns[i] + " = @" + (columns[i].ToString().Replace(" ", String.Empty));
                updateRecordSql += ", " + columns[i] + " = @" + i;
            }
            updateRecordSql += " WHERE _id = " + rowId.ToString();

            // Create the command with its string
            command = new SQLiteCommand(updateRecordSql, cnn);

            // Insert values into their places
            for (int i = 0; i < columns.Length; i++)
            {
                //command.Parameters.AddWithValue("@" + (columns[i].ToString().Replace(" ", String.Empty)), values[i]);
                command.Parameters.AddWithValue("@" + i, values[i]);
            }

            // Execute the command
            int updatedRow = -1;
            try
            {
                updatedRow = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }

            command.Dispose();

            // Return updated row
            return updatedRow;
        }

        /**
         * Updating multiple records
         */
        private int updateMultipleRecords(String table, String[] columns, Object[] values, String selection, Object[] selectionArgs, SQLiteConnection cnn)
        {
            SQLiteCommand command;

            // Create the command string
            String updateRecordSql = "UPDATE " + table + " SET ";
            //updateRecordSql += columns[0] + " = @" + (columns[0].ToString().Replace(" ", String.Empty));
            updateRecordSql += columns[0] + " = @" + 0;
            for (int i = 1; i < columns.Length; i++)
            {
                //updateRecordSql += ", " + columns[i] + " = @" + (columns[i].ToString().Replace(" ", String.Empty));
                updateRecordSql += ", " + columns[i] + " = @" + i;
            }

            if (selection != null)
                updateRecordSql += " WHERE ";
            int startingIndex = 0;
            if (selectionArgs != null)
                for (int i = 0; i < selectionArgs.Length; i++)
                {
                    updateRecordSql += selection.Substring(startingIndex, selection.IndexOf("?", selection.IndexOf("?", startingIndex)) - startingIndex);
                    //updateRecordSql += "@" + (selectionArgs[i].ToString().Replace(" ", String.Empty));
                    updateRecordSql += "@" + columns.Length + i;
                    startingIndex = selection.IndexOf("?", startingIndex) + 1;
                }

            // Create the command with its string
            command = new SQLiteCommand(updateRecordSql, cnn);

            // Insert values into their places
            if (selectionArgs != null)
                for (int i = 0; i < selectionArgs.Length; i++)
                {
                    //command.Parameters.AddWithValue("@" + (selectionArgs[i].ToString().Replace(" ", String.Empty)), selectionArgs[i]);
                    command.Parameters.AddWithValue("@" + columns.Length + i, selectionArgs[i]);
                }

            // Insert values into their places
            for (int i = 0; i < columns.Length; i++)
            {
                //command.Parameters.AddWithValue("@" + (columns[i].ToString().Replace(" ", String.Empty)), values[i]);
                command.Parameters.AddWithValue("@" + i, values[i]);
            }

            // Execute the command
            int updatedRow = -1;
            try
            {
                updatedRow = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }

            command.Dispose();

            // Return updated row
            return updatedRow;
        }

        /**
        * Deleting record by args
        */
        private void deleteRecordByArgs(String table, String selection, Object[] selectionArgs, SQLiteConnection cnn)
        {
            SQLiteCommand command;

            // Create the command string
            String deleteRecordSql = "DELETE FROM " + table;

            deleteRecordSql += " WHERE ";
            int startingIndex = 0;
            for (int i = 0; i < selectionArgs.Length; i++)
            {
                deleteRecordSql += selection.Substring(startingIndex, selection.IndexOf("?", selection.IndexOf("?", startingIndex)) - startingIndex);
                //deleteRecordSql += "@" + (selectionArgs[i].ToString().Replace(" ", String.Empty));
                deleteRecordSql += "@" + i;
                startingIndex = selection.IndexOf("?", startingIndex) + 1;
            }

            // Create the command with its string
            command = new SQLiteCommand(deleteRecordSql, cnn);

            // Insert values into their places
            for (int i = 0; i < selectionArgs.Length; i++)
            {
                //command.Parameters.AddWithValue("@" + (selectionArgs[i].ToString().Replace(" ", String.Empty)), selectionArgs[i]);
                command.Parameters.AddWithValue("@" + i, selectionArgs[i]);
            }

            // Execute the command
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }

            command.Dispose();
        }

        /**
        * Drop table
        */
        private void dropTable(String table, SQLiteConnection cnn)
        {
            SQLiteCommand command;

            // Create the command string
            String deleteRecordSql = "DROP TABLE IF EXISTS " + table;

            // Create the command with its string
            command = new SQLiteCommand(deleteRecordSql, cnn);

            // Execute the command
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }

            command.Dispose();
        }

        public void bindToRecordsDbStimulatorReportHolder(DbStimulatorReportHolder recordsDbStimulatorReportHolder)
        {
            _recordsDbStimulatorReportHolder = recordsDbStimulatorReportHolder;
        }
    }
}

