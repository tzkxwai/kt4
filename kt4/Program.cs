using System;
using System.IO;
using System.Collections.Generic;

public class FileProcessor
{
    private const long MaxFileSize = 1024 * 1024;

    public void ProcessFile(string filePath)
    {
        Console.WriteLine($"[INFO] Начало обработки файла: {filePath}");

        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {filePath}");
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length == 0)
            {
                throw new Exception("Файл пуст");
            }

            if (fileInfo.Length > MaxFileSize)
            {
                throw new Exception("Файл слишком большой");
            }

            string content;
            using (StreamReader reader = new StreamReader(filePath))
            {
                content = reader.ReadToEnd();
                Console.WriteLine($"[INFO] Файл прочитан успешно. Размер: {content.Length} байт");
            }

            if (content.Contains("malicious"))
            {
                throw new Exception("Обнаружены запрещенные данные");
            }

            if (content.Contains("error"))
            {
                throw new Exception("Ошибка в данных");
            }

            string processedContent = content.ToUpper();

            string resultPath = Path.GetFileNameWithoutExtension(filePath) + ".processed";
            using (StreamWriter writer = new StreamWriter(resultPath))
            {
                writer.Write(processedContent);
            }

            Console.WriteLine($"[INFO] Результат сохранен в: {resultPath}");
            Console.WriteLine($"[INFO] Обработка файла завершена успешно");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"[ERROR] Ошибка обработки файла: {ex.Message}");
            Console.WriteLine($"[WARN] Ошибка в файле: {filePath} - Файл не существует");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Ошибка обработки файла: {ex.Message}");
            Console.WriteLine($"[WARN] Ошибка в файле: {filePath} - {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"[INFO] Завершение обработки файла: {filePath}");
        }
    }
}

public class Program
{
    static void Main()
    {
        CreateTestFiles();

        FileProcessor processor = new FileProcessor();

        List<string> filesToProcess = new List<string>
        {
            "valid_file.txt",
            "nonexistent_file.txt",
            "empty_file.txt",
            "large_file.txt",
            "malicious_file.txt"
        };

        int successCount = 0;
        int errorCount = 0;

        Console.WriteLine($"[INFO] Начало обработки {filesToProcess.Count} файлов");

        foreach (string filePath in filesToProcess)
        {
            try
            {
                processor.ProcessFile(filePath);
                successCount++;
                Console.WriteLine($"[INFO] Файл обработан: {filePath}");
            }
            catch (Exception ex)
            {
                errorCount++;
                Console.WriteLine($"[ERROR] Критическая ошибка при обработке файла {filePath}: {ex.Message}");
            }
        }

        Console.WriteLine($"[INFO] Обработка завершена. Успешно: {successCount}, С ошибками: {errorCount}");
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static void CreateTestFiles()
    {
        try
        {
            File.WriteAllText("valid_file.txt", "This is valid content");
            File.WriteAllText("empty_file.txt", "");
            File.WriteAllText("large_file.txt", new string('x', 2000000));
            File.WriteAllText("malicious_file.txt", "This contains malicious content");
            Console.WriteLine("[INFO] Тестовые файлы созданы");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Ошибка создания тестовых файлов: {ex.Message}");
        }
    }
}