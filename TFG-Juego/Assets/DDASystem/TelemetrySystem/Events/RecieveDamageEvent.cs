using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class RecieveDamageXML : DataXML
{
    public string vidaRestante;
}

public class RecieveDamageEvent : TrackerEvent
{
    // Atributos del evento
    int vidaRestante;

    public RecieveDamageEvent(int v) : base(typeof(RecieveDamageEvent).Name)
    {
        vidaRestante = v;
    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"Vida\": \"" + vidaRestante.ToString() + "\"},";
        return cadena;
    }

    public override string toServerJSON()
    {
        string cadena = base.toServerJSON();
        cadena += ", \"Vida\": \"" + vidaRestante.ToString() + "\"}";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "," + "\"" + vidaRestante.ToString() + "\"";
        return cadena;
    }

    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("Vida", vidaRestante.ToString());

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();

        return stringWriter.ToString();
    }
}