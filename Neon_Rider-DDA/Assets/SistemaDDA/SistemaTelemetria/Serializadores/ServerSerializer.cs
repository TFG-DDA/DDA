using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ServerSerializer : ISerializer
{
    public override string Serialize(TrackerEvent e)
    {
        string cadena = e.toServerJSON();

        return cadena;
    }
}
