using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;

namespace Menus
{
    public class DatabaseHandler
    {
        Windows.Storage.StorageFolder DataBaseFolder;
        private string DatabaseFile = "menus.db";
        private string DataBaseFullPath;

        public DatabaseHandler()
        {
            DataBaseFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            DataBaseFullPath = Path.Combine(DataBaseFolder.Path, DatabaseFile);
        }

        private SQLiteConnection Connect()
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source=" + DataBaseFullPath + "; Version=3");

            connection.Open();

            return connection;
        }

        private void Disconnect(SQLiteConnection connection)
        {
            connection.Close();
        }

        public Exception InsertPlat(Plat plat)
        {
            SQLiteConnection connection = Connect();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "INSERT INTO plats (nom, type, saison, temps, note, ingredients, description) VALUES (@nom, @type, @saison, @temps, @note, @ingredients, @description);";

            command.Parameters.Add("@nom", System.Data.DbType.String).Value = plat.nom;
            command.Parameters.Add("@type", System.Data.DbType.String).Value = plat.type;
            command.Parameters.Add("@saison", System.Data.DbType.String).Value = plat.saison;
            command.Parameters.Add("@temps", System.Data.DbType.VarNumeric).Value = plat.temps;
            command.Parameters.Add("@note", System.Data.DbType.VarNumeric).Value = plat.note;
            command.Parameters.Add("@ingredients", System.Data.DbType.String).Value = plat.ingredients;
            command.Parameters.Add("@description", System.Data.DbType.String).Value = plat.description;

            Exception e = null;
            try
            {
                command.ExecuteNonQuery();
                command = new SQLiteCommand("select last_insert_rowid();", connection);
                long LastRowId = (long)command.ExecuteScalar();
                plat.id = LastRowId;
            }
            catch (Exception ex)
            {
                e = ex;
            }

            Disconnect(connection);

            return e;
        }

        public List<string> GetTypesPlat()
        {
            List<string> types = null;//new List<string>();

            SQLiteConnection connection = Connect();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT DISTINCT type FROM plats;";

            try
            {
                SQLiteDataReader sQLiteDataReader = command.ExecuteReader();
                types = new List<string>();

                while (sQLiteDataReader.Read()) // Read() returns true if there is still a result line to read
                {
                    string type = sQLiteDataReader.GetString(0);
                    types.Add(type);
                }
            }
            catch (Exception ex)
            {
                types = null;
            }

            Disconnect(connection);

            return types;
        }

        public List<Plat> GetAllPlat()
        {
            List<Plat> plats = null;

            SQLiteConnection connection = Connect();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM plats;";

            try
            {
                SQLiteDataReader sQLiteDataReader = command.ExecuteReader();
                plats = new List<Plat>();

                while (sQLiteDataReader.Read())
                {
                    long id = sQLiteDataReader.GetInt64(0);
                    string nom = sQLiteDataReader.GetString(1);
                    string type = sQLiteDataReader.GetString(2);
                    plats.Add(new Plat(id, nom, type, null, TimeSpan.Zero, 0, null, null));
                }
            }
            catch (Exception ex)
            {
                plats = null;
            }

            Disconnect(connection);

            return plats;
        }

        public Windows.Storage.StorageFolder GetDataBaseFolder()
        {
            return this.DataBaseFolder;
        }
    }
}
