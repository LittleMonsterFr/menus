package code.menu.dao;

import java.io.File;
import java.sql.*;
import java.util.ArrayList;

public class DatabaseHandler {
    private static DatabaseHandler singleInstance = null;
    private static final String DATABASE_PREFIX = "jdbc:sqlite:";
    private File databaseFile;

    private DatabaseHandler()
    {
        databaseFile = new File(System.getProperty("user.home") + File.separator + "Menu.db");
    }

    public static DatabaseHandler getInstance()
    {
        if (singleInstance == null)
            singleInstance = new DatabaseHandler();

        return singleInstance;
    }

    private Connection connect() {
        String url = DATABASE_PREFIX + databaseFile;
        Connection conn = null;
        try {
            conn = DriverManager.getConnection(url);
        } catch (SQLException e) {
            System.out.println(e.getMessage());
        }
        return conn;
    }

    private void disconnect(Connection conn) {
        try {
            if (conn != null) {
                conn.close();
            }
        } catch (SQLException ex) {
            System.out.println(ex.getMessage());
        }
    }

    public ArrayList<String> getTypePlat(String plat) {
        ArrayList<String> result = new ArrayList<>();

        Connection conn = connect();

        String query = "SELECT * FROM plats where type = ?";

        try {
            PreparedStatement preparedStatement = conn.prepareStatement(query);
            preparedStatement.setString(1, plat);
            ResultSet resultSet = preparedStatement.executeQuery();

            // loop through the result set
            while (resultSet.next()) {
                result.add(resultSet.getString("nom"));
            }
        } catch (SQLException e) {
            System.out.println(e.getMessage());
        }

        disconnect(conn);

        return result;
    }

    public File getDatabaseFile() {
        return databaseFile;
    }

    public void setDatabaseFile(File databaseFile) {
        this.databaseFile = databaseFile;
    }
}
