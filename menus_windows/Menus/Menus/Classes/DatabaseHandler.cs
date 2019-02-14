using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Menus
{
    public class DatabaseHandler
    {
        private string DatabaseFile = "menus.db";
        private string DataBaseFullPath;

        private List<Type> types = null;
        private List<Saison> saisons = null;

        private static DatabaseHandler instance = null;
        private static readonly object padlock = new object();

        public static DatabaseHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DatabaseHandler();
                    }
                    return instance;
                }
            }
        }

        public StorageFolder DataBaseFolder { get; set; }

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

        public async Task<bool> InsertPlat(Plat plat)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "insert into plats (nom, type, saison, temps, note, ingredients, description) values (@nom, @type, @saison, @temps, @note, @ingredients, @description);";

            command.Parameters.Add("@nom", System.Data.DbType.String).Value = plat.nom;
            command.Parameters.Add("@type", System.Data.DbType.Int64).Value = plat.type.Id;
            command.Parameters.Add("@saison", System.Data.DbType.Int64).Value = plat.saison.Id;
            command.Parameters.Add("@temps", System.Data.DbType.Int32).Value = plat.temps;
            command.Parameters.Add("@note", System.Data.DbType.Int32).Value = plat.note;
            command.Parameters.Add("@ingredients", System.Data.DbType.String).Value = plat.ingredients;
            command.Parameters.Add("@description", System.Data.DbType.String).Value = plat.description;

            bool result = false;
            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                    command = new SQLiteCommand("select last_insert_rowid();", connection);
                    long LastRowId = (long)command.ExecuteScalar();
                    plat.id = LastRowId;
                    result = true;
                }
                
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de l'ajout du plat.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return result;
        }

        public async Task<List<Type>> GetTypes()
        {
            // Return the type list if it has already been retreived previously
            if (types != null)
                return types;

            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "select * from types order by id asc";

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
                await new Alert("Erreur lors de la récupération des types de plat.", e.Message, e.StackTrace).ShowAsync();
                types = null;
            }

            Disconnect(connection);
            return types;
        }
        
        public async Task<List<Saison>> GetSaisons()
        {
            if (saisons != null)
                return saisons;

            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "select * from saisons order by id asc";

            try
            {
                SQLiteDataReader sQLiteDataReader = command.ExecuteReader();
                saisons = new List<Saison>();

                while (sQLiteDataReader.Read())
                {
                    long id = sQLiteDataReader.GetInt64(0);
                    string nom = sQLiteDataReader.GetString(1);
                    saisons.Add(new Saison(id, nom));
                }
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de la récupération des saisons.", e.Message, e.StackTrace).ShowAsync();
                saisons = null;
            }

            Disconnect(connection);
            return saisons;
        }
        
        public async Task<List<Plat>> GetPlats()
        {
            List<Plat> plats = null;
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "select * from plats;";

            try
            {
                SQLiteDataReader sQLiteDataReader = command.ExecuteReader();
                plats = new List<Plat>();

                while (sQLiteDataReader.Read())
                {
                    long id = sQLiteDataReader.GetInt64(0);
                    string nom = sQLiteDataReader.GetString(1);
                    long typeId = sQLiteDataReader.GetInt64(2);
                    long saisonId = sQLiteDataReader.GetInt64(3);
                    int temps = sQLiteDataReader.GetInt32(4);
                    int note = sQLiteDataReader.GetInt32(5);
                    string ingredients = sQLiteDataReader.GetString(6);
                    string description = sQLiteDataReader.GetString(7);

                    List<Type> types = await GetTypes();
                    string typeName = types[(int) typeId - 1].Name;
                    Type type = new Type(typeId, typeName);

                    List<Saison> saisons = await GetSaisons();
                    string saisonName = saisons[(int) saisonId - 1].Name;
                    Saison saison = new Saison(saisonId, saisonName);

                    plats.Add(new Plat(id, nom, type, saison, temps, note, ingredients, description));
                }
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de la récupération des plats.", e.Message, e.StackTrace).ShowAsync();
                plats = null;
            }

            Disconnect(connection);
            return plats;
        }

        public async Task<long> GetTypeIdForTypeName(string name)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "select id from types where name = @name;";
            command.Parameters.Add("@name", System.Data.DbType.String).Value = name;

            long id = 0;
            try
            {
               id = (long) command.ExecuteScalar();
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de la récupération du type de plat.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return id;
        }

        public async Task<long> GetSaisonIdForSaisonName(string name)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "select id from saisons where name = @name;";
            command.Parameters.Add("@name", System.Data.DbType.String).Value = name;

            long id = 0;
            try
            {
                id = (long)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de la récupération de l'ID de la saison.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return id;
        }

        public async Task<string> GetSaisonNameForSaisonId(long id)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "select nom from saisons where id = @id;";
            command.Parameters.Add("@id", System.Data.DbType.Int64).Value = id;

            string name = null;
            try
            {
                name = (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de la récupération de la saison.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return name;
        }

        public async Task<int> DeletePlatById(long id)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "delete from plats where id = @id;";
            command.Parameters.Add("@id", System.Data.DbType.Int64).Value = id;

            int res = 0;
            try
            {
                res = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de la suppression du plat.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return res;
        }

        public async Task<int> EditPlat(Plat plat)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "update plats set nom = @nom, type = @type, saison = @saison, temps = @temps, note = @note, ingredients = @ingredients, description = @description where id = @id;";

            command.Parameters.Add("@id", System.Data.DbType.Int64).Value = plat.id;
            command.Parameters.Add("@nom", System.Data.DbType.String).Value = plat.nom;
            command.Parameters.Add("@type", System.Data.DbType.Int64).Value = plat.type.Id;
            command.Parameters.Add("@saison", System.Data.DbType.Int64).Value = plat.saison.Id;
            command.Parameters.Add("@temps", System.Data.DbType.Int32).Value = plat.temps;
            command.Parameters.Add("@note", System.Data.DbType.Int32).Value = plat.note;
            command.Parameters.Add("@ingredients", System.Data.DbType.String).Value = plat.ingredients;
            command.Parameters.Add("@description", System.Data.DbType.String).Value = plat.description;

            int res = 0;
            try
            {
                res = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de l'édition du plat.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return res;
        }

        public async Task<bool> InsertPlatInSemaines(Plat plat, DateTime date)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "insert or replace into semaines (id, date, plat_id) values " +
                "((select semaines.id from semaines inner join plats on semaines.plat_id = plats.id where date = @date and type = @type_id), @date, @plat_id);";

            command.Parameters.Add("@date", System.Data.DbType.Int64).Value = date.ToMyUnixTimeSeconds();
            command.Parameters.Add("@plat_id", System.Data.DbType.Int64).Value = plat.id;
            command.Parameters.Add("@type_id", System.Data.DbType.Int64).Value = plat.type.Id;

            bool result = false;
            try
            {
                if (command.ExecuteNonQuery() == 1)
                    result = true;
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de l'ajout du repas.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return result;
        }

        public async Task<Dictionary<DateTime, Dictionary<long, long>>> GetPlatIdsForStartOfWeek(DateTime date)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "select plat_id, date, type from semaines inner join plats on semaines.plat_id = plats.id where date between @date1 and @date2;";

            command.Parameters.Add("@date1", System.Data.DbType.Int64).Value = date.ToMyUnixTimeSeconds();
            command.Parameters.Add("@date2", System.Data.DbType.Int64).Value = date.AddDays(7).ToMyUnixTimeSeconds();

            Dictionary<DateTime, Dictionary<long, long>> plats = null;
            try
            {
                SQLiteDataReader sQLiteDataReader = command.ExecuteReader();
                plats = new Dictionary<DateTime, Dictionary<long, long>>();
                while (sQLiteDataReader.Read())
                {
                    long plat_id = sQLiteDataReader.GetInt64(0);
                    DateTime _date = DateTimeExtensions.FromMyUnixTimeSeconds(sQLiteDataReader.GetInt64(1));
                    long type_id = sQLiteDataReader.GetInt64(2);

                    if (!plats.ContainsKey(_date))
                    {
                        plats[_date] = new Dictionary<long, long>();
                    }
                    plats[_date][type_id] = plat_id;
                }
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de la récupération des plats pour la semaine du " + date.ToLongDateString() + ".", e.Message, e.StackTrace).ShowAsync();
                plats = null;
            }

            Disconnect(connection);
            return plats;
        }

        public async Task<bool> DeletePlatInSemaine(Plat plat, DateTime date)
        {
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "delete from semaines where plat_id = @plat_id and date = @date;";

            command.Parameters.Add("@plat_id", System.Data.DbType.Int64).Value = plat.id;
            command.Parameters.Add("@date", System.Data.DbType.Int64).Value = date.ToMyUnixTimeSeconds();

            bool result = false;
            try
            {
                if (command.ExecuteNonQuery() == 1)
                    result = true;
            }
            catch (Exception e)
            {
                await new Alert("Erreur lors de la suppression du repas.", e.Message, e.StackTrace).ShowAsync();
            }

            Disconnect(connection);
            return result;
        }
    }
}


