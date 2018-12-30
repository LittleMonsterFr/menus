package code.menu.loader;

import javafx.fxml.FXML;
import javafx.scene.control.Label;
import javafx.scene.control.ProgressBar;

public class LoaderController {
    @FXML
    Label pathLabel;

    @FXML
    Label fileLabel;

    @FXML
    ProgressBar progressBar;

    void setPathLabelText(String pathLabelText)
    {
        pathLabel.setText(pathLabelText);
    }

    void setFileLabelText(String fileLabelText)
    {
        fileLabel.setText(fileLabelText);
    }

}
