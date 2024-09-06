package soap;

import javax.xml.ws.Endpoint;
//Endpoint publisher
public class SOAPPublisher{
    public static void main(String[] args) {
        Endpoint.publish("http://0.0.0.0:7779/ws/data",
                new SOAPService());
    }
}
