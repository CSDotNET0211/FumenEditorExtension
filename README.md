# FumenEditorExtension
[FumenEditor](https://github.com/CSDotNET0211/FumenEditor)のデスクトップ拡張ソフト。
連携して様々な機能を提供する。

# Features
- [TETR.IO](http://tetr.io)からリアルタイムで盤面情報を取得し、FumenEditorに送信
- FumenEditorから送信された盤面データから、各種AIをローカルで計算し操作を返す
- AIを用いた画面認識機能より、リアルタイムで盤面情報を取得しFumenEditorに送信

# Usage
TETR.IOと接続するためには、現在は[tetrio.js](URL)をChromeのオーバーライド機能で置き換える必要がある。
FumenEditorと接続するためには、同サイトのCONNECTタブより接続可能。
