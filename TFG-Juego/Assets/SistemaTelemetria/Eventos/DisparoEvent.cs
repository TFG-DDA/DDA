using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class DisparoXML : DataXML
{
    public string weapon;
}

public class DisparoEvent : TrackerEvent
{
    // Atributos del evento
    string weapon;
    int count;
    int hitCount;

    public DisparoEvent(string w, int c, int h) : base(typeof(DisparoEvent).Name)
    {
        weapon = w;
        count = c;
        hitCount = h;
    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"Weapon\": \"" + weapon + "\", \"Count\": \"" + count.ToString() + "\", \"Hits\": \"" + hitCount.ToString() + "\"},";
        return cadena;
    }

    public override string toServerJSON()
    {
        string cadena = base.toServerJSON();
        cadena += ", \"Weapon\": \"" + weapon + "\", \"Count\": \"" + count.ToString() + "\", \"Hits\": \"" + hitCount.ToString() + "\"},";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "," + "\"" + weapon + "\"" + "," + "\"" + count.ToString() + "," + "\"" + hitCount.ToString() + "\"";
        return cadena;
    }
    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("Weapon", weapon);

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();
        return stringWriter.ToString();
    }
}