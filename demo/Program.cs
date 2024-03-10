using System;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;

class Program {
  static void Main() {
    // 获取系统中的打印机列表
    var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();

    foreach (var printer in printers) {
      // 创建打印机对应的文件夹路径，位于C盘下
      string folderPath = Path.Combine("C:\\", $"print_{printer}");

      // 如果文件夹不存在，则创建文件夹
      if (!Directory.Exists(folderPath)) {
        // 需要管理员权限才能在C盘创建文件夹
        try {
          Directory.CreateDirectory(folderPath);
          Console.WriteLine($"创建文件夹：{folderPath}");
        } catch (UnauthorizedAccessException) {
          Console.WriteLine($"无法创建文件夹，需要管理员权限：{folderPath}");
        }
      }

      // 创建文件系统监视器
      FileSystemWatcher watcher = new FileSystemWatcher();
      watcher.Path = folderPath;

      // 监视文件变化事件
      watcher.Created += (sender, e) => OnFileChanged(e, printer);

      // 启动监视器
      watcher.EnableRaisingEvents = true;

      Console.WriteLine($"正在监视文件夹：{folderPath}");
    }

    Console.WriteLine("按任意键退出...");
    Console.ReadKey();
  }

  private static void OnFileChanged(FileSystemEventArgs e, string printer) {
    // 文件创建事件触发时执行打印操作
    Console.WriteLine($"检测到文件变化：{e.FullPath}");

    // 调用打印方法
    PrintFile(e.FullPath, printer);
  }

  private static void PrintFile(string filePath, string printer) {
    try {
      // 使用指定打印机和纸张大小打印文件
      PrintDocument printDocument = new PrintDocument();
      printDocument.PrinterSettings.PrinterName = printer;

      // 设置不同打印机的纸张大小
      PaperSize paperSize;
      if (printer.Equals("PX-049A Series(Network)")) {
        // A4纸张大小
        paperSize = new PaperSize("A4", 827, 1169);
      }
      else if (printer.Equals("打印机B")) {
        // A5纸张大小
        paperSize = new PaperSize("A5", 583, 827);
      }
      else {
        // 默认使用Letter纸张大小
        paperSize = new PaperSize("A4", 827, 1169);
      }

      printDocument.DefaultPageSettings.PaperSize = paperSize;
      printDocument.PrinterSettings.PrintFileName = filePath;
      printDocument.Print();

      Console.WriteLine($"文件已发送到打印机：{filePath}使用{paperSize}纸开始打印！");
    } catch (Exception ex) {
      Console.WriteLine($"打印时发生错误：{ex.Message}");
    }
  }
}
