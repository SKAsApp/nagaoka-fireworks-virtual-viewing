// Copilot作成
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components.Base;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SynchronizedHlsVideoPlayer : UdonSharpBehaviour
{
	private const int RECORDED_MODE = 0;
	private const int LIVE_DVR_MODE = 1;

	[Header("基本設定")]
	[SerializeField]
	private BaseVRCVideoPlayer videoPlayer;

	[Header("再生モード")]
	[Tooltip("0は録画HLS、1はDVR対応ライブHLSです。")]
	[Range(0, 1)]
	[SerializeField]
	private int playbackMode = RECORDED_MODE;

	[Header("固定URL")]
	[SerializeField]
	private VRCUrl recordedHlsUrl;

	[SerializeField]
	private VRCUrl liveDvrHlsUrl;

	[Header("初期安定化")]
	[Tooltip("入室後、動画URLの読み込みを始めるまでの最短待機時間です。")]
	[SerializeField]
	private float initialNetworkWaitSeconds = 5.0f;

	[Tooltip("動画準備完了後、最初の同期まで待つ時間です。")]
	[SerializeField]
	private float videoReadyWaitSeconds = 1.5f;

	[Tooltip("所有権移譲後、新オーナーが同期を引き継ぐまでの待機時間です。")]
	[SerializeField]
	private float ownershipStabilizationSeconds = 1.0f;

	[Header("同期設定")]
	[Tooltip("オーナーが同期アンカーを更新する間隔です。")]
	[SerializeField]
	private float synchronizationIntervalSeconds = 15.0f;

	[Tooltip("通常同期では、この秒数以内のずれを補正しません。初回同期とローカル再読込後は例外です。")]
	[SerializeField]
	private float ignoredDifferenceSeconds = 10.0f;

	[Tooltip("動画末尾ぴったりへのシークを避けるための余白です。")]
	[SerializeField]
	private float maximumSeekPositionMarginSeconds = 1.0f;

	[Header("ローカル再読込設定")]
	[Tooltip("停止後、同じURLを再読込するまでの待機時間です。")]
	[SerializeField]
	private float localReloadDelaySeconds = 0.5f;

	[Tooltip("同じ利用者が再びローカル再読込できるまでの時間です。5秒未満にはなりません。")]
	[SerializeField]
	private float localReloadCooldownSeconds = 10.0f;

	[Tooltip("オーナーがローカル再同期した後、共有アンカー更新を再開するまでの待機時間です。")]
	[SerializeField]
	private float ownerResumeDelaySeconds = 1.0f;

	[Tooltip("ローカル再読込状態を強制解除するまでの時間です。")]
	[SerializeField]
	private float localReloadTimeoutSeconds = 45.0f;

	[Header("シーク確認設定")]
	[Tooltip("録画HLSのシーク結果を確認するまでの待機時間です。")]
	[SerializeField]
	private float seekVerificationDelaySeconds = 1.0f;

	[Tooltip("録画HLSのシークを再試行する最大回数です。")]
	[SerializeField]
	private int maximumSeekRetryCount = 3;

	[Tooltip("シーク成功と判断する再生位置の許容差です。")]
	[SerializeField]
	private float seekVerificationToleranceSeconds = 3.0f;

	[Header("エラー再試行設定")]
	[SerializeField]
	private float retryIntervalSeconds = 10.0f;

	/// <summary>動画の読み込みを再試行する最大回数です。</summary>
	[SerializeField]
	private int maximumRetryCount = 10;

	/// <summary>共有同期状態が初期化済みかどうかを示します。</summary>
	[UdonSynced]
	private bool synchronizedInitialized;

	/// <summary>共有されている動画の再生状態を示します。</summary>
	[UdonSynced]
	private bool synchronizedPlaying;

	/// <summary>共有されている動画の再生モードを示します。</summary>
	[UdonSynced]
	private int synchronizedPlaybackMode;

	/// <summary>同期アンカーを更新した時点の共有再生位置を秒単位で保持します。</summary>
	[UdonSynced]
	private float synchronizedMediaTime;

	/// <summary>同期アンカーを更新した時点のサーバー時刻を保持します。</summary>
	[UdonSynced]
	private double synchronizedServerTime;

	/// <summary>共有同期状態が更新された世代を識別する番号です。</summary>
	[UdonSynced]
	private int synchronizedGeneration;

	/// <summary>このクライアントで初期化処理が開始済みかどうかを示します。</summary>
	private bool localInitializationStarted;

	/// <summary>このクライアントで動画URLの読み込みを要求済みかどうかを示します。</summary>
	private bool localUrlRequested;

	/// <summary>このクライアントで動画の準備が完了しているかどうかを示します。</summary>
	private bool localVideoReady;

	/// <summary>このクライアントで動画の再生が開始されているかどうかを示します。</summary>
	private bool localVideoStarted;

	/// <summary>このクライアントで共有状態に基づく同期を適用済みかどうかを示します。</summary>
	private bool localSynchronizationApplied;

	/// <summary>このクライアントでオーナー用の定期同期処理を開始済みかどうかを示します。</summary>
	private bool ownerSynchronizationLoopStarted;

	/// <summary>このクライアントで動画の再読込開始を待機しているかどうかを示します。</summary>
	private bool localReloadPending;

	/// <summary>このクライアントで動画を再読込した後に共有状態との同期が必要かどうかを示します。</summary>
	private bool localReloadRequiresSynchronization;

	/// <summary>このクライアントで同期シークの結果確認を待機しているかどうかを示します。</summary>
	private bool localSeekVerificationPending;

	/// <summary>実行中の同期シークがローカル再読込後の同期処理によるものかどうかを示します。</summary>
	private bool localSeekVerificationForReload;

	/// <summary>このクライアントで動画の読み込みを再試行した回数を保持します。</summary>
	private int localRetryCount;

	/// <summary>このクライアントで同期位置へのシークを再試行した回数を保持します。</summary>
	private int localSeekRetryCount;

	/// <summary>このクライアントで確認対象となっているシーク先の再生位置を秒単位で保持します。</summary>
	private float localSeekTargetMediaTime;

	/// <summary>このクライアントへ最後に適用した共有同期状態の世代番号を保持します。</summary>
	private int localAppliedGeneration;

	/// <summary>このクライアントで初期化処理を開始した時点のサーバー時刻を保持します。</summary>
	private double initializationStartedServerTime;

	/// <summary>このクライアントでローカル再読込のクールダウン管理を開始済みかどうかを示します。</summary>
	private bool localReloadCooldownStarted;

	/// <summary>このクライアントで最後にローカル再読込を開始した時点のサーバー時刻を保持します。</summary>
	private double lastLocalReloadServerTime;

	/// <summary>現在のローカル再読込処理を開始した時点のサーバー時刻を保持します。</summary>
	private double localReloadStartedServerTime;

	/// <summary>
	/// ローカル状態を初期化し、ネットワーク安定後の初期化処理を予約します。
	/// </summary>
	private void Start( )
	{
		localInitializationStarted = false;
		localUrlRequested = false;
		localVideoReady = false;
		localVideoStarted = false;
		localSynchronizationApplied = false;
		ownerSynchronizationLoopStarted = false;
		localReloadPending = false;
		localReloadRequiresSynchronization = false;
		localSeekVerificationPending = false;
		localSeekVerificationForReload = false;
		localRetryCount = 0;
		localSeekRetryCount = 0;
		localSeekTargetMediaTime = 0.0f;
		localAppliedGeneration = -1;
		localReloadCooldownStarted = false;
		lastLocalReloadServerTime = 0.0;
		localReloadStartedServerTime = 0.0;

		initializationStartedServerTime = Networking.GetServerTimeInSeconds( );

		SendCustomEventDelayedSeconds(nameof(CheckInitialStability), Mathf.Max(0.1f, initialNetworkWaitSeconds));
	}

	/// <summary>
	/// ネットワークと待機時間の条件を確認し、動画の初期読み込みを開始します。
	/// </summary>
	public void CheckInitialStability( )
	{
		if (localInitializationStarted)
		{
			return;
		}
		// ローカル利用者情報が未確定の場合はネットワーク状態を判断できないため、初期化を延期します。
		if (!Utilities.IsValid(Networking.LocalPlayer))
		{
			SendCustomEventDelayedSeconds(nameof(CheckInitialStability), 1.0f);
			return;
		}
		// ネットワーク同期が安定する前にURLを読み込むと初期状態が競合するため、初期化を延期します。
		if (!Networking.IsNetworkSettled)
		{
			SendCustomEventDelayedSeconds(nameof(CheckInitialStability), 1.0f);
			return;
		}

		double currentServerTime = Networking.GetServerTimeInSeconds( );
		double elapsedSeconds = Networking.CalculateServerDeltaTime(currentServerTime, initializationStartedServerTime);
		float requiredWaitSeconds = Mathf.Max(0.0f, initialNetworkWaitSeconds);

		if (elapsedSeconds >= 0.0 && elapsedSeconds < requiredWaitSeconds)
		{
			float remainingSeconds = requiredWaitSeconds - (float)elapsedSeconds;
			SendCustomEventDelayedSeconds(nameof(CheckInitialStability), Mathf.Max(0.5f, remainingSeconds));
			return;
		}

		localInitializationStarted = true;

		if (Networking.IsOwner(gameObject) && !synchronizedInitialized)
		{
			InitializeSynchronizedState( );
		}

		LoadConfiguredUrl( );
	}

	/// <summary>
	/// オーナーが共有同期状態を初期化してシリアライズします。
	/// </summary>
	private void InitializeSynchronizedState( )
	{
		// 非オーナーが共有状態を更新すると競合するため、この処理を行いません。
		if (!Networking.IsOwner(gameObject))
		{
			return;
		}

		synchronizedInitialized = true;
		synchronizedPlaying = true;
		synchronizedPlaybackMode = NormalizePlaybackMode(playbackMode);
		synchronizedMediaTime = 0.0f;
		synchronizedServerTime = Networking.GetServerTimeInSeconds( );
		synchronizedGeneration++;

		RequestSerialization( );
	}

	/// <summary>
	/// 指定された再生モードを対応済みの値へ正規化します。
	/// </summary>
	private int NormalizePlaybackMode(int requestedPlaybackMode)
	{
		if (requestedPlaybackMode == LIVE_DVR_MODE)
		{
			return LIVE_DVR_MODE;
		}
		return RECORDED_MODE;
	}

	/// <summary>
	/// 現在の同期状態に対応する固定HTTP Live Streaming URLを読み込みます。
	/// </summary>
	private void LoadConfiguredUrl( )
	{
		// 同じURLの重複読み込みを避けるため、この処理を行いません。
		if (localUrlRequested)
		{
			return;
		}
		// 動画プレイヤー未設定時は後続処理を実行できないため、この処理を行いません。
		if (!Utilities.IsValid(videoPlayer))
		{
			Debug.LogError("動画プレイヤーが設定されていません。");
			return;
		}

		VRCUrl selectedUrl = GetSelectedUrl( );
		string selectedUrlText = selectedUrl.Get( );

		// 再生URLが空の場合は読み込みに失敗するため、この処理を行いません。
		if (string.IsNullOrEmpty(selectedUrlText))
		{
			Debug.LogError("選択した再生モードのHLS固定URLが設定されていません。");
			return;
		}

		localUrlRequested = true;
		localVideoReady = false;
		localVideoStarted = false;
		Debug.Log("HLS動画の読み込みを開始します。");
		videoPlayer.PlayURL(selectedUrl);
	}

	/// <summary>
	/// 現在選択されている再生モードに対応するURLを返します。
	/// </summary>
	private VRCUrl GetSelectedUrl( )
	{
		int selectedMode = NormalizePlaybackMode(playbackMode);
		if (synchronizedInitialized)
		{
			selectedMode = NormalizePlaybackMode(synchronizedPlaybackMode);
		}
		if (selectedMode == LIVE_DVR_MODE)
		{
			return liveDvrHlsUrl;
		}
		return recordedHlsUrl;
	}

	/// <summary>
	/// 動画の準備完了を記録し、初回同期処理を予約します。
	/// </summary>
	public override void OnVideoReady( )
	{
		localVideoReady = true;
		localRetryCount = 0;
		Debug.Log("HLS動画の準備が完了しました。");
		SendCustomEventDelayedSeconds(nameof(ApplySynchronizationAfterReady), Mathf.Max(0.0f, videoReadyWaitSeconds));
	}

	/// <summary>
	/// 動画準備完了後に、所有権と再読込状態に応じた同期を適用します。
	/// </summary>
	public void ApplySynchronizationAfterReady( )
	{
		// 動画の準備前は再生位置を操作できないため、この処理を行いません。
		if (!localVideoReady)
		{
			return;
		}

		if (!synchronizedInitialized && !Networking.IsOwner(gameObject))
		{
			SendCustomEventDelayedSeconds(nameof(ApplySynchronizationAfterReady), 1.0f);
			return;
		}
		if (!synchronizedInitialized)
		{
			InitializeSynchronizedState( );
		}
		if (localReloadRequiresSynchronization)
		{
			ApplySynchronizationAfterLocalReload( );
			return;
		}
		if (Networking.IsOwner(gameObject))
		{
			UpdateOwnerAnchor( );
			StartOwnerSynchronizationLoop( );
			return;
		}
		ApplyRemoteSynchronization( );
	}

	/// <summary>
	/// 動画の再生開始を記録し、所有権に応じた同期処理を開始します。
	/// </summary>
	public override void OnVideoStart( )
	{
		localVideoReady = true;
		localVideoStarted = true;
		Debug.Log("HLS動画の再生を開始しました。");
		if (localReloadRequiresSynchronization)
		{
			SendCustomEventDelayedSeconds(nameof(ApplySynchronizationAfterReady), Mathf.Max(0.1f, videoReadyWaitSeconds));
			return;
		}
		if (Networking.IsOwner(gameObject))
		{
			UpdateOwnerAnchor( );
			StartOwnerSynchronizationLoop( );
			return;
		}
		SendCustomEventDelayedSeconds(nameof(ApplyRemoteSynchronization), Mathf.Max(0.1f, videoReadyWaitSeconds));
	}

	/// <summary>
	/// 再生状態を記録し、オーナーの場合は共有同期アンカーを更新します。
	/// </summary>
	public override void OnVideoPlay( )
	{
		localVideoReady = true;
		localVideoStarted = true;

		if (localReloadPending)
		{
			return;
		}
		if (localReloadRequiresSynchronization)
		{
			SendCustomEventDelayedSeconds(nameof(ApplySynchronizationAfterReady), Mathf.Max(0.1f, videoReadyWaitSeconds));
			return;
		}
		// 非オーナーが共有状態を更新すると競合するため、この処理を行いません。
		if (!Networking.IsOwner(gameObject))
		{
			return;
		}

		synchronizedPlaying = true;
		UpdateOwnerAnchor( );
	}

	/// <summary>
	/// 一時停止状態と再生位置を共有同期状態へ反映します。
	/// </summary>
	public override void OnVideoPause( )
	{
		localVideoStarted = false;

		// ローカル再読込中に通常処理を行うと状態が競合するため、この処理を行いません。
		if (localReloadPending || localReloadRequiresSynchronization)
		{
			return;
		}

		// 非オーナーが共有状態を更新すると競合するため、この処理を行いません。
		if (!Networking.IsOwner(gameObject))
		{
			return;
		}

		synchronizedPlaying = false;
		synchronizedMediaTime = videoPlayer.GetTime( );
		synchronizedServerTime = Networking.GetServerTimeInSeconds( );

		RequestSerialization( );
	}

	/// <summary>
	/// 共有同期状態の受信後に、ローカル動画へ同期を適用します。
	/// </summary>
	public override void OnDeserialization( )
	{
		if (!localInitializationStarted || !synchronizedInitialized)
		{
			return;
		}
		if (!localUrlRequested)
		{
			if (!localReloadPending)
			{
				LoadConfiguredUrl( );
			}
			return;
		}
		if (!localVideoReady || localReloadPending)
		{
			return;
		}
		if (localReloadRequiresSynchronization)
		{
			ApplySynchronizationAfterLocalReload( );
			return;
		}

		ApplyRemoteSynchronization( );
	}

	/// <summary>
	/// 非オーナーの再生位置と再生状態を共有同期状態へ合わせます。
	/// </summary>
	public void ApplyRemoteSynchronization( )
	{
		if (Networking.IsOwner(gameObject))
		{
			return;
		}
		if (!localVideoReady || !synchronizedInitialized || localReloadPending)
		{
			return;
		}

		float targetMediaTime = CalculateTargetMediaTime( );
		float currentMediaTime = videoPlayer.GetTime( );
		float differenceSeconds = Mathf.Abs(targetMediaTime - currentMediaTime);
		bool generationChanged = localAppliedGeneration != synchronizedGeneration;
		bool firstSynchronization = !localSynchronizationApplied;
		bool requiresCorrection = differenceSeconds > ignoredDifferenceSeconds;

		if (!firstSynchronization && !generationChanged && !requiresCorrection)
		{
			Debug.Log("同期誤差が許容範囲内のため、再生位置を変更しません。差: " + differenceSeconds + "秒");
		}
		if ((firstSynchronization || generationChanged || requiresCorrection) && !localSeekVerificationPending)
		{
			Debug.Log("動画を同期します。現在位置: " + currentMediaTime + "秒、目標位置: " + targetMediaTime + "秒、差: " + differenceSeconds + "秒");
			StartVerifiedSynchronizationSeek(targetMediaTime, false);
		}
		if (synchronizedPlaying && !localVideoStarted)
		{
			videoPlayer.Play( );
		}
		if (!synchronizedPlaying && localVideoStarted)
		{
			videoPlayer.Pause( );
		}
	}

	/// <summary>
	/// ローカル再読込後の動画へ共有同期位置を適用します。
	/// </summary>
	private void ApplySynchronizationAfterLocalReload( )
	{
		if (!localVideoReady || !synchronizedInitialized || localReloadPending || localSeekVerificationPending)
		{
			return;
		}

		float targetMediaTime = CalculateTargetMediaTime( );
		Debug.Log("ローカル再読込後の同期位置を適用します。目標位置: " + targetMediaTime + "秒");
		StartVerifiedSynchronizationSeek(targetMediaTime, true);
	}

	/// <summary>
	/// 同期位置へのシークを開始し、結果確認を予約します。
	/// </summary>
	private void StartVerifiedSynchronizationSeek(float targetMediaTime, bool synchronizationAfterReload)
	{
		// 動画の準備前は再生位置を操作できないため、この処理を行いません。
		if (!localVideoReady)
		{
			return;
		}

		float safeTargetMediaTime = GetSafeSeekPosition(targetMediaTime);
		localSeekTargetMediaTime = safeTargetMediaTime;
		localSeekRetryCount = 0;
		localSeekVerificationPending = true;
		localSeekVerificationForReload = synchronizationAfterReload;
		Debug.Log("同期位置へのシークを開始します。目標位置: " + safeTargetMediaTime + "秒");
		videoPlayer.SetTime(safeTargetMediaTime);

		if (synchronizedPlaying)
		{
			videoPlayer.Play( );
		}

		SendCustomEventDelayedSeconds(nameof(VerifySynchronizationSeek), Mathf.Max(0.2f, seekVerificationDelaySeconds));
	}

	/// <summary>
	/// シーク結果を確認し、必要に応じて同期位置へのシークを再試行します。
	/// </summary>
	public void VerifySynchronizationSeek( )
	{
		// 確認待ちのシークがない場合は結果を評価できないため、この処理を行いません。
		if (!localSeekVerificationPending)
		{
			return;
		}
		// 動画の準備前は再生位置を操作できないため、この処理を行いません。
		if (!localVideoReady)
		{
			SendCustomEventDelayedSeconds(nameof(VerifySynchronizationSeek), Mathf.Max(0.2f, seekVerificationDelaySeconds));
			return;
		}

		float currentMediaTime = videoPlayer.GetTime( );
		float differenceSeconds = Mathf.Abs(localSeekTargetMediaTime - currentMediaTime);
		if (differenceSeconds <= Mathf.Max(0.5f, seekVerificationToleranceSeconds))
		{
			CompleteVerifiedSynchronizationSeek( );
			return;
		}
		if (localSeekRetryCount >= maximumSeekRetryCount)
		{
			Debug.LogWarning("同期位置へのシークが許容差へ収まりませんでした。現在位置: " + currentMediaTime + "秒、目標位置: " + localSeekTargetMediaTime + "秒、差: " + differenceSeconds + "秒");

			CompleteVerifiedSynchronizationSeek( );
			return;
		}

		localSeekRetryCount++;
		localSeekTargetMediaTime = GetSafeSeekPosition(CalculateTargetMediaTime( ));
		Debug.Log("同期位置へのシークを再試行します。回数: " + localSeekRetryCount + "、現在位置: " + currentMediaTime + "秒、目標位置: " + localSeekTargetMediaTime + "秒");
		videoPlayer.SetTime(localSeekTargetMediaTime);

		SendCustomEventDelayedSeconds(nameof(VerifySynchronizationSeek), Mathf.Max(0.2f, seekVerificationDelaySeconds));
	}

	/// <summary>
	/// 確認済みシークを完了し、再生状態と再読込状態を整えます。
	/// </summary>
	private void CompleteVerifiedSynchronizationSeek( )
	{
		bool synchronizationAfterReload = localSeekVerificationForReload;
		localSeekVerificationPending = false;
		localSeekVerificationForReload = false;
		localSeekRetryCount = 0;
		localSynchronizationApplied = true;
		localAppliedGeneration = synchronizedGeneration;

		if (synchronizationAfterReload)
		{
			localReloadPending = false;
			localReloadRequiresSynchronization = false;
			Debug.Log("ローカル再読込後の同期処理が完了しました。");
		}
		if (synchronizedPlaying)
		{
			videoPlayer.Play( );
		}
		if (!synchronizedPlaying)
		{
			videoPlayer.Pause( );
		}
		if (synchronizationAfterReload && Networking.IsOwner(gameObject))
		{
			SendCustomEventDelayedSeconds(nameof(ResumeOwnerAfterLocalReload), Mathf.Max(0.1f, ownerResumeDelaySeconds));
		}
	}

	/// <summary>
	/// 共有アンカーとサーバー経過時間から現在の目標再生位置を計算します。
	/// </summary>
	private float CalculateTargetMediaTime( )
	{
		float targetMediaTime = synchronizedMediaTime;
		if (synchronizedPlaying)
		{
			double currentServerTime = Networking.GetServerTimeInSeconds( );
			double elapsedSeconds = Networking.CalculateServerDeltaTime(currentServerTime, synchronizedServerTime);
			if (elapsedSeconds > 0.0)
			{
				targetMediaTime += (float)elapsedSeconds;
			}
		}
		return targetMediaTime;
	}

	/// <summary>
	/// 動画の範囲と末尾余白を考慮した安全なシーク位置を返します。
	/// </summary>
	private float GetSafeSeekPosition(float requestedMediaTime)
	{
		float safeMediaTime = Mathf.Max(0.0f, requestedMediaTime);
		float durationSeconds = videoPlayer.GetDuration( );
		if (durationSeconds > maximumSeekPositionMarginSeconds)
		{
			float maximumMediaTime = durationSeconds - maximumSeekPositionMarginSeconds;
			safeMediaTime = Mathf.Min(safeMediaTime, maximumMediaTime);
		}
		return safeMediaTime;
	}

	/// <summary>
	/// オーナーの現在位置を共有同期アンカーへ反映します。
	/// </summary>
	private void UpdateOwnerAnchor( )
	{
		if (!Networking.IsOwner(gameObject) || !localVideoReady || localReloadPending || localReloadRequiresSynchronization)
		{
			return;
		}

		synchronizedInitialized = true;
		synchronizedPlaybackMode = NormalizePlaybackMode(playbackMode);
		synchronizedMediaTime = videoPlayer.GetTime( );
		synchronizedServerTime = Networking.GetServerTimeInSeconds( );

		RequestSerialization( );
	}

	/// <summary>
	/// オーナーによる定期同期アンカー更新を開始します。
	/// </summary>
	private void StartOwnerSynchronizationLoop( )
	{
		if (ownerSynchronizationLoopStarted)
		{
			return;
		}

		ownerSynchronizationLoopStarted = true;

		SendCustomEventDelayedSeconds(nameof(OwnerSynchronizationTick), Mathf.Max(1.0f, synchronizationIntervalSeconds));
	}

	/// <summary>
	/// オーナーの同期アンカーを定期更新し、次回実行を予約します。
	/// </summary>
	public void OwnerSynchronizationTick( )
	{
		// 非オーナーが共有状態を更新すると競合するため、この処理を行いません。
		if (!Networking.IsOwner(gameObject))
		{
			ownerSynchronizationLoopStarted = false;
			return;
		}

		if (!localReloadPending && !localReloadRequiresSynchronization && localVideoReady && synchronizedInitialized)
		{
			UpdateOwnerAnchor( );
		}

		SendCustomEventDelayedSeconds(nameof(OwnerSynchronizationTick), Mathf.Max(1.0f, synchronizationIntervalSeconds));
	}

	/// <summary>
	/// ローカル利用者への所有権移譲を検出し、引き継ぎ処理を予約します。
	/// </summary>
	public override void OnOwnershipTransferred(VRCPlayerApi newOwner)
	{
		if (!Utilities.IsValid(newOwner) || !newOwner.isLocal)
		{
			return;
		}
		Debug.Log("このクライアントが動画同期の新しいオーナーになりました。");
		SendCustomEventDelayedSeconds(nameof(ResumeAsNewOwner), Mathf.Max(0.0f, ownershipStabilizationSeconds));
	}

	/// <summary>
	/// 新しいオーナーとして再生位置を補正し、共有同期更新を引き継ぎます。
	/// </summary>
	public void ResumeAsNewOwner( )
	{
		// 非オーナーが共有状態を更新すると競合するため、この処理を行いません。
		if (!Networking.IsOwner(gameObject))
		{
			return;
		}

		if (!synchronizedInitialized)
		{
			InitializeSynchronizedState( );
		}
		if (!localUrlRequested)
		{
			if (!localReloadPending)
			{
				LoadConfiguredUrl( );
			}
			return;
		}
		if (!localVideoReady || localReloadPending)
		{
			return;
		}
		if (localReloadRequiresSynchronization)
		{
			ApplySynchronizationAfterLocalReload( );
			return;
		}
		float targetMediaTime = CalculateTargetMediaTime( );
		float currentMediaTime = videoPlayer.GetTime( );
		float differenceSeconds = Mathf.Abs(targetMediaTime - currentMediaTime);

		if (differenceSeconds > ignoredDifferenceSeconds)
		{
			videoPlayer.SetTime(GetSafeSeekPosition(targetMediaTime));
		}

		UpdateOwnerAnchor( );
		StartOwnerSynchronizationLoop( );
	}

	/// <summary>
	/// クールダウンと状態を確認し、このクライアントだけの動画再読込を要求します。
	/// </summary>
	public void RequestLocalReload( )
	{
		if (!localInitializationStarted)
		{
			Debug.Log("動画プレイヤーの初期化が完了していないため、ローカル再読込を開始できません。");
			return;
		}
		// ローカル再読込中に通常処理を行うと状態が競合するため、この処理を行いません。
		if (localReloadPending || localReloadRequiresSynchronization)
		{
			Debug.Log("ローカル再読込はすでに実行中です。" + " localReloadPending: " + localReloadPending + " localReloadRequiresSynchronization: " + localReloadRequiresSynchronization + " localVideoReady: " + localVideoReady + " localVideoStarted: " + localVideoStarted);
			return;
		}

		double currentServerTime = Networking.GetServerTimeInSeconds( );
		double cooldownSeconds = Mathf.Max(5.0f, localReloadCooldownSeconds);
		if (localReloadCooldownStarted)
		{
			double elapsedSeconds = Networking.CalculateServerDeltaTime(currentServerTime, lastLocalReloadServerTime);
			if (elapsedSeconds >= 0.0 && elapsedSeconds < cooldownSeconds)
			{
				float remainingSeconds = Mathf.Ceil((float)(cooldownSeconds - elapsedSeconds));

				Debug.Log("ローカル再読込のクールダウン中です。残り: " + remainingSeconds + "秒");
				return;
			}
			if (elapsedSeconds < 0.0)
			{
				Debug.LogWarning("サーバー時間差が負数になったため、ローカル再読込のクールダウンを解除します。");

				localReloadCooldownStarted = false;
			}
		}
		// 動画プレイヤー未設定時は後続処理を実行できないため、この処理を行いません。
		if (!Utilities.IsValid(videoPlayer))
		{
			Debug.LogError("動画プレイヤーが設定されていません。");
			return;
		}

		lastLocalReloadServerTime = currentServerTime;
		localReloadStartedServerTime = currentServerTime;
		localReloadCooldownStarted = true;
		localReloadPending = true;
		localReloadRequiresSynchronization = true;
		localSeekVerificationPending = false;
		localSeekVerificationForReload = false;
		localSeekRetryCount = 0;
		localUrlRequested = false;
		localVideoReady = false;
		localVideoStarted = false;
		localSynchronizationApplied = false;
		Debug.Log("このクライアントだけHLS動画を再読込します。");
		videoPlayer.Stop( );

		SendCustomEventDelayedSeconds(nameof(ExecuteLocalReload), Mathf.Max(0.1f, localReloadDelaySeconds));
		SendCustomEventDelayedSeconds(nameof(RecoverLocalReloadState), Mathf.Max(10.0f, localReloadTimeoutSeconds));
	}

	/// <summary>
	/// ローカル動画の状態を初期化して固定URLを再読込します。
	/// </summary>
	public void ExecuteLocalReload( )
	{
		localReloadPending = false;
		localUrlRequested = false;
		localVideoReady = false;
		localVideoStarted = false;
		localSynchronizationApplied = false;
		Debug.Log("このクライアントで固定HLS URLを再読込します。");
		LoadConfiguredUrl( );
	}

	/// <summary>
	/// 規定時間を超えたローカル再読込状態を強制的に解除します。
	/// </summary>
	public void RecoverLocalReloadState( )
	{
		if (!localReloadPending && !localReloadRequiresSynchronization && !localSeekVerificationPending)
		{
			return;
		}

		double currentServerTime = Networking.GetServerTimeInSeconds( );
		double elapsedSeconds = Networking.CalculateServerDeltaTime(currentServerTime, localReloadStartedServerTime);
		float timeoutSeconds = Mathf.Max(10.0f, localReloadTimeoutSeconds);
		if (elapsedSeconds >= 0.0 && elapsedSeconds < timeoutSeconds)
		{
			float remainingSeconds = timeoutSeconds - (float)elapsedSeconds;
			SendCustomEventDelayedSeconds(nameof(RecoverLocalReloadState), Mathf.Max(0.5f, remainingSeconds));
			return;
		}
		Debug.LogWarning("ローカル再読込が規定時間内に完了しなかったため、再読込状態を強制解除します。" + " localReloadPending: " + localReloadPending + " localReloadRequiresSynchronization: " + localReloadRequiresSynchronization + " localVideoReady: " + localVideoReady + " localVideoStarted: " + localVideoStarted);

		localReloadPending = false;
		localReloadRequiresSynchronization = false;
		localSeekVerificationPending = false;
		localSeekVerificationForReload = false;
		localSeekRetryCount = 0;

		if (Networking.IsOwner(gameObject) && localVideoReady)
		{
			UpdateOwnerAnchor( );
			StartOwnerSynchronizationLoop( );
		}
	}

	/// <summary>
	/// ローカル再読込後にオーナーの共有同期更新を再開します。
	/// </summary>
	public void ResumeOwnerAfterLocalReload( )
	{
		if (!Networking.IsOwner(gameObject) || !localVideoReady || localReloadPending || localReloadRequiresSynchronization)
		{
			return;
		}

		UpdateOwnerAnchor( );
		StartOwnerSynchronizationLoop( );
	}

	/// <summary>
	/// 動画終了状態と末尾位置を共有同期状態へ反映します。
	/// </summary>
	public override void OnVideoEnd( )
	{
		localVideoStarted = false;

		// ローカル再読込中に通常処理を行うと状態が競合するため、この処理を行いません。
		if (localReloadPending || localReloadRequiresSynchronization)
		{
			return;
		}
		// 非オーナーが共有状態を更新すると競合するため、この処理を行いません。
		if (!Networking.IsOwner(gameObject))
		{
			return;
		}

		synchronizedPlaying = false;
		synchronizedMediaTime = videoPlayer.GetDuration( );
		synchronizedServerTime = Networking.GetServerTimeInSeconds( );

		RequestSerialization( );
		Debug.Log("HLS動画の再生が終了しました。");
	}

	/// <summary>
	/// 動画再生エラーを記録し、上限内で読み込み再試行を予約します。
	/// </summary>
	public override void OnVideoError(VideoError videoError)
	{
		localVideoReady = false;
		localVideoStarted = false;
		localUrlRequested = false;
		localReloadPending = false;
		Debug.LogError("HLS動画の再生エラーが発生しました。エラー: " + videoError);
		if (localRetryCount >= maximumRetryCount)
		{
			localReloadRequiresSynchronization = false;
			Debug.LogError("最大再試行回数へ到達しました。");
			return;
		}
		localRetryCount++;
		SendCustomEventDelayedSeconds(nameof(RetryVideoLoading), Mathf.Max(5.0f, retryIntervalSeconds));
	}

	/// <summary>
	/// エラー後の動画読み込みを再試行します。
	/// </summary>
	public void RetryVideoLoading( )
	{
		// 同じURLの重複読み込みを避けるため、この処理を行いません。
		if (localUrlRequested)
		{
			return;
		}

		Debug.Log("HLS動画の読み込みを再試行します。回数: " + localRetryCount);
		LoadConfiguredUrl( );
	}
}
