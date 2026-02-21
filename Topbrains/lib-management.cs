using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem
{
    class Program
    {
        static List<dynamic> books = new List<dynamic>();
        static int bookIdCounter = 1;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n===== BOOK LIBRARY MANAGEMENT SYSTEM =====");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. User");
                Console.WriteLine("3. Exit");
                Console.Write("Select role: ");

                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        AdminMenu();
                        break;
                    case 2:
                        UserMenu();
                        break;
                    case 3:
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        // ---------------- ADMIN ----------------

        static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n----- ADMIN MENU -----");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. Update Book");
                Console.WriteLine("3. Delete Book");
                Console.WriteLine("4. View All Books");
                Console.WriteLine("5. Back");
                Console.Write("Choice: ");

                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        AddBook();
                        break;
                    case 2:
                        UpdateBook();
                        break;
                    case 3:
                        DeleteBook();
                        break;
                    case 4:
                        ViewAllBooks();
                        break;
                    case 5:
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        static void AddBook()
        {
            Console.Write("Enter Book Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Publisher: ");
            string publisher = Console.ReadLine();

            Console.Write("Enter Price: ");
            double price = Convert.ToDouble(Console.ReadLine());

            dynamic book = new
            {
                Id = bookIdCounter++,
                Name = name,
                Publisher = publisher,
                Price = price
            };

            books.Add(book);
            Console.WriteLine("Book added successfully.");
        }

        static void UpdateBook()
        {
            Console.Write("Enter Book ID to update: ");
            int id = Convert.ToInt32(Console.ReadLine());

            var book = books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            Console.Write("Enter New Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter New Publisher: ");
            string publisher = Console.ReadLine();

            Console.Write("Enter New Price: ");
            double price = Convert.ToDouble(Console.ReadLine());

            books.Remove(book);

            dynamic updatedBook = new
            {
                Id = id,
                Name = name,
                Publisher = publisher,
                Price = price
            };

            books.Add(updatedBook);
            Console.WriteLine("Book updated successfully.");
        }

        static void DeleteBook()
        {
            Console.Write("Enter Book ID to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());

            var book = books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            books.Remove(book);
            Console.WriteLine("Book deleted successfully.");
        }

        static void ViewAllBooks()
        {
            if (books.Count == 0)
            {
                Console.WriteLine("No books available.");
                return;
            }

            Console.WriteLine("\n--- BOOK LIST ---");
            foreach (var book in books)
            {
                DisplayBook(book);
            }
        }

        // ---------------- USER ----------------

        static void UserMenu()
        {
            while (true)
            {
                Console.WriteLine("\n----- USER MENU -----");
                Console.WriteLine("1. Browse Books");
                Console.WriteLine("2. Search by Name");
                Console.WriteLine("3. Search by Publisher");
                Console.WriteLine("4. Highest Price Book");
                Console.WriteLine("5. Lowest Price Book");
                Console.WriteLine("6. Back");
                Console.Write("Choice: ");

                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        ViewAllBooks();
                        break;
                    case 2:
                        SearchByName();
                        break;
                    case 3:
                        SearchByPublisher();
                        break;
                    case 4:
                        HighestPriceBook();
                        break;
                    case 5:
                        LowestPriceBook();
                        break;
                    case 6:
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        static void SearchByName()
        {
            Console.Write("Enter book name: ");
            string name = Console.ReadLine();

            var result = books.Where(b => b.Name.ToLower().Contains(name.ToLower())).ToList();

            if (result.Count == 0)
            {
                Console.WriteLine("No books found.");
                return;
            }

            foreach (var book in result)
            {
                DisplayBook(book);
            }
        }

        static void SearchByPublisher()
        {
            Console.Write("Enter publisher name: ");
            string publisher = Console.ReadLine();

            var result = books.Where(b => b.Publisher.ToLower().Contains(publisher.ToLower())).ToList();

            if (result.Count == 0)
            {
                Console.WriteLine("No books found.");
                return;
            }

            foreach (var book in result)
            {
                DisplayBook(book);
            }
        }

        static void HighestPriceBook()
        {
            if (books.Count == 0)
            {
                Console.WriteLine("No books available.");
                return;
            }

            var book = books.OrderByDescending(b => b.Price).First();
            Console.WriteLine("\nHighest Priced Book:");
            DisplayBook(book);
        }

        static void LowestPriceBook()
        {
            if (books.Count == 0)
            {
                Console.WriteLine("No books available.");
                return;
            }

            var book = books.OrderBy(b => b.Price).First();
            Console.WriteLine("\nLowest Priced Book:");
            DisplayBook(book);
        }

        // ---------------- COMMON ----------------

        static void DisplayBook(dynamic book)
        {
            Console.WriteLine($"ID: {book.Id}, Name: {book.Name}, Publisher: {book.Publisher}, Price: {book.Price}");
        }
    }
}
