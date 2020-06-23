using UnityEngine;
using System.Collections;

public class MusicInstance : MonoBehaviour
{
	[SerializeField]MusicData _data;

	public MusicData Data { get { return _data; } }
}
