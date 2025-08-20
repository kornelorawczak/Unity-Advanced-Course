using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }
    [SerializeField] private string currentState;
    public EnemyPath path;
    private GameObject player;
    public GameObject Player { get => player; }
    private Vector3 lastKnownPosition;
    public Vector3 LastKnownPosition { get => lastKnownPosition; set => lastKnownPosition = value; }
    private float health;
    public float maxHealth = 100f;
    public PlayerHealth playerHealth;

    [Header("Sight Values")]
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight;

    [Header("Weapon Values")]
    public Transform gunBarrel;
    [Range(0.1f, 10)] public float fireRate;

    void Start() {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player");
        health = maxHealth;
    }

    void Update() {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
        if (health <= 0) {
            Die();
        }
    }

    public bool CanSeePlayer() {
        if (player != null) {
            if (Vector3.Distance(transform.position, player.transform.position) < sightDistance){
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);

                if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView) {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(ray, out hitInfo, sightDistance)) {
                        if (hitInfo.transform.gameObject == player) {
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                            return true;
                        }
                    }
                }
            }
        }
        return false;

    }

    public void TakeDamage(float ammount) {
        health -= ammount;
        transform.LookAt(Player.transform);
        stateMachine.ChangeState(new AttackState());
    }

    private void Die() {
        SoundManager.PlaySound(SoundType.ENEMY_DEATH, 0.8f);
        playerHealth.UpdateKilledText();
        Destroy(gameObject);
    }

}

