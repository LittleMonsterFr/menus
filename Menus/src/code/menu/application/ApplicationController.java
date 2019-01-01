package code.menu.application;

import code.menu.dao.DatabaseHandler;
import code.menu.plat.Plat;
import javafx.beans.value.ObservableValue;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.fxml.FXML;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.*;
import javafx.scene.input.MouseEvent;
import javafx.stage.Modality;
import javafx.stage.Stage;

import java.io.IOException;
import java.util.ArrayList;

public class ApplicationController {

    private static ApplicationController singleInstance = null;
    private ApplicationView applicationView = null;
    private ApplicationModel applicationModel = null;
    private DatabaseHandler databaseHandler = null;

    @FXML
    private ListView<Plat> entreesList;

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

        entreesList.setCellFactory(list -> new PlatFormatCell());

        entreesList.getSelectionModel().selectedItemProperty().addListener(
                (ObservableValue<? extends Plat> ov, Plat old_val, Plat new_val) -> System.out.println("Selected value : " + new_val));
    }

    static ApplicationController getInstance()
    {
        if (singleInstance == null)
            singleInstance = new ApplicationController();

        return singleInstance;
    }

    void updateListPlats() {
        ArrayList<Plat> entreeNameList = databaseHandler.getPlatsByType("entree");
        this.entreesList.getItems().clear();
        ObservableList<Plat> entrees = FXCollections.observableArrayList(entreeNameList);
        this.entreesList.setItems(entrees);
    }

    @FXML
    void onMouseEvent(MouseEvent event)
    {
        if (event.getEventType() == MouseEvent.MOUSE_CLICKED) {

            FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("../../../resources/views/plat.fxml"));
            Parent root = null;
            try {
                root = fxmlLoader.load();
                Stage platScene = new Stage();
                platScene.initModality(Modality.APPLICATION_MODAL);
                platScene.setScene(new Scene(root));
                platScene.setTitle("Plat");
                platScene.show();
            } catch (IOException e) {
                e.printStackTrace();
            }




            System.out.println("event : " + event.toString());
            button.setText("Hello");
            System.out.println("Button clicked");
        }
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
