# 長岡花火バーチャル鑑賞会のワールド

If you speak English, please read [English version](./README.en.md).


## 告知など

なんだかんだ7年連続で開催することができました。ご来場してくださったみなさま、本当にありがとうございました。


## 概要

長岡花火バーチャル鑑賞会のVRChatワールド、およびバーチャルキャストVCI背景のUnityプロジェクトです（はじめての3DCGでのワールド作成）。  
バーチャル信濃川土手から長岡花火の映像が見れるワールドです。

2026年から動画プレイヤーとして自前実装の「HLS同期プレイヤー」を利用しています。HTTP Live Streaming形式の動画・生配信をインスタンス内の全員で再生位置を同期して再生できます。


## 使い方

### (1) VRChatのワールドに入りたい

- VRChatを起動し、ワールド検索で「nagaoka」と検索し、「Nagaoka Fireworks Virtual Viewing」というワールドに入ってください。

### (2) バーチャルキャストのスタジオとして使いたい

- [長岡花火バーチャル鑑賞会 跡地](https://virtualcast.jp/products/87af152a3e5ad1799ca218b72237dc42505b809bcf016978bb9251cf14139ce2)を買ってください（無料です）。

### (3) Unityから使いたい（改変やワールド作成の参考用）

1. [Releases](https://github.com/SKAsApp/nagaoka-fireworks-virtual-viewing/releases)からダウンロードするか，このリポジトリーをクローンして，Unityで開いてください。
1. 下の依存関係に書かれたパッケージ・素材をUnityにimportしてください。画像素材は適宜加工しAssets/imagesに配置してください。


## 依存関係

- 開発時使用Unityバージョン：2022.3.22f1
- 依存Unityパッケージ
	- VRChat SDK（BaseおよびWorld）
		- 開発時使用バージョン：3.10.4
		- [https://www.vrchat.com/home/download](https://www.vrchat.com/home/download)
	- UniVCI
		- 開発時使用バージョン：0.32.1
		- [https://github.com/virtual-cast/VCI](https://github.com/virtual-cast/VCI)
	- UniVRM
		- 開発時使用バージョン：0.67.4_4689（↑UniVCI 0.32.1に付属するもの）
		- [https://github.com/vrm-c/UniVRM](https://github.com/vrm-c/UniVRM)
- 画像素材
	- アスファルト
		- フリーテクスチャ素材館  
		[https://free-texture.net/seamless-pattern/asphalt-pattern-set.html](https://free-texture.net/seamless-pattern/asphalt-pattern-set.html)
	- 草
		- Paper-co  
		[https://free-paper-texture.com/free-lawn-texture-2/](https://free-paper-texture.com/free-lawn-texture-2/)


## 権利・利用条件

このリポジトリーはGPLv3です。  
Assets/images以下の画像素材のみを使用する場合はCC-BY-SAライセンスと考えてください。


## リンク集

- 鑑賞会告知動画2021：[sm39095807](https://www.nicovideo.jp/watch/sm39095807)
- VCI：[https://virtualcast.jp/products/87af152a3e5ad1799ca218b72237dc42505b809bcf016978bb9251cf14139ce2](https://virtualcast.jp/products/87af152a3e5ad1799ca218b72237dc42505b809bcf016978bb9251cf14139ce2)
- Unity：[https://unity.com/ja](https://unity.com/ja)
- VRChat：[https://www.vrchat.com/](https://www.vrchat.com/)
- バーチャルキャスト：[https://virtualcast.jp/](https://virtualcast.jp/)
- 作者関連
	- Twitter：[@SK_Animation](https://twitter.com/SK_Animation)
	- niconico：[user/28511019](https://www.nicovideo.jp/user/28511019)
	- YouTube：[https://www.youtube.com/c/skasweb](https://www.youtube.com/c/skasweb)
	- VRChat：[ひま_2525(hima)](https://vrchat.com/home/user/usr_0df45cf1-acaf-46c2-a972-a64f6cbbf9ac)


## 更新履歴

| 日付（YYYY/MM/DD） | バージョン | 更新内容 |
| -- | -- | -- |
| 2026/08/28 | Ver 2026.2.1 | HLS同期プレイヤーのリファクタリング、ドキュメント作成 |
| 2026/08/27 | Ver 2026.2.0 | HLS同期プレイヤーの動画部分実装 |
| 2026/08/02 | Ver 2026.1.0 | リロード土管実装、HLS同期プレイヤーの生配信部分実装（2026年鑑賞会時点） |
| 2025 | | （VRChatの仕様変更で色々おかしくなる） |
| 2022〜2024 | | （大きな変更なし） |
| 2021/08/08 | Ver 2021.2.0 | 枝豆VCI修正 |
| 2021/08/06 | Ver 2021.1.1 | 動画URL変更 |
| 2021/08/03 | Ver 2021.1.0 | VRChatワールド完成（VRChat席開催時点） |
| 2021/08/02 | Ver 2021.0.2 | VCI仮完成（バーチャルキャスト席開催時点） |
| 2021/07/31 | Ver 2021.0.1 | VRCSDK2→VRCSDK3 ＆ Quest動画対応 |
| 2020/08/03 | Ver 1.0.1 | 鑑賞会実施した直後のワールド（動画URL変更） |
| 2020/08/03 | Ver 1.0.0 | 鑑賞会実施時のワールド（コミットしてません） |
| 2020/07/22 | Ver 0.2.2 | Quest仮対応 |
| 2020/07/22 | | 過去のコミットに再配布禁止素材が含まれていたため開発ブランチを削除 |  |
