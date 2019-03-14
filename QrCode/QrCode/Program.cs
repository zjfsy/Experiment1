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
        static void Main(string[] args)
        {
            if (args.Length == 0)//判断无参数时无效
            {
                Console.WriteLine("输入不符合要求！");
            }
            else if (args[0] == "-f")//判断参数为-f代表从指定文件读入并生成位图
            {
                if (args.Length == 1 || !File.Exists(args[1]))//-f后未指定文件路径或路径所指定文件不存在时无效
                {
                    Console.WriteLine("输入不符合要求！");
                }
                else
                {
                    StreamReader sr = new System.IO.StreamReader(args[1]);//读入文件
                    string input;
                    for (int i = 0; (input = sr.ReadLine()) != null; i++)//每行读入直到文件读取完毕
                    {
                        QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                        QrCode qrCode = new QrCode();
                        qrEncoder.TryEncode(input, out qrCode);
                        GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                        string path = Path.Combine(Path.GetDirectoryName(args[1]), i.ToString("000") + input.Substring(0, 4) + ".bmp");//设置存放bmp位图的路径
                        using (FileStream stream = new FileStream(path, FileMode.Create))//生成位图
                        {
                            render.WriteToStream(qrCode.Matrix, ImageFormat.Bmp, stream);
                        }
                    }
                    sr.Close();//关闭文件
                }
            }
            else//参数不为-f时从参数中读取字符串生成QrCode并在控制台打印
            {
                if (args[0].Length > 2048)//限制字符串长度在2048以内确保能生成QrCode
                {
                    Console.WriteLine("输入的字符串过长！");
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
