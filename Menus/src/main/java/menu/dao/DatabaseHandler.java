package main.java.menu.dao;
import java.io.File;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class DatabaseHandler {
    private static DatabaseHandler singleInstance = null;
    private static final String DATABASE_PREFIX = "jdbc:sqlite:";
    private String databaseFile = System.getProperty("user.home") + File.separator + "Menu.db";

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

    public String getDatabaseFile() {
        return databaseFile;
    }

    public void setDatabaseFile(String databaseFile) {
        this.databaseFile = databaseFile;
    }
}
