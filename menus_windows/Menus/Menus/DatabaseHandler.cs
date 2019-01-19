using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

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

        public void InsertPlat(Plat plat)
        {
            SQLiteConnection connection = Connect();

            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "INSERT INTO plats (nom, type, saison, temps, note, ingredients, description) VALUES (@nom, @type, @saison, @temps, @note, @ingredients, @description);";

            command.Parameters.Add("@nom", System.Data.DbType.String).Value = plat.nom;
            command.Parameters.Add("@type", System.Data.DbType.Int64).Value = plat.type;
            command.Parameters.Add("@saison", System.Data.DbType.Int64).Value = plat.saison;
            command.Parameters.Add("@temps", System.Data.DbType.Int32).Value = plat.temps;
            command.Parameters.Add("@note", System.Data.DbType.Int32).Value = plat.note;
            command.Parameters.Add("@ingredients", System.Data.DbType.String).Value = plat.ingredients;
            command.Parameters.Add("@description", System.Data.DbType.String).Value = plat.description;

            try
            {
                command.ExecuteNonQuery();
                command = new SQLiteCommand("select last_insert_rowid();", connection);
                long LastRowId = (long) command.ExecuteScalar();
                plat.id = LastRowId;
            }
            catch (Exception e)
            {
                new Alert("Erreur lors de l'ajout du plat.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
        }

        public List<Type> GetTypes()
        {
            List<Type> types = null;
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM types ORDER BY id ASC";

            try
            {
                SQLiteDataReader sQLiteDataReader = command.ExecuteReader();
                types = new List<Type>();

                while (sQLiteDataReader.Read())
                {
                    long id = sQLiteDataReader.GetInt64(0);
                    string nom = sQLiteDataReader.GetString(1);
                    types.Add(new Type(id, nom));
                }
            }
            catch (Exception e)
            {
                new Alert("Erreur lors de la récupération des types de plat.", e.Message, e.StackTrace).ShowAsync();
                types = null;
            }

            Disconnect(connection);
            return types;
        }
        
        public List<Saison> GetSaisons()
        {
            List<Saison> types = null;
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM saisons ORDER BY id ASC";

            try
            {
                SQLiteDataReader sQLiteDataReader = command.ExecuteReader();
                types = new List<Saison>();

                while (sQLiteDataReader.Read())
                {
                    long id = sQLiteDataReader.GetInt64(0);
                    string nom = sQLiteDataReader.GetString(1);
                    types.Add(new Saison(id, nom));
                }
            }
            catch (Exception e)
            {
                new Alert("Erreur lors de la récupération des types de plat.", e.Message, e.StackTrace).ShowAsync();
                types = null;
            }

            Disconnect(connection);
            return types;
        }


        public List<Plat> GetPlats()
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
                    long type = sQLiteDataReader.GetInt64(2);
                    long saison = sQLiteDataReader.GetInt64(3);
                    int temps = sQLiteDataReader.GetInt32(4);
                    int note = sQLiteDataReader.GetInt32(5);
                    string ingredients = sQLiteDataReader.GetString(6);
                    string description = sQLiteDataReader.GetString(7);
                    plats.Add(new Plat(id, nom, type, saison, temps, note, ingredients, description));
                }
            }
            catch (Exception e)
            {
                new Alert("Erreur lors de la récupération des plats.", e.Message, e.StackTrace).ShowAsync();
                plats = null;
            }

            Disconnect(connection);

            return plats;
        }

        public Windows.Storage.StorageFolder GetDataBaseFolder()
        {
            return this.DataBaseFolder;
        }

        public long GetIdForTypeName(string name)
        {
            long id = 0;
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT id FROM types where name = @name;";
            command.Parameters.Add("@name", System.Data.DbType.String).Value = name;

            try
            {
               id = (long) command.ExecuteScalar();
            }
            catch (Exception e)
            {
                new Alert("Erreur lors de la récupération des types de plat.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return id;
        }
    }
}
