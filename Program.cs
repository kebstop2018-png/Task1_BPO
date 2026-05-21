using System;

namespace BPO_1
{
    // Интерфейс
    interface IAccountOperations
    {
        void Deposit(decimal amount);
        void Withdraw(decimal amount);
        void AccrueInterest();
        void ChangeOwner(string newOwner);
        void PrintInWords();
    }

    // Абстрактный класс
    abstract class FinancialAccount
    {
        public abstract void ShowInfo();
    }

    class Account : FinancialAccount, IAccountOperations
    {
        private string owner;
        private string accountNumber;
        private double interestRate;
        private decimal balance;

        public Account(string owner, string accountNumber, double interestRate, decimal balance)
        {
            this.owner = owner;
            this.accountNumber = accountNumber;
            this.interestRate = interestRate;
            this.balance = balance;
            Console.WriteLine("Банковский счет успешно создан.");
        }

        ~Account()
        {
            Console.WriteLine($"Деструктор: Счет {accountNumber} удален.");
        }

        public void ChangeOwner(string newOwner)
        {
            if (string.IsNullOrWhiteSpace(newOwner))
                throw new ArgumentException("Фамилия владельца не может быть пустой!");
            owner = newOwner;
            Console.WriteLine($"Владелец изменен на: {newOwner}");
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Сумма пополнения должна быть больше нуля!");
            balance += amount;
            Console.WriteLine($"Пополнено: {amount:F2} руб.");
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Сумма снятия должна быть больше нуля!");
            if (amount > balance)
                throw new ArgumentException("Недостаточно средств на счете!");
            balance -= amount;
            Console.WriteLine($"Снято: {amount:F2} руб.");
        }

        public void AccrueInterest()
        {
            decimal interest = balance * (decimal)interestRate / 100;
            balance += interest;
            Console.WriteLine($"Начислено процентов: {interest:F2} руб.");
        }

        public void ConvertToUSD(decimal usdRate)
        {
            if (usdRate <= 0) throw new ArgumentException("Курс USD должен быть положительным!");
            Console.WriteLine($"Эквивалент в USD: {(balance / usdRate):F2}");
        }

        public void ConvertToEUR(decimal eurRate)
        {
            if (eurRate <= 0) throw new ArgumentException("Курс EUR должен быть положительным!");
            Console.WriteLine($"Эквивалент в EUR: {(balance / eurRate):F2}");
        }

        public void PrintInWords()
        {
            long rubles = (long)balance;
            int kopecks = (int)Math.Round((balance - rubles) * 100, MidpointRounding.AwayFromZero);

            string text = NumberToWords(rubles) + " " + GetRubleWord(rubles);
            if (kopecks > 0)
                text += " " + NumberToWords(kopecks) + " " + GetKopeckWord(kopecks);

            Console.WriteLine(text);
        }

