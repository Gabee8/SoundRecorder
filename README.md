 # SoundRecorder 
[![Version](https://img.shields.io/badge/1.4-passing?label=Release)](https://github.com/Gabee8/SoundRecorder/releases/tag/release)
[![License](https://img.shields.io/github/license/Gabee8/SoundRecorder)](https://github.com/Gabee8/SoundRecorder/blob/main/LICENSE)

Simple Windows soundrecorder program written C# by Visual Studio 2010

![](https://tandemradio.hu/wp-content/uploads/Hangrogzito-1.png)

![](http://tandemradio.hu/wp-content/uploads/snrec_2023_11_15_en.png)

![](https://tandemradio.hu/wp-content/uploads/soundrec1.4_en.png)
-------------
The recorder is saving in MP3 format file. I use and [NAudio](https://github.com/naudio/NAudio) license by.
The program include the two languages (Hungarian/English) in the Languages folder. The your language modification is free.
##### Supported Windows: Windows 7 SP1 or newer.
##### Edit your language:
Language path: bin\debug\Languages or application root folder\Languages.

Create mylanguage.xaml file example:
```xaml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:system="clr-namespace:System;assembly=mscorlib">

    <system:String x:Key="mainTitle">Soundrecoder</system:String>
    <system:String x:Key="rec">Record</system:String>
.....
</ResourceDictionary>
```
