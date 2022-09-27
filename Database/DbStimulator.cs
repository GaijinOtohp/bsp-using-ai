using BSP_Using_AI.Database;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BSP_Using_AI
{
    class DbStimulator
    {
        DbStimulatorReportHolder _recordsDbStimulatorReportHolder;

        SQLiteConnection _cnn;

        bool _insert = false;
        bool _query = false;
        bool _update = false;
        bool _delete = false;
        bool _dropTable = false;

        String _queryingTable;
        String[] _firstArray;
        String _queryinSelection;
        Object[] _secondArray;
        String _queryinSortOrder;
        long _rowId;

        public String _callingClassName;

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
                                    "the_model blob", "selected_variables blob", "outputs_thresholds blob",
                                    "model_path text NOT NULL", "dataset_size integer NOT NULL", "model_updates integer NOT NULL",
                                    "trainings_details blob NOT NULL","validation_data blob"});
            createTable("dataset", new string[] { "_id integer PRIMARY KEY", "sginal_name text NOT NULL", "starting_index integer NOT NULL",
                                    "signal blob NOT NULL", "sampling_rate integer NOT NULL", "features blob NOT NULL" });
        }

        /**
         * Initialize for querying records
         */
        ///<summary>
        ///For querying for records
        ///</summary>
        public void initialize(String table, String[] projection, String selection, Object[] selectionArgs, String sortOrder, String callingClassName)
        {
            _queryingTable = table;
            _firstArray = projection;
            _queryinSelection = selection;
            _secondArray = selectionArgs;
            _queryinSortOrder = sortOrder;

            _query = true;

            _callingClassName = callingClassName;
        }

        /**
         * Initialize for creating new record
         */
        ///<summary>
        ///For creating new record
        ///</summary>
        public void initialize(String table, String[] columns, Object[] values, String callingClassName)
        {
            _queryingTable = table;
            _firstArray = columns;
            _secondArray = values;

            _insert = true;

            _callingClassName = callingClassName;
        }

        /**
         * Initialize for updating record
         */
        ///<summary>
        ///For updating a record
        ///</summary>
        public void initialize(String table, String[] columns, Object[] values, long rowId, String callingClassName)
        {
            _queryingTable = table;
            _firstArray = columns;
            _secondArray = values;
            _rowId = rowId;

            _update = true;

            _callingClassName = callingClassName;
        }

        /**
         * Initialize for deleting record
         */
        ///<summary>
        ///For deleting a record
        ///</summary>
        public void initialize(String table, String selection, Object[] selectionArgs, String callingClassName)
        {
            _queryingTable = table;
            _queryinSelection = selection;
            _secondArray = selectionArgs;

            _delete = true;

            _callingClassName = callingClassName;
        }

        /**
         * Initialize for droping table
         */
        ///<summary>
        ///For droping a table
        ///</summary>
        public void initialize(String table, String callingClassName)
        {
            _queryingTable = table;

            _dropTable = true;

            _callingClassName = callingClassName;
        }

        /**
         * Run Db stimulator
         */
        public void run()
        {
            if (_insert)
                createRecord(_queryingTable, _firstArray, _secondArray);
            else if (_query)
            {
                SQLiteDataReader dataReader = selectRecordByArgs(_queryingTable, _firstArray, _queryinSelection, _secondArray, _queryinSortOrder);

                List<Object[]> records = new List<object[]>();
                while (dataReader.Read())
                {
                    if (_queryingTable.Equals("models"))
                    {
                        if (_callingClassName.Equals("MainFormForModels"))
                            records.Add(new object[] { dataReader.GetString(0), dataReader.GetString(1), dataReader.GetValue(2), dataReader.GetValue(3), dataReader.GetValue(4), dataReader.GetString(5), dataReader.GetInt64(6) });
                        else if (_callingClassName.Equals("AIToolsForm"))
                            records.Add(new object[] { dataReader.GetInt64(0), dataReader.GetString(1), dataReader.GetString(2), dataReader.GetString(3), dataReader.GetInt32(4), dataReader.GetInt32(5), dataReader.GetValue(6) });
                        else if (_callingClassName.Equals("DetailsFormForModels"))
                            records.Add(new object[] { dataReader.GetValue(0), dataReader.GetValue(1) });
                        else if (_callingClassName.Equals("PCADataVis"))
                            records.Add(new object[] { dataReader.GetString(0) });
                    }
                    else if (_queryingTable.Equals("dataset"))
                    {
                        if (_callingClassName.Equals("MainFormForDataset") || _callingClassName.Equals("ModelsFlowLayoutPanelItemUserControl"))
                            records.Add(new object[] { dataReader.GetInt64(0) });
                        else if (_callingClassName.Equals("DatasetExplorerFormForDataset") || _callingClassName.Equals("DetailsFormForDataset"))
                            records.Add(new object[] { dataReader.GetInt64(0), dataReader.GetString(1), dataReader.GetInt32(2), dataReader.GetInt32(3) });
                        else if (_callingClassName.Equals("DatasetFlowLayoutPanelItemUserControl"))
                            records.Add(new object[] { dataReader.GetValue(0), dataReader.GetValue(1) });
                        else if (_callingClassName.Equals("DatasetExplorerFormForFeatures"))
                            records.Add(new object[] { dataReader.GetValue(0) });
                        else if (_callingClassName.Equals("KNNBackThread") || _callingClassName.Equals("DetailsFormForFeatures") || _callingClassName.Equals("DetailsFormForThresholds") || _callingClassName.Equals("ValidationFlowLayoutPanelUserControl"))
                            records.Add(new object[] { dataReader.GetValue(0) });
                    }
                }
                dataReader.Close();

                _recordsDbStimulatorReportHolder.holdRecordReport(records, _callingClassName);
            }
            else if (_update)
                updateRecord(_queryingTable, _firstArray, _secondArray, _rowId);
            else if (_delete)
                deleteRecordByArgs(_queryingTable, _queryinSelection, _secondArray);
            else if (_dropTable)
                dropTable(_queryingTable);
        }

        /**
         * Creating table
         */
        private void createTable(String tableName, String[] columnsNames)
        {
            String createTableSql = "CREATE TABLE IF NOT EXISTS " + tableName + " (";
            foreach (String column in columnsNames)
            {
                createTableSql += column + ",";
            }
            createTableSql = createTableSql.Remove(createTableSql.Length - 1, 1);
            createTableSql += ");";

            SQLiteCommand command = new SQLiteCommand(createTableSql, _cnn);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /**
         * Insert column to existing table
         */
        private void insertColumn(String columnName, String dataType, String tableName)
        {
            String alterTableSql = "ALTER TABLE " + tableName + " ADD COLUMN " + columnName + " " + dataType;

            SQLiteCommand command = new SQLiteCommand(alterTableSql, _cnn);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /**
         * Drop column from existing table
         */
        private void dropColumn(String columnName, String tableName)
        {
            String alterTableSql = "ALTER TABLE " + tableName + " DROP COLUMN " + columnName;

            SQLiteCommand command = new SQLiteCommand(alterTableSql, _cnn);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        /**
         * Selecting record by args
         */
        private SQLiteDataReader selectRecordByArgs(String table, String[] projection, String selection, Object[] selectionArgs, String sortOrder)
        {
            SQLiteCommand command;
            SQLiteDataReader dataReader;

            // Create the command string
            String selectRecordSql = "SELECT ";
            selectRecordSql += projection[0] + " ";
            for (int i = 1; i < projection.Length; i++)
            {
                selectRecordSql += "," + projection[i] + " ";
            }
            if (selection == null)
                selectRecordSql += "FROM " + table;
            else
                selectRecordSql += "FROM " + table + " WHERE ";
            int startingIndex = 0;
            if (selectionArgs != null)
                for (int i = 0; i < selectionArgs.Length; i++)
                {
                    selectRecordSql += selection.Substring(startingIndex, selection.IndexOf("?", selection.IndexOf("?", startingIndex)) - startingIndex);
                    selectRecordSql += "@" + (selectionArgs[i].ToString().Replace(" ", String.Empty));
                    startingIndex = selection.IndexOf("?", startingIndex) + 1;
                }
            selectRecordSql += " " + sortOrder;

            // Create the command with its string
            command = new SQLiteCommand(selectRecordSql, _cnn);

            // Insert values into their places
            if (selectionArgs != null)
                for (int i = 0; i < selectionArgs.Length; i++)
                {
                    command.Parameters.AddWithValue("@" + (selectionArgs[i].ToString().Replace(" ", String.Empty)), selectionArgs[i]);
                }

            // Execute the command
            dataReader = command.ExecuteReader();

            command.Dispose();

            // Return data reader
            return dataReader;
        }

        /**
         * Creating record
         */
        private int createRecord(String table, String[] columns, Object[] values)
        {
            SQLiteCommand command;

            // Create the command string
            String createRecordSql = "INSERT INTO " + table + "(";
            createRecordSql += columns[0] + " ";
            for (int i = 1; i < columns.Length; i++)
            {
                createRecordSql += "," + columns[i] + " ";
            }
            createRecordSql += ") VALUES(";
            createRecordSql += "@" + (columns[0].ToString().Replace(" ", String.Empty)) + " ";
            for (int i = 1; i < columns.Length; i++)
            {
                createRecordSql += ",@" + (columns[i].ToString().Replace(" ", String.Empty)) + " ";
            }
            createRecordSql += ")";

            // Create the command with its string
            command = new SQLiteCommand(createRecordSql, _cnn);

            // Insert values into their places
            for (int i = 0; i < columns.Length; i++)
            {
                command.Parameters.AddWithValue("@" + (columns[i].ToString().Replace(" ", String.Empty)), values[i]);
            }

            // Execute the command
            int lastRowAdded = -1;
            try
            {
                lastRowAdded = command.ExecuteNonQuery();
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
        private int updateRecord(String table, String[] columns, Object[] values, long rowId)
        {
            SQLiteCommand command;

            // Create the command string
            String updateRecordSql = "UPDATE " + table + " SET ";
            updateRecordSql += columns[0] + " = @" + (columns[0].ToString().Replace(" ", String.Empty));
            for (int i = 1; i < columns.Length; i++)
            {
                updateRecordSql += " ," + columns[i] + " = @" + (columns[i].ToString().Replace(" ", String.Empty));
            }
            updateRecordSql += " WHERE _id = " + rowId.ToString();

            // Create the command with its string
            command = new SQLiteCommand(updateRecordSql, _cnn);

            // Insert values into their places
            for (int i = 0; i < columns.Length; i++)
            {
                command.Parameters.AddWithValue("@" + (columns[i].ToString().Replace(" ", String.Empty)), values[i]);
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
        private int updateMultipleRecords(String table, String[] columns, Object[] values, String selection, Object[] selectionArgs)
        {
            SQLiteCommand command;

            // Create the command string
            String updateRecordSql = "UPDATE " + table + " SET ";
            updateRecordSql += columns[0] + " = @" + (columns[0].ToString().Replace(" ", String.Empty));
            for (int i = 1; i < columns.Length; i++)
            {
                updateRecordSql += " ," + columns[i] + " = @" + (columns[i].ToString().Replace(" ", String.Empty));
            }

            if (selection != null)
                updateRecordSql += " WHERE ";
            int startingIndex = 0;
            if (selectionArgs != null)
                for (int i = 0; i < selectionArgs.Length; i++)
                {
                    updateRecordSql += selection.Substring(startingIndex, selection.IndexOf("?", selection.IndexOf("?", startingIndex)) - startingIndex);
                    updateRecordSql += "@" + (selectionArgs[i].ToString().Replace(" ", String.Empty));
                    startingIndex = selection.IndexOf("?", startingIndex) + 1;
                }

            // Create the command with its string
            command = new SQLiteCommand(updateRecordSql, _cnn);

            // Insert values into their places
            if (selectionArgs != null)
                for (int i = 0; i < selectionArgs.Length; i++)
                {
                    command.Parameters.AddWithValue("@" + (selectionArgs[i].ToString().Replace(" ", String.Empty)), selectionArgs[i]);
                }

            // Insert values into their places
            for (int i = 0; i < columns.Length; i++)
            {
                command.Parameters.AddWithValue("@" + (columns[i].ToString().Replace(" ", String.Empty)), values[i]);
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
        private void deleteRecordByArgs(String table, String selection, Object[] selectionArgs)
        {
            SQLiteCommand command;

            // Create the command string
            String deleteRecordSql = "DELETE FROM " + table;

            deleteRecordSql += " WHERE ";
            int startingIndex = 0;
            for (int i = 0; i < selectionArgs.Length; i++)
            {
                deleteRecordSql += selection.Substring(startingIndex, selection.IndexOf("?", selection.IndexOf("?", startingIndex)) - startingIndex);
                deleteRecordSql += "@" + (selectionArgs[i].ToString().Replace(" ", String.Empty));
                startingIndex = selection.IndexOf("?", startingIndex) + 1;
            }

            // Create the command with its string
            command = new SQLiteCommand(deleteRecordSql, _cnn);

            // Insert values into their places
            for (int i = 0; i < selectionArgs.Length; i++)
            {
                command.Parameters.AddWithValue("@" + (selectionArgs[i].ToString().Replace(" ", String.Empty)), selectionArgs[i]);
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
        private void dropTable(String table)
        {
            SQLiteCommand command;

            // Create the command string
            String deleteRecordSql = "DROP TABLE IF EXISTS " + table;

            // Create the command with its string
            command = new SQLiteCommand(deleteRecordSql, _cnn);

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

