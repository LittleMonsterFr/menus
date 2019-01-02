package code.menu.plat;

import code.menu.application.ApplicationController;
import code.menu.dao.DatabaseHandler;
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
        if (b.equals(validate)) {
            long seconds = 0;

            if (heures.getValue() != null)
                seconds += heures.getValue() * 3600;

            if(minutes.getValue() != null)
                seconds += minutes.getValue() * 60;

            Duration temps = null;
            if (seconds != 0)
                temps = Duration.ofSeconds(seconds);

            Plat plat = new Plat(0, types.getValue(), nom.getText(), ingredients.getText(), description.getText(), temps, note.getValue());

            DatabaseHandler databaseHandler = DatabaseHandler.getInstance();
            if (!databaseHandler.createPlat(plat))
                return;

            ApplicationController applicationController = ApplicationController.getInstance();
            applicationController.addPlatToList(plat);
        }

        platView.hide();
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
}
