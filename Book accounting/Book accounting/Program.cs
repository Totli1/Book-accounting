using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

namespace Book_accounting
{
    internal class Program
    {
        static void Main(string[] args)
        {
                var library = new Library();
                int id = 0;

     
            
            while (true)
            {
                Console.WriteLine("1. Добавить книгу | 2. Удалить книгу | 3. Список книг | 4. Поиск | 5. Выйти");
                var books = FileManager.LoadFromJson("books.json");
                var choice = Console.ReadLine();
                string name = null, author = null;
                int year = 0;
                switch (choice)
                {
                    case "1":
                        try
                        {
                            Console.Write("Введите название:");
                            name = Console.ReadLine();
                            Console.Write("Введите автора:");
                            author = Console.ReadLine();
                            Console.Write("Введите год:");
                            year = int.Parse(Console.ReadLine());

                            if (books.Count != 0) id = books[books.Count - 1].Id + 1;

                            var book = new Book(id, name, author, year);
                            library.AddBook(book);
                            FileManager.SaveToJson(library.GetAllBooks(), "books.json");

                            Console.WriteLine("Книга добавлена!");
                        }
                        catch 
                        {
                            Console.WriteLine("Ошибка");
                        }
                        break;
                    case "2":
                        try
                        {
                            Console.Write("Введите id: ");
                            int idRemove = int.Parse(Console.ReadLine());
                            library.RemoveBook(idRemove);
                            FileManager.SaveToJson(library.GetAllBooks(), "books.json");
                        }
                        catch
                        {
                            Console.WriteLine("Ошибка");
                        }
                        
                        break;

                    case "3":
                        foreach (var b in library.GetAllBooks())
                            Console.WriteLine($"{b.Id}. {b.Title} ({b.Author}, {b.Year})");
                        break;

                    case "4":
                        Console.WriteLine("Поиск по 1. id | 2. Названию | 3. Автору | 4. Году");
                        var choiceFind = Console.ReadLine();
                        Console.Write("Вводите символ: ");
                        var keyword = Console.ReadLine();

                        foreach (var b in library.SearchBooks(choiceFind,keyword))
                            Console.WriteLine($"{b.Id}. {b.Title} ({b.Author}, {b.Year})");
                        break;

                    case "5":
                        FileManager.SaveToCsv(library.GetAllBooks(),"books.csv");
                        return;
                }
            }
        }
    }

    class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public Book(int id, string title, string author, int year)
        {
            Id = id;
            Title = title;
            Author = author;
            Year = year;
        }
    }
     class Library
    {
        private List<Book> _books = FileManager.LoadFromJson("books.json");

        public void AddBook(Book book) => _books.Add(book);
        
        public void RemoveBook(int id) => _books.RemoveAt(id);
        
        public List<Book> GetAllBooks() => _books;

        public List<Book> SearchBooks(string choice,string keyword)
        {
            var booksFind = choice switch
            {
                "1" => _books.FindAll(x => x.Id == int.Parse(keyword)),
                "2" => _books.Where(x => x.Title.StartsWith(keyword,StringComparison.OrdinalIgnoreCase)).ToList(),
                "3" => _books.FindAll(x => x.Author.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)).ToList(),
                "4" => _books.FindAll(x => x.Year == int.Parse(keyword)),
                _ => _books
            };
            return booksFind;

        }
        
    }
 
     static class FileManager
    { 
        public static void SaveToCsv(List<Book> books, string filePath)
        {
            var lines = new List<string> { "ID,Title,Author,Year" };
            lines.AddRange(books.Select(b => $"{b.Id},{b.Title},{b.Author},{b.Year}"));
            File.WriteAllLines(filePath, lines);
        }

        public static void SaveToJson(List<Book> books, string filePath)
        {

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };

            string json = JsonSerializer.Serialize(books, options);
            File.WriteAllText(filePath, json);
        }
        public static List<Book> LoadFromJson(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                };
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Book>>(json, options);
            }
            catch (JsonException)
            {
               
                return new List<Book>();
            }
        }


    }
}