        private string NumberToWords(long number)
        {
            if (number == 0) return "ноль";

            string[] units = { "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять", "десять",
                               "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать",
                               "семнадцать", "восемнадцать", "девятнадцать" };

            string[] tens = { "", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };

            string[] hundreds = { "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };

            if (number < 20) return units[number];
            if (number < 100) return tens[number / 10] + (number % 10 > 0 ? " " + units[number % 10] : "");
            if (number < 1000) return hundreds[number / 100] + (number % 100 > 0 ? " " + NumberToWords(number % 100) : "");

            if (number < 1_000_000)
            {
                long thousands = number / 1000;
                return NumberToWords(thousands) + " " + GetThousandsForm(thousands) +
                       (number % 1000 > 0 ? " " + NumberToWords(number % 1000) : "");
            }

            return NumberToWords(number / 1_000_000) + " миллионов " + NumberToWords(number % 1_000_000);
        }

        private string GetThousandsForm(long n)
        {
            long mod100 = n % 100;
            if (mod100 >= 11 && mod100 <= 19) return "тысяч";
            long mod10 = n % 10;
            if (mod10 == 1) return "тысяча";
            if (mod10 >= 2 && mod10 <= 4) return "тысячи";
            return "тысяч";
        }

        private string GetRubleWord(long rub)
        {
            long mod100 = rub % 100;
            if (mod100 >= 11 && mod100 <= 19) return "рублей";
            long mod10 = rub % 10;
            if (mod10 == 1) return "рубль";
            if (mod10 >= 2 && mod10 <= 4) return "рубля";
            return "рублей";
        }

        private string GetKopeckWord(int kop)
        {
            int mod100 = kop % 100;
            if (mod100 >= 11 && mod100 <= 19) return "копеек";
            int mod10 = kop % 10;
            if (mod10 == 1) return "копейка";
            if (mod10 >= 2 && mod10 <= 4) return "копейки";
            return "копеек";
        }

        public override void ShowInfo()
        {
            Console.WriteLine("\n=== ИНФОРМАЦИЯ О СЧЕТЕ ===");
            Console.WriteLine($"Владелец: {owner}");
            Console.WriteLine($"Номер счета: {accountNumber}");
            Console.WriteLine($"Процент: {interestRate}%");
            Console.WriteLine($"Баланс: {balance:F2} руб.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Account acc = null;

            try
            {
                Console.WriteLine("Банковский счет (Вариант 12) \n");

                Console.Write("Фамилия владельца: ");
                string owner = ReadNonEmptyString("Фамилия не может быть пустой!");

                Console.Write("Номер счета: ");
                string number = ReadNonEmptyString("Номер счета не может быть пустым!");

                Console.Write("Процент начисления (%): ");
                double rate = ReadPositiveDouble();

                Console.Write("Начальный баланс: ");
                decimal bal = ReadPositiveDecimal();

                acc = new Account(owner, number, rate, bal);
                acc.ShowInfo();

                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine("\n1. Сменить владельца");
                    Console.WriteLine("2. Пополнить счет");
                    Console.WriteLine("3. Снять деньги");
                    Console.WriteLine("4. Начислить проценты");
                    Console.WriteLine("5. Перевести в USD");
                    Console.WriteLine("6. Перевести в EUR");
                    Console.WriteLine("7. Сумма прописью");
                    Console.WriteLine("8. Информация");
                    Console.WriteLine("0. Выход");
                    Console.Write("Выберите операцию: ");

                    string choice = Console.ReadLine();

                    try
                    {
                        if (choice == "1")
                        {
                            Console.Write("Новый владелец: ");
                            acc.ChangeOwner(Console.ReadLine());
                        }
                        else if (choice == "2")
                        {
                            Console.Write("Сумма: ");
                            acc.Deposit(ReadPositiveDecimal());
                        }
                        else if (choice == "3")
                        {
                            Console.Write("Сумма: ");
                            acc.Withdraw(ReadPositiveDecimal());
                        }
                        else if (choice == "4") acc.AccrueInterest();
                        else if (choice == "5")
                        {
                            Console.Write("Курс USD: ");
                            acc.ConvertToUSD(ReadPositiveDecimal());
                        }
                        else if (choice == "6")
                        {
                            Console.Write("Курс EUR: ");
                            acc.ConvertToEUR(ReadPositiveDecimal());
                        }
                        else if (choice == "7") acc.PrintInWords();
                        else if (choice == "8") acc.ShowInfo();
                        else if (choice == "0") exit = true;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Ошибка: Введено некорректное число!");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Непредвиденная ошибка: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nПрограмма завершена.");
                if (acc != null)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }

        // Вспомогательные методы безопасного ввода
        static string ReadNonEmptyString(string errorMsg)
        {
            string input;
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    Console.WriteLine(errorMsg);
            } while (string.IsNullOrWhiteSpace(input));
            return input;
        }

        static double ReadPositiveDouble()
        {
            double value;
            while (!double.TryParse(Console.ReadLine(), out value) || value < 0)
            {
                Console.Write("Введите корректное положительное число: ");
            }
            return value;
        }

        static decimal ReadPositiveDecimal()
        {
            decimal value;
            while (!decimal.TryParse(Console.ReadLine(), out value) || value < 0)
            {
                Console.Write("Введите корректное положительное число: ");
            }
            return value;
        }
    }
}