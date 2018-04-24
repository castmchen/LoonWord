using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastmRepository
{
    public class SqliteBaseRepository
    {

        private static readonly string connectionStr = "Data Source=Loon.sqlite;Version=3;";
        private static readonly string dbName = "Loon.sqlite";

        SQLiteConnection sqliteConnection = null;

        public void CreateNewDatabase()
        {
            SQLiteConnection.CreateFile(dbName);
        }

        public void ConnectToDatabase()
        {
            sqliteConnection = new SQLiteConnection(connectionStr);
            sqliteConnection.Open();
        }

        public void CreateTable()
        {
            string tocie_sql = "PRAGMA [main].foreign_keys = 'off'; " +
                         "SAVEPOINT[sqlite_expert_apply_design_transaction]; " +
                         "CREATE TABLE[main].[Language_TOEIC] " +
                         "([Id] VARCHAR PRIMARY KEY ON CONFLICT ROLLBACK NOT NULL UNIQUE, " +
                         "[Word] NVARCHAR(200) NOT NULL, " +
                         "[Trascation] NVARCHAR(200) NOT NULL, " +
                         "[Phonetic] NVARCHAR(200), " +
                         "[Voice] NVARCHAR, " +
                         "[Flag] INT, " +
                         "[CreateTime] INT64); " +
                         "RELEASE[sqlite_expert_apply_design_transaction]; " +
                         "PRAGMA[main].foreign_keys = 'on';";

            string cet4_sql = "PRAGMA [main].foreign_keys = 'off'; " +
                         "SAVEPOINT[sqlite_expert_apply_design_transaction]; " +
                         "CREATE TABLE[main].[Language_CET4] " +
                         "([Id] VARCHAR PRIMARY KEY ON CONFLICT ROLLBACK NOT NULL UNIQUE, " +
                         "[Word] NVARCHAR(200) NOT NULL, " +
                         "[Trascation] NVARCHAR(200) NOT NULL, " +
                         "[Phonetic] NVARCHAR(200), " +
                         "[Voice] NVARCHAR, " +
                         "[Flag] INT, " +
                         "[CreateTime] INT64); " +
                         "RELEASE[sqlite_expert_apply_design_transaction]; " +
                         "PRAGMA[main].foreign_keys = 'on';";

            string cet6_sql = "PRAGMA [main].foreign_keys = 'off'; " +
                         "SAVEPOINT[sqlite_expert_apply_design_transaction]; " +
                         "CREATE TABLE[main].[Language_CET6] " +
                         "([Id] VARCHAR PRIMARY KEY ON CONFLICT ROLLBACK NOT NULL UNIQUE, " +
                         "[Word] NVARCHAR(200) NOT NULL, " +
                         "[Trascation] NVARCHAR(200) NOT NULL, " +
                         "[Phonetic] NVARCHAR(200), " +
                         "[Voice] NVARCHAR, " +
                         "[Flag] INT, " +
                         "[CreateTime] INT64); " +
                         "RELEASE[sqlite_expert_apply_design_transaction]; " +
                         "PRAGMA[main].foreign_keys = 'on';";

            string level1 = "PRAGMA [main].foreign_keys = 'off'; " +
                         "SAVEPOINT[sqlite_expert_apply_design_transaction]; " +
                         "CREATE TABLE[main].[Language_Level1] " +
                         "([Id] VARCHAR PRIMARY KEY ON CONFLICT ROLLBACK NOT NULL UNIQUE, " +
                         "[Word] NVARCHAR(200) NOT NULL, " +
                         "[Trascation] NVARCHAR(200) NOT NULL, " +
                         "[Phonetic] NVARCHAR(200), " +
                         "[Voice] NVARCHAR, " +
                         "[Flag] INT, " +
                         "[CreateTime] INT64); " +
                         "RELEASE[sqlite_expert_apply_design_transaction]; " +
                         "PRAGMA[main].foreign_keys = 'on';";

            string level2 = "PRAGMA [main].foreign_keys = 'off'; " +
                         "SAVEPOINT[sqlite_expert_apply_design_transaction]; " +
                         "CREATE TABLE[main].[Language_Level2] " +
                         "([Id] VARCHAR PRIMARY KEY ON CONFLICT ROLLBACK NOT NULL UNIQUE, " +
                         "[Word] NVARCHAR(200) NOT NULL, " +
                         "[Trascation] NVARCHAR(200) NOT NULL, " +
                         "[Phonetic] NVARCHAR(200), " +
                         "[Voice] NVARCHAR, " +
                         "[Flag] INT, " +
                         "[CreateTime] INT64); " +
                         "RELEASE[sqlite_expert_apply_design_transaction]; " +
                         "PRAGMA[main].foreign_keys = 'on';";

            string othersql = "PRAGMA [main].foreign_keys = 'off'; " +
                         "SAVEPOINT[sqlite_expert_apply_design_transaction]; " +
                         "CREATE TABLE[main].[Language_Other] " +
                         "([Id] VARCHAR PRIMARY KEY ON CONFLICT ROLLBACK NOT NULL UNIQUE, " +
                         "[Word] NVARCHAR(200) NOT NULL, " +
                         "[Trascation] NVARCHAR(200) NOT NULL, " +
                         "[Phonetic] NVARCHAR(200), " +
                         "[Voice] NVARCHAR, " +
                         "[Flag] INT, " +
                         "[CreateTime] INT64); " +
                         "RELEASE[sqlite_expert_apply_design_transaction]; " +
                         "PRAGMA[main].foreign_keys = 'on';";

            string customsql = "PRAGMA [main].foreign_keys = 'off'; " +
                         "SAVEPOINT[sqlite_expert_apply_design_transaction]; " +
                         "CREATE TABLE[main].[Language_Custom] " +
                         "([Id] VARCHAR PRIMARY KEY ON CONFLICT ROLLBACK NOT NULL UNIQUE, " +
                         "[Word] NVARCHAR(200) NOT NULL, " +
                         "[Trascation] NVARCHAR(200) NOT NULL, " +
                         "[Phonetic] NVARCHAR(200), " +
                         "[Voice] NVARCHAR, " +
                         "[Flag] INT, " +
                         "[CreateTime] INT64); " +
                         "RELEASE[sqlite_expert_apply_design_transaction]; " +
                         "PRAGMA[main].foreign_keys = 'on';";

            var sql = tocie_sql + cet4_sql + cet6_sql + level1 + level2 + othersql + customsql;
            SQLiteCommand command = new SQLiteCommand(sql, sqliteConnection);
                       command.ExecuteNonQuery();
        }
    }
}
