using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class FinNivelEvent : TrackerEvent
{
    // Atributos del evento
    int levelId;
    string levelName;

    public FinNivelEvent(int rId, string name) : base(typeof(FinNivelEvent).Name)
    {
        levelId = rId;
        levelName = name;
    }
    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"LevelID\": \"" + levelId.ToString() + "\"";
        cadena += ", \"LevelName\": \"" + levelName + "\"},";
        return cadena;
    }

    public override string toServerJSON()
    {
        string cadena = base.toServerJSON();
        cadena += ", \"LevelID\": \"" + levelId.ToString() + "\"";
        cadena += ", \"LevelName\": \"" + levelName + "\"}";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "," + levelId.ToString();
        cadena += "," + "\"" + levelName + "\"";
        return cadena;
    }
    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("LevelId", levelId.ToString());
        xml_writer.WriteAttributeString("LevelName", levelName);

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();
        return stringWriter.ToString();
    }
}