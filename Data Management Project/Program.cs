using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;
using System.Text.Json;
using System.IO;

namespace Data_Management_Project
{
    class Program
    {   //// GLOBAL VARIABLES //// 
        // FILTER / PRICE / ITEM
        public static string[,] food = {{"Fruit", "1", "Apple"}, {"Fruit", "1", "Banana"}, {"Dairy", "3", "Butter"},
                                        {"Meat", "4", "Beef"}, {"Vegetable", "1", "Carrot"}, {"Dairy", "2", "Cheese"},
                                        {"Meat", "3", "Chicken"}, {"Dairy", "3", "Cream"}, {"Vegetable", "2", "Cucumber"},
                                        {"Dairy", "4", "Ice Cream"}, {"Vegetable", "3", "Lettuce"}, {"Dairy", "4", "Milk"},
                                        {"Vegetable", "2", "Onion"}, {"Fruit", "1", "Orange"}, {"Vegetable", "2", "Potato"},
                                        {"Fruit", "3", "Peach"}, {"Fruit", "3", "Pineapple"}, {"Meat", "3", "Salmon"},
                                        {"Meat", "4", "Shrimp"}, {"Meat", "4", "Turkey"}, {"Dairy", "2", "Yogurt"}};
        // SHOPPING LIST
        public static List<string> shoppingList = new List<string>();
        public static int shoppingListCost;
        // SAVE USERNAME AND PASSWORD
        public static string usernameLoggedIn, passwordLoggedIn;

