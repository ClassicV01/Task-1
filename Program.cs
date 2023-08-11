
using System.IO;
using System.Text;

namespace Task_1;

class Program
{
    static async Task Main(string[] args)
    {
        ///Здесь создаются файлы и наполняются
        ///Также создается класс для работы с БД
        string path = @"D:\MyTXT";
        Console.WriteLine("Создание 100 файлов...");
        DateTime start = DateTime.Now.AddYears(-5);
        Random random = new Random();
        var files = new List<string>();
        int range = (DateTime.Today - start).Days;
        try
        {
            for (int i = 0; i < 100; i++)
            {
                string name = path + i.ToString() + ".txt";
                files.Add(name);
                using (FileStream fs = File.Create(name))
                { 
                    for (int j = 0; j < 100000; j++)
                    {
                        string input = RandomDate(start, random, range).ToShortDateString();
                        input += "||" + RandomLatina() + "||" + RandomRussian() + "||" + RandomNumb() + "||" + RandomDouble() + "||" + "\n";
                        byte[] info = new UTF8Encoding(true).GetBytes(input);
                        fs.Write(info, 0, info.Length);
                    }
                    fs.Close();
                }
            }          
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        var db = new DatabaseManagment();

        Console.WriteLine("Для объединения всех файлов в один введите 1\n" +
            "Для импорта файлов в БД нажмите 2\n" +
            "Для подсчета суммы всех целых чисел и медианы всех дробных чисел нажмите 3\n" +
            "Для завершения программы введите 4");
        ///Далее в цикле пункты 2-3-4 из задания можно повторять
        ///Также создается БД если ее нет и туда импортируются все файлы
        while (true) 
        {
            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    {
                        Console.WriteLine("Если хотите удалить набор символов из всех файлов введите 1");
                        var flag = Console.ReadLine();
                        string? delete = "";
                        if (flag == "1")
                        {
                            Console.WriteLine("Введите набор удаляемых символов, за исключением чисел и '||'...");
                            delete += Console.ReadLine()?.ToString();
                            while (delete == "||" || float.TryParse(delete, out _))
                            {
                                Console.WriteLine("Введите другой набор удаляемых символов, эти нельзя...");
                                delete = Console.ReadLine()?.ToString();
                            }
                        }

                        int countOfDeleteSubstrings = 0;
                        using (var commonfile = File.Create(@"D:\CommonFile.txt"))
                        {
                            foreach (var file in files)
                            {
                                if (flag == "1" && delete != "")
                                {
                                    string textFromFile = "";
                                    using (var fs = File.OpenRead(file))
                                    {
                                        byte[] buffer = new byte[fs.Length];
                                        fs.Read(buffer, 0, buffer.Length);
                                        textFromFile = Encoding.Default.GetString(buffer);
                                        var position = textFromFile.IndexOf(delete);
                                        while (position != -1)
                                        {
                                            textFromFile = textFromFile.Remove(position, delete.Length);
                                            countOfDeleteSubstrings++;
                                            position = textFromFile.IndexOf(delete);
                                        }
                                        fs.Close();
                                    }
                                    using (var fs = File.Create(file))
                                    {
                                        byte[] info = new UTF8Encoding(true).GetBytes(textFromFile);
                                        fs.Write(info, 0, info.Length);
                                        fs.CopyTo(commonfile);
                                        fs.Close();
                                    }
                                    using (var fs = File.OpenRead(file))
                                    {
                                        fs.CopyTo(commonfile);
                                        fs.Close();
                                    }
                                }
                                else
                                {
                                    using (var fs = File.OpenRead(file))
                                    {
                                        fs.CopyTo(commonfile);
                                        fs.Close();
                                    }
                                }
                            }
                            if (flag == "1")
                            {
                                Console.WriteLine($"Количество удаленных подстрок равно {countOfDeleteSubstrings}.");
                            }

                        }

                        Console.WriteLine("Для объединения всех файлов в один введите 1\n" +
                                            "Для импорта файлов в БД нажмите 2\n" +
                                            "Для подсчета суммы всех целых чисел и медианы всех дробных чисел нажмите 3\n" +
                                            "Для завершения программы введите 4");
                    }
                    break;

                case "2":
                    {
                        await db.CreateAndImportDatabase(files);
                        Console.WriteLine("Для объединения всех файлов в один введите 1\n" +
                                            "Для импорта файлов в БД нажмите 2\n" +
                                            "Для подсчета суммы всех целых чисел и медианы всех дробных чисел нажмите 3\n" +
                                            "Для завершения программы введите 4");
                    }
                    break;

                case "3":
                    {
                        await db.GetSumm();
                        await db.GetMedian();
                        Console.WriteLine("Для объединения всех файлов в один введите 1\n" +
                                            "Для импорта файлов в БД нажмите 2\n" +
                                            "Для подсчета суммы всех целых чисел и медианы всех дробных чисел нажмите 3\n" +
                                            "Для завершения программы введите 4");
                    }
                    break;

                case "4":
                    {
                        Environment.Exit(0);
                    }
                    break;

                default:
                    Environment.Exit(0);
                    break;
            }
        }
    }

    // функции для работы с рандомными числами и датами для заполнения файлов
    static DateTime RandomDate(DateTime start, Random gen, int range)
    {
        return start.AddDays(gen.Next(range));
    }

    static string RandomLatina()
    {
        Random rnd = new Random();
        string latina = "";
        for (int i = 0; i < 10; i++)
        {
            var position = rnd.Next(0, 52);
            latina += LatinAlphabet[position];
        }
        return latina;
    }

    static string RandomRussian()
    {
        Random rnd = new Random();
        string russian = "";
        for (int i = 0; i < 10; i++)
        {
            var position = rnd.Next(0, 66);
            russian += RussianAlphabet[position];
        }
        return russian;
    }

    static string RandomNumb()
    {
        Random rnd = new Random();
        int numb;
        do
        {
            numb = rnd.Next(0, 100000001);
        } 
        while (numb % 2 != 0);

        return numb.ToString();
    }

    static string RandomDouble()
    {
        Random rnd = new Random();
        var d = rnd.NextDouble();
        return Math.Round(d * (20 - 1) + 1, 8).ToString();
    }

    private static string LatinAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";   //52
    private static string RussianAlphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";   //66
}
