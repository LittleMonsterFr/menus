package menu.main;

import javafx.event.Event;
import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.TabPane;
import javafx.scene.input.MouseEvent;
import javafx.stage.FileChooser;
import javafx.stage.Stage;

import java.io.File;

public class MainController {

    private static MainController singleInstance = null;
    private MainView mainView = null;
    private MainModel mainModel = null;
    private FileChooser fileChooser = null;

    @FXML
    private TabPane tabPane;

    @FXML
    private Button button;

    public MainController() {
        // because the controller is instantiated by the FXMLLoader, set the instance manually
        // Every next access to the controller should be done with getInstance
        singleInstance = this;
        fileChooser = new FileChooser();
        fileChooser.getExtensionFilters().addAll(
                new FileChooser.ExtensionFilter("Comma Separated Values (CSV)", "*.csv")
        );

        System.out.println("MainController created");
    }

    public void initialize()
    {
        // Handle here what you want to edit on the fields once they have been populated after the constructor call
        System.out.println("MainController initialised");
    }

    static MainController getInstance()
    {
        if (singleInstance == null)
            singleInstance = new MainController();

        return singleInstance;
    }

    @FXML
    void onMouseEvent(MouseEvent event)
    {
        System.out.println("event : " + event.toString());
        button.setText("Hello");
        System.out.println("Button clicked");
    }

    @FXML
    void OnEntreesCsvFileEdition(Event event)
    {
        Stage topStage = mainView.getStageStack().lastElement();
        File selectedFile = fileChooser.showOpenDialog(topStage);
    }

    void setMainView(MainView mainView) {
        this.mainView = mainView;
    }

    void setMainModel(MainModel mainModel) {
        this.mainModel = mainModel;
    }
}
