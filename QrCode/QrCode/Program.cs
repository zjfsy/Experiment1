using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace qrCode
{
    class Program
    {
        static void PrintHelp()
        {
            Console.WriteLine("帮助：输入格式：");
            Console.WriteLine("1: 读取文件中的信息并将QrCode保存在位图中：");
            Console.WriteLine("QrCode -f*** (***为有效文件路径)");
            Console.WriteLine("2: 传入字符串并直接在控制台输出QrCode：");
            Console.WriteLine("QrCode *** (***为用于生成QrCode的信息，其长度应在256字符以内)");
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)//判断无参数时无效
            {
                Console.WriteLine("输入不符合要求！没有参数！");
                PrintHelp();
            }
            else if (args[0].StartsWith("-f"))//判断参数是否以-f开头以判断是否是从指定文件读入并生成位图
            {
                string file = args[0].Substring(2);//从参数中截取路径
                if (file.Length == 0)//判断参数只有-f的情况
                {
                    Console.WriteLine("未找到有关文件的信息！");
                    PrintHelp();
                }
                else if (!File.Exists(file))//判断文件是否存在
                {
                    Console.WriteLine("文件不存在！");
                    PrintHelp();
                }
                else
                {
                    StreamReader sr = new StreamReader(file);//读入文件
                    string input;
                    for (int i = 1; (input = sr.ReadLine()) != null; i++)//每行读入直到文件读取完毕
                    {
                        QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                        QrCode qrCode = new QrCode();
                        qrEncoder.TryEncode(input, out qrCode);
                        GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                        string path = Path.Combine(Path.GetDirectoryName(file), i.ToString("000") + input.Substring(0, 4) + ".bmp");//设置存放bmp位图的路径，将位图放在txt文件同一文件夹下
                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            render.WriteToStream(qrCode.Matrix, ImageFormat.Bmp, stream);//生成位图
                        }
                    }
                    sr.Close();//关闭文件
                }
            }
            else//参数不以-f开头时从参数中读取字符串生成QrCode并在控制台打印
            {
                if (args[0].Length > 256)//限制字符串长度在256以内
                {
                    Console.WriteLine("字符串长度应在256个字符以内！");
                    PrintHelp();
                }
                else
                {
                    QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
                    QrCode qrCode = qrEncoder.Encode(args[0]);
                    Console.Write('█');
                    for (int i = 0; i < qrCode.Matrix.Width; i++)//打印QrCode上边框
                    {
                        Console.Write('█');
                    }
                    Console.WriteLine('█');
                    for (int j = 0; j < qrCode.Matrix.Width; j++)
                    {
                        Console.Write('█');//打印QrCode左边框
                        for (int i = 0; i < qrCode.Matrix.Width; i++)
                        {
                            Console.Write(qrCode.Matrix[i, j] ? '　' : '█');
                        }
                        Console.WriteLine('█');//打印QrCode右边框并换行
                    }
                    Console.Write('█');
                    for (int i = 0; i < qrCode.Matrix.Width; i++)//打印QrCode下边框
                    {
                        Console.Write('█');
                    }
                    Console.WriteLine('█');
                }
            }
        }
    }
}
