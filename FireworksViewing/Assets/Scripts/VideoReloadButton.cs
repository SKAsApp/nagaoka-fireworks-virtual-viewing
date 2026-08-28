// Copilot作成
using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class VideoReloadButton : UdonSharpBehaviour
{
	[Header("対象動画プレイヤー")]
	[SerializeField]
	private SynchronizedHlsVideoPlayer synchronizedVideoPlayer;

	[Header("ボタン設定")]
	[Tooltip("同じ利用者による短時間の連続操作を防ぐローカル待機時間です。")]
	[SerializeField]
	private float localButtonCooldownSeconds = 2.0f;

	private bool localButtonLocked;

	private void Start( )
	{
		localButtonLocked = false;
	}

	public override void Interact( )
	{
		if (localButtonLocked)
		{
			Debug.Log("リロードボタンは一時的に操作できません。");

			return;
		}

		if (synchronizedVideoPlayer == null)
		{
			Debug.LogError("リロード対象の動画プレイヤーが設定されていません。");

			return;
		}

		localButtonLocked = true;

		Debug.Log("このクライアントだけ動画を再読込します。");

		synchronizedVideoPlayer.RequestLocalReload( );

		SendCustomEventDelayedSeconds(nameof(UnlockLocalButton), Mathf.Max(0.5f, localButtonCooldownSeconds));
	}

	public void UnlockLocalButton( )
	{
		localButtonLocked = false;
	}
	
}
