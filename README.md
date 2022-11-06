# Sn.AsciiArt

字符画生成与播放器. 通过判断字符与画面的相似度, 选取最合适的字符作为画面的一部分.

> 生成时, 使用 新宋体 (16, 尺寸为 8x16), 故播放时使用新宋体能够获得最好效果

- [x] 自定义字符
- [x] 多线程处理
- [x] 自定义背景色与前景色
- [x] 基于 ANSI 转义序列的彩色输出

## 生成器

Sn.AsciiArtApp 可以将 JPG/PNG/BMP 图片转换为与之对应的字符画.

```txt
Sn.AsciiArtApp [-m] [-a] [-b <colors>] [-f <colors>] [-c <charactors>] [-t <colored/gray>] <path> ...
```

示例: 将 XXX 目录下的所有图片转换为字符画

```bash
Sn.AsciiArtApp XXX
```

可选项:

```txt
-m: 使用多线程
-a: 使用 ANSI 转义序列
-b: 指定要使用的背景颜色 (16 位 hex)
-f: 指定要使用的前景颜色 (16 位 hex)
-c: 指定要使用的字符 (默认为 ASCII 中的所有可见字符)
-t: 指定要使用的渲染颜色模式 (colored/gray)
path: 文件或目录路径, 指定要转换为字符画的目标
```

## 播放器

Sn.AsciiArtPlayer 用来播放由生成器生成的, 使用时, 它会查找指定目录下以数字命名的文本文件, 并播放.

```txt
Sn.AsciiArtPlayer [-s <start file number>] [-a <audio file>] [-d <duration>] folder
```

示例: 播放 XXX 目录下的所有字符画, 并使用 XXX\audio.mp3 作为背景音乐

```bash
Sn.AsciiArtPlayer -a XXX\audio.mp3 XXX
```

可选项:

```txt
-s: 指定起始文件编号
-a: 指定背景音乐文件
-d: 指定播放时长 (格式 小时:分钟:秒)
folder: 指定要播放的目录
```

> 注: 如果不指定播放时长, 默认为 10 秒, 如果指定了背景音乐, 那么就以背景音乐的时长为默认播放时长

> 提示: 某些时候音频的时长不能被正确的识别, 这个时候则需要用户手动指定播放时长. \
> 有时候 ffmpeg 也会在转换时, 出现播放时长的变化, 这个需要注意.

## 性能

使用 Intel i5-11320H 运行程序, 转换 512x384 的图片时, 开启多线程的速度约为 40/s, 不开启多线程约为 12/s

## 帮助

### 将视频转换为字符画

1. 使用 [FFmpeg](https://ffmpeg.org/) 将视频转换为图片序列

```bash
ffmpeg -i input.mp4 %d.png
```

2. 使用 Sn.AsciiArtApp 将图片序列转换为字符画

```bash
Sn.AsciiArtApp -m -t gray folder
```

