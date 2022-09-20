# FumenEditorExtension
[FumenEditor](https://github.com/CSDotNET0211/FumenEditor)のデスクトップ拡張ソフト。
連携して様々な機能を提供する。

Desktop Client for [FumenEditor](https://github.com/CSDotNET0211/FumenEditor).
It offers various functinos.

# Features/機能
- [TETR.IO](http://tetr.io)からリアルタイムで盤面情報を取得し、FumenEditorに送信
- FumenEditorから送信された盤面データから、各種AIをローカルで計算し操作を返す
- AIを用いた画面認識機能より、リアルタイムで盤面情報を取得しFumenEditorに送信  

- Get gamedata from TETR.IO in realtime, and send to FumenEdtior. 
- Return operations by AI calculated with local from send by FumenEditor.
- Send field-data to FumenEditor by Image-Recognition.

# Usage/使い方
TETR.IOと接続するためには、現在は[tetrio.js](URL)をChromeのオーバーライド機能で置き換える必要がある。
FumenEditorと接続するためには、同サイトのCONNECTタブより接続可能。
また、この改造されたjsファイルによって予期せぬクラッシュを引き起こす可能性があるので、必要な時以外は有効にしないでください。

Now you need to use Chrome Local Override to replace the file [tetrio.js](URL) to connect to TETR.IO.
To connect to FumenEditor, use CONNECT tab in the site.


