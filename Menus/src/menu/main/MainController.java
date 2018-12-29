package menu.main;

import javafx.beans.value.ObservableValue;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.event.Event;
import javafx.fxml.FXML;
import javafx.scene.control.*;
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

        entreeNameList.getSelectionModel().selectedItemProperty().addListener(
                (ObservableValue<? extends String> ov, String old_val, String new_val) -> System.out.println(new_val));
    }

    static MainController getInstance()
    {
        if (singleInstance == null)
            singleInstance = new MainController();

        return singleInstance;
    }

    void updateListPlats() {
        ArrayList<String> entreeNameList = mainModel.getTypePlatFromDatabase("entree");
        if (entreeNameList == null) {
            mainView.showAlert(Alert.AlertType.WARNING, "Fichier d'entrées.", null, "Une erreur est survenus lors de la récupération de la liste des entrées.");
            return;
        }
        this.entreeNameList.getItems().clear();
        ObservableList<String> entrees = FXCollections.observableArrayList(entreeNameList);
        this.entreeNameList.setItems(entrees);
    }

    @FXML
    void onMouseEvent(MouseEvent event)
    {
        System.out.println("event : " + event.toString());
        button.setText("Hello");
        System.out.println("Button clicked");
    }

    void setMainView(MainView mainView) {
        this.mainView = mainView;
    }

    void setMainModel(MainModel mainModel) {
        this.mainModel = mainModel;
    }
}
