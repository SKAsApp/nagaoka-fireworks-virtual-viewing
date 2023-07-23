# 長岡花火バーチャル鑑賞会のワールド

If you speak English, please read [English version](./README.en.md).


## 告知

- 2020/08/03（月）
- 2021/08/02（月）
- 2021/08/03（火）

と、2年連続で開催することができました。ご来場してくださったみなさま、本当にありがとうございました。


## 概要

長岡花火バーチャル鑑賞会のVRChatワールドおよびVCI背景のUnityプロジェクトです。

はじめての3DCGのワールド作成だったので，これから始める人の役に立ったらいいなという感じでプロジェクトを公開します。


## 使い方

### (1) VRChatのワールドに入りたい

- VRChatを起動し，ワールド検索で「nagaoka」と検索し，「Nagaoka Fireworks Virtual Viewing」というワールド（Community Labs）に入ってください。

### (2) バーチャルキャストのスタジオとして使いたい

- [長岡花火バーチャル鑑賞会 跡地]()を買ってください（無料です）。

### (3) 動画を作るためにUnityから使いたい

1. [Releases](https://github.com/SKAsApp/nagaoka-fireworks-virtual-viewing/releases)からダウンロードするか，このリポジトリーをクローンして，Unityで開いてください。
2. 下の依存関係に書かれたパッケージ・素材をUnityにimportしてください。画像素材は適宜加工しAssets/imagesに配置してください。
3. お好きに動画を作ってください。
4. 完成した動画は可能であればCreative Commons CC-BY-SAライセンスで公開してください。また，ワールド作者に報告いただけると嬉しく思います（下にリンクがあります）。

### (4) このワールドを使ったゲームを作りたい or 改変ワールドを作りたい

1. このリポジトリーをフォーク（推奨）またはクローンして，Unityで開いてください。
2. 下の依存関係に書かれたパッケージ・素材をUnityにimportしてください。画像素材は適宜加工しAssets/imagesに配置してください。
3. お好きにお使いください。
4. ゲームや改変ワールドを公開（アップロード）する際は必ずそのソースコード・プロジェクトもGPL互換ライセンスで公開してください。ただし，テクスチャーの画像等 再配布禁止の素材は除外してください（gitであればgitignoreに追記してください）。 ※ (3)とは違い，絶対にお願いします。


## 依存関係

- 開発時使用Unityバージョン：2018.4.20
- 依存Unityパッケージ
	- VRCSDK3
		- 開発時使用バージョン：2021.07.12.18.53  
		[https://www.vrchat.com/home/download](https://www.vrchat.com/home/download)
	- UniVCI
		- 開発時使用バージョン：0.32.1  
		[https://github.com/virtual-cast/VCI](https://github.com/virtual-cast/VCI)
	- UniVRM
		- 開発時使用バージョン：0.67.4_4689（↑UniVCI 0.32.1に付属するもの）  
		[https://github.com/vrm-c/UniVRM](https://github.com/vrm-c/UniVRM)
- 画像素材
	- アスファルト
		- フリーテクスチャ素材館  
		[https://free-texture.net/seamless-pattern/asphalt-pattern-set.html](https://free-texture.net/seamless-pattern/asphalt-pattern-set.html)
	- 草
		- Paper-co  
		[https://free-paper-texture.com/free-lawn-texture-2/](https://free-paper-texture.com/free-lawn-texture-2/)


## 権利・利用条件

このリポジトリーはGPLv3です。改変し公開（アップロード）する場合は そのソースコード・プロジェクトもGPL互換ライセンスで公開してください。

Assets/images以下の画像素材のみを使用する場合はCC-BY-SAライセンスと考えてください。


## 今後の予定

2022年の梅雨ごろになったら考えます。


## リンク集

- 鑑賞会振り返り動画2021：
- 鑑賞会告知動画2021：[sm39095807](https://www.nicovideo.jp/watch/sm39095807)
- 鑑賞会告知動画2020：[sm37234258](https://www.nicovideo.jp/watch/sm37234258)
- 鑑賞会告知ブロマガ2020：[ar1926250](https://ch.nicovideo.jp/skas-web/blomaga/ar1926250)
- VCI：（未対応）
- Unity：[https://unity.com/ja](https://unity.com/ja)
- VRChat：[https://www.vrchat.com/](https://www.vrchat.com/)
- バーチャルキャスト：[https://virtualcast.jp/](https://virtualcast.jp/)
- 作者（SKA）関連
	- Twitter：[@SK_Animation](https://twitter.com/SK_Animation)
	- niconico：[user/28511019](https://www.nicovideo.jp/user/28511019)
	- YouTube：[https://www.youtube.com/c/skasweb](https://www.youtube.com/c/skasweb)
	- ブロマガ：[ar1739328](https://ch.nicovideo.jp/skas-web/blomaga/ar1739328)
	- VRChat：SKA_2525


## 更新履歴

YYYY/MM/DD

- 2021/08/08　Ver 2021.2.0　枝豆VCI修正
- 2021/08/06　Ver 2021.1.1　動画URL変更
- 2021/08/03　Ver 2021.1.0　VRChatワールド完成（VRChat席開催時点）
- 2021/08/02　Ver 2021.0.2　VCI仮完成（バーチャルキャスト席開催時点）
- 2021/07/31　Ver 2021.0.1　VRCSDK2→VRCSDK3 ＆ Quest動画対応
- 2020/08/03　Ver 1.0.1　鑑賞会実施した直後のワールド（動画URL変更）
- 2020/08/03　Ver 1.0.0　鑑賞会実施時のワールド（コミットしてません）
- 2020/07/22　Ver 0.2.2　Quest仮対応
- 2020/07/22　過去のコミットに再配布禁止素材が含まれていたため開発ブランチを削除　
