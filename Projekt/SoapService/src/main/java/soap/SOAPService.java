package soap;

import data.DailyCase;
import org.hibernate.Criteria;
import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;
import org.hibernate.boot.Metadata;
import org.hibernate.boot.MetadataSources;
import org.hibernate.boot.registry.StandardServiceRegistry;
import org.hibernate.boot.registry.StandardServiceRegistryBuilder;
import org.hibernate.criterion.Restrictions;

import javax.jws.WebService;
import java.time.LocalDate;
import java.util.*;

@WebService(endpointInterface =
        "soap.SOAPInterface")

public class SOAPService implements SOAPInterface {
    public int[] getData(String date)
    {
        LocalDate dateObj = LocalDate.parse(date);
        StandardServiceRegistry ssr = new StandardServiceRegistryBuilder().configure("hibernate.cfg.xml").build();

        Metadata meta = new MetadataSources(ssr).getMetadataBuilder().build();

        SessionFactory factory = meta.getSessionFactoryBuilder().build();
        Session session = factory.openSession();
        Transaction t = session.beginTransaction();
        Criteria criteria = session.createCriteria(DailyCase.class);
        criteria.add(Restrictions.between("date", dateObj.plusDays(-1), dateObj));
        List employees = criteria.list();

        Iterator itr = employees.iterator();
        Map<UUID, List<DailyCase>> cases = new HashMap<>();
        while (itr.hasNext()) {
            DailyCase dc = (DailyCase) itr.next();
            if(cases.containsKey(dc.getCountryId()))
            {
                cases.get(dc.getCountryId()).add(dc);
            }
            else
            {
                List<DailyCase> list = new ArrayList<>();
                list.add(dc);
                cases.put(dc.getCountryId(), list);
            }
        }

        int confirmed = 0;
        int deaths = 0;
        try {
            confirmed = cases
                    .entrySet()
                    .stream()
                    .filter(x -> x.getValue().size() > 1)
                    .map(x -> Math.abs(x.getValue().get(0).getNewConfirmed() - x.getValue().get(1).getNewConfirmed()))
                    .reduce((x, y) -> x+y).get();

            deaths = cases
                    .entrySet()
                    .stream()
                    .filter(x -> x.getValue().size() > 1)
                    .map(x -> Math.abs(x.getValue().get(0).getNewDeaths() - x.getValue().get(1).getNewDeaths()))
                    .reduce((x, y) -> x+y).get();
        }
        catch (NoSuchElementException ex)
        {
            System.out.println("No data for: " + date);
        }

        t.commit();

        factory.close();
        session.close();

        return new int[] {confirmed, deaths};
    }
}