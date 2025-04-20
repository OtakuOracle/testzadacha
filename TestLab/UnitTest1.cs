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
        public void Otk_MultipleAttempts_ShouldSucceedWithValidInpu() // создание аккаунта с несколькими попытками ввода данными
        {
            Console.SetIn(new StringReader("Некорректное имя\n\nСидоров Сергей\n300\nСидоров Сергей\n2300\n"));

            testAccount.otk();

            Assert.That(testAccount.name, Is.EqualTo("Сидоров Сергей"));
            Assert.That(testAccount.sum, Is.EqualTo(2300));
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
        public void Perevod_ValidAmount_ShouldReduceSourceAndIncreaseTarget() // перевод средств между двумя счетами
        {
            testAccount.sum = 2000;
            var targetAccount = new account { sum = 1000 };
            Console.SetIn(new StringReader("500\n" + targetAccount));

            testAccount.perevod();

            Assert.That(testAccount.sum, Is.EqualTo(1500));
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
        public void Umen_Subtract200From1500_ShouldResult1300() // снятие какой-то суммы
        {
            testAccount.sum = 1500; 
            Console.SetIn(new StringReader("200\n")); 

            testAccount.umen(); 

            Assert.That(testAccount.sum, Is.EqualTo(1300)); 
        }

        [Test]
        public void Umen_SubtractMoreThanBalance_ShouldNotChangeBalance_WhenBalanceIs800AndSubtracting1000() // снять сумму, которая больше текущей
        {
            testAccount.sum = 800; 
            Console.SetIn(new StringReader("1000\n")); 

            testAccount.umen(); 

            Assert.That(testAccount.sum, Is.EqualTo(800)); 
            StringAssert.Contains("Недостаточно средств", consoleOutput.ToString()); 
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
        public void Perevod_ZeroAmount_ShouldNotAllowTransfer() // перевод с нулевой суммой
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("0\n200\n"));

            testAccount.perevod();

            StringAssert.Contains("Сумма перевода должна быть больше нуля", consoleOutput.ToString());
        }


        [Test]
        public void Perevod_NonIntegerAmount_ShouldRoundCorrectly() // перевод нецелой суммы с проверкой округления
        {
            testAccount.sum = 1000;
            var targetAccount = new account { sum = 500 };
            Console.SetIn(new StringReader("250.75\n" + targetAccount));

            testAccount.perevod();

            Assert.That(testAccount.sum, Is.EqualTo(749.25)); // Проверка округления
            Assert.That(testAccount.sum, Is.EqualTo(750.75));
        }



        [Test]
        public void Perevod_InsufficientFunds_ShouldShowErrorMessage() // перевод, когда запрашиваемая сумма превышает доступные средства на счете
        {
            testAccount.sum = 2000; 
            Console.SetIn(new StringReader("2500\n0\n")); 

            testAccount.perevod(); 

            Assert.That(testAccount.sum, Is.EqualTo(2000)); 
            StringAssert.Contains("Недостаточно средств", consoleOutput.ToString()); 
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
        public void Umen_AttemptToWithdrawNegativeAmount_ShouldNotChangeBalance() // попытка снять отрицательную сумму со счета
        {
            testAccount.sum = 2000;
            Console.SetIn(new StringReader("-50\n"));

            testAccount.umen();

            Assert.That(testAccount.sum, Is.EqualTo(2000));
            StringAssert.Contains("Недопустимая сумма", consoleOutput.ToString());
        }

        [Test]
        public void Perevod_SameAccount_ShouldDisplayErrorMessage() // попытка перевода средств на тот же самый счет
        {
            testAccount.sum = 1500;
            Console.SetIn(new StringReader("700\n700\n"));

            testAccount.perevod();

            StringAssert.Contains("Нельзя перевести на тот же счет", consoleOutput.ToString());
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
