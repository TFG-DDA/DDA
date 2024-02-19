using System.Collections;
using System.Collections.Generic;

public abstract class IPersistence
{
    public abstract void Send(TrackerEvent e);
    public abstract void Release();

    public abstract void Flush();
}