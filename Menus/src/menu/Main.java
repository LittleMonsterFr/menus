package menu;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;

import java.awt.*;

public class Main extends Application {

    @Override
    public void start(Stage primaryStage) throws Exception{
        Parent root = FXMLLoader.load(getClass().getResource("main.fxml"));

        primaryStage.setScene(new Scene(root));
        primaryStage.setMaximized(true);
        primaryStage.setTitle("Menus");

        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        primaryStage.setMinHeight(screenSize.height * 0.50);
        primaryStage.setMinWidth(screenSize.width * 0.50);

        primaryStage.show();
    }

    public static void main(String[] args) {
        launch(args);
    }

}