        static void Main(string[] args)
        {
            bool loginMenuRun = true;
            while(loginMenuRun)
            {
                Console.Clear();

                // DRAW LOGIN MENU
                Console.WriteLine("1| Login");
                Console.WriteLine("2| Register");
                Console.WriteLine("3| Exit");
                Console.Write("Input| ");

                // MENU FUNCTIONALITY
                string loginMenuInput = Convert.ToString(Console.ReadLine());
                Console.WriteLine("");
                switch (loginMenuInput)
                {
                    case "1": // LOGIN TO EXISTING ACCOUNT
                        // GET OLD INFORMATION
                        Console.Clear();
                        Console.Write("Input username| ");
                        string username = Convert.ToString(Console.ReadLine());
                        Console.Write("Input password| ");
                        string password = Convert.ToString(Console.ReadLine());

                        // TEST ACCOUNTS
                        bool loggedIn = Files.testForAccount(username, password);

                        // LOGIN
                        if (loggedIn)
                        {
                            usernameLoggedIn = username;
                            passwordLoggedIn = password;
                            dataManagementDisplay();
                        }


                        break;
                    case "2": // REGISTER NEW ACCOUNT
                        // GET NEW INFORMATION
                        Console.Clear();
                        Console.Write("Input new username| ");
                        string newUsername = Convert.ToString(Console.ReadLine());
                        Console.Write("Input new password| ");
                        string newPassword = Convert.ToString(Console.ReadLine());

                        // TEST TO ADD TO FILE
                        string newUsernameAndPassword = $"{newUsername}\n{newPassword}";
                        Files.addNewAccount(newUsernameAndPassword, newUsername, newPassword);

                        break;
                    case "3": // EXIT PROGRAM
                        // SET RUN TO FALSE
                        loginMenuRun = false;

                        break;
                    default: // N/A
                        // DISPLAY WARNING
                        Warning.popUp("Please input a correct value.\nPress any key to continue.\n");

                        break;
                }
            }
        }
        public static void dataManagementDisplay()
        {
            Console.Clear();
            Files.addToShoppinglist();

            bool mainMenuRun = true;
            while (mainMenuRun)
            {
                // DRAW MAIN MENU
                Console.WriteLine("1| Display all data.");
                Console.WriteLine("2| Display some data.");
                Console.WriteLine("3| Add to favourites/ shopping cart.");
                Console.WriteLine("4| Remove from favourites/ shopping cart.");
                Console.WriteLine("5| Display favourites/ shopping cart.");
                Console.WriteLine("6| Logout.");
                Console.Write("Input| ");

                // MENU FUNCTIONALITY
                string mainMenuInput = Convert.ToString(Console.ReadLine());
                Console.WriteLine("");
                switch (mainMenuInput)
                {
                    case "1": // DISPLAY ALL
                        for (int i = 0; i < food.GetLength(0); i++)
                            foodDataAndDisplay(i);
                        Console.WriteLine("");
                        break;
                    case "2": // DISPLAY SOME
                        // DRAW FILTER SEARCH MENU
                        Console.WriteLine("Filters are cost (1, 7, 25) or type (Meat, Vegetable, Fruit, and Dairy).");
                        Console.Write("Input| ");

                        // FILTER SEARCH FUNCTIONALITY
                        string filterInput = Convert.ToString(Console.ReadLine()).ToLower();
                        Console.WriteLine("");
                        if (filterInput == "fruit" || filterInput == "vegetable" || filterInput == "meat" || filterInput == "dairy")
                        {
                            for (int i = 0; i < food.GetLength(0); i++)
                                if (food[i, 0].ToLower() == filterInput)
                                    foodDataAndDisplay(i);
                            Console.WriteLine("");
                        } else if (int.TryParse(filterInput, out _))
                        {// THANKS TO https://stackoverflow.com/questions/894263/identify-if-a-string-is-a-number TO TEST IF STRING IS A NUMBER
                            for (int i = 0; i < food.GetLength(0); i++)
                            {
                                if (Convert.ToInt32(food[i, 1]) == Convert.ToInt32(filterInput))
                                    foodDataAndDisplay(i);
                            }
                            Console.WriteLine("");
                        } else
                            Warning.popUp("There are no items that fit that filter, Please try again.\n");

                        break;
                    case "3": // ADD FAV/SHOP
                        // VARIABLES
                        bool addShopRun = true;
                        List<string> addToShoppingList = new List<string>(21);


                        // MAIN LOOP
                        while (addShopRun)
                        {
                            Console.Clear();

                            // DISPLAY TOTAL COST OF SHOPPING LIST
                            shoppingListCost = 0;
                            for (int i = 0; i < shoppingList.Count; i++)
                                for (int it = 0; it < food.GetLength(0); it++)
                                    if (shoppingList[i] == food[it, 2])
                                        shoppingListCost += Convert.ToInt32(food[it, 1]);

                            // WRITE SHOPPING LIST
                            Console.Write("|| SHOPPING LIST ||");
                            ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write($" ${shoppingListCost}");
                            ResetColor();
                            Console.WriteLine("");
                            for (int i = 0; i < shoppingList.Count; i++)
                                for (int it = 0; it < food.GetLength(0); it++)
                                    if (shoppingList[i] == food[it, 2])
                                        foodDataAndDisplay(it);

                            // WRITE ALL ITEMS
                            Console.WriteLine("\n|| ALL ITEMS ||");
                            int inputIncreament = 1;
                            for (int i = 0; i < food.GetLength(0); i++)
                            {
                                if (inputIncreament >= 10)
                                    Console.Write($"{inputIncreament} | ");
                                else
                                    Console.Write($"{inputIncreament}  | ");
                                foodDataAndDisplay(i);
                                addToShoppingList.Add(food[i, 2]);
                                inputIncreament++;
                            }
                            Console.WriteLine($"{inputIncreament} | Exit");
                            Console.Write("\nInput| ");

                            // MENU FUNCTIONALAITY       
                            string shopMenuInput = Convert.ToString(Console.ReadLine());                          
                            for (int i = 0; i < 21; i++)
                                if (int.TryParse(shopMenuInput, out _))
                                    if (Convert.ToInt32(shopMenuInput) - 1 == i)
                                        shoppingList.Add(addToShoppingList[i]);
                                    else if (Convert.ToInt32(shopMenuInput) == 22)
                                        addShopRun = false;

                            // SAVE SHOPPING LIST TO FILES
                            if (addShopRun == false)
                                Files.addToFile();
                        }
                        break;
                    case "4": // REMOVE FAV/SHOP
                        // VARIABLES
                        bool removeShopRun = true;

                        // MAIN LOOP
                        while (removeShopRun)
                        {
                            // WRITE SHOPPING LIST
                            Console.WriteLine($"|| SHOPPING LIST ||");
                            int inputIncreament = 1;
                            for (int i = 0; i < shoppingList.Count; i++)
                            {
                                for (int it = 0; it < food.GetLength(0); it++)
                                {
                                    if (shoppingList[i] == food[it, 2])
                                    {
                                        if (inputIncreament >= 10)
                                            Console.Write($"{inputIncreament} | ");
                                        else
                                            Console.Write($"{inputIncreament}  | ");
                                        foodDataAndDisplay(it);
                                        inputIncreament++;
                                    }
                                }
                            }
                            if (inputIncreament >= 10)
                                Console.WriteLine($"{inputIncreament} | Exit");
                            else
                                Console.WriteLine($"{inputIncreament}  | Exit");
                            Console.Write("\nInput| ");

                            // MENU FUNCTIONALITY
                            string shopMenuRemoveInput = Convert.ToString(Console.ReadLine());
                            if (int.TryParse(shopMenuRemoveInput, out _))
                                if (Convert.ToInt32(shopMenuRemoveInput) < inputIncreament)
                                    shoppingList.RemoveAt(Convert.ToInt32(shopMenuRemoveInput) - 1);
                                else
                                    removeShopRun = false;
                            Console.WriteLine("");

                            // SAVE SHOPPING LIST TO FILES
                            if (removeShopRun == false)
                                Files.addToFile();
                        }
                        break;
                    case "5": // DISPLAY FAV/SHOP
                        for (int i = 0; i < shoppingList.Count; i++)
                            for (int it = 0; it < food.GetLength(0); it++)
                                if (shoppingList[i] == food[it, 2])
                                    foodDataAndDisplay(it);
                        Console.WriteLine("");

                        break;
                    case "6": // GO BACK TO LOGIN
                        // SET LOOP TO FALSE
                        mainMenuRun = false;

                        break;
                    default: // N/A
                        // DISPLAY WARNING
                        Warning.popUp("Please input a correct value.\nPress any key to continue.\n");
                        Console.WriteLine("");

                        break;
                }
            }
        }
        static void foodDataAndDisplay(int i)
        {
            // DISPLAY ITEM AND TYPE
            if (Convert.ToString(food[i, 0]) == "Fruit")
                ForegroundColor = ConsoleColor.Green;
            else if (Convert.ToString(food[i, 0]) == "Meat")
                ForegroundColor = ConsoleColor.Red;
            else if (Convert.ToString(food[i, 0]) == "Vegetable")
                ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"{food[i, 2]} ");
            ResetColor();

