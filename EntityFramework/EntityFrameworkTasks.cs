using DatabaseCourseExam.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseCourseExam.EntityFramework;

public static class EntityFrameworkTasks
{
    // --- Задание 4: LINQ: группировка ---
    public static async Task ShowStudentsAsync()
    {
        await using var context = SchoolDbContextFactory.Create();

        List<Student> students = await context.Students
            .Include(student => student.Group)
            .OrderByDescending(student => student.Id)
            .ToListAsync();

        // Группируем в памяти
        var grouped = students.GroupBy(s => s.Group.Name);

        Console.WriteLine("--- Студенты по группам (EF Core) ---");
        foreach (var group in grouped)
        {
            Console.WriteLine($"\nГруппа: {group.Key} | Студентов: {group.Count()}");
            foreach (var student in group)
            {
                Console.WriteLine($"  {student.Id}. {student.FirstName} {student.LastName}, возраст: {student.Age}");
            }
        }
    }

    // --- Задание 5: конфликт уникальности ---
    public static async Task UpdateStudentAgeAsync(int studentId, int newAge)
    {
        await using var context = SchoolDbContextFactory.Create();

        Student? student = await context.Students.FirstOrDefaultAsync(x => x.Id == studentId);
        if (student is null)
        {
            Console.WriteLine("Студент не найден.");
            return;
        }

        student.Age = newAge;

        try
        {
            int affectedRows = await context.SaveChangesAsync();
            Console.WriteLine($"Возраст изменён. Сохранено изменений: {affectedRows}");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Ошибка БД: Нарушено ограничение уникальности или целостности.");

            Console.WriteLine($"Детали: {ex.InnerException?.Message}");
        }
    }

    // --- Задание 9: EF Core: проекция ---
    public static async Task ShowCoursesAsync()
    {
        await using var context = SchoolDbContextFactory.Create();

        var courses = await context.Courses
            .OrderBy(course => course.Name)
            .Select(course => new
            {
                course.Name,
                course.Price,
                course.AvailablePlaces,
                // Вычисляемый признак
                IsFull = course.AvailablePlaces == 0
            })
            .ToListAsync();

        Console.WriteLine("--- Курсы (EF Core) ---");
        foreach (var course in courses)
        {
            string status = course.IsFull ? " (МЕСТ НЕТ)" : "";
            Console.WriteLine($"{course.Name} | Цена: {course.Price:N2} | Свободных мест: {course.AvailablePlaces}{status}");
        }
    }
}