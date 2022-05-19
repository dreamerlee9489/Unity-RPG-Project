using System;
using System.Collections.Generic;
using UnityEngine;
using App.Config;
using App.Manager;

namespace App.Control
{
    [System.Serializable]
    public class Quest
    {
        public string name = "";
        public string description = "";
        public string target { get; set; }
        public int current = 0, total = 1;
        public bool isCompleted = false;
        public Dictionary<string, int> rewards { get; set; }
        public NPCController npc { get; set; }

        public void UpdateProgress(int count)
        {
            current += count;
            UIManager.Instance.questPanel.UpdateQuest(this);
            if (current >= total && !isCompleted)
                npc.CompleteQuest(this);
        }
    }

    [RequireComponent(typeof(MoveEntity))]
    public class NPCController : MonoBehaviour
    {
        public DialoguesConfig dialoguesConfig = null;
        public List<Quest> quests = new List<Quest>();
        public Dictionary<string, Action> actions = new Dictionary<string, Action>();
        public int index { get; set; }

        void Awake()
        {
            actions.Add("GiveQuest_KillUndeadKnight", () =>
            {
                GiveQuest("不死骑士", null);
            });
            actions.Add("GiveReward_KillUndeadKnight", () =>
            {
                GiveReward();
            });
        }

        void GiveQuest(string target, Dictionary<string, int> rewards)
        {
            quests[index].npc = this;
            quests[index].target = target;
            quests[index].rewards = rewards;
            GameManager.Instance.ongoingQuests.Add(quests[index]);
            UIManager.Instance.questPanel.Add(quests[index]);
            GameManager.Instance.entities[target].GetComponent<CombatEntity>().isQuestTarget = true;
            dialoguesConfig = Resources.LoadAsync("Config/Dialogue/DialoguesConfig_KillUndeadKnight_Accept").asset as DialoguesConfig;
            Debug.Log("领取任务: " + quests[index].name);
        }

        void GiveReward()
        {
            quests[index].current -= quests[index].total;
            GameManager.Instance.ongoingQuests.Remove(quests[index]);
            UIManager.Instance.questPanel.Remove(quests[index]);
            GameManager.Instance.entities[quests[index].target].GetComponent<CombatEntity>().isQuestTarget = false;
            dialoguesConfig = Resources.LoadAsync("Config/Dialogue/DialoguesConfig_KillUndeadKnight_Start").asset as DialoguesConfig;
            Debug.Log("获取奖励: " + quests[index].name);
            index = 0;
        }

        public void CompleteQuest(Quest quest)
        {
            quest.isCompleted = true;
            dialoguesConfig = Resources.LoadAsync("Config/Dialogue/DialoguesConfig_KillUndeadKnight_Submit").asset as DialoguesConfig;
            Debug.Log("任务已完成: " + quest.current);
        }

        public void ActionTrigger(string action)
        {
            if (actions.ContainsKey(action))
                actions[action].Invoke();
        }
    }
}
