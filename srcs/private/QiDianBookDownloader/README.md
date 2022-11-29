# Qidian subscribed book downloader

AKA. 起点已订阅图书下载器

This is a C# port for https://github.com/hcoona/qidian-book-downloader. The origin project is unmaintained because of difficult to debug issue & unfamiliar with NodeJS eco-system.

This project is still under construction.

起点中文网已经订阅的图书也不提供 VIP 部分的 MOBI 格式下载。为了方便在 Kindle 上看已经买了的书，搞了这么一个下载器。

特别提示有些完本的小说已经可以在 Kindle 商店中买到。

## 基本原理

使用 ~~Puppeteer~~ Playwright 调用 Chromium 模拟用户登陆起点，然后打开图书的目录页开始爬，最后将结果写到 html 文件中。

之后可以使用 Calibre 之类的工具直接转换成 MOBI 格式传到 Kindle 中。

## 使用方法（待更新）

```bash
yarn run build
node .\bin\index.js run --chrome <Chrome Path> -u <QiDian username> -p <QiDian password> -i <book id>
```

例如小说的目录页在 https://book.qidian.com/info/1011280743#Catalog 那么它的 BookID 就是 1011280743。

如果报错的话，可以加上参数 `--no-chrome-headless` 看看是怎么回事。
