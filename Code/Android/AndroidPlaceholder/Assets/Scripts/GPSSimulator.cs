using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSSimulator {
    private List<Vector3> simulatedGPSTrack;
    private int trackIndex;
    private const int offset = 1;

    public GPSSimulator(string trackPath)
    {
        simulatedGPSTrack = new List<Vector3>();
        trackIndex = 0;

        // Read csv track

        using (var reader = new System.IO.StreamReader(trackPath))
        {
            if (!reader.EndOfStream)
            {
                reader.ReadLine();
            }
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                var values = line.Split(',');
                float[] convertedValues = new float[3];

                for(int i=0; i < 3; i++)
                {
                    convertedValues[i] = float.Parse(values[i + offset], System.Globalization.CultureInfo.InvariantCulture);
                }

                Vector3 v = new Vector3(convertedValues[0], convertedValues[1], convertedValues[2]);
                MonoBehaviour.print(v);
                simulatedGPSTrack.Add(v);
            }
        }
    }

    public Vector3 acquireData()
    {
		trackIndex = (trackIndex + 1) % simulatedGPSTrack.Count;
        return simulatedGPSTrack[trackIndex];
    }

}
