using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : IObjectManager
{

    private Dictionary<string, GameObject> _parentTracker = new Dictionary<string, GameObject>();
    private Dictionary<string,Queue<GameObject>> _objectPool = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// Spawns an Object within the game. Creates a new one if it can't be recycled from the object pool
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="location"></param>
    /// <param name="parentName"></param>
    /// <returns></returns>
    public GameObject SpawnObject(GameObject obj,Vector3 location,string parentName = "" )
    {
        if(parentName == "")
            parentName = "Parent." + obj.name;
        if(_parentTracker.TryGetValue(parentName,out var parent) == false)
        {
            AddParent(parentName);
            parent = _parentTracker[parentName];
        } 
        if (_objectPool.TryGetValue(parentName, out var objectQueue))
        {
            if (objectQueue.Count > 0)
            {
                var nextObj = objectQueue.Dequeue();
                nextObj.transform.position = location;
                obj = nextObj;
                obj.SetActive(true);
                return obj;
            }
        }
        else
        {
            _objectPool.Add(parentName, new Queue<GameObject>());
        }

        obj = GameObject.Instantiate(obj, location, Quaternion.identity, parent.transform);
        return obj;
    }
    /// <summary>
    /// Frees up an object that is no longer needed and adds it to the object pool for its type
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parentName"></param>
    public void FreeObject(GameObject obj, string parentName = "")
    {
        if(parentName == "")
            parentName = "Parent." + obj.name;
        if (_parentTracker.TryGetValue(parentName, out var parent)) { 
            if (_objectPool.ContainsKey(parentName))
            {
                obj.SetActive(false);
                obj.transform.position = parent.transform.position;
                _objectPool[parentName].Enqueue(obj);
            }
        }
        Debug.Log("No Object named: " +obj.name + " came from the pool");
    }
    /// <summary>
    /// Adds a new parent node for the object pool
    /// </summary>
    /// <param name="parentName"></param>
    public void AddParent(string parentName)
    {
        _parentTracker.Add(parentName, new GameObject(parentName));
        _objectPool.Add(parentName, new Queue<GameObject>());      
    }
    /// <summary>
    /// Frees all objects from a parent node
    /// </summary>
    /// <param name="parentName"></param>
    public void ClearAll(string parentName = "")
    {
        if(_parentTracker.TryGetValue(parentName, out var parent))
        {
            int childCount = parent.transform.childCount;
            for(int i = 0; i < childCount; i++)
            {
                var child = parent.transform.GetChild(i);
                child.gameObject.SetActive(false);
                child.gameObject.transform.position = parent.transform.position;
                _objectPool[parentName].Enqueue(child.gameObject);
            }
        }
    }
    /// <summary>
    /// Gets the parent name for an object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string GetParentName(GameObject obj)
    {
        if(obj.transform.parent == null)
        {
            return "Parent." + obj.name;
        } 
        else 
        { 
            return obj.transform.parent.name;
        }
    }
}
