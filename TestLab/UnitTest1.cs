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
        public void Otk_MultipleAccounts_ShouldSucceed() // �������� ���������� ������� ������
        {
            var inputs = new[]
            {
                "������ ����\n1500\n",
                "������ ����\n2000\n"
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
        public void Otk_WithInsufficientFunds_ShouldDisplayErrorMessage() // �������� ��������, �� � ������ �����, ������� ������������ ��� ���������� ��������
        {
            Console.SetIn(new StringReader("������ ����\n300\n������ ����\n1500\n"));

            testAccount.otk();

            StringAssert.Contains("����� ������� ����", consoleOutput.ToString());

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
        public void TopUp_WithZero_Input_NoChangeInSum() // ���������� ������� �� 0
        {
            testAccount.sum = 1000;
            string input = "0"; 

            Console.SetIn(new StringReader(input));

            testAccount.top_up();

            Assert.That(testAccount.sum, Is.EqualTo(1000));
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
        public void Umen_WithZero_Input_NoChangeInSum() // ������ 0
        {
            testAccount.sum = 1000; 
            string input = "0";

            Console.SetIn(new StringReader(input)); 

            testAccount.umen(); 


            Assert.That(testAccount.sum, Is.EqualTo(1000));
        }

        [Test]
        public void Umen_MultipleOperations_ShouldCalculateCorrectBalance() // ������������ ���������� ������� ����� ���������� �������� ������
        {
            testAccount.sum = 1500; 
            Console.SetIn(new StringReader("400\n600\n200\n")); 

            testAccount.umen();
            testAccount.umen(); 
            testAccount.umen(); 

            Assert.That(testAccount.sum, Is.EqualTo(300)); 
        }


        [Test]
        public void Perevod_InsufficientFunds_ShouldNotTransfer() // ������� �������, ����������� ������
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
                StringAssert.Contains("�� ����� ������������ �������!", sw.ToString());
            }
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
        public void Perevod_ValidTransfer_ShouldDecreaseBalance() // ���������� �������
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
                StringAssert.Contains("������: 700 �.", sw.ToString());
            }
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
        public void NumGen_ShouldGenerateAccountNumber() // �������� �� ���� ������ �����
        {
            testAccount.num_gen();

            Assert.That(testAccount.num, Is.Not.Empty, "����� ����� �� ������ ���� ������");
            Assert.That(testAccount.num.Length, Is.EqualTo(20), "����� ������ ����� ������ ���� 20 ��������");
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
