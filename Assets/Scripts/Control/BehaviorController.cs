using UnityEngine;
using UnityEngine.AI;
using App.Control.BT;

namespace App.Control
{
    [RequireComponent(typeof(MoveEntity), typeof(CombatEntity))]
    public class BehaviorController : MonoBehaviour
    {
        float wanderTimer = 6f;
        Animator animator = null;
        NavMeshAgent agent = null;
        Transform player = null;
        MoveEntity moveEntity = null;
        CombatEntity combatEntity = null;
        Selector root = new Selector();

        private void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            moveEntity = GetComponent<MoveEntity>();
            combatEntity = GetComponent<CombatEntity>();
            player = GameObject.FindWithTag("Player").transform;
            Sequence retreat = new Sequence();
            Parallel wander = new Parallel();
            Parallel chase = new Parallel();
            Condition canSeePlayer = new Condition(() =>
            {
                if (combatEntity.CanSee(player))
                {
                    agent.speed = moveEntity.abilityConfig.runSpeed * moveEntity.abilityConfig.runFactor;
                    return true;
                }
                agent.speed = moveEntity.abilityConfig.walkSpeed * moveEntity.abilityConfig.walkFactor;
                return false;
            });
            root.AddChildren(retreat, wander, chase);
            retreat.AddChildren(new Condition(() =>
            {
                return false;
            }), new Action(() =>
            {
                if (moveEntity.Flee(player.position))
                    return Status.SUCCESS;
                return Status.RUNNING;
            }), new Action(() =>
            {
                return Status.SUCCESS;
            }));
            wander.AddChildren(new UntilSuccess(canSeePlayer), new Action(() =>
            {
                wanderTimer += Time.deltaTime;
                if (wanderTimer >= 6f)
                {
                    moveEntity.Wander();
                    wanderTimer = 0;
                }
                return Status.RUNNING;
            }));
            chase.AddChildren(new UntilFailure(canSeePlayer), new Action(() =>
            {
                combatEntity.ExecuteAction(player);
                return Status.RUNNING;
            }));
        }

        private void Update()
        {
            if(!combatEntity.isDead)
                root.Execute();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, GetComponent<MoveEntity>().abilityConfig.viewRadius);
        }
    }
}
