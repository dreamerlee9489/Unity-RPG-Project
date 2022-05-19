﻿using UnityEngine;
using App.Config;
using App.Manager;
using App.Control;
using App.UI;

namespace App.Items
{
    public class Potion : GameItem
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ItemSlot itemSlot = UIManager.Instance.bagPanel.GetStackSlot(this);
                InventoryManager.Instance.Add(Instantiate(config.item, InventoryManager.Instance.inventory), Instantiate(config.itemUI, itemSlot.icons.transform));
                itemSlot.count.text = itemSlot.count.text == "" ? "1" : (int.Parse(itemSlot.count.text) + 1).ToString();
                Destroy(gameObject);
            }
        }

        public override void Use(CombatEntity user)
        {
            PotionConfig potionConfig = config as PotionConfig;
            user.currAtk += potionConfig.atk;
            user.currDef += potionConfig.def;
            user.currHp = Mathf.Max(user.currHp + potionConfig.hp, user.abilityConfig.hp);
            user.healthBar.UpdateBar(new Vector3(user.currHp / user.abilityConfig.hp, 1, 1));
            InventoryManager.Instance.Remove(this);
            ItemSlot itemSlot = UIManager.Instance.bagPanel.GetStackSlot(this);
            itemSlot.count.text = itemSlot.count.text == "1" ? "" : (int.Parse(itemSlot.count.text) - 1).ToString();
            Destroy(this.itemUI.gameObject);
            Destroy(this.gameObject);
        }
    }
}