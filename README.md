 # SoundRecorder 
![Static Badge](https://img.shields.io/badge/1.4-passing?label=Release&link=https%3A%2F%2Fgithub.com%2FGabee8%2FSoundRecorder%2Freleases%2Ftag%2Frelease)

Simple Windows soundrecorder program writing the c# by Visual Studio 2010

![](https://tandemradio.hu/wp-content/uploads/Hangrogzito-1.png)

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