            // DISPLAY COST
            ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"${food[i, 1]}");
            ResetColor();

            Console.WriteLine("");
        }
    }
    class Warning
    {
        public static void popUp(string warning)
        {
            ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(warning);
            ResetColor();
            ReadKey(true);
        }
    }
    class Files
    {
        public static bool testForAccount(string oldUsername, string oldPassword)
        {
            // THANKS TO https://stackoverflow.com/questions/13175868/how-to-get-full-file-path-from-file-name TO GET FILE PATH
            // GET THE MAIN LOOP AMOUNT
            string amountOfAccountsLocation = Path.GetFullPath("data.txt");
            int amountOfAccounts = 0;
            if (File.Exists(amountOfAccountsLocation) == true)
                amountOfAccounts = Convert.ToInt32(File.ReadAllText(amountOfAccountsLocation)); // UPDATE VARIABLE

            // MAIN LOOP
            for (int account = 0; account <= amountOfAccounts; account++)
            {
                string accountPath = Path.GetFullPath($"account{Convert.ToString(account)}.txt");
                if (File.Exists(accountPath))
                {
                    string[] oldAccount = File.ReadAllLines(accountPath);
                    if (oldAccount[0] == oldUsername && oldAccount[1] == oldPassword)
                        return true;
                } else
                {
                    Warning.popUp("\nThere is no account with this info\nPlease try again if you have registered an account.\n" +
                                  "Or create one if there are none,\nPress any key to continue.");
                    return false;

                }
            }
            return false;
        }
        public static void addNewAccount(string newUsernameAndPassword, string newUsername, string newPassword)
        {
            // GET THE MAIN LOOP AMOUNT
            string amountOfAccountsLocation = Path.GetFullPath("data.txt");
            int amountOfAccounts = 0;
            if (File.Exists(amountOfAccountsLocation) == true)
                amountOfAccounts = Convert.ToInt32(File.ReadAllText(amountOfAccountsLocation)); // UPDATE VARIABLE

            // MAIN LOOP
            for (int account = 0; account <= amountOfAccounts; account++)
            {
                string accountPath = Path.GetFullPath($"account{Convert.ToString(account)}.txt");
                if (File.Exists(accountPath) == false) // IF ACCOUNT DOES NOT EXIST
                {
                    File.WriteAllText(accountPath, newUsernameAndPassword);
                    int increase = amountOfAccounts += 1;
                    File.WriteAllText(amountOfAccountsLocation, Convert.ToString(increase));
                    ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nAccount created, login to access data management.");
                    ResetColor();
                    ReadKey(true);
                    break;
                }
                else // ACCOUNT DOES EXIST
                {
                    string[] oldAccount = File.ReadAllLines(accountPath);
                    if (oldAccount[0] == newUsername || oldAccount[1] == newPassword)
                    {
                        Warning.popUp("\nAn account with this information already exists.\nPlease try again with new information." +
                                      "\nPress any ket to continue.");
                        break;
                    }
                }
            }
        }
        public static void addToFile()
        {
            // GET THE MAIN LOOP AMOUNT
            string amountOfAccountsLocation = Path.GetFullPath("data.txt");
            int amountOfAccounts = 0;
            if (File.Exists(amountOfAccountsLocation) == true)
                amountOfAccounts = Convert.ToInt32(File.ReadAllText(amountOfAccountsLocation)); // UPDATE VARIABLE

            // MAIN LOOP
            for (int account = 0; account <= amountOfAccounts; account++)
            {
                string accountPath = Path.GetFullPath($"account{Convert.ToString(account)}.txt");
                if (File.Exists(accountPath))
                {
                    string[] oldAccount = File.ReadAllLines(accountPath);
                    if (oldAccount[0] == Program.usernameLoggedIn && oldAccount[1] == Program.passwordLoggedIn)
                    {
                        // ADD OLD INFO INTO ONE LIST
                        List<string> info = new List<string>();
                        info.Add(Program.usernameLoggedIn);
                        info.Add(Program.passwordLoggedIn);
                        info.AddRange(Program.shoppingList);

                        // THANKS TO https://stackoverflow.com/questions/15300572/saving-lists-to-txt-file TO UPDATE AND SAVE TO FILE
                        TextWriter updatedFile = new StreamWriter(accountPath);
                        foreach (string word in info)
                            updatedFile.WriteLine(word);
                        updatedFile.Close();
                    }
                }
            }
        }
        public static void addToShoppinglist()
        {
            // GET THE MAIN LOOP AMOUNT
            string amountOfAccountsLocation = Path.GetFullPath("data.txt");
            int amountOfAccounts = 0;
            if (File.Exists(amountOfAccountsLocation) == true)
                amountOfAccounts = Convert.ToInt32(File.ReadAllText(amountOfAccountsLocation)); // UPDATE VARIABLE

            // MAIN LOOP
            for (int account = 0; account <= amountOfAccounts; account++)
            {
                string accountPath = Path.GetFullPath($"account{Convert.ToString(account)}.txt");
                if (File.Exists(accountPath))
                {
                    string[] oldAccount = File.ReadAllLines(accountPath);
                    if (oldAccount[0] == Program.usernameLoggedIn && oldAccount[1] == Program.passwordLoggedIn)
                    {
                        Program.shoppingList.Clear();

                        // UPDATE SHOPPING LIST 
                        for (int i = 2; i < oldAccount.Length; i++)
                            Program.shoppingList.Add(oldAccount[i]);
                    }
                }
            }
        }
    }
}
