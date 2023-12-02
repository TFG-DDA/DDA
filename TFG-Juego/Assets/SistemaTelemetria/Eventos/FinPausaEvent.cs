using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class FinPausaXML : DataXML
{
}

public class FinPausaEvent : TrackerEvent
{
    // Atributos del evento

    public FinPausaEvent() : base(typeof(FinPausaEvent).Name)
    {
    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON() + "}]";
        return cadena;
    }

    public override string toServerJSON()
    {
        string cadena = base.toServerJSON() + "}";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        return cadena;
    }

    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();

        return stringWriter.ToString();
    }
}