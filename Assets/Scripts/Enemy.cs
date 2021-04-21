using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour {
    private Transform player;

    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // Update is called once per frame
    void Update() {
        if (player is null)
            return;

        agent.destination = player.position;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            GameplayController.instance.GameOver();
        }
    }
}
