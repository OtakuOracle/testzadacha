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
        public void Otk_CreateAccountWithValidInput_ShouldSucceed() // �������� �������� � ����������
        {
            string input = "������� ����� ���������\n1800\n";
            Console.SetIn(new StringReader(input));

            testAccount.otk();

            Assert.That(testAccount.name, Is.Not.Null);
            Assert.That(testAccount.name, Is.EqualTo("������� ����� ���������").IgnoreCase); 
            Assert.That(testAccount.sum, Is.EqualTo(1800m)); 
            Assert.That(testAccount.num, Is.Not.Null);
        }


        [Test]
        public void Otk_MultipleAttempts_ShouldSucceedWithValidInpu() // �������� �������� � ����������� ��������� ����� �������
        {
            Console.SetIn(new StringReader("������������ ���\n\n������� ������\n300\n������� ������\n2300\n"));

            testAccount.otk();

            Assert.That(testAccount.name, Is.EqualTo("������� ������"));
            Assert.That(testAccount.sum, Is.EqualTo(2300));
        }


        [Test]
        public void Otk_WithInsufficientFunds_ShouldDisplayErrorMessage() // �������� ��������, �� � ������ �����, ������� ������������ ��� ���������� ��������
        {
            Console.SetIn(new StringReader("������ ����\n300\n������ ����\n1500\n"));

            testAccount.otk();

            StringAssert.Contains("����� ������� ����", consoleOutput.ToString());

            Assert.That(testAccount.sum, Is.EqualTo(1500));
        }

        [Test]
        public void Perevod_ValidAmount_ShouldReduceSourceAndIncreaseTarget() // ������� ������� ����� ����� �������
        {
            testAccount.sum = 2000;
            var targetAccount = new account { sum = 1000 };
            Console.SetIn(new StringReader("500\n" + targetAccount));

            testAccount.perevod();

            Assert.That(testAccount.sum, Is.EqualTo(1500));
            Assert.That(testAccount.sum, Is.EqualTo(1500));
        }

        [Test]
        public void TopUp_Add300To2000_ShouldResult2300() // ���������� �����
        {
            testAccount.sum = 2000; 
            Console.SetIn(new StringReader("300\n")); 

            testAccount.top_up(); 

            Assert.That(testAccount.sum, Is.EqualTo(2300)); 
        }


        [Test]
        public void Umen_Subtract200From1500_ShouldResult1300() // ������ �����-�� �����
        {
            testAccount.sum = 1500; 
            Console.SetIn(new StringReader("200\n")); 

            testAccount.umen(); 

            Assert.That(testAccount.sum, Is.EqualTo(1300)); 
        }

        [Test]
        public void Umen_SubtractMoreThanBalance_ShouldNotChangeBalance_WhenBalanceIs800AndSubtracting1000() // ����� �����, ������� ������ �������
        {
            testAccount.sum = 800; 
            Console.SetIn(new StringReader("1000\n")); 

            testAccount.umen(); 

            Assert.That(testAccount.sum, Is.EqualTo(800)); 
            StringAssert.Contains("������������ �������", consoleOutput.ToString()); 
        }


        [Test]
        public void Obnul_InitialBalance500_ShouldResultZero() // ��������� �������
        {
            testAccount.sum = 700; 

            testAccount.obnul(); 

            Assert.That(testAccount.sum, Is.EqualTo(0)); 
        }


        [Test]
        public void Perevod_ValidTransferFrom1500To1000_ShouldUpdateBalance() // ������� ������� � ������ ����� �� ������ 
        {
            testAccount.sum = 1500;
            Console.SetIn(new StringReader("500\n0\n")); 

            testAccount.perevod(); 

            Assert.That(testAccount.sum, Is.EqualTo(1000)); 
            Assert.That(testAccount.summ, Is.EqualTo(500)); 
        }


        [Test]
        public void Perevod_ZeroAmount_ShouldNotAllowTransfer() // ������� � ������� ������
        {
            testAccount.sum = 1000;
            Console.SetIn(new StringReader("0\n200\n"));

            testAccount.perevod();

            StringAssert.Contains("����� �������� ������ ���� ������ ����", consoleOutput.ToString());
        }


        [Test]
        public void Perevod_NonIntegerAmount_ShouldRoundCorrectly() // ������� ������� ����� � ��������� ����������
        {
            testAccount.sum = 1000;
            var targetAccount = new account { sum = 500 };
            Console.SetIn(new StringReader("250.75\n" + targetAccount));

            testAccount.perevod();

            Assert.That(testAccount.sum, Is.EqualTo(749.25)); // �������� ����������
            Assert.That(testAccount.sum, Is.EqualTo(750.75));
        }



        [Test]
        public void Perevod_InsufficientFunds_ShouldShowErrorMessage() // �������, ����� ������������� ����� ��������� ��������� �������� �� �����
        {
            testAccount.sum = 2000; 
            Console.SetIn(new StringReader("2500\n0\n")); 

            testAccount.perevod(); 

            Assert.That(testAccount.sum, Is.EqualTo(2000)); 
            StringAssert.Contains("������������ �������", consoleOutput.ToString()); 
        }

        [Test]
        public void TopUp_MultipleOperations_ShouldCalculateCorrectBalance() // ������������ ���������� ������� ����������� ����� ����� ���������� �������� ����������
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
        public void Umen_AttemptToWithdrawNegativeAmount_ShouldNotChangeBalance() // ������� ����� ������������� ����� �� �����
        {
            testAccount.sum = 2000;
            Console.SetIn(new StringReader("-50\n"));

            testAccount.umen();

            Assert.That(testAccount.sum, Is.EqualTo(2000));
            StringAssert.Contains("������������ �����", consoleOutput.ToString());
        }

        [Test]
        public void Perevod_SameAccount_ShouldDisplayErrorMessage() // ������� �������� ������� �� ��� �� ����� ����
        {
            testAccount.sum = 1500;
            Console.SetIn(new StringReader("700\n700\n"));

            testAccount.perevod();

            StringAssert.Contains("������ ��������� �� ��� �� ����", consoleOutput.ToString());
        }

        [Test]
        public void Show_DisplayAccountInformation_ShouldShowCorrectDetails() // ������������ ����������� ���������� � ���������� �����
        {
            testAccount.num = "98765432109876543210";
            testAccount.name = "�������� �����";
            testAccount.sum = 10000;
            testAccount.putt = "new_account.txt";

            testAccount.show();

            StringAssert.Contains("����� �����: 98765432109876543210", consoleOutput.ToString());
            StringAssert.Contains("��� ���������: �������� �����", consoleOutput.ToString());
            StringAssert.Contains("������: 10000 �.", consoleOutput.ToString());

            File.Delete("new_account.txt");
        }


        [TearDown]
        public void Cleanup() // ������� ��������
        {
            if (consoleOutput != null)
            {
                consoleOutput.Dispose();
                consoleOutput = null; 
            }
        }
    }
}
