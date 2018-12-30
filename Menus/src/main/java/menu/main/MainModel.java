package main.java.menu.main;

class MainModel {
    private static MainModel singleInstance = null;

    private MainModel() {
    }

    static MainModel getInstance()
    {
        if (singleInstance == null)
            singleInstance = new MainModel();

        return singleInstance;
    }

//    private void createDatabase(String databaseFile) {
//        try (Connection conn = DriverManager.getConnection( DATABASE_PREFIX + databaseFile)) {
//            if (conn != null) {
//                DatabaseMetaData meta = conn.getMetaData();
//                System.out.println("The driver name is " + meta.getDriverName());
//                System.out.println("A new database has been created.");
//            }
//        } catch (SQLException e) {
//            System.out.println(e.getMessage());
//        }
//    }





//    ArrayList<String> getTypePlatFromDatabase(String plat) {
//        ArrayList<String> result = new ArrayList<>();
//
//        Connection conn = connect();
//
//        String query = "SELECT * FROM plats where type = ?";
//
//        try {
//            PreparedStatement preparedStatement = conn.prepareStatement(query);
//            preparedStatement.setString(1, plat);
//            ResultSet resultSet = preparedStatement.executeQuery();
//
//            // loop through the result set
//            while (resultSet.next()) {
//                result.add(resultSet.getString("nom"));
//            }
//        } catch (SQLException e) {
//            System.out.println(e.getMessage());
//        }
//
//        disconnect(conn);
//
//        return result;
//    }
}
