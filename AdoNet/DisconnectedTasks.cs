using System.Data;
using DatabaseCourseExam.Data;
using Npgsql;

namespace DatabaseCourseExam.AdoNet;

public static class DisconnectedTasks
{
    public static async Task<DataTable> LoadStudentsAsync()
    {
        var dataSet = new DataSet();

        await using var connection = DatabaseConnection.Create();
        await connection.OpenAsync();

        using var adapterStudents = new NpgsqlDataAdapter("SELECT id, first_name, last_name, age, group_id FROM students ORDER BY id;", connection);
        adapterStudents.Fill(dataSet, "Students");

        using var adapterGroups = new NpgsqlDataAdapter("SELECT id, name FROM student_groups ORDER BY id;", connection);
        adapterGroups.Fill(dataSet, "Groups");

        DataTable students = dataSet.Tables["Students"]!;
        DataTable groups = dataSet.Tables["Groups"]!;


        DataRelation relation = new DataRelation(
            "GroupStudents",
            groups.Columns["id"]!,
            students.Columns["group_id"]!
        );
        dataSet.Relations.Add(relation);

        students.PrimaryKey = [students.Columns["id"]!];

        return students;
    }

    public static void PrintTable(DataTable table)
    {
        foreach (DataRow row in table.Rows)
        {
            Console.WriteLine($"{row["id"]}: {row["first_name"]} {row["last_name"]}, возраст {row["age"]}");
        }
    }

    public static void ChangeAgeInMemory(DataTable table, int studentId, int newAge)
    {
        DataRow? row = table.Rows.Find(studentId);
        if (row is null)
        {
            Console.WriteLine("Студент не найден в DataTable.");
            return;
        }
        row["age"] = newAge;
        Console.WriteLine($"Возраст изменён в памяти. Состояние строки: {row.RowState}");
    }

    public static int SaveChanges(DataTable table)
    {
        using var connection = DatabaseConnection.Create();
        connection.Open();

        const string sql = "UPDATE students SET age = @age WHERE id = @id;";
        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("@age", NpgsqlTypes.NpgsqlDbType.Integer, 4, "age");
        command.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer, 4, "id");

        using var adapter = new NpgsqlDataAdapter();
        adapter.UpdateCommand = command;

        int affectedRows = adapter.Update(table);
        Console.WriteLine($"Сохранено изменений: {affectedRows}");

        DataRow[] modifiedRows = table.Select("", "", DataViewRowState.ModifiedCurrent);
        Console.WriteLine($"Осталось строк в состоянии Modified: {modifiedRows.Length}");

        return affectedRows;
    }
}