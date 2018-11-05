using UnityEngine;
using UnityEngine.UI;

namespace SpectrumFPSExampleGame
{
	internal class PlayerUIController : MonoBehaviour
	{
		[SerializeField]
		private Slider HealthSlider;

		[SerializeField]
		private Slider ManaSlider;

		private int MaxHealth;
		private int MaxMana;

		internal void SetMaxHealth(int maxHealth, bool v)
		{
			MaxHealth = maxHealth;
			HealthSlider.maxValue = MaxHealth;
			if (v)
			{
				HealthSlider.value = MaxHealth;
			}
		}

		internal void SetMaxMana(int maxMana, bool v)
		{
			MaxMana = maxMana;
			ManaSlider.maxValue = MaxMana;
			if (v)
			{
				ManaSlider.value = MaxMana;
			}
		}

		internal void SetHealth(int Health)
		{
			HealthSlider.value = Health;
		}

		internal void SetMana(int Mana)
		{
			ManaSlider.value = Mana;
		}
	}
}