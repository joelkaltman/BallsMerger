using TMPro;
using UnityEngine;

public class OptionsUI : MonoBehaviour 
{
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown shadowsDropdown;
    public TMP_Dropdown aimingDropdown;

    private void Awake()
	{
		qualityDropdown.onValueChanged.AddListener(OnChangeQuality);
		shadowsDropdown.onValueChanged.AddListener(OnChangeShadows);
		aimingDropdown.onValueChanged.AddListener(OnChangeAiming);
	}

	private void Start()
	{
		shadowsDropdown.SetValueWithoutNotify(LightsManager.Instance.Shadows ? 1 : 0);
		qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
		aimingDropdown.SetValueWithoutNotify(UserManager.Instance.AimingAutomatic ? 0 : 1);
	}

	private void OnChangeQuality(int index)
	{
		QualitySettings.SetQualityLevel (index, false);
	}
	
	private void OnChangeShadows(int index)
	{
		if (index == 0)
		{
			LightsManager.Instance.ShadowsOff();
		}
		else
		{
			LightsManager.Instance.ShadowsOn();
		}
	}
	
	private void OnChangeAiming(int index)
	{
		UserManager.Instance.SaveAutoAimingPrefs(index == 0);
	}
}