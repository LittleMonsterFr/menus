package code.menu.plat;

import code.menu.utils.Utils;
import javafx.fxml.FXMLLoader;
import javafx.scene.Parent;
import javafx.scene.Scene;
import javafx.stage.Modality;
import javafx.stage.Stage;
import javafx.stage.StageStyle;

import java.awt.*;
import java.io.IOException;

public class PlatView {

    public enum PlatAction {
        CREATE,
        EDIT,
    }

    private Stage ownerStage;
    private Stage actualStage;
    private PlatController platController;

    public PlatView(Stage ownerStage) {
        this.ownerStage = ownerStage;
    }

    public void createView(PlatAction platAction) {
        FXMLLoader fxmlLoader = new FXMLLoader(getClass().getResource("../../../resources/views/plat.fxml"));
        Parent root;
        try {
            root = fxmlLoader.load();
            platController = fxmlLoader.getController();
            platController.setPlatView(this);
            if (platAction == PlatAction.CREATE)
                platController.setValidateButtonText("Créer");
            else
                platController.setValidateButtonText("Editer");
            actualStage = new Stage();
            actualStage.initModality(Modality.WINDOW_MODAL);
            actualStage.initOwner(ownerStage);
            actualStage.initStyle(StageStyle.UNDECORATED);
            actualStage.setScene(new Scene(root));
            actualStage.setMinWidth(actualStage.getWidth());
            actualStage.setResizable(true);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void show() {
        actualStage.show();

        actualStage.setX(Utils.getCenterX(actualStage));
        actualStage.setY(Utils.getCenterY(actualStage));
    }

    public void hide() {
        actualStage.hide();
    }
}
