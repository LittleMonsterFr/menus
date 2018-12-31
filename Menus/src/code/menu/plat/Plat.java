package code.menu.plat;

import java.time.Duration;

public class Plat {
    private int id;
    private String type;
    private String nom;
    private String ingredients;
    private String description;
    private Duration temps;
    private int note;

    public Plat(int id, String type, String nom, String ingredients, String description, Duration temps, int note) {
        this.id = id;
        this.type = type;
        this.nom = nom;
        this.ingredients = ingredients;
        this.description = description;
        this.temps = temps;
        this.note = note;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getNom() {
        return nom;
    }

    public void setNom(String nom) {
        this.nom = nom;
    }

    public String getIngredients() {
        return ingredients;
    }

    public void setIngredients(String ingredients) {
        this.ingredients = ingredients;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Duration getTemps() {
        return temps;
    }

    public String getHumanTemps()
    {
        return String.format("%dh %02dm", this.temps.toHours(), this.temps.toMinutes());
    }

    public void setTemps(Duration temps) {
        this.temps = temps;
    }

    public int getNote() {
        return note;
    }

    public void setNote(int note) {
        this.note = note;
    }

    public String getRepresentation() {
        return this.nom + " - " + getHumanTemps() + " - " + this.note + "/10";
    }

    @Override
    public String toString() {
        return "Plat{" +
                "id=" + id +
                ", nom='" + nom + '\'' +
                '}';
    }
}
