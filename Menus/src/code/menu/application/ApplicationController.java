package code.menu.application;

import code.menu.dao.DatabaseHandler;
import code.menu.plat.Plat;
import code.menu.plat.PlatView;
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
    private PlatView platView = null;

    @FXML
    private ListView<Plat> entreesList;

    @FXML
    Button addEntreeButton;

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

    public static ApplicationController getInstance()
    {
        if (singleInstance == null)
            singleInstance = new ApplicationController();

        return singleInstance;
    }

    public void updateListPlats() {
        ArrayList<Plat> entreeNameList = databaseHandler.getPlatsByType("Entrée");
        entreeNameList.sort((plat1, plat2) -> plat1.getNom().compareToIgnoreCase(plat2.getNom()));
        this.entreesList.getItems().clear();
        ObservableList<Plat> entrees = FXCollections.observableArrayList(entreeNameList);
        this.entreesList.setItems(entrees);
    }

    public void addPlatToList(Plat plat) {
        if (plat.getType().equals("Entrée")) {
            ObservableList<Plat> list = entreesList.getItems();
            list.add(plat);
            list.sort((plat1, plat2) -> plat1.getNom().compareToIgnoreCase(plat2.getNom()));
            entreesList.setItems(list);
        }
    }

    @FXML
    void onMouseEvent(MouseEvent event) {
        Button b = (Button) event.getSource();
        if (b.equals(addEntreeButton)) {
            displayPlatView(null);
        }
    }

    void displayPlatView(Plat plat) {
        PlatView.PlatAction platAction;
        if (plat == null)
            platAction = PlatView.PlatAction.CREATE;
        else
            platAction = PlatView.PlatAction.EDIT;
        platView = new PlatView(applicationView.getMainStage());
        platView.createView(platAction, plat);
        platView.show();
    }

    void setApplicationView(ApplicationView applicationView) {
        this.applicationView = applicationView;
    }

    public ApplicationView getApplicationView() {
        return applicationView;
    }

    void setApplicationModel(ApplicationModel applicationModel) {
        this.applicationModel = applicationModel;
    }

    void setDatabaseHandler(DatabaseHandler databaseHandler) {
        this.databaseHandler = databaseHandler;
    }
}
