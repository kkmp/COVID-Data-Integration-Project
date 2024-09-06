package data;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.Date;
import java.util.UUID;

@Entity
@Table(name = "DailyCases")
public class DailyCase {
    @Id
    @Column(name = "Id")
    private UUID id;

    @Column(name = "CountryId")
    private UUID countryId;

    @Column(name = "NewConfirmed")
    private int newConfirmed;

    @Column(name = "NewDeaths")
    private int newDeaths ;

    @Column(name = "NewRecovered")
    private int NewRecovered;

    @Column(name = "Date")
    private LocalDate date;

    public UUID getId() {
        return id;
    }

    public void setId(UUID id) {
        this.id = id;
    }

    public UUID getCountryId() {
        return countryId;
    }

    public void setCountryId(UUID countryId) {
        this.countryId = countryId;
    }

    public int getNewConfirmed() {
        return newConfirmed;
    }

    public void setNewConfirmed(int newConfirmed) {
        this.newConfirmed = newConfirmed;
    }

    public int getNewDeaths() {
        return newDeaths;
    }

    public void setNewDeaths(int newDeaths) {
        this.newDeaths = newDeaths;
    }

    public int getNewRecovered() {
        return NewRecovered;
    }

    public void setNewRecovered(int newRecovered) {
        NewRecovered = newRecovered;
    }

    public LocalDate getDate() {
        return date;
    }

    public void setDate(LocalDate date) {
        this.date = date;
    }

    @Override
    public String toString() {
        return "data.DailyCase{" +
                "id=" + id +
                ", countryId=" + countryId +
                ", newConfirmed=" + newConfirmed +
                ", newDeaths=" + newDeaths +
                ", NewRecovered=" + NewRecovered +
                ", date=" + date +
                '}';
    }
}
