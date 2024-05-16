using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class JSONSerializer : ISerializer
{
    public override string Serialize(TrackerEvent e)
    {
        string cadena = e.toJSON();

        return cadena;
    }
}
