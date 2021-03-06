using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using App.Manager;

namespace App.Control
{
    [RequireComponent(typeof(Entity))]
    public class PlayerController : MonoBehaviour
    {
        RaycastHit hit;
        Animator animator = null;
        NavMeshAgent agent = null;
        Entity entity = null;
        List<Command> commands = new List<Command>();
        public Transform bag = null;
        public Transform skills = null;

        void Awake()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            entity = GetComponent<Entity>();
            commands.Add(new CombatCommand(entity));
            commands.Add(new DialogueCommand(UIManager.Instance));
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            UIManager.Instance.hudPanel.UpdatePanel();
            UIManager.Instance.attributePanel.UpdatePanel();
        }

        void Update()
        {
            if (!entity.isDead)
            {
                if (entity.target != null)
                    entity.ExecuteAction(entity.target);
                if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
                    CancelCommand();
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                    {
                        entity.CancelAction();
                        UIManager.Instance.target = null;
                        switch (hit.collider.tag)
                        {
                            case "Terrain":
                                ExecuteCommand(0, hit.point);
                                break;
                            case "Enemy":
                                ExecuteCommand(0, hit.transform);
                                agent.stoppingDistance = 1f + 0.3f * entity.target.localScale.x;
                                entity.sqrAttackRadius = Mathf.Pow(agent.stoppingDistance, 2);
                                break;
                            case "NPC":
                                ExecuteCommand(1, hit.transform);
                                agent.stoppingDistance = 1f;
                                break;
                            case "DropItem":
                                ExecuteCommand(0, hit.transform);
                                agent.stoppingDistance = 1f;
                                break;
                            case "Portal":
                                ExecuteCommand(0, hit.transform.position + new Vector3(-2.5f, 0, 0));
                                agent.stoppingDistance = 1f;
                                break;
                        }
                    }
                }
            }
        }

        void ExecuteCommand(int index, Vector3 point) => commands[index].Execute(point);
        void ExecuteCommand(int index, Transform target) => commands[index].Execute(target);
        void CancelCommand()
        {
            foreach (var command in commands)
                command.Cancel();
        }
    }
}