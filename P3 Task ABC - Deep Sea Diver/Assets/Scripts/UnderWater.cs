using UnityEngine;
using System.Collections;

// This script simulates an underwater effect for the level.
public class UnderWater : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        float R = 6f / 255f;                                // Initialise the R value for the colour this script will use. 
        float G = 112f /255f;                               // Initialise the G value for the colour this script will use. 
        float B = 165f / 255f;                              // Initialise the B value for the colour this script will use. 
        RenderSettings.fogColor = new Color(R, G, B, 0.7f); // Set the colour of the fog using the variables above. This will make the fog appear like being underwater.
        RenderSettings.fogDensity = 0.03f;                  // Set the density of the fog to 0.03f. To create a more realistic underwater feel. 
        RenderSettings.fog = true;                          // The fog for this script is set to being true. This will make this underwater effect enabled for the level.
    }
}
