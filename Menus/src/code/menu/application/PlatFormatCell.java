package code.menu.application;

import code.menu.plat.Plat;
import javafx.scene.Cursor;
import javafx.scene.control.ListCell;
import javafx.scene.text.Font;
import javafx.scene.text.FontWeight;

public class PlatFormatCell extends ListCell<Plat> {

    public PlatFormatCell() {
    }

    @Override
    protected void updateItem(Plat plat, boolean b) {
        super.updateItem(plat, b);
        setText(plat == null ? "" : plat.getRepresentation());
        setFont(Font.font(getFont().getFamily(), FontWeight.BOLD, getFont().getSize()));

        setOnMouseEntered(mouseEvent -> {
            setUnderline(true);
            setCursor(Cursor.HAND);}
        );

        setOnMouseExited(mouseEvent -> {
            setUnderline(false);
            setCursor(Cursor.DEFAULT);
        });
    }
}
