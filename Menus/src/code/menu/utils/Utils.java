package code.menu.utils;

import javafx.stage.Stage;

import java.awt.*;
import java.io.PrintWriter;
import java.io.StringWriter;

public class Utils {

    public static String getStackTrace(Exception e) {
        StringWriter sw = new StringWriter();
        PrintWriter pw = new PrintWriter(sw);
        e.printStackTrace(pw);
        return sw.toString();
    }

    public static Double getCenterX(Stage stage) {
        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        return (screenSize.width - stage.getWidth()) / 2.0;
    }

    public static Double getCenterY(Stage stage) {
        Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
        return (screenSize.height - stage.getHeight()) / 2.0;
    }
}
