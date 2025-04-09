using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    public class account
    {
        private string num;
        private string name;
        public float sum;
        public float summ;
        public int index;
        public string putt;

        public void otk()
        {
            //Метод открытия счёта.

            int i = 0;
            while (i == 0)
            {
                Console.WriteLine("\nПожалуйста, введите своё ФИО:");
                name = Console.ReadLine();
                Console.WriteLine("Также введите сумму для первичного пополнения счёта:");
                sum = Convert.ToSingle(Console.ReadLine());

                if (sum < 1000)
                {
                    Console.WriteLine("Сумма слишком мала, попробуйте ещё раз!");
                }
                else
                {
                    Console.WriteLine("Готово! Счёт успешно открыт!\n");
                    num_gen();
                    show();
                    i++;
                }
            }
        }

        public void num_gen()
        {
            //Генерация номера счёта, состоящего из случайных чисел.

            int[] array = new int[20];
            Random rand = new Random();
            for (int i = 0; i < array.Length; i++)
                array[i] = rand.Next(0, 9);
            num = string.Join("", array);
        }

        public void show()
        {
            //Вывод введённой пользователем информации на консоль.

            putt = num + ".txt";
            StreamWriter sw = new StreamWriter(putt, true);

            Console.WriteLine("\nНиже Вы можете увидеть реквизиты и актуальные данные вашего банковского счёта.");
            Console.WriteLine("Номер счёта: " + num);
            Console.WriteLine("ФИО владельца: " + name);
            Console.WriteLine("Баланс: " + sum + " р.");

            sw.WriteLine("Номер счёта: " + num);
            sw.WriteLine("ФИО владельца: " + name);
            sw.WriteLine("Баланс: " + sum + " р.");
            sw.Close();
        }

        public void top_up()
        {
            //Операция пополнения баланса на счету.

            Console.WriteLine("\nВведите сумму, которую Вы собираетесь положить на счёт:");
            int sum_t = Convert.ToInt32(Console.ReadLine());
            sum = sum + sum_t;
            //pereschet();
            show();
        }

        public void umen()
        {
            //Операция снятия определённой суммы со счёта.

            Console.WriteLine("\nВведите сумму, которую Вы собираетесь снять:");
            int sum_t = Convert.ToInt32(Console.ReadLine());
            sum = sum - sum_t;
            //pereschet();
            show();
        }

        public void obnul()
        {
            //Операция снятия всей суммы со счёта.

            sum = sum - sum;
            //pereschet();
            show();
        }

        public void perevod()
        {
            //Операция перевода средств с одного счёта на другой.

            Console.WriteLine("\nКакую сумму хотите перевести?");
            summ = Convert.ToSingle(Console.ReadLine());
            Console.WriteLine("Введите индекс счёта, на который хотите осуществить перевод.");
            index = Convert.ToInt32(Console.ReadLine());
            if (summ > sum)
            {
                Console.WriteLine("\nНа счету недостаточно средств!");
            }
            else
            {
                sum = sum - summ;
                show();
            }
            //pereschet();
        }

        private async void pereschet()
        {
            string[] lines = new string[3];

            using (StreamReader reader = new StreamReader(putt))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = reader.ReadLine();
                    lines[i] = line;
                }
                reader.Close();
            }

            using (StreamWriter sww = new StreamWriter(putt))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i < lines.Length - 1)
                        sww.WriteLine(lines[i]);
                }
                sww.WriteLine("Номер счёта: " + num);
                sww.WriteLine("ФИО владельца: " + name);
                sww.WriteLine("Баланс: " + sum + " р.");
                sww.Close();
            }
        }
    }
}

