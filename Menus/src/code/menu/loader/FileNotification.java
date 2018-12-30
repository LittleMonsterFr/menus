package code.menu.loader;

import javafx.application.Preloader;

public class FileNotification implements Preloader.PreloaderNotification {
    private String text;

    public FileNotification(String text) {
        this.text = text;
    }

    String getText() {
        return text;
    }
}
