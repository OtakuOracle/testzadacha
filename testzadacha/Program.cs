using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace bank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Здравствуйте!\nВы хотите открыть счёт в нашем банке?");
            string ans = Console.ReadLine();
            if ((ans == "ДА") || (ans == "Да") || (ans == "да"))
            {
                Console.WriteLine("\nСколько счетов Вы собираетесь создать?");
                int kolvo = Convert.ToInt32(Console.ReadLine());
                account[] user = new account[kolvo];

                for (int i = 0; i < kolvo; i++)
                {
                    user[i] = new account();
                    user[i].otk();
                    Console.WriteLine("Счёту присвоен индекс: " + i);
                }

                Console.WriteLine("\nВыберите индекс счёта, с которым Вы хотели бы произвести операцию.");
                int a = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("\nТеперь выберите операцию, которую хотели бы произвести.\n(пополнить, снять сумму, снять всё, перевести на другой счёт)");
                string strr = Console.ReadLine();

                switch (strr)
                {
                    case "Пополнить":
                        user[a].top_up();
                        break;

                    case "пополнить":
                        goto case "Пополнить";
                        break;

                    case "Снять сумму":
                        user[a].umen();
                        break;

                    case "снять сумму":
                        goto case "Снять сумму";
                        break;

                    case "Снять всё":
                        user[a].obnul();
                        break;

                    case "снять всё":
                        goto case "Снять всё";
                        break;

                    case "Перевести":
                        user[a].perevod();
                        int ind = user[a].index;
                        float per = user[a].summ;
                        user[ind].summ = per;
                        user[ind].top_up();
                        break;

                    case "перевести":
                        goto case "Перевести";
                        break;
                }
            }
            else
            {
                Console.WriteLine("\nОчень жаль! Если передумаете, будем рады видеть вас в качестве нашего клиента!\nВсего доброго.");
            }

            Console.ReadKey();
        }
    }
}