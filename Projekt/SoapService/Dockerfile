FROM maven

COPY . /app
WORKDIR /app

RUN chmod -R 777 /app
RUN mvn clean install && mvn package

CMD bash -c "cd /app/target && java -jar SoapService-1.0-SNAPSHOT.jar"