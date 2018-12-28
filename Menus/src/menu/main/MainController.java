package menu.main;

import javafx.event.Event;
import javafx.fxml.FXML;
import javafx.scene.control.Alert;
import javafx.scene.control.Button;
import javafx.scene.control.ListView;
import javafx.scene.input.MouseEvent;
import javafx.stage.FileChooser;
import javafx.stage.Stage;

import java.io.File;
import java.util.ArrayList;

public class MainController {

    private static MainController singleInstance = null;
    private MainView mainView = null;
    private MainModel mainModel = null;
    private FileChooser fileChooser;

    @FXML
    private ListView<String> entreeNameList;

    @FXML
    private Button button;

    public MainController() {
        // because the controller is instantiated by the FXMLLoader, set the instance manually
        // Every next access to the controller should be done with getInstance
        singleInstance = this;
        fileChooser = new FileChooser();
        fileChooser.getExtensionFilters().addAll(
                new FileChooser.ExtensionFilter("Comma Separated Values (CSV)", "*.csv")
        );

        System.out.println("MainController created");
    }

    public void initialize()
    {
        // Handle here what you want to edit on the fields once they have been populated after the constructor call
        System.out.println("MainController initialised");
    }

    static MainController getInstance()
    {
        if (singleInstance == null)
            singleInstance = new MainController();

        return singleInstance;
    }

    @FXML
    void onMouseEvent(MouseEvent event)
    {
        System.out.println("event : " + event.toString());
        button.setText("Hello");
        System.out.println("Button clicked");
    }

    @FXML
    void OnEntreesCsvFileEdition(Event event)
    {
        Stage topStage = mainView.getStageStack().lastElement();
        File selectedFile = fileChooser.showOpenDialog(topStage);
        mainModel.setentreesCsvFile(selectedFile);
        ArrayList<String> entreeNameList = mainModel.readEntreesFromCsvFile();
        if (entreeNameList == null) {
            mainView.showAlert(Alert.AlertType.WARNING, "Fichier d'entrées.", null, "Une erreur est survenus lors de la récupération de la liste des entrées.");
            return;
        }
        this.entreeNameList.getItems().clear();
        for (String entreeName : entreeNameList)
            this.entreeNameList.getItems().add(entreeName);
    }

    void setMainView(MainView mainView) {
        this.mainView = mainView;
    }

    void setMainModel(MainModel mainModel) {
        this.mainModel = mainModel;
    }
}
