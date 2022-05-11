using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace git
{
    class Program
    {
        private static void Main(string[] args)
        {
            bool isGuestUserValid = false;
            bool isStaffUserValid = false;
            int loginTry = 1;
            string firstName = "";
            string lastName = "";
            int guestId = 0;
            string S_File = "Staff.txt", U_File = "Users.txt", R_File = "Rooms.txt";

            ConsoleKeyInfo guestOrStaff;
            HotelManager hotelManager = new HotelManager();
            UserAuthentication userAuthentication = new UserAuthentication();

            hotelManager.AddNewRoom(R_File);
            userAuthentication.AddGuestUser(U_File);
            userAuthentication.AddStaffUser(S_File);


            while (true)
            {
                Console.WriteLine("Добро пожаловать в отель \"Заря\"");
                Console.WriteLine("Сделайте выбор");
                Console.WriteLine("[1] - Гость");
                Console.WriteLine("[2] - Персонал");
                Console.WriteLine("Нажмите [Esc] для выхода");
                Console.Write("Выбор: ");

                guestOrStaff = Console.ReadKey();

                switch (guestOrStaff.Key)
                {
                    case ConsoleKey.D1: //Вход гостя
                        ChoiceForGuest(hotelManager);
                        break;

                    case ConsoleKey.D2: //Вход персонала
                        Console.WriteLine("\nВход персонала");
                        TryLogin();
                        ChoiceForStaff(hotelManager, userAuthentication);
                        break;

                    case ConsoleKey.Escape:
                        ExitProgram();
                        break;

                    default:
                        Console.WriteLine("Неверный ввод, вы можете ввести [1], [2] или [Esc]");
                        Console.ReadKey();
                        break;
                }
            }

            //Основные методы

            //Выбор для гостя
            void ChoiceForGuest(HotelManager hotelManager)
            {
                ConsoleKeyInfo bookingMenuChoice;

                Console.WriteLine("Добро пожаловать!");

                bool getNumberOfBedsLoop = true;
                do
                {
                    int numberOfBeds = GetNumberOfBeds();

                    Console.WriteLine("Доступные для вас комнаты\n");

                    do
                    {
                        string rooms = PrintAvailableRooms(numberOfBeds);
                        Console.WriteLine(rooms);

                        Console.WriteLine("[1] - Забронировать комнату\n[2] - Изменить количество кроватей \n[Esc] - Вернуться в главное меню");
                        Console.Write("Выбор: ");
                        bookingMenuChoice = Console.ReadKey();

                        switch (bookingMenuChoice.Key)
                        {
                            case ConsoleKey.D1:
                                Console.WriteLine("\n- Забронировать комнату -\n");

                                int chosenRoomNumber = GetRoomChoice(hotelManager.AddToListOfAvailableRooms(numberOfBeds));
                                DateTime checkDate = GetCheckDate();
                                DateTime departureDate = GetDepartureDate();
                                Console.WriteLine("\nВойдите или создайте новый аккаунт для продолжения\n");

                                Console.WriteLine("[1] - Вход \n[2] - Создать новый аккаунт");
                                Console.Write("Выбор: ");
                                ConsoleKeyInfo inputKey = Console.ReadKey();

                                switch (inputKey.Key)
                                {
                                    case ConsoleKey.D1:

                                        TryLogin();
                                        break;

                                    case ConsoleKey.D2:

                                        Console.WriteLine("Новый пользователь");
                                        Console.WriteLine("Пожалуйста, заполните данные ниже\n");
                                        SetFirstName();
                                        SetLastName();
                                        CreateAccount(firstName, lastName, guestOrStaff);
                                        break;

                                    default:
                                        Console.WriteLine("Неверный ввод. Нажмите [1] или [2]");
                                        break;
                                }

                                hotelManager.BookRoom(chosenRoomNumber, guestId, checkDate, departureDate);

                                Random r = new Random();
                                if (r.Next(1, 100) == 99)
                                {
                                    Console.WriteLine("Поздравляем, у вас будет скидка в размере 3%");
                                    Console.WriteLine($"\nС вас спишется сумма в размере: {(departureDate - checkDate).Days * hotelManager.allRoomsList[chosenRoomNumber].PricePerNight - (3 * (departureDate - checkDate).Days * hotelManager.allRoomsList[chosenRoomNumber].PricePerNight / 100)}");
                                }
                                else
                                {
                                    Console.WriteLine($"\nС вас спишется сумма в размере: {(departureDate - checkDate).Days * hotelManager.allRoomsList[chosenRoomNumber].PricePerNight}");
                                }
                                Console.WriteLine("\nВаше бронирование подтверждено!\n");
                                Console.Write("Нажмите любую кнопку для выхода");
                                Console.ReadKey();
                                ExitProgram();
                                break;


                            case ConsoleKey.D2:
                                //Разрываем цикл и возвращаемся к выбору кроватей
                                break;

                            case ConsoleKey.Escape:
                                getNumberOfBedsLoop = false;
                                break;

                            default:
                                Console.WriteLine("\nНеверный ввод. Вы можете выбрать [1], [2] или [Esc]");
                                Console.Write("Нажмите любую кнопку для продолжения");
                                Console.ReadKey();
                                break;
                        }

                        break;

                    } while (true);

                } while (getNumberOfBedsLoop);
            }

            //Для персонала
            void ChoiceForStaff(HotelManager hotelManager, UserAuthentication userAuthentication)
            {
                ConsoleKeyInfo input;
                do
                {
                    Console.Clear();
                    Console.WriteLine("ПЕРСОНАЛ\n");
                    Console.WriteLine("Сделайте выбор ниже");
                    Console.WriteLine("[1] - Проверить гостей\n[2] - Посмотреть все комнаты\n[3] - Посмотреть все доступные комнаты\n[4] - Добавить новый аккаунт персонала\n[5] - Добавить новую комнату \n[6] - Выход из программы");
                    Console.Write("Выбор: ");
                    input = Console.ReadKey();

                    switch (input.Key)
                    {
                        case ConsoleKey.D1:

                            StaffCheckOut();
                            break;

                        case ConsoleKey.D2:
                            // Посмотреть все номера
                            Console.Clear();
                            Console.WriteLine(hotelManager.ViewAllRooms());
                            Console.Write("Нажмите любую кнопку для возвращения");
                            Console.ReadKey();
                            break;

                        case ConsoleKey.D3:
                            //Посмотреть все доступные номера
                            Console.Clear();
                            string allAvailableRooms = PrintAllAvailableRooms();
                            Console.WriteLine(allAvailableRooms);
                            Console.Write("Нажмите любую кнопку для возвращения");
                            Console.ReadKey();
                            break;

                        case ConsoleKey.D4:
                            //Добавить персонал
                            Console.Clear();
                            Console.WriteLine("ДОБАВИТЬ НОВОГО СОТРУДНИКА\n");
                            SetFirstName();
                            SetLastName();
                            CreateAccount(firstName, lastName, guestOrStaff);
                            break;

                        case ConsoleKey.D5:
                            //Добавить новый номер
                            Console.Clear();
                            SetRoomDetails();
                            break;

                        case ConsoleKey.D6:
                            //Выход
                            ExitProgram();
                            break;

                        case ConsoleKey.Escape:
                            //Вернуться в главное меню
                            break;

                        default:
                            Console.WriteLine("Неверный ввод. Вы можете выбрать между 1-6");
                            Console.Write("Нажмите любую кнопку для продолжения");
                            Console.ReadKey();
                            break;
                    }
                } while (input.Key != ConsoleKey.Escape);
            }

            //Авторизация
            void TryLogin()
            {
                string userName;
                string password;

                do
                {
                    Console.Write("Логин: ");
                    userName = Console.ReadLine();
                    Console.Write("Пароль: ");
                    password = Console.ReadLine();

                    switch (guestOrStaff.Key)
                    {
                        case ConsoleKey.D2:
                            isStaffUserValid = userAuthentication.TryValidateStaffUser(userName, password);
                            break;
                        case ConsoleKey.D1:
                            isGuestUserValid = userAuthentication.TryValidateGuestUser(userName, password);
                            break;
                    }

                    if (isGuestUserValid == true || isStaffUserValid == true)
                    {
                        Console.Write("Вход успешен");
                        Thread.Sleep(1500);
                        loginTry = 0;
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Неверное имя пользователя или пароль. Сделайте еще одну попытку.\nПопытка {loginTry++} из 3");
                        if (loginTry == 4)
                        {
                            Console.WriteLine("Количество попыток превышено");
                            Console.ReadKey();
                            ExitProgram();
                        }
                    }
                }
                while (true);
            }

            //Создание нового аккаунта

            void CreateAccount(string firstName, string lastName, ConsoleKeyInfo guestOrStaff)
            {
                bool isUserExisting = false;
                string password;
                string userName;

                do
                {
                    userName = GetUserName();
                    password = GetPassword();

                    if (guestOrStaff.Key == ConsoleKey.D2)
                    {
                        string newStaffUser = userAuthentication.AddStaffUser(userName, password, firstName, lastName);

                        StreamWriter sw = new StreamWriter(S_File, true);
                        sw.WriteLine($"{firstName} {lastName} {userName} {password}");
                        sw.Close();

                        Console.WriteLine(newStaffUser);
                        Console.WriteLine("ПЕРСОНАЛ ДОБАВЛЕН");
                        Console.Write("Нажмите любую кнопку для продолжения");
                        Console.ReadKey();
                    }
                    else
                    {
                        GetGuestDetails(firstName, lastName, userName, password);
                    }

                    break;

                } while (isUserExisting == true);
            }

            void GetGuestDetails(string firstName, string lastName, string userName, string password)
            {
                string email = SetEmail();
                long phoneNumber = SetPhoneNumber();
                string streetAddress = SetStreetAddress();
                int postalCode = SetPostalCode();
                string city = SetCity();
                long creditCardNumber = SetCreditcardNumber();

                var output = userAuthentication.AddGuestUser(firstName, lastName, userName, password, email, phoneNumber, streetAddress, postalCode, city, creditCardNumber);

                StreamWriter sw = new StreamWriter(U_File, true);
                sw.WriteLine($"{firstName} {lastName} {userName} {password} {email} {phoneNumber} {streetAddress} {postalCode} {city} {creditCardNumber}");
                sw.Close();

                Console.WriteLine("Новый аккаунт создан!\n");
                Console.WriteLine($"Ваш id гостя: {output.GuestId}. \nПожалуйста, сохраните свой гостевой id для дальнейшего использования в системе бронирования.");
            }

            //ВЫВОД ДОСТУПНЫХ НОМЕРОВ

            string PrintAvailableRooms(int numberOfBeds)
            {
                string printSpecificAvailableRooms = "";
                int index = 1;
                List<Room> availableRooms = hotelManager.AddToListOfAvailableRooms(numberOfBeds);

                for (int i = 0; i < availableRooms.Count; i++)
                {
                    printSpecificAvailableRooms += "[" + index + "] Количество спальных мест: " + availableRooms[i].NumberOfBeds + "\n" +
                        "Площадь: " + availableRooms[i].SquareMeters + "\n" +
                        "Цена за ночь: " + availableRooms[i].PricePerNight + "\n\n";
                    index++;
                }

                return printSpecificAvailableRooms;
            }

            string PrintAllAvailableRooms()
            {
                string printAvailableRooms = "";
                List<Room> availableRooms = hotelManager.AddToListOfAvailableRooms();

                foreach (Room room in availableRooms)
                {
                    printAvailableRooms += room.RoomNumber + "\n" +
                        "Количество спальных мест: " + room.NumberOfBeds + "\n" +
                        "Площадь: " + room.SquareMeters + "\n" +
                        "Цена за ночь: " + room.PricePerNight + "\n\n";
                }

                return printAvailableRooms;
            }

            void SetFirstName()
            {
                do
                {
                    Console.Write("Имя: ");
                    firstName = Console.ReadLine();
                    if (string.IsNullOrEmpty(firstName))
                    {
                        Console.WriteLine("Вы должны указать имя");
                        Console.Write("Нажмите любую клавишу, чтобы сделать еще одну попытку");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }

            void SetLastName()
            {
                do
                {
                    Console.Write("Фамилия: ");
                    lastName = Console.ReadLine();
                    if (string.IsNullOrEmpty(lastName))
                    {
                        Console.WriteLine("Вы должны указать фамилию");
                        Console.Write("Нажмите любую клавишу, чтобы сделать еще одну попытку");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }

            string GetUserName()
            {
                string userName;
                bool isUserExisting;
                do
                {
                    Console.Write("Введите логин от 6 до 16 символов. \nЛогин: ");
                    userName = Console.ReadLine();
                    isUserExisting = userAuthentication.CheckIfUsernameExist(userName);

                    if (userName.Length < 6 || userName.Length > 16)
                    {
                        Console.WriteLine("Логин должен содержать от 6 до 16 символов.");
                        Console.Write("Нажмите любую клавишу, чтобы сделать еще одну попытку");
                        Console.ReadKey();
                    }
                    else if (isUserExisting == true)
                    {
                        Console.WriteLine("Логин уже существует");
                        Console.Write("Нажмите любую клавишу, чтобы сделать еще одну попытку");
                        Console.ReadKey();
                    }

                } while (userName.Length < 6 || userName.Length > 16 || isUserExisting == true);

                return userName;
            }

            string GetPassword()
            {
                string password;
                do
                {
                    Console.Write("Введите пароль от 6 до 16 символов \nПароль: ");
                    password = Console.ReadLine();

                    if (password.Length < 6 || password.Length > 16)
                    {
                        Console.WriteLine("Пароль должен содержать от 6 до 16 символов");
                        Console.Write("Нажмите любую клавишу, чтобы сделать еще одну попытку");
                        Console.ReadKey();
                    }

                } while (password.Length < 6 || password.Length > 16);

                return password;
            }

            void ExitProgram()
            {
                Console.WriteLine("\nВыход из программы");
                Environment.Exit(0);
            }

            int GetNumberOfBeds()
            {
                int numberOfBeds = 0;

                do
                {
                    Console.Clear();
                    Console.Write("У нас есть комнаты на 1-6 человек. \nСколько человек? : ");

                    try
                    {
                        numberOfBeds = Convert.ToInt16(Console.ReadLine());
                        if (numberOfBeds > 0 && numberOfBeds < 7)
                        {
                            break;
                        }
                        else if (numberOfBeds > 6 || numberOfBeds <= 0)
                        {
                            Console.WriteLine("Вы можете выбрать только число от 1 до 6.");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Вы можете выбрать только число от 1 до 6.");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }

                } while (true);

                return numberOfBeds;
            }

            int GetRoomChoice(List<Room> availableRooms)
            {
                int roomChoice = 0;

                do
                {
                    Console.Write("Какую комнату вы хотите забронировать : ");
                    try
                    {
                        roomChoice = Convert.ToInt32(Console.ReadLine());

                        {
                            if (roomChoice < 0 || roomChoice > availableRooms.Count)
                            {
                                Console.WriteLine("Вы можете выбрать только тот номер комнаты, который существует в списке доступных комнат.\n");
                                Console.Write("Нажмите любую клавишу чтобы продолжить");
                                Console.ReadKey();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Вы можете выбрать только тот номер комнаты, который существует в списке доступных комнат.\n");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }

                } while (true);

                return roomChoice;


            }

            DateTime GetCheckDate()
            {
                int d, m, y;
                Console.WriteLine("Введите день въезда: ");
                d = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Введите месяц въезда: ");
                m = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Введите год въезда: ");
                y = Convert.ToInt32(Console.ReadLine());

                return new DateTime(y, m, d);
            }

            DateTime GetDepartureDate()
            {
                int d, m, y;
                Console.WriteLine("Введите день выезда: ");
                d = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Введите месяц выезда: ");
                m = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Введите год выезда: ");
                y = Convert.ToInt32(Console.ReadLine());

                return new DateTime(y, m, d);
            }

            void StaffCheckOut()
            {
                int roomNumberToCheckOut = 0;
                bool isBooked = false;

                do
                {
                    Console.Clear();
                    Console.Write("Какую комнату вы хотите проверить : ");
                    do
                    {
                        try
                        {
                            roomNumberToCheckOut = Convert.ToInt32(Console.ReadLine());
                            break;
                        }
                        catch
                        {
                            System.Console.WriteLine("Неправильный ввод");
                            Console.WriteLine("Пожалуйста, сделайте еще одну попытку.\n");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }

                    } while (true);

                    isBooked = hotelManager.IsBooked(roomNumberToCheckOut);
                    if (isBooked == true)
                    {
                        Console.WriteLine("\nПроверка успешна!");
                        Console.WriteLine($"Комната {roomNumberToCheckOut} забронирована!");
                        Console.Write("Нажмите любую клавишу для продолжения проверки");
                        Console.ReadKey();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Введенный номер комнаты не забронирован!");
                        Console.WriteLine("Пожалуйста, сделайте еще одну попытку \n");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }

                } while (true);
            }

            void SetRoomDetails()
            {
                Console.WriteLine("Добавить новый номер в отель\n");

                int roomNumber = SetRoomNumber();
                int squareMeters = SetSquareMeters();
                int numberOfBeds = SetNumberOfBeds();
                int pricePerNight = SetPricePerNight();

                hotelManager.AddNewRoom(roomNumber, squareMeters, numberOfBeds, pricePerNight);

                StreamWriter sw = new StreamWriter(R_File, true);
                sw.WriteLine($"{roomNumber} {squareMeters} {numberOfBeds} {pricePerNight}");
                sw.Close();

                Console.Write("\nКомната успешно добавлена\nНажмите любую клавишу чтобы продолжить");
                Console.ReadKey();
            }

            int SetRoomNumber()
            {
                bool roomExists = false;
                int roomNumber = 0;
                do
                {
                    do
                    {
                        Console.Clear();
                        Console.Write("Номер комнаты: ");
                        try
                        {
                            roomNumber = Convert.ToInt32(Console.ReadLine());
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("Вы можете вводить только числа");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }
                    } while (true);

                    roomExists = hotelManager.CheckIfRoomExists(roomNumber);

                    if (roomExists == true)
                    {
                        Console.WriteLine("Номер комнаты, который вы выбрали, уже существует.\nПожалуйста, выберите другой.\n");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }
                    else if (roomNumber <= 0)
                    {
                        Console.WriteLine("Вы должны выбрать число больше 0.\n");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }

                } while (roomNumber <= 0 || roomExists == true);

                return roomNumber;
            }

            int SetSquareMeters()
            {
                int squareMeters = 0;

                do
                {
                    bool sqmLoop = true;
                    do
                    {
                        Console.Write("Площадь: ");
                        try
                        {
                            squareMeters = Convert.ToInt32(Console.ReadLine());
                            sqmLoop = false;
                        }
                        catch
                        {
                            Console.WriteLine("Вы можете вводить только числа");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }
                    } while (sqmLoop);

                    if (squareMeters < 10 || squareMeters > 100)
                    {
                        Console.WriteLine("Вы можете вводить только числа от 10 до 100.\n");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }

                } while (squareMeters < 10 || squareMeters > 100);

                return squareMeters;
            }

            int SetNumberOfBeds()
            {
                int numberOfBeds = 0;
                do
                {
                    bool numberOfBedsLoop = true;
                    do
                    {
                        Console.Write("Количество спальных мест: ");
                        try
                        {
                            numberOfBeds = Convert.ToInt32(Console.ReadLine());
                            numberOfBedsLoop = false;
                        }
                        catch
                        {
                            Console.WriteLine("Вы можете вводить только числа");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }
                    } while (numberOfBedsLoop);

                    if (numberOfBeds <= 0)
                    {
                        Console.WriteLine("Вы должны выбрать число больше 0");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }
                    else if (numberOfBeds > 6)
                    {
                        Console.WriteLine("Вы должны выбрать число меньше 7");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }

                } while (numberOfBeds < 1 || numberOfBeds > 6);

                return numberOfBeds;
            }

            int SetPricePerNight()
            {
                int pricePerNight = 0;
                do
                {
                    bool setPricePerNightLoop = true;
                    do
                    {
                        Console.Write("Цена за ночь: ");
                        try
                        {
                            pricePerNight = Convert.ToInt32(Console.ReadLine());
                            setPricePerNightLoop = false;
                        }
                        catch
                        {
                            Console.WriteLine("Вы можете вводить только числа");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }

                    } while (setPricePerNightLoop);

                    if (pricePerNight < 1)
                    {
                        Console.WriteLine("Вы должны установить цену выше 0");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }

                } while (pricePerNight < 1);

                return pricePerNight;
            }

            string SetEmail()
            {
                string email = "";

                do
                {
                    Console.Write("Эл. Почта: ");
                    email = Console.ReadLine();

                    if (string.IsNullOrEmpty(email))
                    {
                        Console.WriteLine("Вы должны вставить что-нибудь в качестве своего адреса электронной почты!");
                    }
                    else
                    {
                        break;
                    }

                } while (true);

                return email;
            }

            string SetStreetAddress()
            {
                string streetAddress = "";

                do
                {
                    Console.Write("Адрес улицы: ");
                    streetAddress = Console.ReadLine();

                    if (string.IsNullOrEmpty(streetAddress))
                    {
                        Console.WriteLine("Вы должны вставить что-нибудь в качестве вашего адреса!");
                    }
                    else
                    {
                        break;
                    }

                } while (true);

                return streetAddress;
            }

            string SetCity()
            {
                string city = "";

                do
                {
                    Console.Write("Город: ");
                    city = Console.ReadLine();

                    if (string.IsNullOrEmpty(city))
                    {
                        Console.WriteLine("Вы должны вставить что-нибудь в качестве своего города!");
                    }
                    else
                    {
                        break;
                    }

                } while (true);

                return city;
            }

            long SetPhoneNumber()
            {
                long phoneNumber = 0;
                do
                {
                    bool setPhoneNumberLoop = true;
                    do
                    {
                        Console.Write("Номер телефона: ");
                        try
                        {
                            phoneNumber = Convert.ToInt64(Console.ReadLine());
                            setPhoneNumberLoop = false;
                        }
                        catch
                        {
                            Console.WriteLine("Вы можете вводить только числа");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }

                    } while (setPhoneNumberLoop);

                    if (phoneNumber < 79999999999 || phoneNumber > 89999999999)
                    {
                        Console.WriteLine("Вы должны ввести действующий номер телефона");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }

                } while (phoneNumber < 79999999999 || phoneNumber > 89999999999);

                return phoneNumber;
            }

            int SetPostalCode()
            {
                int postalCode = 0;
                do
                {
                    bool setPostalCodeLoop = true;
                    do
                    {
                        Console.Write("Индекс: ");
                        try
                        {
                            postalCode = Convert.ToInt32(Console.ReadLine());
                            setPostalCodeLoop = false;
                        }
                        catch
                        {
                            Console.WriteLine("Вы можете вводить только числа");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }

                    } while (setPostalCodeLoop);

                    if (postalCode < 99999 || postalCode > 999999)
                    {
                        Console.WriteLine("Вы должны ввести 6-значный номер");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }

                } while (postalCode < 99999 || postalCode > 999999);

                return postalCode;
            }

            long SetCreditcardNumber()
            {
                long creditcardNumber = 0;
                do
                {
                    bool setcreditCardNumber = true;
                    do
                    {
                        Console.Write("Номер кредитной карты: ");
                        try
                        {
                            creditcardNumber = Convert.ToInt64(Console.ReadLine());
                            setcreditCardNumber = false;
                        }
                        catch
                        {
                            Console.WriteLine("Вы можете вводить только числа");
                            Console.Write("Нажмите любую клавишу чтобы продолжить");
                            Console.ReadKey();
                        }

                    } while (setcreditCardNumber);

                    if (creditcardNumber < 999999999999999 || creditcardNumber > 9999999999999999)
                    {
                        Console.WriteLine("Вы должны ввести действующий номер кредитной карты");
                        Console.Write("Нажмите любую клавишу чтобы продолжить");
                        Console.ReadKey();
                    }
                    else
                    {
                        break;
                    }

                } while (creditcardNumber < 999999999999999 || creditcardNumber > 9999999999999999);

                return creditcardNumber;
            }


        }
    }
}