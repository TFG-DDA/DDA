using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class FilePersistence : IPersistence
{
    List<TrackerEvent> eventsBuff;

    bool serializeInJSON;
    bool serializeInXML;
    bool serializeInCSV;

    JSONSerializer serializerJSON = null;   //JSON
    StreamWriter jsonStream = null;

    XMLSerializer serializerXML = null;     //XML
    StreamWriter xmlStream = null;

    CSVSerializer serializerCSV = null;     //CSV
    StreamWriter csvStream = null;

    string ruta_base = "Trazas\\";
    string ruta_json = "JSON\\";
    string ruta_xml = "XML\\";
    string ruta_csv = "CSV\\";

    public FilePersistence(bool inJSON, bool inCSV, bool inXML)
    {
        eventsBuff = new();
        string id = Tracker.Instance.GetSessionId().ToString();

        // Crear carpetas del tracker
        if (!Directory.Exists(ruta_base))
            Directory.CreateDirectory(ruta_base);
        if (!Directory.Exists(ruta_base + ruta_json))
            Directory.CreateDirectory(ruta_base + ruta_json);
        if (!Directory.Exists(ruta_base + ruta_xml))
            Directory.CreateDirectory(ruta_base + ruta_xml);
        if (!Directory.Exists(ruta_base + ruta_csv))
            Directory.CreateDirectory(ruta_base + ruta_csv);

        serializeInJSON = inJSON;
        if (inJSON)
        {
            serializerJSON = new JSONSerializer();
            jsonStream = new StreamWriter(ruta_base + ruta_json + id + ".json");
        }

        serializeInCSV = inCSV;
        if (inCSV)
        {
            serializerCSV = new CSVSerializer();
            csvStream = new StreamWriter(ruta_base + ruta_csv + id + ".csv");
        }

        serializeInXML = inXML;
        if (inXML)
        {
            serializerXML = new XMLSerializer();
            xmlStream = new StreamWriter(ruta_base + ruta_xml + id + ".xml");
        }
    }

    public override void Release()
    {
        Flush();
        if (jsonStream != null) jsonStream.Close();
        if (xmlStream != null) xmlStream.Close();
        if (csvStream != null) csvStream.Close();
    }

    public override void Send(TrackerEvent e)
    {
        eventsBuff.Add(e);
    }

    public override void Flush()
    {
        List<TrackerEvent> events = new List<TrackerEvent>(eventsBuff);
        eventsBuff.Clear();
        Write(events);
    }

    private void Write(List<TrackerEvent> events)
    {
        foreach (TrackerEvent e in events)
        {
            if (serializeInJSON)
            {
                string ser = serializerJSON.Serialize(e);
                jsonStream.WriteLine(ser);
            }
            if (serializeInXML)
            {
                string ser = serializerXML.Serialize(e);
                xmlStream.WriteLine(ser);
            }
            if (serializeInCSV)
            {
                string ser = serializerCSV.Serialize(e);
                csvStream.WriteLine(ser);
            }
        }
    }
}
