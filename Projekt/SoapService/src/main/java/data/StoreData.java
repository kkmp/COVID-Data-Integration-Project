package data;

import org.hibernate.Criteria;
import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;
import org.hibernate.boot.Metadata;
import org.hibernate.boot.MetadataSources;
import org.hibernate.boot.registry.StandardServiceRegistry;
import org.hibernate.boot.registry.StandardServiceRegistryBuilder;
import org.hibernate.criterion.Restrictions;

import java.time.LocalDate;
import java.util.Iterator;
import java.util.List;


public class StoreData {
    public static void main(String[] args) {
        var date = "2022-04-01";
        LocalDate dateObj = LocalDate.parse(date);
        //Create typesafe ServiceRegistry object
        StandardServiceRegistry ssr = new StandardServiceRegistryBuilder().configure("hibernate.cfg.xml").build();

        Metadata meta = new MetadataSources(ssr).getMetadataBuilder().build();

        SessionFactory factory = meta.getSessionFactoryBuilder().build();
        Session session = factory.openSession();
        Transaction t = session.beginTransaction();
        Criteria criteria = session.createCriteria(DailyCase.class);
        criteria.add(Restrictions.between("date", dateObj.plusDays(-1), dateObj));
        List employees = criteria.list();

        Iterator itr = employees.iterator();
        while (itr.hasNext()) {

            DailyCase emp = (DailyCase) itr.next();
            System.out.println(emp);
        }

        t.commit();

        System.out.println("successfully saved");
        factory.close();
        session.close();

    }
}   