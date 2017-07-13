using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;

namespace eFarma2ReportsRetrive
{
    class Program
    {
        private static int ReportCounter = 0;
        static void Main(string[] args)
        {

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString))
            {
                connection.Open();
                ExportReports(connection);
            }

            
        }
        
        /// <summary>
        /// Перебираем все репорты в базе и выгружаем по одной штуке
        /// </summary>
        /// <param name="connection">проинициализированое подключение к базе. Строка подключения в .config</param>
        private static void ExportReports(SqlConnection connection)
        {

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "select * from meta_report";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WriteCertificate(reader);
                    }
                }

            }

        }
        /// <summary>
        /// Записывает сертификат отчета из мета репорт
        /// </summary>
        /// <param name="reader">SQL Reader с таблиценй отчета</param>
        private static void WriteCertificate(SqlDataReader reader)
        {
            string ReportAssembly;
            string ReportCertificate;
            string pPath = Directory.GetCurrentDirectory();

            /*for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.WriteLine(i.ToString() + "   " + reader[i].GetType().ToString());
                Console.WriteLine(reader[i].ToString());

            }*/
            ReportCounter++;

            ReportAssembly = reader[9].ToString();
            ReportCertificate = reader[15].ToString();
            //using (var output = new StreamWriter(Path.Combine(pPath, ReportAssembly.Replace(".dll", ".cert")), false, Encoding.GetEncoding("Windows-1251"))) // добавить дату fname
            using (var output = new StreamWriter(Path.Combine(pPath, ReportAssembly.Replace(".dll", ".cert")), false, Encoding.Unicode )) // добавить дату fname
            {
             Console.WriteLine(ReportCounter.ToString() +  ". Записываю " +reader[10].ToString() +" " + ReportAssembly.Replace(".dll", ".cert"));
             output.Write(ReportCertificate);

            }

                WrireReportAssembly(pPath, ReportAssembly );
            
        }
        /// <summary>
        /// Записывает сборку отчета из мета репорт
        /// </summary>
        /// <param name="pPath">путь куда сохраняем </param>
        /// <param name="AssemblyName">имя файла для сохранения отчета</param>
        private static void WrireReportAssembly (String pPath , String AssemblyName )
            {

            Console.WriteLine("Записываю " +  AssemblyName );
             
            /// взято на стэковерфлоу
            SqlConnection connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            connection.Open();
            // Select binary data from db
            SqlCommand command = new
            SqlCommand("select assembly from meta_report where source = '" + AssemblyName + "'", connection);
             byte[] buffer = (byte[])command.ExecuteScalar();
            connection.Close();
            // storing file to C drive
            FileStream fs = new FileStream(Path.Combine(pPath , AssemblyName) , FileMode.Create);
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();

            } 
    }
}
