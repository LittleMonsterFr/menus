package main.java.menu.loader;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;
import javafx.stage.StageStyle;
import main.java.menu.dao.DatabaseHandler;

import java.awt.*;
import java.io.File;

public class LoaderView extends Application {

    public static void main(String[] args) {
        launch(args);
    }

    @Override
    public void start(Stage primaryStage) throws Exception {
        FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("../../../../resources/views/loader.fxml"));
        Parent root = fxmlLoader.load();

        primaryStage.setScene(new Scene(root));
        primaryStage.setResizable(false);
        primaryStage.initStyle(StageStyle.UNDECORATED);

        primaryStage.show();

        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        primaryStage.setX((screenSize.width - primaryStage.getWidth()) / 2.0 );
        primaryStage.setY((screenSize.height - primaryStage.getHeight()) / 2.0);

        DatabaseHandler databaseHandler = DatabaseHandler.getInstance();
        String databaseFile = databaseHandler.getDatabaseFile();
        File file = new File(databaseFile);
        if (!file.exists())
        {
            
        }
    }
}
