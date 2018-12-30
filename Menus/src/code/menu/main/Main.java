package code.menu.main;

import code.menu.application.ApplicationView;
import javafx.application.Application;

public class Main {
    public static void main(String[] args) {
        System.setProperty("javafx.preloader", "code.menu.loader.LoaderView");
        Application.launch(ApplicationView.class, args);
    }
}
