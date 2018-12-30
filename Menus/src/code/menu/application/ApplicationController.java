package code.menu.application;

import code.menu.dao.DatabaseHandler;
import javafx.beans.value.ObservableValue;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.input.MouseEvent;

import java.util.ArrayList;

public class ApplicationController {

    private static ApplicationController singleInstance = null;
    private ApplicationView applicationView = null;
    private ApplicationModel applicationModel = null;
    private DatabaseHandler databaseHandler = null;

    @FXML
    private ListView<String> entreeNameList;

    @FXML
    private Button button;

    public ApplicationController() {
        // Because the controller is instantiated by the FXMLLoader, set the instance manually
        // Every next access to the controller should be done with getInstance
        singleInstance = this;
    }

    public void initialize()
    {
        // Handle here what you want to edit on the fields once they have been populated after the constructor call
        entreeNameList.getSelectionModel().selectedItemProperty().addListener(
                (ObservableValue<? extends String> ov, String old_val, String new_val) -> System.out.println(new_val));
    }

    static ApplicationController getInstance()
    {
        if (singleInstance == null)
            singleInstance = new ApplicationController();

        return singleInstance;
    }

    void updateListPlats() {
        ArrayList<String> entreeNameList = databaseHandler.getTypePlat("entree");
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

    void setApplicationView(ApplicationView applicationView) {
        this.applicationView = applicationView;
    }

    void setApplicationModel(ApplicationModel applicationModel) {
        this.applicationModel = applicationModel;
    }

    void setDatabaseHandler(DatabaseHandler databaseHandler) {
        this.databaseHandler = databaseHandler;
    }
}
