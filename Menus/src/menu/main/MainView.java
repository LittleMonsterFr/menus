package menu.main;

import javafx.application.Application;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.stage.Stage;

import java.awt.*;
import java.util.Stack;

public class MainView extends Application {

    private Stack<Stage> stageStack = null;

    @Override
    public void start(Stage primaryStage) throws Exception {
        FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("main.fxml"));
        Parent root = fxmlLoader.load();

        MainController mainController = MainController.getInstance();
        mainController.setMainView(this);

        MainModel mainModel = MainModel.getInstance();
        mainController.setMainModel(mainModel);
        mainController.updateListPlats();

        primaryStage.setScene(new Scene(root));
        primaryStage.setMaximized(true);
        primaryStage.setTitle("Menus");

        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        primaryStage.setMinHeight(screenSize.height * 0.50);
        primaryStage.setMinWidth(screenSize.width * 0.50);

        stageStack = new Stack<>();
        stageStack.push(primaryStage);

        primaryStage.show();
    }

    public static void main(String[] args) {
        launch(args);
    }

    void showAlert(Alert.AlertType alertType, String title, String header, String message)
    {
        Alert alert = new Alert(alertType);
        alert.setTitle(title);
        if (header != null)
            alert.setHeaderText(header);
        if (message != null)
            alert.setContentText(message);
    }

    public Stack<Stage> getStageStack() {
        return stageStack;
    }
}
