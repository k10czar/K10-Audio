

using UnityEngine;
using UnityEngine.UI;

public class ButtonSFXs : MonoBehaviour
{
	[SerializeField] SfxInstance _onClick;
	[SerializeField] Button _button;

	void Awake()
	{
		if( _button == null ) _button = GetComponent<Button>();

		_button.onClick.AddListener( Play );
	}

	void Play()
	{
		if( !isActiveAndEnabled )
			return;

		_onClick.PlaySomeClip();
	}
}