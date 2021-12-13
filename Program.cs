using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Leaf.xNet;

namespace ConsoleApp2
{
    internal class Program
    {
        static string token;
        static int gen;
        static string name;
        static void Main(string[] args)
        {
            Console.Write("CapMonster api keyiniz: ");
            token = Console.ReadLine();
            Console.Write("Oluşturulacak hesap sayısı: ");
            gen = Convert.ToInt32(Console.ReadLine());
            Console.Write("Hesap adı giriniz: ");
            name = Console.ReadLine();
            HesapOlustur();
        }
        static void HesapOlustur()
        {
            HttpRequest req = new HttpRequest();
            try
            {
                for (int i = 0; i < gen; i++)
                {
                    Random random = new Random();
                    var mail = name + random.Next(1, 9999) + "%40mail.com.tr";
                    var pass = "MadeCharon1985.84";
                    var response = req.Post("http://mail.com.tr/signup/mail", "user=" + mail + "&pass=" + pass + "&confpass=" + pass + "&altmail=&g-recaptcha-response=" + CaptchaBypass() + "&terms=1", "application/x-www-form-urlencoded").ToString();
                    if (response.Contains("https://mail.com.tr/auth/message/type/new/user"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Başarıyla hesap oluşturuldu");
                        File.AppendAllText(Environment.CurrentDirectory + $@"\Hesaplar.txt", String.Format("{0}{1}", mail.Replace("%40", "@") + ":" + pass, $"{Environment.NewLine}"));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Hesap oluşturulamadı");
                        //Console.WriteLine(response);
                    }
                    Console.WriteLine("");
                    Thread.Sleep(60000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
        static string CaptchaBypass()
        {
            HttpRequest req = new HttpRequest();
            string response;
            var timer = new Stopwatch();
            timer.Start();
            var degers = req.Post("https://api.capmonster.cloud/createTask", "{\"clientKey\":\"" + token + "\",\"task\":{\"type\":\"NoCaptchaTaskProxyless\",\"websiteURL\":\"http://mail.com.tr/signup\",\"websiteKey\":\"6Ld66BETAAAAALErsHw0hEkLpYsIvRUkxCnVMXkX\"}}", "application/json;charset=UTF-8").ToString();
            var id = degers.Substring("taskId\"", "}").Replace(":", "");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Captcha bekleniyor...");
            while (true)
            {
                try
                {
  
                    var response1 = req.Post("https://api.capmonster.cloud/getTaskResult ", "{\"clientKey\":\"" + token + "\",\"taskId\":" + id + "}", "application/json;charset=UTF-8").ToString();
                    if (response1.Contains("ready"))
                    {
                        timer.Stop();
                        TimeSpan timeTaken = timer.Elapsed;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Captcha çözüldü " + timeTaken.ToString() +" ms.");
                        response = response1.Substring("gRecaptchaResponse\":\"", "\"},");
                        break;
                    }
                    Thread.Sleep(800);
                }
                catch (Exception ex)
                {

                }
            }
            return response;
        }
    }
}
