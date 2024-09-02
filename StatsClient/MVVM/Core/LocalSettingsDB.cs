using System;
using System.Data.SQLite;
using System.IO;

namespace StatsClient.MVVM.Core
{
    public class LocalSettingsDB
    {
        const string DataBaseFileName = "Settings.Config24";
        public static string DataBaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Stats_Client\\";
        static string DataBasePath = DataBaseFolder + DataBaseFileName;

        #region Creating Local Config File
        public static string CreatingLocalConfigFiles()
        {
            Directory.CreateDirectory(DataBaseFolder);

            if (!File.Exists(DataBasePath))
                SQLiteConnection.CreateFile(DataBasePath);

            try
            {
                using SQLiteConnection m_dbConnection = new ("Data Source=" + DataBasePath + ";Version=3;");
                m_dbConnection.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS main.Settings (
                                 Name   TEXT PRIMARY KEY, 
                                Value   TEXT
                               ) WITHOUT ROWID;";

                SQLiteCommand command = new (sql, m_dbConnection);
                command.ExecuteNonQuery();
            }
            catch
            {
            }

            return "all good";
        }
        #endregion

        #region Write Local Settings with SQLite
        public static string WriteLocalSetting(string KeyName, string Value)
        {
            try
            {
                using SQLiteConnection m_dbConnection = new ("Data Source=" + DataBasePath + ";Version=3;");
                m_dbConnection.Open();

                if (Value == "True" || Value == "False")
                    Value = Value.ToLower();

                string sql = @"INSERT OR REPLACE INTO main.Settings (Name, Value) VALUES ( '" + KeyName + @"', '" + Value + @"' );";

                SQLiteCommand command = new (sql, m_dbConnection);
                command.ExecuteNonQuery();
                return "all good";
            }
            catch
            {
                return "error";
            }
        }
        #endregion

        #region Read Local Settings with SQLite

        public static string ReadLocalSetting(String KeyName)
        {
            if (File.Exists(DataBasePath))
            {
                try
                {
                    using (SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=" + DataBasePath + ";Version=3;"))
                    {
                        m_dbConnection.Open();
                        string sql = @"SELECT Value FROM main.Settings WHERE Name = '" + KeyName + @"'";
                        SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                            while (reader.Read())
                                return (String)reader.GetValue(0);
                    }
                }
                catch
                {
                }
            }
            return "";
        }
        #endregion
    }
}
