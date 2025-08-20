using UnityEngine;

public class AttackState : BaseState
{
    private float moveTimer;
    private float losePlayerTimer;
    private float shotTimer;

    public override void Enter()
    {
        
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer()) {
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            shotTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform);
            if (shotTimer > enemy.fireRate) {
                Shoot();
            }
            if (moveTimer > Random.Range(3, 7)) {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                moveTimer = 0;
            }
            enemy.LastKnownPosition = enemy.Player.transform.position;
        }
        else {
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer > 4) {
                stateMachine.ChangeState(new SearchState());
            }
        }
    }

    public override void Exit() {

    }

    public void Shoot() {
        Transform gunBarrel = enemy.gunBarrel;
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/EnemyBullet") as GameObject, gunBarrel.position, enemy.transform.rotation);
        Vector3 shootDirection = (enemy.Player.transform.position - gunBarrel.transform.position).normalized;
        bullet.GetComponent<Rigidbody>().linearVelocity = Quaternion.AngleAxis(Random.Range(-1f, 1f), Vector3.up) * shootDirection * 80;
        shotTimer = 0;
    }
}
