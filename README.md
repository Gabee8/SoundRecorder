# SoundRecorder 
![](https://github.com/Gabee8/SoundRecorder/blob/main/SoundRecorder/images/mainicon.png) Simple Windows soundrecorder program writing the c# by Visual Studio 2010
![](https://tandemradio.hu/wp-content/uploads/Hangrogzito-1.png)
-------------
The recorder is saving in MP3 format file. I use and [NAudio](https://github.com/naudio/NAudio) license by.
The program include the two languages (Hungarian/English) in the Languages folder. The your language modification is free.

##### Edit your language:
Language path: bin\debug\Languages or application root folder\Languages.
```xaml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:system="clr-namespace:System;assembly=mscorlib">

    <system:String x:Key="mainTitle">Soundrecoder</system:String>
    <system:String x:Key="rec">Record</system:String>
.....
```
