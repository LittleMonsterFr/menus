package code.menu.application;

import code.menu.plat.Plat;
import javafx.geometry.Pos;
import javafx.scene.Cursor;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.ListCell;
import javafx.scene.layout.HBox;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;

public class PlatFormatCell extends ListCell<Plat> {

    private HBox hBox;
    private Label label;
    private ApplicationController applicationController;

    PlatFormatCell() {
        super();
        applicationController = ApplicationController.getInstance();
        hBox = new HBox();
        label = new Label();
        Button button = new Button();
        label.setOnMouseEntered(mouseEvent -> {
            label.setUnderline(true);
        });
        label.setOnMouseExited(mouseEvent -> {
            label.setUnderline(false);
        });
        setCursor(Cursor.HAND);
        label.setFont(Font.font(getFont().getFamily(), FontWeight.BOLD, getFont().getSize()));
        button.setText("✎");
        button.setOnMouseClicked(mouseEvent -> {
            applicationController.displayPlatView(getItem());
        });
        hBox.getChildren().setAll(button, label);
        hBox.setSpacing(5);
        hBox.setAlignment(Pos.CENTER_LEFT);
    }

    @Override
    protected void updateItem(Plat plat, boolean b) {
        super.updateItem(plat, b);
        setText(null);
        label.setText(plat == null ? "" : plat.getRepresentation());
        setGraphic(hBox);
    }
}
