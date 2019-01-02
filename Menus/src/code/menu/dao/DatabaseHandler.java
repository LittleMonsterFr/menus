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
                Integer id = resultSet.getInt("id");
                String type = resultSet.getString("type");
                String nom = resultSet.getString("nom");
                String ingredient = resultSet.getString("ingredients");
                String description = resultSet.getString("description");
                Duration temps = Duration.ofSeconds(resultSet.getInt("temps"));
                if (resultSet.wasNull())
                    temps = null;
                Integer note = resultSet.getInt("note");
                if (resultSet.wasNull())
                    note = null;
                Plat plat = new Plat(id, type, nom, ingredient, description, temps, note);
                result.add(plat);
            }
        } catch (SQLException e) {
            applicationView.showAlert(Alert.AlertType.ERROR, e.getClass().toString(), "The following error happened :", Utils.getStackTrace(e));
        }

        disconnect(conn);

        return result;
    }

    public boolean createPlat(Plat plat) {
        String query = "INSERT INTO plats(type, nom, ingredients, description, temps, note) VALUES(?,?,?,?,?,?)";

        return createUpdate(plat, query);
    }

    public boolean editPlat(Plat plat) {
        String query = "UPDATE plats SET type = ?, nom = ?, ingredients = ?, description = ?, temps = ?, note = ? where id = ?";

        return createUpdate(plat, query);
    }

    boolean createUpdate(Plat plat, String query) {
        Connection conn = connect();

        boolean res = true;

        try {
            PreparedStatement pstmt = conn.prepareStatement(query);
            pstmt.setString(1, plat.getType());
            pstmt.setString(2, plat.getNom());
            pstmt.setString(3, plat.getIngredients());
            pstmt.setString(4, plat.getDescription());

            Duration temps = plat.getTemps();
            if (temps == null)
                pstmt.setNull(5, Types.INTEGER);
            else
                pstmt.setLong(5, temps.toSeconds());

            Integer note = plat.getNote();
            if (note == null)
                pstmt.setNull(6, Types.INTEGER);
            else
                pstmt.setInt(6, plat.getNote());

            if (query.startsWith("UPDATE"))
                pstmt.setInt(7, plat.getId());

            pstmt.executeUpdate();
        } catch (SQLException e) {
            applicationView.showAlert(Alert.AlertType.ERROR, e.getClass().toString(), "The following error happened :", Utils.getStackTrace(e));
            res = false;
        }

        disconnect(conn);
        return res;
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
