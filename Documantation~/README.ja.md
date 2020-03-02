# FrameDebuggerSave


## ツールについて
このツールはFrameDebuggerの情報を別の形式で保存してそれを閲覧するツールです。<br />
また、保存したFrameDebuggerの描画情報を元にShaderVariantCollectionの構築もサポートしています。<br />
![alt text](img/FrameDebuggerSave.png)


## 導入方法
コチラのリポジトリを、プロジェクト直下にあるPackageManagerフォルダ以下に配置してください。

## 動作確認環境
Unity 2018.4 / Unity 2019.3<br/>
Windows Editor実行時、及びPlayerへ繋げた時の実行<br />
※Player実行時の際、ShaderTextureの保存等には対応していません

## 利用方法
Menuの「Tools/FrameDebuggerSave」を押して、下記のウィンドウを立ち上げます。<br/>
![alt text](img/FrameDebuggerLaunch.png)<br />

1.保存された結果を元に既存のShaderVariantCollectionにVariantを追加する機能<br/ >
"Add"ボタンを押すと、保存された結果一覧を元にShaderVariantCollectionにVariantを追加していきます
<br />
2.FrameDebugger越しにデータを取得してキャプチャーする機能<br />
Captureを押すとFrameDebuggerを立ち上げて、FrameDebugger越しにデータを取得して来てセーブします <br />

スクリーンの保存の有無、Shaderに渡されたTextureの保存の有無等をFlagで指定できます。<br />
キャプチャーデータが重くなる場合、Flagを切り替えて保存するデータをおさえてください<br />

3.Captureで保存された情報一覧<br />
クリックすると右側で保存した結果を表示します。<br />

