using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Developer
{
    [MenuItem("Developer/MazeStart")]
    public static void MazeStart()
    {
        GameObject alice = GameObject.Find("Alice");
        if (alice != null)
        {
            alice.transform.position = new Vector3(-0.01f, 1.26f, -6.27f);
        }

        GameObject queen = GameObject.Find("Queen");
        if (queen != null)
        {
            queen.transform.position = new Vector3(-0.21f, 0.20f, -9.4f);
        }
        Debug.Log("Place Alice and Queen to the start position");
    }

    [MenuItem("Developer/MazeEnd")]
    public static void MazeEnd()
    {
        GameObject alice = GameObject.Find("Alice");
        if (alice != null)
        {
            alice.transform.position = new Vector3(-0.01f, 1.26f, 31.38f);
        }

        GameObject queen = GameObject.Find("Queen");
        if (queen != null)
        {
            queen.transform.position = new Vector3(-0.21f, 0.2f, 26.52f);
        }
        Debug.Log("Place Alice and Queen to the end position");
    }
    
    [MenuItem("Developer/TestQueenFollow")]
    public static void TestQueenFollow()
    {
        GameObject alice = GameObject.Find("Alice");
        if (alice != null)
        {
            alice.transform.position = new Vector3(-0.01f, 1.26f, 31.38f);
        }

        GameObject queen = GameObject.Find("Queen");
        if (queen != null)
        {
            queen.transform.position = new Vector3(-0.21f, 0.20f, -9.4f);
        }
        Debug.Log("Place Alice to the end; Queen to the start position");
    }
}
