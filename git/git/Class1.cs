using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace git
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int RoomNumber { get; set; }
        public int GuestId { get; set; }
        public DateTime CheckDate { get; set; }
        public DateTime DepartureDate { get; set; }

        public Booking(int bookingId, int roomId, int guestId, DateTime CheckDate, DateTime DepartureDate)
        {
            BookingId = bookingId;
            RoomNumber = roomId;
            GuestId = guestId;
            this.CheckDate = CheckDate;
            this.DepartureDate = DepartureDate;
        }

        public Booking()
        {

        }

    }
    public class Room
    {
        public int RoomNumber { get; set; }
        public int SquareMeters { get; set; }
        public int NumberOfBeds { get; set; }
        public int PricePerNight { get; set; }
        public bool IsBooked { get; set; }

        public Room(int roomNumber, int squareMeters, int numberOfBeds, int pricePerNight)
        {
            RoomNumber = roomNumber;
            SquareMeters = squareMeters;
            NumberOfBeds = numberOfBeds;
            PricePerNight = pricePerNight;
            IsBooked = false;
        }
    }

    public class HotelManager
    {
        internal List<Booking> bookingsList = new List<Booking>();
        internal List<Room> allRoomsList = new List<Room>();
        internal List<Room> availableRoomsList = new List<Room>();

        private int bookingCounter = 0;

        public void BookRoom(int roomIndexNumber, int guestId, DateTime check, DateTime departure)
        {
            int roomId = 0;
            List<Room> availableRooms = AddToListOfAvailableRooms();
            for (int i = 0; i < availableRooms.Count; i++)
            {
                roomId = availableRoomsList[roomIndexNumber - 1].RoomNumber;
            }
            Booking newBooking = new Booking(bookingCounter + 1, roomId, guestId, check, departure);
            bookingsList.Add(newBooking);
            SetRoomBooked(roomId);
        }

        public List<Room> AddToListOfAvailableRooms()
        {
            availableRoomsList.Clear();

            foreach (Room room in allRoomsList)
            {
                if (room.IsBooked == false)
                {
                    availableRoomsList.Add(room);
                }
            }
            return availableRoomsList;
        }

        public List<Room> AddToListOfAvailableRooms(int numberOfBeds)
        {
            availableRoomsList.Clear();

            foreach (Room room in allRoomsList)
            {
                if (room.IsBooked == false && room.NumberOfBeds == numberOfBeds)
                {
                    availableRoomsList.Add(room);
                }
            }
            return availableRoomsList;
        }

        private void SetRoomBooked(int roomNumber)
        {
            foreach (Room room in allRoomsList)
            {
                if (roomNumber == room.RoomNumber)
                {
                    room.IsBooked = true;
                }
            }
        }

        private void SetRoomAvailable(int roomNumber)
        {
            foreach (Room room in allRoomsList)
            {
                if (roomNumber == room.RoomNumber)
                {
                    room.IsBooked = false;
                }
            }
        }

        public Booking GetGuestBooking(int bookingId)
        {
            Booking guestBooking = new Booking();

            foreach (Booking booking in bookingsList)
            {
                if (booking.BookingId == bookingId)
                {
                    guestBooking = booking;
                }
            }
            return guestBooking;
        }


        public void AddNewRoom(int roomNumber, int squareMeters, int numberOfBeds, int pricePerNight)
        {
            Room newRoom = new Room(roomNumber, squareMeters, numberOfBeds, pricePerNight);

            allRoomsList.Add(newRoom);
        }

        public void AddNewRoom(string inp)
        {
            try
            {
                StreamReader sr = new StreamReader(inp);
                while (true)
                {
                    string a = sr.ReadLine();

                    if (a == null)
                        break;

                    string[] a_s = a.Split(' ');

                    Room newRoom = new Room(Convert.ToInt32(a_s[0]), Convert.ToInt32(a_s[1]), Convert.ToInt32(a_s[2]), Convert.ToInt32(a_s[3]));


                    allRoomsList.Add(newRoom);
                }
                sr.Close();
            }

            catch
            {
                Console.WriteLine("Исходного файла комнат не существует");
            }
        }

        public string ViewAllRooms()
        {
            string roomDescriptions = "";

            foreach (Room room in allRoomsList)
            {
                roomDescriptions += room.RoomNumber + "\n" +
                "Количество спальных мест: " + room.NumberOfBeds + "\n" +
                "Площадь: " + room.SquareMeters + "\n" +
                "Цена за ночь: " + room.PricePerNight + "\n\n";
            }
            return roomDescriptions;
        }


        public Booking ViewBookedRoom(int guestId)
        {
            Booking newBooking = new Booking();

            foreach (Booking booking in bookingsList)
            {
                if (booking.GuestId == guestId)
                {
                    newBooking = new Booking(booking.BookingId, booking.RoomNumber, booking.GuestId, booking.CheckDate, booking.DepartureDate);
                    return newBooking;
                }
            }
            return newBooking;
        }

        public bool IsBooked(int roomNumber)
        {
            bool booked = false;

            foreach (Room room in allRoomsList)
            {
                if (room.RoomNumber == roomNumber)
                {
                    if (room.IsBooked == true)
                    {
                        booked = true;
                    }
                }
            }
            return booked;
        }

        public bool CheckIfRoomExists(int tryNumber)
        {
            bool roomExists = false;

            foreach (Room room in allRoomsList)
            {
                if (room.RoomNumber == tryNumber)
                {
                    roomExists = true;
                }
            }
            return roomExists;
        }

    }

    class Staff : User
    {
        public int StaffId { get; set; }

        public Staff(string firstName, string lastName, string userName, string password, int staffId)
        : base(firstName, lastName, userName, password)
        {
            this.StaffId = staffId;
        }
    }

    public class Guest : User
    {
        public int GuestId { get; set; }
        public string Email { get; set; }
        public long PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public int PostalCode { get; set; }
        public string City { get; set; }
        public long CreditCardNumber { get; set; }

        public Guest(string firstName, string lastName, string userName, string password, int guestId, string email, long phonenumber, string streetaddress, int postalcode, string city, long creditCardNumber)
        : base(firstName, lastName, userName, password)
        {
            this.GuestId = guestId;
            this.Email = email;
            this.PhoneNumber = phonenumber;
            this.StreetAddress = streetaddress;
            this.PostalCode = postalcode;
            this.City = city;
            this.CreditCardNumber = creditCardNumber;
        }
    }

    public abstract class User
    {
        internal string FirstName { get; set; }
        internal string LastName { get; set; }
        internal string UserName { get; set; }
        internal string Password { get; set; }

        public User(string firstName, string lastName, string userName, string password)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.UserName = userName;
            this.Password = password;
        }
    }


    public class UserAuthentication
    {
        int staffIdCount = 0;
        int guestIdCount = 0;
        Dictionary<int, Guest> dictionaryOfGuest = new Dictionary<int, Guest>();
        Dictionary<int, Staff> dictionaryOfStaff = new Dictionary<int, Staff>();

        public Guest AddGuestUser(string firstName, string lastName, string userName, string password, string email, long phoneNumber, string streetAddress, int postalCode, string city, long creditCardNumber)
        {
            guestIdCount++;

            Guest guestUser = new Guest(firstName, lastName, userName, password, guestIdCount, email, phoneNumber, streetAddress, postalCode, city, creditCardNumber);
            Guest guestUser2 = new Guest(firstName, lastName, userName, password, guestIdCount, email, phoneNumber, streetAddress, postalCode, city, creditCardNumber);
            dictionaryOfGuest.Add(guestIdCount, guestUser);
            return guestUser2;
        }

        public void AddGuestUser(string inp)
        {
            try
            {
                StreamReader sr = new StreamReader(inp);
                while (true)
                {
                    string a = sr.ReadLine();

                    if (a == null)
                        break;

                    string[] a_s = a.Split(' ');

                    guestIdCount++;

                    Guest guestUser = new Guest(a_s[0], a_s[1], a_s[2], a_s[3], guestIdCount, a_s[4], Convert.ToInt64(a_s[5]), a_s[6], Convert.ToInt32(a_s[7]), a_s[8], Convert.ToInt64(a_s[9]));
                    dictionaryOfGuest.Add(guestIdCount, guestUser);
                }
                sr.Close();
            }

            catch
            {
                Console.WriteLine("Исходного файла гостей не существует");
            }
        }

        public string AddStaffUser(string userName, string password, string firstName, string lastName)
        {
            staffIdCount++;

            Staff newStaffUser = new Staff(firstName, lastName, userName, password, staffIdCount);
            dictionaryOfStaff.Add(staffIdCount, newStaffUser);
            return $"Добавление этого сотрудника выполнено! \n\nЛогин: {userName} \nПароль: {password} \nId сотрудника: {staffIdCount}";
        }

        public void AddStaffUser(string inp)
        {
            try
            {
                StreamReader sr = new StreamReader(inp);
                while (true)
                {
                    string a = sr.ReadLine();

                    if (a == null)
                        break;

                    string[] a_s = a.Split(' ');

                    staffIdCount++;

                    Staff newStaffUser = new Staff(a_s[0], a_s[1], a_s[2], a_s[3], staffIdCount);
                    dictionaryOfStaff.Add(staffIdCount, newStaffUser);
                }
                sr.Close();
            }

            catch
            {
                Console.WriteLine("Исходного файла сотрудников не существует\n");
            }
        }

        public bool CheckIfUsernameExist(string userName)
        {
            foreach (KeyValuePair<int, Staff> staff in dictionaryOfStaff)
            {
                if (staff.Value.UserName == userName)

                    return true;
            }
            foreach (KeyValuePair<int, Guest> guest in dictionaryOfGuest)
            {
                if (guest.Value.UserName == userName)

                    return true;
            }
            return false;
        }

        public bool TryValidateGuestUser(string userName, string password)
        {
            foreach (KeyValuePair<int, Guest> guest in dictionaryOfGuest)
            {
                if (guest.Value.UserName == userName && guest.Value.Password == password)

                    return true;
            }
            return false;
        }

        public bool TryValidateStaffUser(string userName, string password)
        {
            foreach (KeyValuePair<int, Staff> staff in dictionaryOfStaff)
            {
                if (staff.Value.UserName == userName && staff.Value.Password == password)

                    return true;
            }
            return false;
        }
    }
}