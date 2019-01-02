package code.menu.plat;

import code.menu.application.ApplicationController;
import code.menu.utils.Utils;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.scene.control.Alert;
import javafx.stage.Modality;
import javafx.stage.Stage;
import javafx.stage.StageStyle;

import java.io.IOException;

public class PlatView {

    public enum PlatAction {
        CREATE,
        EDIT,
    }

    private Stage ownerStage;
    private Stage actualStage;

    public PlatView(Stage ownerStage) {
        this.ownerStage = ownerStage;
    }

    public void createView(PlatAction platAction, Plat plat) {
        FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("../../../resources/views/plat.fxml"));
        Parent root;
        try {
            root = fxmlLoader.load();
            PlatController platController = fxmlLoader.getController();
            actualStage = new Stage();
            actualStage.initModality(Modality.WINDOW_MODAL);
            actualStage.initOwner(ownerStage);
            actualStage.initStyle(StageStyle.UNDECORATED);
            actualStage.setScene(new Scene(root));
            actualStage.getScene().getStylesheets().add(getClass().getResource("../../../resources/css/styles.css").toExternalForm());
            actualStage.setMinWidth(actualStage.getWidth());
            actualStage.setResizable(true);
            platController.setPlatView(this);
            platController.setPlatAction(platAction);
            if (platAction == PlatAction.CREATE)
                platController.setValidateButtonText("Créer");
            else if (platAction == PlatAction.EDIT) {
                platController.setValidateButtonText("Editer et Fermer");
                platController.setEditAndContinueVisible(true);
                platController.fillPlat(plat);
            }
        } catch (IOException e) {
            ApplicationController.getInstance().getApplicationView().showAlert(Alert.AlertType.ERROR,
                    e.getClass().toString(), "The following error happened :", Utils.getStackTrace(e));
        }
    }

    public void show() {
        actualStage.show();
        actualStage.setX(Utils.getCenterX(actualStage));
        actualStage.setY(Utils.getCenterY(actualStage));
    }

    void hide() {
        actualStage.hide();
    }

    Stage getActualStage() {
        return actualStage;
    }
}
