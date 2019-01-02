package code.menu.plat;

import java.time.Duration;

public class Plat {
    private Integer id;
    private String type;
    private String nom;
    private String ingredients;
    private String description;
    private Duration temps;
    private Integer note;

    public Plat(Integer id, String type, String nom, String ingredients, String description, Duration temps, Integer note) {
        this.id = id;
        this.type = type;
        this.nom = nom;
        this.ingredients = ingredients;
        this.description = description;
        this.temps = temps;
        this.note = note;
    }

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
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

    private String getHumanTemps()
    {
        if (temps == null)
            return null;
        else
            return this.temps.toHours() + "h " + this.temps.toMinutesPart() + "m";
    }

    public void setTemps(Duration temps) {
        this.temps = temps;
    }

    public Integer getNote() {
        return note;
    }

    public void setNote(Integer note) {
        this.note = note;
    }

    public String getRepresentation() {
        String representation = "";
        representation += this.nom;

        String humanTemps = getHumanTemps();
        if (humanTemps != null)
            representation += " - " + humanTemps;

        if(note != null)
            representation += " - " + this.note + "/10";

        return representation;
    }

    @Override
    public String toString() {
        return "Plat{" +
                "id=" + id +
                ", nom='" + nom + '\'' +
                '}';
    }
}
