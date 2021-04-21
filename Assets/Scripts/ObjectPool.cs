using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    [SerializeField] GameObject prefab;
    [SerializeField] int capacity;
    [SerializeField] int increments;

    private Stack<GameObject> objects;

    void Awake() {
        objects = new Stack<GameObject>(capacity);
        while (objects.Count < capacity) {
            InstantiateObject();
        }
    }

    public GameObject Get() {
        if (objects.Count == 0)
            Grow();
        return objects.Pop();
    }

    public void Put(GameObject obj) {
        objects.Push(obj);
        obj.SetActive(false);
    }

    private void InstantiateObject() {
        GameObject obj = Instantiate(prefab);
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        objects.Push(obj);
    }

    private void Grow() {
        for (int i=0; i < increments; i++) {
            InstantiateObject();
        }
        capacity += increments;
    }
}
