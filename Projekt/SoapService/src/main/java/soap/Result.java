package soap;

import java.time.LocalDateTime;


public class Result {
    private LocalDateTime date;
    private int confirmed;
    private int deaths;

    public Result(int confirmed, int deaths) {
        this.date = LocalDateTime.now();
        this.confirmed = confirmed;
        this.deaths = deaths;
    }

    public LocalDateTime getDate() {
        return date;
    }

    public int getConfirmed() {
        return confirmed;
    }

    public int getDeaths() {
        return deaths;
    }

    @Override
    public String toString() {
        return "Result{" +
                "date=" + date +
                ", confirmed=" + confirmed +
                ", deaths=" + deaths +
                '}';
    }
}
