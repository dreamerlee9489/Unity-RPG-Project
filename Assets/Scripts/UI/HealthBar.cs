using UnityEngine;
using UnityEngine.UI;

namespace App
{
	public class HealthBar : MonoBehaviour
	{
		bool isShow = false;
		float timer = 6;
		public Image foreground = null;

		void Awake()
		{
			if(CompareTag("Enemy"))
				gameObject.SetActive(false);
		}

		void LateUpdate()
		{
			if(isShow)
			{
				timer -= Time.deltaTime;
				if(timer <= 0)
				{
					isShow = false;
					gameObject.SetActive(isShow);
				}
			}
			transform.forward = Camera.main.transform.forward;
		}

		public void UpdateBar(Vector3 vector)
		{
			if(CompareTag("Enemy") && !isShow)
			{
				isShow = true;
				gameObject.SetActive(isShow);
				timer = 6;
			}
			foreground.transform.localScale = vector;
		}
	}
}
