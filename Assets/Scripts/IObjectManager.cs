using UnityEngine;

public interface IObjectManager
{
    /// <summary>
    /// Spawns an Object within the game. Creates a new one if it can't be recycled from the object pool
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="location"></param>
    /// <param name="parentName"></param>
    /// <returns></returns>
    GameObject SpawnObject(GameObject obj, Vector3 location, string parentName = "");
    /// <summary>
    /// Frees up an object that is no longer needed and adds it to the object pool for its type
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parentName"></param>
    void FreeObject(GameObject obj, string parentName = "");
    /// <summary>
    /// Adds a new parent node for the object pool
    /// </summary>
    /// <param name="parentName"></param>
    void AddParent(string parentName);
    /// <summary>
    /// Frees all objects from a parent node
    /// </summary>
    /// <param name="parentName"></param>
    public void ClearAll(string parentName = "");
    /// <summary>
    /// Gets the parent name for an object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string GetParentName(GameObject obj);
}