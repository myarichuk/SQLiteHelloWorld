using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Net;

namespace SQLiteHelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            if(File.Exists("HelloWorld.s3db")) //just to allow repeatable runs of this demo
                File.Delete("HelloWorld.s3db");

            using (var connection = new SQLiteConnection("Data Source=HelloWorld.s3db;Version=3;New=True;"))
            {
                connection.Open();

                var commandText = 
                    @"CREATE TABLE names
                       (key INTEGER NOT NULL,
                       FirstName TEXT,
                       LastName TEXT,
                       Email TEXT,
                       PRIMARY KEY (key),
                       UNIQUE (Email));

                       INSERT INTO names (FirstName, LastName, Email) VALUES ('John', 'Dow', 'john.dow@foobar.notrealemail');
                       INSERT INTO names (FirstName, LastName, Email) VALUES ('Jane', 'Dow', 'jane.dow@foobar.alsonotrealemail');
                       INSERT INTO names (FirstName, LastName, Email) VALUES ('Jack', 'Dow', 'jack.dow@barfoo.alsonotrealemail');
                   ";

                var createTableAndPopulateCommand = new SQLiteCommand(commandText, connection);
                createTableAndPopulateCommand.ExecuteNonQuery();

                //disposing stuff properly is important
                using (var dataSet = new DataSet())
                {
                    var dataAdapter = new SQLiteDataAdapter("SELECT * FROM names", connection);
                    dataAdapter.Fill(dataSet);
                    var dataTable = dataSet.Tables[0];

                    for (var i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var dataRow = dataTable.Rows[i];

                        if (dataRow.RowState != DataRowState.Deleted)
                        {
                            Console.WriteLine($"{dataRow["FirstName"]} {dataRow["LastName"]} ({dataRow["Email"]})");
                        }
                    }
                }

                connection.Close();
            }
        }
    }
}
