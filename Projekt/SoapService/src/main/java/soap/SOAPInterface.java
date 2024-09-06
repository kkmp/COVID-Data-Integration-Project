package soap;

import javax.jws.WebMethod;
import javax.jws.WebService;
import javax.jws.soap.SOAPBinding;
import javax.jws.soap.SOAPBinding.Style;

//Service Endpoint Interface
@WebService // oznaczenie klasy jako SEO (Service Endpoint
@SOAPBinding(style = Style.RPC)
public interface SOAPInterface {
    @WebMethod int[] getData(String name);
}