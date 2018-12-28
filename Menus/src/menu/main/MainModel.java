package menu.main;

import java.io.File;
import java.util.List;

class MainModel {
    private static MainModel singleInstance = null;

    private List<String> entreeList = null;
    private File EntreesCsvFile = null;

    private MainModel() {

    }

    static MainModel getInstance()
    {
        if (singleInstance == null)
            singleInstance = new MainModel();

        return singleInstance;
    }

//    private List<String> readEntreesFromCsvFilePath()
//    {
//
//    }


    public File getEntreesCsvFile() {
        return EntreesCsvFile;
    }

    public void setEntreesCsvFile(File entreesCsvFile) {
        EntreesCsvFile = entreesCsvFile;
    }
}
