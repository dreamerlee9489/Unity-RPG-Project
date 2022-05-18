using UnityEngine.UI;
using App.Items;

namespace App.UI
{
    public class WeaponUI : ItemUI
    {
        Text count = null;

        void Awake()
        {
            count = transform.GetChild(0).GetComponent<Text>();
            count.text = "";
        }
    }
}
