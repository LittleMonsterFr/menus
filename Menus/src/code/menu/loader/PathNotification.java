package code.menu.loader;

import javafx.application.Preloader;

public class PathNotification implements Preloader.PreloaderNotification {
    private String text;

    public PathNotification(String text) {
        this.text = text;
    }

    String getText() {
        return text;
    }
}
