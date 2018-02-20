using System;
using System.Collections.Generic;
using System.IO;
using OsmSharp;
using OsmSharp.IO.Xml.Gpx;
using OsmSharp.IO.Xml.Sources;
using OsmSharp.Streams;
using OsmSharp.Tags;

namespace GpxToOsm
{
    class Program
    {
        static void Main(string[] args)
        {
            var nodeIds = -1;
            var wayIds = -1;

            var geos = new List<OsmGeo>();
            foreach (var gpxFile in Directory.EnumerateFiles("./data/", "*.gpx"))
            {
                using (var stream = File.OpenRead(gpxFile))
                {
                    // read gpx.
                    var xmlSource = new XmlStreamSource(stream);
                    var gpxDocument = new GpxDocument(xmlSource);

                    var gpxType = gpxDocument.Gpx as OsmSharp.IO.Xml.Gpx.v1_1.gpxType;

                    if (gpxType.trk != null)
                    {
                        foreach (var trk in gpxType.trk)
                        {
                            if (trk == null ||
                                trk.trkseg == null)
                            {
                                continue;
                            }

                            foreach (var trkseg in trk.trkseg)
                            {
                                var nodeLocations = new List<Itinero.LocalGeo.Coordinate>();
                                if (trkseg.trkpt == null)
                                {
                                    continue;
                                }

                                foreach (var wpt in trkseg.trkpt)
                                {
                                    nodeLocations.Add(
                                        new Itinero.LocalGeo.Coordinate((float)wpt.lat, 
                                            (float)wpt.lon));

                                }

                                var nodeLocationArray = Itinero.LocalGeo.Extensions.Simplify(
                                    nodeLocations.ToArray(), 1);

                                var nodes = new List<long>();
                                foreach (var nodeLocation in nodeLocationArray)
                                {
                                    var node = new Node()
                                    {
                                        Id = nodeIds,
                                        Latitude = (float)nodeLocation.Latitude,
                                        Longitude = (float)nodeLocation.Longitude,
                                        UserId = 0,
                                        UserName = "Missing Maps",
                                        ChangeSetId = -1
                                    };
                                    nodes.Add(nodeIds);
                                    nodeIds--;

                                    geos.Add(node);
                                }

                                if (nodes.Count > 0)
                                {
                                    var way = new Way()
                                    {
                                        Id = wayIds,
                                        Nodes = nodes.ToArray(),
                                        UserId = 0,
                                        UserName = "Missing Maps",
                                        ChangeSetId = -1
                                    };
                                    wayIds--;

                                    geos.Add(way);
                                }
                            }
                        }
                    }

                    if (gpxType.wpt != null)
                    {
                        foreach (var wpt in gpxType.wpt)
                        {
                            var node = new Node()
                            {
                                Id = nodeIds,
                                Latitude = (float)wpt.lat,
                                Longitude = (float)wpt.lon,
                                Tags = new TagsCollection(
                                    new Tag("name", wpt.name)
                                ),
                                UserId = 0,
                                UserName = "Missing Maps",
                                ChangeSetId = -1
                            };
                            nodeIds--;

                            geos.Add(node);
                        }
                    }
                }
            }

            geos.Sort((x, y) =>
            {
                    if (x.Type == y.Type)
                    {
                        return x.Id.Value.CompareTo(y.Id.Value);
                    }
                    if (x.Type == OsmSharp.OsmGeoType.Node)
                    {
                        return -1;
                    }
                    else if (x.Type == OsmSharp.OsmGeoType.Way)
                    {
                        if (y.Type == OsmSharp.OsmGeoType.Node)
                        {
                            return 1;
                        }
                            return -1;
                    }
                    return 1;
            });

            using (var targetStream = File.Open("output.osm", FileMode.Create))
            {
                var target = new OsmSharp.Streams.XmlOsmStreamTarget(targetStream);
                target.RegisterSource(geos);
                target.Pull();
                target.Flush();
            }
        }
    }
}
