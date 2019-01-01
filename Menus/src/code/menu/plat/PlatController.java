package code.menu.plat;

import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.input.MouseEvent;

public class PlatController {

    private PlatView platView;

    @FXML
    Button validate;

    @FXML
    Button cancel;

    @FXML
    void onMouseEvent(MouseEvent event) {
        Button b = (Button) event.getSource();
        if (b.equals(validate)) {
        } else if (b.equals(cancel)) {
            platView.hide();
        }
    }

    void setValidateButtonText(String text){
        validate.setText(text);
    }

    void setPlatView(PlatView platView) {
        this.platView = platView;
    }
}
