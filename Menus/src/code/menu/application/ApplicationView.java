package code.menu.application;

import code.menu.dao.DatabaseHandler;
import code.menu.loader.PathNotification;
import code.menu.utils.Utils;
import javafx.application.Application;
import javafx.application.Preloader;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.scene.layout.Pane;
import javafx.stage.Stage;
import code.menu.loader.FileNotification;

import java.awt.*;
import java.io.File;
import java.io.IOException;
import java.nio.file.DirectoryStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Stack;

public class ApplicationView extends Application {

    private Stack<Stage> stageStack = null;
    private File databaseFile;
    private DatabaseHandler databaseHandler;

    public static void main(String[] args) {
        launch(args);
    }

    @Override
    public void init() throws Exception {
        databaseHandler = DatabaseHandler.getInstance();
        databaseHandler.setApplicationView(this);
        File databaseFile = databaseHandler.getDatabaseFile();

        this.databaseFile = getDatabaseFileRecursively(databaseFile.getParentFile().toPath(), databaseFile.getName()).toFile();

        Thread.sleep(2500);

        notifyPreloader(new Preloader.StateChangeNotification(Preloader.StateChangeNotification.Type.BEFORE_START));
    }

    @Override
    public void start(Stage primaryStage) throws Exception {
        FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("../../../resources/views/application.fxml"));
        Parent root = fxmlLoader.load();

        ApplicationController applicationController = ApplicationController.getInstance();
        applicationController.setApplicationView(this);
        applicationController.setDatabaseHandler(databaseHandler);

        ApplicationModel applicationModel = ApplicationModel.getInstance();
        applicationController.setApplicationModel(applicationModel);

        if (databaseFile == null)
            showAlert(Alert.AlertType.WARNING, "Fichier de données absent.", null,
                    "Le fichier de données " + databaseHandler.getDatabaseFile() + " n'a pas été trouvé.");
        else {
            databaseHandler.setDatabaseFile(databaseFile);
            applicationController.updateListPlats();
        }

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

    public void showAlert(Alert.AlertType alertType, String title, String header, String message)
    {
        Alert alert = new Alert(alertType);


        alert.setTitle(title);
        if (header != null)
            alert.setHeaderText(header);
        if (message != null) {
            Pane pane = new Pane();
            TextArea textArea = new TextArea();
            textArea.setText(message);
            alert.getDialogPane().setContent(pane);
        }
        alert.showAndWait();
    }

    private Path getDatabaseFileRecursively(Path folder, String fileName)
    {
        Path path = null;

        try (DirectoryStream<Path> stream = Files.newDirectoryStream(folder)) {
            notifyPreloader(new PathNotification(folder.toString()));

            for (Path entry : stream) {
                if (Files.isHidden(entry))
                    continue;

                if (!Files.isDirectory(entry)) {
                    notifyPreloader(new FileNotification(entry.getName(entry.getNameCount() - 1).toString()));
                    if (entry.getName(entry.getNameCount() - 1).equals(Path.of(fileName)))
                        return entry;
                }
            }

            for (Path entry : stream) {
                if (Files.isHidden(entry))
                    continue;

                if (Files.isDirectory(entry)) {
                    notifyPreloader(new PathNotification(entry.toString()));
                    path = getDatabaseFileRecursively(entry, fileName);
                    if (path != null)
                        return path;
                }
            }

        } catch (IOException e) {
            this.showAlert(Alert.AlertType.ERROR, e.getClass().toString(), "The following error happened :", Utils.getStackTrace(e));
        }

        return path;
    }

    public Stack<Stage> getStageStack() {
        return stageStack;
    }
}
