using ConsoleTests.Data;
using ConsoleTests.Data.Entityes;
using MailSender.lib.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {

            Assembly asm = Assembly.GetEntryAssembly();

            Type type = asm.GetType("ConsoleTests.Program");
            Type type2 = asm.GetTypes().First(t => t.Name == "Program");

            var str = "Hello World!";

            var type3 = GetObjectType(str);

            var type_string = typeof(string);

            var test_lib_file = new FileInfo("TestLib.dll");
            var test_lib_assembly = Assembly.LoadFile(test_lib_file.FullName);

            var printer_type = test_lib_assembly.GetType("TestLib.Printer");

            //ConstructorInfo
            //MethodInfo
               //ParameterInfo
            //PropertyInfo
            //EventInfo
            //FieldInfo

            foreach(var method in printer_type.GetMethods())
            {
                var return_type = method.ReturnType;
                var parameters = method.GetParameters();

                Console.WriteLine("{0} {1}({2})",
                    return_type.Name,
                    method.Name,
                    string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}")));
            }

            object printer = Activator.CreateInstance(printer_type, ">>>");

            var printer_constructor = printer_type.GetConstructor(new[] { typeof(string) });

            var printer2 = printer_constructor.Invoke(new object[] { "<<<" });

            var print_method_info = printer_type.GetMethod("Print");

            print_method_info.Invoke(printer, new object[] { "Hello World!" });

            var prefix_field_info = printer_type.GetField("_Prefix", BindingFlags.NonPublic | BindingFlags.Instance);

            object prefix_value_object = prefix_field_info.GetValue(printer);
            var prefix_value_string = (string)prefix_field_info.GetValue(printer);

            prefix_field_info.SetValue(printer, "123");

            //var app_domain = AppDomain.CurrentDomain;
            //var test_domain = AppDomain.CreateDomain("TestDomain");
            ////test_domain.ExecuteAssemblyByName()           
            //AppDomain.Unload(test_domain);

            //var admin_process_info = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, "/RegistryWrite")
            //{

            //};

            //Process process = Process.Start(admin_process_info);

            //dynamic dynamic_printer = printer;
            //dynamic_printer.Print("1231212332123");

            Action<string> print_lambda = str => Console.WriteLine(str);

            Expression<Action<string>> print_expression = str => Console.WriteLine(str);

            Action<string> complied_expression = print_expression.Compile();

            var str_parameter = Expression.Parameter(typeof(string), "str");

            var invoke_node = Expression.Call(
                null, 
                typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), 
                str_parameter);

            var result_expression = Expression.Lambda<Action<string>>(invoke_node, str_parameter);

            Action<string> complied_expression2 = print_expression.Compile();

            complied_expression("QWE");
            complied_expression2("===");

            Console.ReadLine();
            
            //var report = new StatisticReport();

            //report.SendedMailsCount = 1000;

            //report.CreatePackage("statistic.docx");

            //Console.ReadLine();

            //const string connection_str = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = Students.DB; Integrated Security = True";

            ////var service_collection = new ServiceCollection();
            ////service_collection.AddDbContext<StudentsDB>(opt => opt.UseSqlServer(connection_str));

            ////var services = service_collection.BuildServiceProvider();

            ////using (var db = services.GetRequiredService<StudentsDB>())
            ////{

            ////}

            //using (var db = new StudentsDB(new DbContextOptionsBuilder<StudentsDB>().UseSqlServer(connection_str).Options))
            //{
            //    //await db.Database.EnsureCreatedAsync();
            //    await db.Database.MigrateAsync();

            //    var students_count = await db.Students.CountAsync();

            //    Console.WriteLine("Число студентов в БД = {0}", students_count);
            //}

            //using (var db = new StudentsDB(new DbContextOptionsBuilder<StudentsDB>().UseSqlServer(connection_str).Options))
            //{

            //    var k = 0;
            //    if (await db.Students.CountAsync() == 0)
            //    {
            //        for (var i = 0; i < 10; i++)
            //        {
            //            var group = new Group
            //            {
            //                Name = $"Группа {i}",
            //                Description = $"Описание группы {i}",
            //                Students = new List<Student>()
            //            };

            //            for (var J = 0; J < 10; J++)
            //            {
            //                var student = new Student
            //                {
            //                    Name = $"Студент {k}",
            //                    Surname = $"Surname {k}",
            //                    Patronymic = $"Patronymic {k}",                                
            //                };
            //                k++;
            //                group.Students.Add(student);
            //            }

            //            await db.Groups.AddAsync(group);
            //        }
            //        await db.SaveChangesAsync();
            //    }             
            //}

            //using (var db = new StudentsDB(new DbContextOptionsBuilder<StudentsDB>().UseSqlServer(connection_str).Options))
            //{
            //    var students = await db.Students
            //        .Include(s => s.Group) //Join
            //        .Where(s => s.Group.Name == "Группа 5")
            //        .ToArrayAsync();

            //    foreach (var student in students)
            //    {
            //        Console.WriteLine("[{0}] {1} - {2}", student.Id, student.Name, student.Group.Name);
            //    }
            //}

            //Console.WriteLine("Главный поток работу закончил!");
            //Console.ReadLine();
        }
        private static Type GetObjectType(object obj)
        {
            return obj.GetType();
        }
    }
}