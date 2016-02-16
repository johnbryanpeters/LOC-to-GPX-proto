using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// jbp

namespace HelloWorld
{
    // The premise of the LocFile01 class is that, given a byte array created 
    // by reading a .LOC file, the class extracts and makes available all 
    // data that we know how to extract.
    class LocFile01
    {
        const string HEADER_STANDARD = "TerraByte Location File Copyright 2003 TopoGrafix";
        const int HEADER_LENGTH = 50;
        const int TRACKPOINT_LENGTH = 36;
        // Known data in trackpoint record:
        const int TRACKPOINT_LATITUDE = 8;
        const int TRACKPOINT_LONGITUDE = 17;
        const int TRACKPOINT_ELEVATION = 25; // Putative; haven't validated.

        int cursor = 0;
        int locByteCount = 0;
        
        public LocFile01(byte[] arrayToParse)
        {
            // Note: Every time we finish extracting information from 
            // arrayToParse we remove the processed segment from the array.
            locByteCount = arrayToParse.Length;

            // Check header. Store if valid.
            Header = ExtractHeader(ref arrayToParse);
            if (Header != HEADER_STANDARD)
            {
                throw new InvalidOperationException ("No valid header found in .LOC file.");
            }

            // Get waypoints.
            Waypoints = GetWaypoints(ref arrayToParse, ref _waypoints);

            // Get tracks.
            GetTrack(ref arrayToParse, ref _tracks);
        }

        public string Header
        {
            private set;
            get;
        }

        public class Waypoint
        {
            public string NameOnGPS
            {
                set;
                get;
            }
            public string LabelOnMap
            {
                set;
                get;
            }
            public double Latitude
            {
                set;
                get;
            }
            public double Longitude
            {
                set;
                get;
            }
        } // End class Waypoint

        private List<Waypoint> _waypoints = new List<Waypoint>();
        public List<Waypoint> Waypoints
        {
            private set { _waypoints= value; }
            get { return _waypoints; }
        }

        public class TrackPoint
        {
            public double Latitude
            {
                set;
                get;
            }
            public double Longitude
            {
                set;
                get;
            }
            public int Elevation
            {
                set;
                get;
            }
        } // End class Trackpoint
        public class Track
        {
            public string NameOnGps
            {
                set;
                get;
            }
            public string LabelOnMap
            {
                set;
                get;
            }
            private List<TrackPoint> _trackpoints = new List<TrackPoint>();
            public List<TrackPoint> Trackpoints
            {
                set { _trackpoints = value; }
                get { return _trackpoints; }
            }
        } // End class Track

        private List<Track> _tracks = new List<Track>();
        public List<Track> Tracks
        {
            private set { value = _tracks; }
            get { return _tracks; }
        }

        string ExtractHeader(ref byte[] arrayToParse)
        {
            if(arrayToParse.Length >= HEADER_LENGTH)
            {
                string header = "";
                while(cursor < HEADER_LENGTH - 1)
                {
                    // Probably want to throw an exception if we
                    // trip across a byte that does not map to char.
                    header = header + (char)arrayToParse[cursor];
                    cursor++;
                }
                // Done, so we chop the header off.
                arrayToParse = Extensions.Right(arrayToParse, HEADER_LENGTH, arrayToParse.Length - HEADER_LENGTH);
                cursor = 0;
                return header;
            }
            else
            {
                throw new InvalidOperationException(".LOC file not big enough to contain header.");
            }
        } // End ExtractHeader()

        List<Waypoint> GetWaypoints(ref byte[] arrayToParse, ref List<Waypoint> waypoints)
        {
            Waypoint waypoint = GetWaypoint(ref arrayToParse);
            while(waypoint != null)
            {
                waypoints.Add(waypoint);
                waypoint = GetWaypoint(ref arrayToParse);
            }
            return waypoints;
        }

