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
        private readonly string DatabaseFile = "menus.db";

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

        public string DataBaseFullPath { get; set; }

        public List<Type> Types { get; set; }

        public List<Saison> Saisons { get; set; }

        public DatabaseHandler()
        {
            DataBaseFolder = ApplicationData.Current.LocalFolder;
            DataBaseFullPath = Path.Combine(DataBaseFolder.Path, DatabaseFile);

            // For the moment, hardcode the list of types and saisons
            Types = new List<Type>
            {
                new Type(1, "Entrée"),
                new Type(2, "Plat de résistance"),
                new Type(3, "Soir"),
                new Type(4, "Déssert"),
                new Type(5, "Apéritif")
            };

            Saisons = new List<Saison>
            {
                new Saison(1, "Toute l'année"),
                new Saison(2, "Printemps"),
                new Saison(3, "Eté"),
                new Saison(4, "Automne"),
                new Saison(5, "Hiver"),
            };
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

        private void CreateTypesTable(SQLiteConnection connection)
        {
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "create table types (id integer not null primary key autoincrement unique, nom TEXT);";
            command.ExecuteNonQuery();

            foreach (Type type in Types)
            {
                command.CommandText = "insert into types (id, nom) values (@id, @nom);";
                command.Parameters.Add("@id", System.Data.DbType.Int64).Value = type.Id;
                command.Parameters.Add("@nom", System.Data.DbType.String).Value = type.Name;
                command.ExecuteNonQuery();
            }
        }

        private void CreateSaisonsTable(SQLiteConnection connection)
        {
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE saisons ( id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, nom TEXT );";
            command.ExecuteNonQuery();

            foreach (Saison saison in Saisons)
            {
                command.CommandText = "insert into saisons (id, nom) values (@id, @nom);";
                command.Parameters.Add("@id", System.Data.DbType.Int64).Value = saison.Id;
                command.Parameters.Add("@nom", System.Data.DbType.String).Value = saison.Name;
                command.ExecuteNonQuery();
            }
        }

        private void CreatePlatsTable(SQLiteConnection connection)
        {
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE plats ( id INTEGER PRIMARY KEY AUTOINCREMENT, nom TEXT NOT NULL, type INTEGER NOT NULL, saison INTEGER NOT NULL, temps INTEGER, note INTEGER, ingredients TEXT, description TEXT, FOREIGN KEY(type) REFERENCES types(id), FOREIGN KEY(saison) REFERENCES saisons(id) );";
            command.ExecuteNonQuery();
        }

        private void CreateSemainesTable(SQLiteConnection connection)
        {
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE semaines ( id INTEGER PRIMARY KEY AUTOINCREMENT, date INTEGER, plat_id INTEGER, FOREIGN KEY(plat_id) REFERENCES plats(id) );";
            command.ExecuteNonQuery();
        }

        public Exception CreateDatabase()
        {
            SQLiteConnection connection = Connect();

            try
            {
                CreateTypesTable(connection);
                CreateSaisonsTable(connection);
                CreatePlatsTable(connection);
                CreateSemainesTable(connection);
            }
            catch (Exception ex)
            {
                Disconnect(connection);
                return ex;
            }

            return null;
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
        
        public async Task<List<Plat>> GetPlats()
        {
            List<Plat> plats = null;
            SQLiteConnection connection = Connect();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "select * from plats order by nom asc;";

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
                    
                    string typeName = Types[(int) typeId - 1].Name;
                    Type type = new Type(typeId, typeName);
                    
                    string saisonName = Saisons[(int) saisonId - 1].Name;
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
            command.CommandText = "select id from types where nom = @nom;";
            command.Parameters.Add("@nom", System.Data.DbType.String).Value = name;

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
            command.CommandText = "select id from saisons where nom = @nom;";
            command.Parameters.Add("@nom", System.Data.DbType.String).Value = name;

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


