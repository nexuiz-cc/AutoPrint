using System.Drawing.Printing;
using System.Text;

class Program {
  static void Main() {
    var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
    foreach (var printer in printers) {
      string folderPath = Path.Combine("C:\\", $"print_{printer}");

      if (!Directory.Exists(folderPath)) {
        try {
          Directory.CreateDirectory(folderPath);
          Console.WriteLine($"创建文件夹：{folderPath}");
        } catch (UnauthorizedAccessException) {
          Console.WriteLine($"无法创建文件夹，需要管理员权限：{folderPath}");
        }
      }

      FileSystemWatcher watcher = new FileSystemWatcher();
      watcher.Path = folderPath;
      watcher.Created += (sender, e) => OnFileChanged(e, printer, folderPath);
      watcher.EnableRaisingEvents = true;
      Console.WriteLine($"正在监视文件夹：{folderPath}");
    }

    Console.WriteLine("按任意键退出...");
    Console.ReadKey();
  }

  private static void OnFileChanged(FileSystemEventArgs e, string printer, string folderPath) {
    string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
    // 文件创建事件触发时执行打印操作
    Console.WriteLine($"{timestamp} 检测到文件夹变化：{folderPath}");
    Console.WriteLine($"    {timestamp} 新增了：{e.Name}");
    PrintFile(e.FullPath, printer, timestamp,folderPath,e);
  }

  private static void PrintFile(string filePath, string printer,string timestamp,string folderPath, FileSystemEventArgs e) {
    try {
      // 使用指定打印机和纸张大小打印文件
      PrintDocument printDocument = new PrintDocument();
      printDocument.PrinterSettings.PrinterName = printer;

      // 设置不同打印机的纸张大小 英寸
      PaperSize paperSize;
      if (printer.Equals("PX-049A Series(Network)")) {
        paperSize = new PaperSize("A4", 827, 1169);
      }
      else if (printer.Equals("打印机B")) {
        paperSize = new PaperSize("A5", 583, 827);
      }
      else {
        // 默认使用A4纸张大小
        paperSize = new PaperSize("A4", 827, 1169);
      }

      if (!Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase)) {
        Console.WriteLine($"{timestamp} 打印机{printer}使用 {paperSize.PaperName}纸开始打印{Path.GetFileName(filePath)}");
        string logFilePath = Path.Combine(Path.GetDirectoryName(filePath), $"{printer}.txt");
        string logMessage = $"{timestamp} 检测到{folderPath}文件夹变化\r\n" +
          $"  新增了: {e.Name}\r\n" +
          $"    打印机{printer}使用{paperSize.PaperName}纸开始打印{Path.GetFileName(filePath)}\r\n";
        File.AppendAllText(logFilePath, logMessage, Encoding.UTF8);
        printDocument.DefaultPageSettings.PaperSize = paperSize;
        printDocument.PrinterSettings.PrintFileName = filePath;
        printDocument.Print();

      }
    } catch (Exception ex) {
      Console.WriteLine($"打印时发生错误：{ex.Message}");
    }
  }
}
