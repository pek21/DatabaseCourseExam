using DatabaseCourseExam.AdoNet;
using System.Data;
using DatabaseCourseExam.EntityFramework;
using DatabaseCourseExam.DapperTasks;

DataTable? studentsTable = null;

while (true)
{
    Console.WriteLine("\n=== Зачёт по работе с БД ===");
    Console.WriteLine("1. Показать студентов (ADO.NET)");
    Console.WriteLine("2. Изменить возраст (ADO.NET)");
    Console.WriteLine("3. Записать на курс (ADO.NET)");
    Console.WriteLine("4. Загрузить в DataTable");
    Console.WriteLine("5. Изменить возраст в DataTable");
    Console.WriteLine("6. Показать DataTable");
    Console.WriteLine("7. Сохранить изменения DataTable");
    Console.WriteLine("8. Показать студентов (EF Core)");
    Console.WriteLine("9. Изменить возраст (EF Core)");
    Console.WriteLine("10. Показать курсы (EF Core)");
    Console.WriteLine("11. Показать студентов (Dapper)");
    Console.WriteLine("12. Изменить возраст (Dapper)");
    Console.WriteLine("13. Показать курсы (Dapper)");
    Console.WriteLine("14. Записать на курс (Dapper)");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите действие: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1": await AdoNetTasks.ShowStudentsAsync(); break;
            case "2":
                Console.Write("ID студента: ");
                if (!int.TryParse(Console.ReadLine(), out int stId)) break;
                Console.Write("Новый возраст: ");
                if (!int.TryParse(Console.ReadLine(), out int age)) break;
                await AdoNetTasks.UpdateStudentAgeAsync(stId, age);
                break;
            case "3":
                Console.Write("ID студента: ");
                int enrollStId = int.Parse(Console.ReadLine()!);
                Console.Write("ID курса: ");
                int enrollCrId = int.Parse(Console.ReadLine()!);
                await AdoNetTasks.EnrollStudentAsync(enrollStId, enrollCrId);
                break;
            case "4":
                studentsTable = await DisconnectedTasks.LoadStudentsAsync();
                DisconnectedTasks.PrintTable(studentsTable);
                break;
            case "5":
                if (studentsTable is null) break;
                Console.Write("ID студента: ");
                int dcStId = int.Parse(Console.ReadLine()!);
                Console.Write("Новый возраст: ");
                int dcAge = int.Parse(Console.ReadLine()!);
                DisconnectedTasks.ChangeAgeInMemory(studentsTable, dcStId, dcAge);
                break;
            case "6":
                if (studentsTable != null) DisconnectedTasks.PrintTable(studentsTable);
                break;
            case "7":
                if (studentsTable != null) DisconnectedTasks.SaveChanges(studentsTable);
                break;
            case "8": await EntityFrameworkTasks.ShowStudentsAsync(); break;
            case "9":
                Console.Write("ID студента: ");
                if (!int.TryParse(Console.ReadLine(), out int efStId)) break;
                Console.Write("Новый возраст: ");
                if (!int.TryParse(Console.ReadLine(), out int efAge)) break;
                await EntityFrameworkTasks.UpdateStudentAgeAsync(efStId, efAge);
                break;
            case "10": await EntityFrameworkTasks.ShowCoursesAsync(); break;
            case "11": await DapperStudentTasks.ShowStudentsAsync(); break;
            case "12":
                Console.Write("ID студента: ");
                if (!int.TryParse(Console.ReadLine(), out int dpStId)) break;
                Console.Write("Новый возраст: ");
                if (!int.TryParse(Console.ReadLine(), out int dpAge)) break;
                await DapperStudentTasks.UpdateStudentAgeAsync(dpStId, dpAge);
                break;
            case "13": await DapperStudentTasks.ShowCoursesAsync(); break;
            case "14":
                Console.Write("ID студента: ");
                int dpEnrollStId = int.Parse(Console.ReadLine()!);
                Console.Write("ID курса: ");
                int dpEnrollCrId = int.Parse(Console.ReadLine()!);
                await DapperStudentTasks.EnrollStudentAsync(dpEnrollStId, dpEnrollCrId);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Неизвестная команда.");
                break;
        }
    }
    catch (FormatException) { Console.WriteLine("Введите целое число."); }
    catch (Exception ex) { Console.WriteLine($"Ошибка: {ex.Message}"); }
}