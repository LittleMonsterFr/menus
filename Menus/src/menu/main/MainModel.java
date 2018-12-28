package menu.main;

import java.io.*;
import java.util.ArrayList;

class MainModel {
    private static MainModel singleInstance = null;

    private ArrayList<String> entreeList = null;
    private File entreesCsvFile = null;

    private MainModel() {

    }

    static MainModel getInstance()
    {
        if (singleInstance == null)
            singleInstance = new MainModel();

        return singleInstance;
    }

    ArrayList<String> readEntreesFromCsvFile()
    {
        ArrayList<String> entreeListTemp = new ArrayList<>();
        BufferedReader br;
        try {
            br = new BufferedReader(new FileReader(entreesCsvFile));
            String line;
            while ((line = br.readLine()) != null) {
                String entreeName = line.trim().split(",")[0].trim().strip("\"");
                entreeListTemp.add(entreeName);
            }
        } catch (IOException e) {
            return null;
        }

        entreeList = entreeListTemp;
        return entreeList;
    }


    public File getentreesCsvFile() {
        return entreesCsvFile;
    }

    public void setentreesCsvFile(File entreesCsvFile) {
        this.entreesCsvFile = entreesCsvFile;
    }
}
