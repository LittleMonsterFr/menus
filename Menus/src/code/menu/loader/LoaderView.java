package code.menu.loader;

import code.menu.utils.Utils;
import javafx.application.Preloader;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Stage;
import javafx.stage.StageStyle;

public class LoaderView extends Preloader {

    private Stage preloader;
    private LoaderController loaderController;

    @Override
    public void start(Stage primaryStage) throws Exception {
        preloader = primaryStage;

        FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("/resources/views/loader.fxml"));
        Parent root = fxmlLoader.load();

        loaderController = fxmlLoader.getController();

        primaryStage.setScene(new Scene(root));
        primaryStage.setResizable(false);
        primaryStage.initStyle(StageStyle.UNDECORATED);

        primaryStage.show();

        primaryStage.setX(Utils.getCenterX(primaryStage));
        primaryStage.setY(Utils.getCenterY(primaryStage));
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
