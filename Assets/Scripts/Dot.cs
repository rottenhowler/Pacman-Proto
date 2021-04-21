using UnityEngine;

public class Dot : MonoBehaviour {
    public Vector2Int cell;

    void OnTriggerEnter(Collider collider) {
        if (!collider.CompareTag("Player"))
            return;

        GameplayController.instance.CollectDot(this);
    }
}
