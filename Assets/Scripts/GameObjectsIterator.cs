using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsIterator : MonoBehaviour
{
    public delegate void ChildHandler(GameObject child);

    /// <summary>
    /// Iterates all children of a game object
    /// </summary>
    /// <param name="gameObject">A root game object</param>
    /// <param name="childHandler">A function to execute on each child</param>
    public static void IterateChildren(GameObject gameObject, ChildHandler childHandler)
    {
        DoIterate(gameObject, childHandler);
    }

    /// <summary>
    /// NOTE: Recursive!!!
    /// </summary>
    /// <param name="gameObject">Game object to iterate</param>
    /// <param name="childHandler">A handler function on node</param>
    private static void DoIterate(GameObject gameObject, ChildHandler childHandler)
    {
        foreach (Transform child in gameObject.transform)
        {
            childHandler(child.gameObject);
            DoIterate(child.gameObject, childHandler);
        }
    }
}
