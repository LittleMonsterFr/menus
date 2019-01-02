package code.menu.plat;

import code.menu.application.ApplicationController;
import code.menu.dao.DatabaseHandler;
import javafx.css.PseudoClass;
import javafx.event.EventHandler;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.ChoiceBox;
import javafx.scene.control.TextArea;
import javafx.scene.control.TextField;
import javafx.scene.input.KeyCode;
import javafx.scene.input.KeyEvent;
import javafx.scene.input.MouseEvent;

import java.time.Duration;

public class PlatController {

    private PlatView platView;
    private PlatView.PlatAction platAction;

    private Integer id;

    @FXML
    ChoiceBox<String> types;

    @FXML
    TextField nom;

    @FXML
    ChoiceBox<Integer> heures;

    @FXML
    ChoiceBox<Integer> minutes;

    @FXML
    ChoiceBox<Integer> note;

    @FXML
    TextArea ingredients;

    @FXML
    TextArea description;

    @FXML
    Button editAndContinue;

    @FXML
    Button validate;

    @FXML
    Button cancel;

    @FXML
    void initialize() {
        types.getItems().addAll("Selectionnez un type de plat", "Entrée", "Midi", "Soir");
        types.setValue("Selectionnez un type de plat");

        heures.getItems().add(null);
        for (int i = 0; i < 10; i++) {
            heures.getItems().addAll(i);
        }
        heures.setValue(heures.getItems().get(0));

        minutes.getItems().add(null);
        for (int i = 0; i < 60; i++) {
            minutes.getItems().addAll(i);
        }

        note.getItems().add(null);
        for (int i = 0; i <= 10; i++) {
            note.getItems().addAll(i);
        }
    }

    @FXML
    void onMouseEvent(MouseEvent event) {
        Button b = (Button) event.getSource();
        if (b.equals(validate) || b.equals(editAndContinue)) {
            if (!verifyInput())
                return;

            Plat plat = getPlatFromInput();

            DatabaseHandler databaseHandler = DatabaseHandler.getInstance();
            ApplicationController applicationController = ApplicationController.getInstance();

            if(platAction == PlatView.PlatAction.CREATE) {
                if (!databaseHandler.createPlat(plat))
                    return;
                applicationController.addPlatToList(plat);
            } else if (platAction == PlatView.PlatAction.EDIT) {
                if (!databaseHandler.editPlat(plat))
                    return;
                applicationController.updateListPlats();
            }

            if (b.equals(validate))
                platView.hide();
        } else if (b.equals(cancel)) {
            platView.hide();
        }
    }

    private Plat getPlatFromInput() {
        long seconds = 0;

        if (heures.getValue() != null)
            seconds += heures.getValue() * 3600;

        if (minutes.getValue() != null)
            seconds += minutes.getValue() * 60;

        Duration temps = null;
        if (seconds != 0)
            temps = Duration.ofSeconds(seconds);

        return new Plat(id, types.getValue(), nom.getText(), ingredients.getText(), description.getText(), temps, note.getValue());
    }

    void setValidateButtonText(String text){
        validate.setText(text);
    }

    void setPlatView(PlatView platView) {
        this.platView = platView;

        EventHandler<KeyEvent> filter = keyEvent -> {
            if (keyEvent.getCode() == KeyCode.TAB) {
                if (keyEvent.getTarget().equals(ingredients)) {
                    description.requestFocus();
                    keyEvent.consume();
                }
                else if (keyEvent.getTarget().equals(description)) {
                    validate.requestFocus();
                    keyEvent.consume();
                }
            }
        };

        platView.getActualStage().addEventFilter(KeyEvent.KEY_PRESSED, filter);
    }

    private boolean verifyInput() {
        final PseudoClass errorClass = PseudoClass.getPseudoClass("error");

        boolean res = true;

        if (types.getValue().equals("Selectionnez un type de plat"))
            res = false;
        types.pseudoClassStateChanged(errorClass, types.getValue().equals("Selectionnez un type de plat"));

        if (nom.getText().trim().isEmpty())
            res = false;
        nom.pseudoClassStateChanged(errorClass, nom.getText().trim().isEmpty());

        if (ingredients.getText().trim().isEmpty())
            res = false;
        ingredients.pseudoClassStateChanged(errorClass, ingredients.getText().trim().isEmpty());

        if (description.getText().trim().isEmpty())
            res = false;
        description.pseudoClassStateChanged(errorClass, description.getText().trim().isEmpty());

        return res;
    }

    void fillPlat(Plat plat) {
        id = plat.getId();
        types.setValue(plat.getType());
        nom.setText(plat.getNom());
        heures.setValue(plat.getTemps() != null ? (int) plat.getTemps().toHours() : null);
        minutes.setValue(plat.getTemps() != null ? plat.getTemps().toMinutesPart() : null);
        note.setValue(plat.getNote());
        ingredients.setText(plat.getIngredients());
        description.setText(plat.getDescription());
    }

    void setEditAndContinueVisible(boolean b) {
        editAndContinue.setVisible(b);
    }

    void setPlatAction(PlatView.PlatAction platAction) {
        this.platAction = platAction;
    }
}
