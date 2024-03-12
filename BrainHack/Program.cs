using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace BrainHack
{
    internal class Program
    {
        static int nowptr = 0;
        /// <summary>
        /// 特定のポインタに移動
        /// </summary>
        /// <param name="to">絶対的な場所</param>
        /// <returns>Brainfuckでの実際の値</returns>
        static string toPtr(int o)
        {
            int to = o - nowptr;
            nowptr += to;
            string ret = string.Empty;
            if(to < 0)
            {
                for(int i = 0; i < Math.Abs(to); ++i)
                {
                    ret += "<";
                }
            }
            else
            {
                for (int i = 0; i < to; ++i)
                {
                    ret += ">";
                }
            }

            
            return ret;
        }
        /// <summary>
        /// 数字変更
        /// </summary>
        /// <param name="content">数</param>
        /// <returns>BFの値</returns>
        static string Set(int content)
        {
            string ret = "[-]";
            for (int i = 0; i < content; ++i)
            {
                ret += "+";
            }
            return ret;
        }
        /// <summary>
        /// 特定の文字を繰り返す
        /// </summary>
        /// <param name="str">文字</param>
        /// <param name="len">長さ</param>
        /// <returns>結果</returns>
        static string forString(char str,int len)
        {
            string ret = string.Empty;
            for(int i = 0;i < len; i++)
            {
                ret += str;
            }
            return ret;
        }
        [STAThread]
        static void Main(string[] args)
        {
            #region ファイル名の取得
            string fpath;
            if(args.Length < 1)
            {
                Console.Write("ファイル名を入力: ");
                fpath = Console.ReadLine();
            }
            else
            {
                fpath = args[0];
            }
            if (!File.Exists(fpath))
            {
                Console.WriteLine("エラー: ファイル\"" + fpath + "\"が存在しません。");
                return;
            }
            #endregion

            StreamReader file = new StreamReader(fpath);
            string ret = string.Empty;
            Console.WriteLine("ビルド中...");
            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                string[] spline = line.Split(' ');
                if (spline.Length == 0) continue;
                switch (spline[0].ToLower())
                {
                    case "printtext":
                        if(spline.Length < 2)
                        {
                            Console.WriteLine("エラー: printtext関数の引数がありません。\n無視して続行します。");
                            continue;
                        }
                        string data = spline[1];
                        for (int i = 1; i < spline.Length - 1; i++)
                        {
                            data += " " + spline[i + 1];
                        }
                        ret += toPtr(0);
                        for(int i = 0;i < data.Length; i++)
                        {
                            ret += "[-]" + forString('+', data[i]) + ".";
                        }
                        break;
                    case "savetext":
                        if (spline.Length < 3)
                        {
                            Console.WriteLine("エラー: savetext関数の引数が足りません。\n無視して続行します。");
                            continue;
                        }
                        if (!int.TryParse(spline[1],out int stptr))
                        {
                            Console.WriteLine("エラー: savetext関数の引数が不正です。\n無視して続行します。");
                            continue;
                        }
                        data = spline[2];
                        for (int i = 1; i < spline.Length - 2; i++)
                        {
                            data += " " + spline[i + 2];
                        }
                        for (int i = 0; i < data.Length; i++)
                        {
                            ret += toPtr(stptr + i);
                            ret += forString('+', data[i]);
                        }
                        break;
                    case "printvar":
                        if(spline.Length < 2)
                        {
                            Console.WriteLine("エラー: printvar関数の引数がありません。\n無視して続行します。");
                            continue;
                        }
                        if (!int.TryParse(spline[1], out int val))
                        {
                            Console.WriteLine("エラー: printvar関数の引数が数字ではありません。\n無視して続行します。");
                            continue;
                        }
                        ret += toPtr(val);
                        ret += ".";
                        break;
                    case "ptr":
                        if (spline.Length < 4)
                        {
                            Console.WriteLine("エラー: ptr関数の引数が十分ではありません。\n無視して続行します。");
                            continue;
                        }
                        if(!int.TryParse(spline[1], out val))
                        {
                            Console.WriteLine("エラー: ptr関数の引数が数字ではありません。\n無視して続行します。");
                            continue;
                        }
                        bool ptrmode = false;
                        if (!int.TryParse(spline[3], out int val2))
                        {
                            if (spline[3].ToLower() == "ptr" && spline.Length > 5 && int.TryParse(spline[4],out val2))
                            {
                                ptrmode = true;
                                goto ptrretmain;
                            }
                            Console.WriteLine("エラー: ptr関数の引数が数字ではありません。\n無視して続行します。");
                            continue;
                        }
                    ptrretmain:;
                        if(ptrmode)
                        {
                            //いつかつくる　ポインタの内容を代入
                            //ret += toPtr(val2);
                        }
                        
                        switch (spline[2])
                        {
                            case "+":
                                ret += toPtr(0);
                                ret += Set(val2);
                                ret += "[" + forString('>', val) + "+" + forString('<', val) + "-]";
                                break;
                            case "-":
                                ret += toPtr(0);
                                ret += Set(val2);
                                ret += "[" + forString('>', val) + "-" + forString('<', val) + "-]";
                                break;
                            case "*":
                                ret += toPtr(0);
                                ret += "[-]";
                                ret += toPtr(val);
                                ret += "[" + forString('<',val) + "+" + forString('>', val) + "-]";
                                ret += toPtr(0);
                                ret += "[" + forString('>', val) + forString('+', val2) + forString('<', val) + "-]";
                                break;
                            case "=":
                                ret += toPtr(val);
                                ret += Set(val2);
                                break;
                        }
                        break;
                }
            }
            File.WriteAllText("out.bf.txt", ret);
            Console.Write("終了しました。コピーしますか？(Y/N)");
            if(Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Clipboard.SetText(ret);
            }
        }
    }
}
