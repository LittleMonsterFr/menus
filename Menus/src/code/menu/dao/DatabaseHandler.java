package code.menu.dao;

import code.menu.application.ApplicationView;
import code.menu.plat.Plat;
import code.menu.utils.Utils;
import javafx.scene.control.Alert;

import java.io.File;
import java.sql.*;
import java.time.Duration;
import java.util.ArrayList;

public class DatabaseHandler {

    private ApplicationView applicationView = null;

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
            applicationView.showAlert(Alert.AlertType.ERROR, e.getClass().toString(), "The following error happened :", Utils.getStackTrace(e));
        }
        return conn;
    }

    private void disconnect(Connection conn) {
        try {
            if (conn != null) {
                conn.close();
            }
        } catch (SQLException e) {
            applicationView.showAlert(Alert.AlertType.ERROR, e.getClass().toString(), "The following error happened :", Utils.getStackTrace(e));
        }
    }

    public ArrayList<Plat> getPlatsByType(String typePlat) {
        ArrayList<Plat> result = new ArrayList<>();

        Connection conn = connect();

        String query = "SELECT * FROM plats where type = ?";

        try {
            PreparedStatement preparedStatement = conn.prepareStatement(query);
            preparedStatement.setString(1, typePlat);
            ResultSet resultSet = preparedStatement.executeQuery();

            // loop through the result set
            while (resultSet.next()) {
                int id = resultSet.getInt("id");
                String type = resultSet.getString("type");
                String nom = resultSet.getString("nom");
                String ingredient = resultSet.getString("ingredients");
                String description = resultSet.getString("description");
                Duration temps = Duration.ofSeconds(resultSet.getInt("temps"));
                int note = resultSet.getInt("note");
                Plat plat = new Plat(id, type, nom, ingredient, description, temps, note);
                result.add(plat);
            }
        } catch (SQLException e) {
            applicationView.showAlert(Alert.AlertType.ERROR, e.getClass().toString(), "The following error happened :", Utils.getStackTrace(e));
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

    public void setApplicationView(ApplicationView applicationView) {
        this.applicationView = applicationView;
    }
}