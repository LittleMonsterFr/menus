package code.menu.loader;

import javafx.application.Preloader;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;
import javafx.stage.StageStyle;

import java.awt.*;

public class LoaderView extends Preloader {

    private Stage preloader;
    private LoaderController loaderController;

    @Override
    public void start(Stage primaryStage) throws Exception {
        preloader = primaryStage;

        FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("../../../resources/views/loader.fxml"));
        Parent root = fxmlLoader.load();

        loaderController = fxmlLoader.getController();

        primaryStage.setScene(new Scene(root));
        primaryStage.setResizable(false);
        primaryStage.initStyle(StageStyle.UNDECORATED);

        primaryStage.show();

        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        primaryStage.setX((screenSize.width - primaryStage.getWidth()) / 2.0 );
        primaryStage.setY((screenSize.height - primaryStage.getHeight()) / 2.0);
    }

    @Override
    public void handleApplicationNotification(PreloaderNotification preloaderNotification) {
        if (preloaderNotification instanceof PathNotification) {
            loaderController.setPathLabelText(((PathNotification) preloaderNotification).getText());
        } else if (preloaderNotification instanceof FileNotification) {
            loaderController.setFileLabelText(((FileNotification) preloaderNotification).getText());
        } else if (preloaderNotification instanceof StateChangeNotification){
            if (((StateChangeNotification) preloaderNotification).getType() == StateChangeNotification.Type.BEFORE_START) {
                preloader.hide();
            }
        } else {
            System.out.println("Unhandled notification");
        }
    }
}
