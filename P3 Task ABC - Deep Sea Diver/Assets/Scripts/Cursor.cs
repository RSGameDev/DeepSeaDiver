using UnityEngine;
using System.Collections;

// This script controls the functionality for the mouse cursor.
public class Cursor : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        UnityEngine.Cursor.visible = false;         // The cursor for the mouse will become invisible. While playing the game the mouse cursor is not required.
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0);   // The mouse cursor is set to be positioned in the centre of the screen.
    }
}
