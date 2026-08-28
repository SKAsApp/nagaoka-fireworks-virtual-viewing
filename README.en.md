# The World of Nagaoka Fireworks Virtual Viewing

もし日本の方が読んでいたら[日本語版](./README.md)をご覧ください。


## Notification

I’ve managed to hold this event for seven years running. To everyone who came along, thank you so much.


## Overview

This is a Unity project comprising the VRChat world for the Nagaoka Fireworks Virtual Viewing Party and the VirtualCast VCI background (my first attempt at creating a world using 3DCG).  
It is a world where you can watch footage of the Nagaoka Fireworks from the virtual Shinano River embankment.

Since 2026, I have been using a custom-implemented “HLS 同期 player (HLS synchronised player)” as the video player. This allows everyone within the instance to play videos and live streams in HTTP Live Streaming format with their playback positions synchronised.


## How to Use

### (1) Enter the VRChat World

- Please launch VRChat, search for “nagaoka” in the World Search, and enter the world called “Nagaoka Fireworks Virtual Viewing”.

### (2) Use as a VirtualCast Studio

- Please buy [長岡花火バーチャル鑑賞会 跡地](https://virtualcast.jp/products/87af152a3e5ad1799ca218b72237dc42505b809bcf016978bb9251cf14139ce2) (it’s free).

### (3) Use in Unity (as a reference for modifications and world creation)

1. Please download the project from [Releases](https://github.com/SKAsApp/nagaoka-fireworks-virtual-viewing/releases) or clone this repository and open it in Unity.
1. Please import the packages and assets listed in the dependencies below into Unity. Please edit the image assets as required and place them in the Assets/images folder.


## Dependencies

- Version of Unity used during development：2022.3.22f1
- Dependent Unity packages
	- VRChat SDK（Base and World）
		- Version used during development：3.10.4
		- [https://www.vrchat.com/home/download](https://www.vrchat.com/home/download)
	- UniVCI
		- Version used during development：0.32.1
		- [https://github.com/virtual-cast/VCI](https://github.com/virtual-cast/VCI)
	- UniVRM
		- Version used during development：0.67.4_4689 (↑included with UniVCI 0.32.1)
		- [https://github.com/vrm-c/UniVRM](https://github.com/vrm-c/UniVRM)
- Image materials
	- Asphalt
		- フリーテクスチャ素材館  
		[https://free-texture.net/seamless-pattern/asphalt-pattern-set.html](https://free-texture.net/seamless-pattern/asphalt-pattern-set.html)
	- Grass
		- Paper-co  
		[https://free-paper-texture.com/free-lawn-texture-2/](https://free-paper-texture.com/free-lawn-texture-2/)


## Rights and Terms of Use

This repository is licensed under the GPLv3.  
If you are using only the image assets in the `Assets/images` directory, please treat them as being licensed under the CC-BY-SA licence.


## Links

- Announcement video of viewing 2021：[sm39095807](https://www.nicovideo.jp/watch/sm39095807)
- VCI：[https://virtualcast.jp/products/87af152a3e5ad1799ca218b72237dc42505b809bcf016978bb9251cf14139ce2](https://virtualcast.jp/products/87af152a3e5ad1799ca218b72237dc42505b809bcf016978bb9251cf14139ce2)
- Unity：[https://unity.com/ja](https://unity.com/ja)
- VRChat：[https://www.vrchat.com/](https://www.vrchat.com/)
- VirtualCast：[https://virtualcast.jp/](https://virtualcast.jp/)
- Developper
	- Twitter：[@SK_Animation](https://twitter.com/SK_Animation)
	- niconico：[user/28511019](https://www.nicovideo.jp/user/28511019)
	- YouTube：[https://www.youtube.com/c/skasweb](https://www.youtube.com/c/skasweb)
	- VRChat：[ひま_2525(hima)](https://vrchat.com/home/user/usr_0df45cf1-acaf-46c2-a972-a64f6cbbf9ac)


## Change Log

| Date (YYYY/MM/DD) | Version | Changes |
| -- | -- | -- |
| 2026/08/28 | Ver 2026.2.1 | Refactoring of the HLS synchronised player; documentation created |
| 2026/08/27 | Ver 2026.2.0 | Implementation of the video component of the HLS synchronised player |
| 2026/08/02 | Ver 2026.1.0 | Implemented reload pipe; implemented live streaming functionality for the HLS synchronised player (as of the 2026 viewing event) |
| 2025 | | (Various issues arose due to changes in VRChat’s specifications) |
| 2022〜2024 | | (No major changes) |
| 2021/08/08 | Ver 2021.2.0 | Edamame VCI fixes |
| 2021/08/06 | Ver 2021.1.1 | Video URL changed |
| 2021/08/03 | Ver 2021.1.0 | VRChat world completed (as of the VRChat event) |
| 2021/08/02 | Ver 2021.0.2 | VCI provisionally completed (at the time of the Virtual Cast event) |
| 2021/07/31 | Ver 2021.0.1 | VRCSDK2 → VRCSDK3 & Quest video support |
| 2020/08/03 | Ver 1.0.1 | The world after the viewing（change video’s URL） |
| 2020/08/03 | Ver 1.0.0 | The world when the viewing was held (not committed) |
| 2020/07/22 | Ver 0.2.2 | Experimental support for Quest |
| 2020/07/22 | | Removed a development branch because it contained redistribution-resistant material in a previous commit. |
