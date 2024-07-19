配合Siri + ShortCuts，向Windows主机发送http请求，从而实现一定程度上的控制主机，目前可以打开某个文件、软件、网址，模拟部分按键实现热键效果。

使用ShortCuts中的GetContentOfUrl向主机发送Post请求，请求体格式为JSON，键名有type，path，opts。

参数说明：

1. type:操作类型，1为运行程序或打开文件、网址，2为模拟按键。

2. path:在type为1时为文件路径、网址，在type为2时为按键序列。

   支持模拟按键有字母、数字、空格、回车、Win、Ctrl、Alt、Shift以及媒体键。
   映射关系：
   字母  <==> 字母
   数字  <==> 数字
     <==> 空格
   /  <==> 回车
   \#  <==> Win
   ^  <==> Ctrl
   \+  <==> Shift
   !  <==> Alt =  <==> 播放暂停
   <  <==> 上一首
   \>  <==> 下一首
   [  <==> 音量加
   ]  <==> 音量减

3. opts:在type为1时为程序参数，在type为2时，为模拟按键重复次数。
