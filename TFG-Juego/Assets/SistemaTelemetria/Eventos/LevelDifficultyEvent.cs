using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;


public class LevelDifficultyEvent : TrackerEvent
{
    // Atributos del evento
    bool level;
    string difficulty = "EASY";

    public LevelDifficultyEvent(bool l) : base(typeof(LevelDifficultyEvent).Name)
    {
        level = l;
        if (level)
            difficulty = "HARD";
    }

    // Serializacion en JSON
    public override string toJSON()
    {
        string cadena = base.toJSON();
        cadena += ", \"levelDifficulty\": \"" + difficulty + "\"},";
        return cadena;
    }

    public override string toServerJSON()
    {
        string cadena = base.toServerJSON();
        cadena += ", \"levelDifficulty\": \"" + difficulty + "\"}";
        return cadena;
    }

    // Serializacion en CSV
    public override string toCSV()
    {
        string cadena = base.toCSV();
        cadena += "," + difficulty;
        return cadena;
    }

    // Serializacion en XML
    public override string toXML(ref XmlWriter xml_writer, ref StringWriter stringWriter)
    {
        base.toXML(ref xml_writer, ref stringWriter);
        xml_writer.WriteAttributeString("levelDifficulty", difficulty);

        // Cerramos el evento y volcamos
        xml_writer.WriteEndElement();
        xml_writer.Flush();

        return stringWriter.ToString();
    }
}