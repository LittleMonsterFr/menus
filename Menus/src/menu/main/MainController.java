package menu.main;

import javafx.fxml.FXML;
import javafx.scene.control.Button;
import javafx.scene.control.TabPane;
import javafx.scene.input.MouseEvent;

public class MainController {

    private static MainController single_instance = null;
    private MainView mainView = null;

    @FXML
    private TabPane tabPane;

    @FXML
    private Button button;

    public MainController() {
        // because the controller is instantiated by the FXMLLoader, set the instance manually
        // Every next access to the controller should be done with getInstance
        single_instance = this;

        System.out.println("MainController created");
    }

    public void initialize()
    {
        // Handle here what you want to edit on the fields once they have been populated after the constructor call
        System.out.println("MainController initialised");
    }

    static MainController getInstance()
    {
        if (single_instance == null)
            single_instance = new MainController();

        return single_instance;
    }

    @FXML
    void onMouseEvent(MouseEvent event)
    {
        System.out.println("event : " + event.toString());
        button.setText("Hello");
        System.out.println("Button clicked");
    }

    void setMainView(MainView mainView) {
        this.mainView = mainView;
    }
}
