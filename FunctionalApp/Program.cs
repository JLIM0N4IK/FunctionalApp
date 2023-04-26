using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalApp
{
    class Instrument //Класс для работы с инструментами
    {
        public int Ind { get; set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public Instrument(int ind, string brand, string type, double price, int quantity)
        {
            Ind = ind;
            Brand = brand;
            Type = type;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Brand} {Type}, Цена: {Price} руб., Количество: {Quantity}";
        }
    }

    class FileReader //Для считывания данных из файла в структуру
    {
        public static List<Instrument> ReadInstrumentsFromFile(string fileName)
        {
            List<Instrument> instruments = new List<Instrument>();

            try //Обработчик исключений, в случае ошибки в строке он пропустит её и запишет только верные
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] values = line.Split(';');
                        if (values.Length < 5) //Проверка на формат
                        {
                            Console.WriteLine($"Ошибка: некорректный формат данных в строке '{line}'");
                            continue;
                        }
                        int index = int.Parse(values[0]);
                        string brand = values[1];
                        string type = values[2];
                        double price;
                        if (!double.TryParse(values[3], out price)) //Проверка на цену
                        {
                            Console.WriteLine($"Ошибка: некорректный формат цены в строке '{line}'");
                            continue;
                        }
                        int quantity;
                        if (!int.TryParse(values[4], out quantity)) //Проверка на количество
                        {
                            Console.WriteLine($"Ошибка: некорректный формат количества в строке '{line}'");
                            continue;
                        }

                        instruments.Add(new Instrument(index, brand, type, price, quantity));
                    }
                }
            }
            catch (FileNotFoundException) //Вывод ошибок
            {
                Console.WriteLine($"Ошибка: файл {fileName} не найден.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return instruments;
        }

        public static void WriteInstrumentsToFile(string fileName, List<Instrument> instruments) //Функция для записи в файл при закрытии приложения
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (Instrument instrument in instruments)
                {
                    sw.WriteLine($"{instrument.Ind};{instrument.Brand};{instrument.Type};{instrument.Price};{instrument.Quantity}");
                }
            }
        }

    }

    class ProgramClient //Функционал программы для клиента
    {
        public static void DisplayInstrumentList(List<Instrument> instruments, int numInstruments) //Вывод инструментов для покупки
        {
            Console.WriteLine("Список инструментов:");
            int count = 0;
            foreach (var instrument in instruments)
            {
                Console.WriteLine(instrument.ToString());
                count++;
                if (count >= numInstruments) break;
            }
        }

        public static List<Instrument> SelectInstruments(List<Instrument> instruments, int numInstruments)
        {
            Console.WriteLine("Выберите инструменты:");

            List<Instrument> selectedInstruments = new List<Instrument>();
            int index = 0;
            while (selectedInstruments.Count < numInstruments)
            {
                Console.WriteLine($"Выберите инструмент {selectedInstruments.Count + 1}:");
                foreach (var instrument in instruments)
                {
                    Console.WriteLine($"{++index}. {instrument.ToString()}");
                }

                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > instruments.Count)
                {
                    Console.WriteLine("Некорректный выбор. Пожалуйста, выберите номер инструмента из списка.");
                }

                var selectedInstrument = instruments[choice - 1];
                if (!selectedInstruments.Contains(selectedInstrument))
                {
                    selectedInstruments.Add(selectedInstrument);
                    index = 0;
                }
                else
                {
                    Console.WriteLine("Инструмент уже выбран. Пожалуйста, выберите другой.");
                    index = 0;
                }
            }

            return selectedInstruments;
        }

        public static double CalculateTotalCost(List<Instrument> selectedInstruments)
        {
            double totalCost = 0;
            foreach (var instrument in selectedInstruments)
            {
                totalCost += instrument.Price;
            }
            return totalCost;
        }

    }

    class ProgramCusomer //Функционал программы для продавца
    {

    }

    internal class MainActivity
    {
        static List<Instrument> instruments;

        static void Main(string[] args)
        {
            //Обработчик событий при закрытии программы
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);

            instruments = FileReader.ReadInstrumentsFromFile("instruments.txt");

            // Вывод меню выбора режима работы программы
            Console.WriteLine("Выберите режим работы программы:");
            Console.WriteLine("1 - для работы с программой в режиме клиента");
            Console.WriteLine("2 - для работы с программой в режиме продавца");
           
            int mode;
            
            while (!int.TryParse(Console.ReadLine(), out mode))
            {
                Console.WriteLine("Некорректный выбор. Пожалуйста, выберите 1 или 2.");
            }

            // Обработка выбора режима работы программы
            if (mode == 1)
            {
                // Логика программы для клиента
                Console.WriteLine("Сколько инструментов Вы хотите приобрести?");
                int numInstruments;
                while (!int.TryParse(Console.ReadLine(), out numInstruments) || numInstruments <= 0 || numInstruments >= 101)
                {
                    Console.WriteLine("Некорректное количество. Пожалуйста, введите положительное целое число.");
                }

                ProgramClient.DisplayInstrumentList(instruments, numInstruments);
                List<Instrument> selectedInstruments = ProgramClient.SelectInstruments(instruments, numInstruments);
                double totalCost = ProgramClient.CalculateTotalCost(selectedInstruments);

                Console.WriteLine($"Общая сумма заказа: {totalCost} руб.");
            }
            else if (mode == 2)
            {
                // Логика программы для продавца
            }
            else
            {
                Console.WriteLine("Некорректный выбор. Пожалуйста, выберите 1 или 2.");
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            FileReader.WriteInstrumentsToFile("instruments.txt", instruments);
        }

        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            FileReader.WriteInstrumentsToFile("instruments.txt", instruments);
        }
    }
}