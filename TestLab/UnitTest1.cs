using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.IO;
using System.Security.Principal;

namespace testzadacha.Tests
{
    [TestFixture]
    public class AccountTests
    {
        private account testAccount;
        private StringWriter consoleOutput;

        [SetUp]
        public void Setup()
        {
            testAccount = new account();
            consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
        }

        [Test]
        public void Otk_CreateAccountWithValidInput_ShouldSucceed() // создание аккаунта с валидацией
        {
            string input = "Петрова Мария Сергеевна\n1800\n";
            Console.SetIn(new StringReader(input));

            testAccount.otk();

            Assert.That(testAccount.name, Is.Not.Null);
            Assert.That(testAccount.name, Is.EqualTo("Петрова Мария Сергеевна").IgnoreCase); 
            Assert.That(testAccount.sum, Is.EqualTo(1800m)); 
            Assert.That(testAccount.num, Is.Not.Null);
        }

        [Test]
        public void Otk_MultipleAccounts_ShouldSucceed() // создание нескольких учетных данных
        {
            var inputs = new[]
            {
                "Иванов Иван\n1500\n",
                "Петров Петр\n2000\n"
            };

            foreach (var input in inputs)
            {
                using (var reader = new StringReader(input))
                {
                    Console.SetIn(reader);

                    var testAccount = new account();
                    testAccount.otk();

                    Assert.That(testAccount.num, Is.Not.Null);
                }
            }
        }

        [Test]
        public void Otk_WithInsufficientFunds_ShouldDisplayErrorMessage() // создание аккаунта, но с вводом суммы, которая недостаточна для выполнения операции
        {
            Console.SetIn(new StringReader("Иванов Иван\n300\nИванов Иван\n1500\n"));

            testAccount.otk();

            StringAssert.Contains("Сумма слишком мала", consoleOutput.ToString());

            Assert.That(testAccount.sum, Is.EqualTo(1500));
        }

        [Test]
        public void TopUp_Add300To2000_ShouldResult2300() // пополнение счета
        {
            testAccount.sum = 2000; 
            Console.SetIn(new StringReader("300\n")); 

            testAccount.top_up(); 

            Assert.That(testAccount.sum, Is.EqualTo(2300)); 
        }

        [Test]
        public void TopUp_WithZero_Input_NoChangeInSum() // пополнение баланса на 0
        {
            testAccount.sum = 1000;
            string input = "0"; 

            Console.SetIn(new StringReader(input));

            testAccount.top_up();

            Assert.That(testAccount.sum, Is.EqualTo(1000));
        }

        [Test]
        public void Umen_Subtract200From1500_ShouldResult1300() // снятие какой-то суммы
        {
            testAccount.sum = 1500; 
            Console.SetIn(new StringReader("200\n")); 

            testAccount.umen(); 

            Assert.That(testAccount.sum, Is.EqualTo(1300)); 
        }

        [Test]
        public void Umen_WithZero_Input_NoChangeInSum() // снятие 0
        {
            testAccount.sum = 1000; 
            string input = "0";

            Console.SetIn(new StringReader(input)); 

            testAccount.umen(); 


            Assert.That(testAccount.sum, Is.EqualTo(1000));
        }

        [Test]
        public void Umen_MultipleOperations_ShouldCalculateCorrectBalance() // корректность вычисления баланса после нескольких операций снятия
        {
            testAccount.sum = 1500; 
            Console.SetIn(new StringReader("400\n600\n200\n")); 

            testAccount.umen();
            testAccount.umen(); 
            testAccount.umen(); 

            Assert.That(testAccount.sum, Is.EqualTo(300)); 
        }


        [Test]
        public void Perevod_InsufficientFunds_ShouldNotTransfer() // перевод средств, превышающих баланс
        {
            testAccount.sum = 500; 
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var input = "600\n1\n"; 
                var reader = new StringReader(input);
                Console.SetIn(reader);

                testAccount.perevod();

                Assert.That(testAccount.sum, Is.EqualTo(500)); 
                StringAssert.Contains("На счету недостаточно средств!", sw.ToString());
            }
        }


        [Test]
        public void Obnul_InitialBalance500_ShouldResultZero() // обнуление баланса
        {
            testAccount.sum = 700; 

            testAccount.obnul(); 

            Assert.That(testAccount.sum, Is.EqualTo(0)); 
        }


        [Test]
        public void Perevod_ValidTransferFrom1500To1000_ShouldUpdateBalance() // перевод средств с одного счета на другой 
        {
            testAccount.sum = 1500;
            Console.SetIn(new StringReader("500\n0\n")); 

            testAccount.perevod(); 

            Assert.That(testAccount.sum, Is.EqualTo(1000)); 
            Assert.That(testAccount.summ, Is.EqualTo(500)); 
        }

        [Test]
        public void Perevod_ValidTransfer_ShouldDecreaseBalance() // уменьшение баланса
        {
            testAccount.sum = 1000; 
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var input = "300\n1\n"; 
                var reader = new StringReader(input);
                Console.SetIn(reader);

                testAccount.perevod();

                Assert.That(testAccount.sum, Is.EqualTo(700)); 
                StringAssert.Contains("Баланс: 700 р.", sw.ToString());
            }
        }


        [Test]
        public void TopUp_MultipleOperations_ShouldCalculateCorrectBalance() // корректность вычисления баланса банковского счета после нескольких операций пополнения
        {
            {
                testAccount.sum = 1500;
                Console.SetIn(new StringReader("400\n600\n200\n"));

                testAccount.top_up();
                testAccount.top_up();
                testAccount.top_up();

                Assert.That(testAccount.sum, Is.EqualTo(2700));
            }
        }


        [Test]
        public void NumGen_ShouldGenerateAccountNumber() // проверка на ввод номера счета
        {
            testAccount.num_gen();

            Assert.That(testAccount.num, Is.Not.Empty, "Номер счёта не должен быть пустым");
            Assert.That(testAccount.num.Length, Is.EqualTo(20), "Длина номера счёта должна быть 20 символов");
        }

        [Test]
        public void Show_DisplayAccountInformation_ShouldShowCorrectDetails() // корректность отображения информации о банковском счете
        {
            testAccount.num = "98765432109876543210";
            testAccount.name = "Смирнова Елена";
            testAccount.sum = 10000;
            testAccount.putt = "new_account.txt";

            testAccount.show();

            StringAssert.Contains("Номер счёта: 98765432109876543210", consoleOutput.ToString());
            StringAssert.Contains("ФИО владельца: Смирнова Елена", consoleOutput.ToString());
            StringAssert.Contains("Баланс: 10000 р.", consoleOutput.ToString());

            File.Delete("new_account.txt");
        }



        [TearDown]
        public void Cleanup() // очистка ресурсов
        {
            if (consoleOutput != null)
            {
                consoleOutput.Dispose();
                consoleOutput = null; 
            }
        }
    }
}