        static Waypoint GetWaypoint(ref byte[] arrayToParse)
        {
            Waypoint waypoint = new Waypoint();
            int cursor = 0;
            int cursorW = 0;
            int cursorWaypoint = 0;
            // Want a searchable string in which we look for data placeholders.
            string parseMe = Encoding.UTF8.GetString(arrayToParse, 0, arrayToParse.Length);

            // Waypoints are found by finding, in order, 
            // The character 'W'
            cursorW = parseMe.IndexOf("W");
            // - The character sequence 'Waypoint'
            cursorWaypoint = parseMe.IndexOf("Waypoint");
            // If we find both
            if ((cursorW > 0) &&
                (arrayToParse[cursorW + 1] != 0x8) &&
                (cursorWaypoint > 0))
            {
                // Start two bytes after 'W'
                cursor = cursorW + 3;
                // Build NameOnGPS until we find x08.
                while (arrayToParse[cursor] != 0x8)
                {
                    waypoint.NameOnGPS = waypoint.NameOnGPS + (char)arrayToParse[cursor];
                    cursor++;
                }
                // Skip x08 
                cursor++;
                // Build LabelOnMap until we find x00 07 08.
                while (arrayToParse[cursor] != 0x0)
                {
                    waypoint.LabelOnMap = waypoint.LabelOnMap + (char)arrayToParse[cursor];
                    cursor++;
                }
                // Byte sequence 'Waypoint'
                // Six bytes, always  x84 56 00 00 00 63 
                // Skip x00 07 08, "Waypoint", and x84 56 00 00 00 63.
                cursor = cursor + 3 + 8 + 6;
                // The eight byte double Latitude 
                waypoint.Latitude = BitConverter.ToDouble(arrayToParse, (int)cursor);
                cursor = cursor + 8;
                // One byte, char 'd'
                cursor++;
                // Eight byte double Longitude
                waypoint.Longitude = BitConverter.ToDouble(arrayToParse, (int)cursor);
                cursor = cursor + 8;
            }
            else
            {
                // Didn't find a waypoint
                waypoint = null;
            }
            // Done, so we chop the waypoint off the source.
            arrayToParse = Extensions.Right(arrayToParse, cursorWaypoint + 8, arrayToParse.Length - cursorWaypoint - 8);
            // *** Debug ***
            parseMe = Encoding.UTF8.GetString(arrayToParse, 0, arrayToParse.Length);
            return waypoint;
        } // End GetWaypoint()

        static void GetTracks(ref byte[] arrayToParse)
        {
            // *** This end test fails if input contains routes.
            while(arrayToParse.Length > TRACKPOINT_LENGTH)
            {
                LocFile01.
            }
        } // End GetTracks()

        static Track GetTrack(ref byte[] arrayToParse, ref LocFile01.Track[] tracks)
        {
            Track track = new Track();
            int cursor = 0;
            int cursor1 = 0;
            // Want a searchable string in which we look for data.
            string parseMe = Encoding.UTF8.GetString(arrayToParse, 0, arrayToParse.Length);

            // 0xFF 52/R 21/! 0D seems to be identifier.
            cursor = parseMe.IndexOf("R!") + 3;
            // Name on GPS is terminated by 0x26/&.
            cursor1 = parseMe.IndexOf("&");
            track.NameOnGps = parseMe.Substring(cursor, cursor1 - cursor);
            cursor1++;
            arrayToParse = Extensions.Right(arrayToParse, cursor1, arrayToParse.Length - cursor1);
            parseMe = Encoding.UTF8.GetString(arrayToParse, 0, arrayToParse.Length);
            cursor = 0;
            // Label on map seems to be terminated by 0x00.
            while (arrayToParse[cursor] != 0x0)
            {
                track.LabelOnMap = track.LabelOnMap + (char)arrayToParse[cursor];
                cursor++;
            }
            cursor++;
            arrayToParse = Extensions.Right(arrayToParse, cursor1, arrayToParse.Length - cursor1);
            // Get rid of eight bytes we don't know what to do with.
            arrayToParse = Extensions.Right(arrayToParse, 8, arrayToParse.Length - 8);
            parseMe = Encoding.UTF8.GetString(arrayToParse, 0, arrayToParse.Length);
            cursor = 0;
            GetTrackPoints(ref arrayToParse, ref track);
            return track;
        } // End GetTrack()

        static void GetTrackPoints(ref byte[] arrayToParse, ref Track track)
        {
            int cursor = TRACKPOINT_LATITUDE;
            TrackPoint trackpoint = new TrackPoint();

            // *** This end test fails if input contains routes.
            while(arrayToParse.Length > TRACKPOINT_LENGTH)
            {
                trackpoint.Latitude = BitConverter.ToDouble(arrayToParse, cursor);
                cursor = TRACKPOINT_LONGITUDE;
                trackpoint.Longitude = BitConverter.ToDouble(arrayToParse, cursor);
                // *** Wasn't really getting elevation - don't know where it is.
                //cursor = TRACKPOINT_ELEVATION;
                //trackpoint.Elevation = BitConverter.ToInt16(arrayToParse, cursor);
                arrayToParse = Extensions.Right(arrayToParse, TRACKPOINT_LENGTH, arrayToParse.Length - TRACKPOINT_LENGTH);

                track.Trackpoints.Add(trackpoint);
            }
            return;
        } // End GetTrackPoints()
    } // End class LocFile01
} // End namespace HelloWorld
