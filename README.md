# WiseSkeletonSimulator-Unity
A simulate tool developed for NCCU WISE platform

#各資料夾說明

## _Main
裡面有兩個主要的 Scene  
DancerDataSender 和 SkeletonSimulator  
  
SkeletonSimulator 可以把接收到的 JSON 資料視覺化成人型骨架  
DancerDataSender 可以傳送假資料，以便平時寫程式可以測試用（不用穿戴裝置也可以測試）  
  
這個資料夾的其他東西都是為了上面兩個 Scene 的功能而寫  
如果要開發新的 Unity 專案，單純只需要與 Wise 連線  
這個資料夾可以刪去  
  
  
## Wise-Unity
包含所有與 Wise 連接需要的程式  
核心的連線程式在 Scripts/Core 裡面  
如果只是要與 Wise 傳接資料（非模擬人型骨架），那就只需要 Core 即可
其他大部分都是把抓到的資料 Mapping 到模型關節、位置座標  
  
  
## Plugins  
SimpleJson 和 rabbitMQ 的 libary  
製作其他專案的話也是必備的  
  
  
## JSON Tests
本專案使用 SimpleJSON 來處理 JSON 資料  
這個資料夾中是一些測試 simpleJSON 的程式  
（因為官網的文件沒有很完整）  
  
如果想要使用 SimpleJSON 卻不知如何寫可參考  
除此之外皆可刪  
  
  
## MVN Data
給 Dancer Data Sender 使用的 Mocap 資料  
製作新專案可刪
  
  

  
  
