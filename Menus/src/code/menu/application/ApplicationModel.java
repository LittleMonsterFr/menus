package code.menu.application;

class ApplicationModel {
    private static ApplicationModel singleInstance = null;

    private ApplicationModel() {
    }

    static ApplicationModel getInstance()
    {
        if (singleInstance == null)
            singleInstance = new ApplicationModel();

        return singleInstance;
    }

}
