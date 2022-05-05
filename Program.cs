using System;
using NLog.Web;
using System.IO;
using System.Linq;
using Week11_NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                string choice;
                string productOptionChoice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Display records from Products table");
                    Console.WriteLine("6) Add new record to Products table");
                    Console.WriteLine("7) Edit record from product table");
                    Console.WriteLine("8) Display record from product table");
                    Console.WriteLine("9) Display active products by category (inactive products in red)");
                    Console.WriteLine("10) Display active products of a specific category (inative products in red)");
                    Console.WriteLine("11) Edit record from categories table");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NWConsole_48_AMSContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "2")
                    {
                        var db = new NWConsole_48_AMSContext();
                        Category category = new Category();
                        Console.Write("Enter Category Name: ");
                        category.CategoryName = Console.ReadLine();
                        Console.Write("Enter the Category Description: ");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                // TODO: Add category
                                db.AddCategory(category);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        var db = new NWConsole_48_AMSContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NWConsole_48_AMSContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if (choice == "5")
                    {
                        //display records from product table (productname only)
                        //user decides if they want to see all products, disc products, or active products
                        Console.WriteLine("1) See all products");
                        Console.WriteLine("2) See all active products");
                        Console.WriteLine("3) See all discontinued products");
                        productOptionChoice = Console.ReadLine();
                        if (productOptionChoice == "1")
                        {
                            //display all products
                            var db = new NWConsole_48_AMSContext();
                            var query = db.Products.OrderBy(p => p.ProductId);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (productOptionChoice == "2")
                        {
                            //display active products (where Discontinued is 0)
                            var db = new NWConsole_48_AMSContext();
                            var query = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductId);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (productOptionChoice == "3")
                        {
                            //display discontinued products (where Discontinued is true)
                            var db = new NWConsole_48_AMSContext();
                            var query = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductId);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else if (choice == "6")
                    {
                        //add record to products table
                        var db = new NWConsole_48_AMSContext();
                        Product product = new Product();
                        Console.WriteLine("Enter Product Name:");
                        //product.ProductName = Console.ReadLine();
                        var productName = Console.ReadLine();
                        Console.WriteLine("Enter product's unit price: ");
                        decimal productUnitPrice = Convert.ToDecimal(Console.ReadLine());
                        Console.WriteLine("Enter quantity in stock: ");
                        short productUnitsInStock = Convert.ToInt16(Console.ReadLine());
                        Console.WriteLine("Enter product's category ID number from list below: ");
                        //show list of categories and IDs
                        var query = db.Categories.OrderBy(p => p.CategoryName);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId} - {item.CategoryName}: {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int productCategoryId = Convert.ToInt32(Console.ReadLine());


                        //What does this do?
                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            // var db = new NWConsole_48_AMSContext();
                            // check for unique name
                            if (db.Products.Any(c => c.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                var newProduct = new Product
                                {
                                    ProductName = productName,
                                    UnitPrice = productUnitPrice,
                                    UnitsInStock = productUnitsInStock,
                                    CategoryId = productCategoryId
                                };
                                // TODO: Add Product
                                db.AddProduct(newProduct);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if (choice == "7")
                    {
                        //edit record from product table 
                        var db = new NWConsole_48_AMSContext();
                        Console.WriteLine("Enter product ID you'd like to edit from list below: ");
                        var query = db.Products.OrderBy(p => p.ProductId);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.ProductId} - {item.ProductName}");
                        }

                        int editedProductId = Convert.ToInt32(Console.ReadLine());
                        Product changedProduct = db.Products.FirstOrDefault(p => p.ProductId == editedProductId);
                        Console.Write("Enter product name: ");
                        changedProduct.ProductName = Console.ReadLine();
                        db.EditProduct(changedProduct);
                    }
                    else if (choice == "8")
                    {
                        //display record from product table (all product fields)
                        var db = new NWConsole_48_AMSContext();
                        Console.WriteLine("Enter product name you'd like to display: ");
                        var productChoice = Console.ReadLine();
                        var query = db.Products.Where(p => p.ProductName == productChoice);
                        Console.WriteLine();
                        if (query.Count() == 0)
                        {
                            logger.Info("Searched for invalid product.");
                        }
                        else
                        {
                            foreach (var item in query)
                            {
                                Console.WriteLine("Product ID: " + item.ProductId);
                                Console.WriteLine("Product name: " + item.ProductName);
                                Console.WriteLine("Supplier ID: " + item.SupplierId);
                                Console.WriteLine("Category ID: " + item.CategoryId);
                                Console.WriteLine("Quantity per unit: " + item.QuantityPerUnit);
                                Console.WriteLine("Unit price: " + item.UnitPrice);
                                Console.WriteLine("Units in stock: " + item.UnitsInStock);
                                Console.WriteLine("Units on order: " + item.UnitsOnOrder);
                                Console.WriteLine("Reorder level: " + item.ReorderLevel);
                                Console.WriteLine("Discontinued: " + item.Discontinued);
                            }
                        }

                    }
                    else if (choice == "9")
                    {
                        var db = new NWConsole_48_AMSContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Product p in item.Products)
                            {
                                if (p.Discontinued == false)
                                {
                                    Console.WriteLine($"\t{p.ProductName}");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"\t{p.ProductName}");
                                    Console.ForegroundColor = ConsoleColor.White;

                                }
                            }
                        }
                    }
                    else if (choice == "10")
                    {
                        var db = new NWConsole_48_AMSContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Product p in category.Products)
                        {
                            if (p.Discontinued == false)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"\t{p.ProductName}");
                                Console.ForegroundColor = ConsoleColor.White;

                            }
                        }
                    }
                    else if (choice == "11")
                    {
                        //edit record from category table 
                        var db = new NWConsole_48_AMSContext();
                        Console.WriteLine("Enter category ID you'd like to edit from list below: ");
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId} - {item.CategoryName}");
                        }

                        int editedCategoryId = Convert.ToInt32(Console.ReadLine());
                        Category changedCategory = db.Categories.FirstOrDefault(p => p.CategoryId == editedCategoryId);
                        Console.Write("Enter category name: ");
                        changedCategory.CategoryName = Console.ReadLine();
                        db.EditCategory(changedCategory);
                    }
                    Console.WriteLine();
                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
    }
}