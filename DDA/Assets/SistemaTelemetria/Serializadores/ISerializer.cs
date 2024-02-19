using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class ISerializer
{
    public abstract string Serialize(TrackerEvent e);
}

