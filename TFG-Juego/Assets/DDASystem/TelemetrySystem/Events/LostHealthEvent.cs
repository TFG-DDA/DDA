using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class LostHealthEvent : DDAEvent
{
    public LostHealthEvent(int h) : base(typeof(LostHealthEvent).Name)
    {
        value = h;
    }
    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"LostHealth\": \"" + value + "\"},";
        return cadena;
    }

    public override string toServerJSON()
    {
        string cadena = base.toServerJSON();
        cadena += ", \"LostHealth\": \"" + value + "\"}";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "," + "\"" + value + "\"";
        return cadena;
    }
    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("LostHealth", value.ToString());

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();
        return stringWriter.ToString();
    }
}