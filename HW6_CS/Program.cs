using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace HW6_CS
{
    delegate void myDel(List<string> _userName, List<string> _userPhone, List<string> _userEmail);
    public class myConnectToMSSQLDB
    {
        public SqlConnection connection;
        public myConnectToMSSQLDB()
        {
            string conStr = @"Data Source = (localdb)\MSSQLLocalDB;" + /* Имя сервера */
                "Initial Catalog = master;" + /* БД подключения*/
                "Integrated Security = True;" + /* Использование уч.записи Windows */
                "Connect Timeout = 30;" + /* Таймаут в секундах*/
                "Encrypt = False;" + /* Поддержка шифрования, работает в паре со сл.параметром */
                "Trust Server Certificate = False;" + /* Только при подключении к экземпляру SQL Server с допустимым сертификатом. Если ключевому слову TrustServerCertificate присвоено значение true, то транспортный уровень будет использовать протокол SSL для шифрования канала и не пойдет по цепочке сертификатов для проверки доверия. */
                "Application Intent = ReadWrite;" + /* Режим подключения*/
                "Multi Subnet Failover = False"; /* true - поддержка уровня доступности: оптимизирует работу для пользователей одной подсети*/
            var myConn = new SqlConnection(conStr);
            try
            {
                myConn.Open();
                Console.WriteLine($"Установлено соединение с параметрами {conStr}");
            }
            catch
            {
                Console.WriteLine($"Не удалось установить соединение с параметрами {conStr}");
            }
            finally
            {
                //myConn.Close();
                connection = myConn;
                Console.WriteLine($"Закрыто соединение с параметрами {conStr}");
            }
        }
    }
    internal class Program
    {
        static void WriteToFile(List<string> _userName, List<string> _userPhone, List<string> _userEmail)
        {
            string _name = "userContacts.txt";
            try
            {
                StreamWriter sw = new StreamWriter(_name, true);
                Console.WriteLine("\nДанные записаны в файл " + _name);
                sw.WriteLine(new string('-', 20 + 13 + 20 + 4));
                sw.WriteLine($"|{"Name",-20}|{"Phone",13}|{"Email",20}|");
                sw.WriteLine(new string('-', 20 + 13 + 20 + 4));
                for (int i = 0; i < _userName.Count; i++)
                {
                    sw.WriteLine($"|{_userName[i],-20}|{_userPhone[i],13}|{_userEmail[i],20}|");
                    sw.WriteLine(new string('-', 20 + 13 + 20 + 4));
                }
                _userName.Clear();
                _userPhone.Clear();
                _userEmail.Clear();
                sw.Close();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException: " + e.Message);
            }
        }
        static void WriteToDB(List<string> _userName, List<string> _userPhone, List<string> _userEmail)
        {
            var connect = new myConnectToMSSQLDB();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = connect.connection;
            for (int i = 0; i < _userName.Count; i++)
            {
                string _cmd = "use master;" + "insert into [UserContacts] (userName, userPhone, userEmail) values (" + _userName[i] + "," + _userPhone[i] + "," + _userEmail[i] + ");";
                sqlCommand.CommandText = _cmd;
            }


            //var dataReader = sqlCommand.ExecuteReader();
            ////Console.WriteLine(_cmd);
            //while (dataReader.Read())
            //{   
            //    int row = dataReader.FieldCount; // Вспомогательная переменная, количество возвращённых столбцов
            //    for (int i = 0; i < row; i++)
            //    {                    
            //       // Console.Write("  " + dataReader[i].ToString());
            //    }
            //    Console.WriteLine();
            //}
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Задание 002.\n" +
                    "Реализовать запись в базу данных таких полей, как имя, адрес эл.почты и телефон.\n" +
                    "Реализовать запись в БД и в файл через вызов делегата.\r\n" +
                    "_________________________________________________________________________________");

            List<string> userName = new List<string>();
            List<string> userEmail = new List<string>();
            List<string> userPhone = new List<string>();
            var regexpName = new Regex(@"([A-Я]{1}[а-яё]{1,23}|[A-Z]{1}[a-z]{1,23})");
            var regexpEmail = new Regex(@"([a-яA-Я0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6})");
            var regexPhone = new Regex(@"(8|\+7)(\d){10}");
            var regexStopEnter = new Regex(@"(END_INPUT)", RegexOptions.IgnoreCase);
            int counterStopEnter = 0;
            string name = string.Empty;
            string email = string.Empty;
            string phone = string.Empty;
            Console.WriteLine("Ведите через пробел в любом порядке:\nимя, номер телефона в формате +7XXXXXXXXXX или 8XXXXXXXXXX и адрес электронной почты\n" +
                "(для завершения ввода наберите END_INPUT): ");
            while (counterStopEnter == 0)
            {
                string str = Console.ReadLine();
                MatchCollection matchesStopEnter = regexStopEnter.Matches(str);
                foreach (Match match in matchesStopEnter)
                {
                    counterStopEnter++;
                }
                MatchCollection matchesName = regexpName.Matches(str);
                if (matchesName.Count > 0)
                {
                    foreach (Match match in matchesName)
                    {
                        name = matchesName[0].ToString();
                    }
                }
                else
                {
                    name = "default_name";
                }
                userName.Add(name);
                MatchCollection matchesEmail = regexpEmail.Matches(str);
                if (matchesEmail.Count > 0)
                {
                    foreach (Match match in matchesEmail)
                    {
                        email = matchesEmail[0].ToString();
                    }
                }
                else
                {
                    email = "default_email";
                }
                userEmail.Add(email);
                MatchCollection matchesPhone = regexPhone.Matches(str);
                if (matchesPhone.Count > 0)
                {
                    foreach (Match match in matchesPhone)
                    {
                        phone = matchesPhone[0].ToString();
                    }
                }
                else
                {
                    phone = "default_phone";
                }
                userPhone.Add(phone);
            }
            Console.WriteLine(new string('-', 20 + 13 + 20 + 4));
            Console.WriteLine($"|{"Name",-20}|{"Phone",13}|{"Email",20}|");
            Console.WriteLine(new string('-', 20 + 13 + 20 + 4));

            for (int i = 0; i < userName.Count; i++)
            {
                Console.WriteLine($"|{userName[i],-20}|{userPhone[i],13}|{userEmail[i],20}|");
                Console.WriteLine(new string('-', 20 + 13 + 20 + 4));
            }
            Console.ReadKey();

            
            myDel output;
            output = WriteToFile;
            output += WriteToDB;
            output.Invoke(userName, userPhone, userEmail);
            //output.DynamicInvoke(userName, userPhone, userEmail);
            Console.ReadKey();

        }
    }
}
            
            
       
