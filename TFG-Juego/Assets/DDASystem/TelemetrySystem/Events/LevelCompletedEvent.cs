using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class LevelCompletedEvent : DDAEvent
{
    public LevelCompletedEvent(int num) : base(typeof(LevelCompletedEvent).Name)
    {
        value = num;
    }
    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"Value\": \"" + value.ToString() + "\"},";
        return cadena;
    }

    public override string toServerJSON()
    {
        string cadena = base.toServerJSON();
        cadena += ", \"Value\": \"" + value.ToString() + "\"}";
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