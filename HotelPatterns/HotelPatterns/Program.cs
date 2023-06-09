using System;
using System.Collections.Specialized;

namespace HotelPatterns
{
    #region Фабричный метод
    // Абстрактный класс отеля
    abstract class Hotel
    {
        public abstract Room CreateRoom(string roomNumber);
    }

    // Конкретный класс отеля
    class LuxuryHotel : Hotel
    {
        public override Room CreateRoom(string roomNumber)
        {
            return new LuxuryRoom(roomNumber);
        }
    }

    // Конкретный класс отеля
    class BudgetHotel : Hotel
    {
        public override Room CreateRoom(string roomNumber)
        {
            return new BudgetRoom(roomNumber);
        }
    }

    // Абстрактный класс комнаты
    abstract class Room
    {
        protected Room(string roomNumber)
        {
            RoomNumber = roomNumber;
        }

        public string RoomNumber { get; internal set; }
    }

    // Конкретный класс комнаты
    class LuxuryRoom : Room
    {
        public LuxuryRoom(string roomNumber) : base(roomNumber)
        {
        }
    }

    // Конкретный класс комнаты
    class BudgetRoom : Room
    {
        public BudgetRoom(string roomNumber) : base(roomNumber)
        {
        }
    }
    #endregion

    #region Адаптер

    // Интерфейс системы управления отелями
    interface IHotelManagementSystem
    {
        void BookRoom(string roomNumber, DateTime checkInDate, DateTime checkOutDate);
        void CancelBooking(string roomNumber);
    }

    // Интерфейс внешней системы управления отелями
    interface IExternalHotelManagementSystem
    {
        void BookRoom(string roomNumber, DateTime checkInDate, DateTime checkOutDate);
        void CancelBooking(string roomNumber);
    }

    // Внешняя система управления отелями
    class ExternalHotelManagementSystem : IExternalHotelManagementSystem
    {
        public void BookRoom(string roomNumber, DateTime checkInDate, DateTime checkOutDate)
        {
            Console.WriteLine("Booking Room {0} from {1} to {2}", roomNumber, checkInDate, checkOutDate);
        }

        public void CancelBooking(string roomNumber)
        {
            Console.WriteLine("Canceling booking for Room {0}", roomNumber);
        }
    }

    // Адаптер для интеграции с внешней системой управления отелями
    class HotelManagementSystemAdapter : IHotelManagementSystem
    {
        private readonly IExternalHotelManagementSystem externalSystem;

        public HotelManagementSystemAdapter(IExternalHotelManagementSystem externalSystem)
        {
            this.externalSystem = externalSystem;
        }

        public void BookRoom(string roomNumber, DateTime checkInDate, DateTime checkOutDate)
        {
            externalSystem.BookRoom(roomNumber, checkInDate, checkOutDate);
        }

        public void CancelBooking(string roomNumber)
        {
            externalSystem.CancelBooking(roomNumber);
        }
    }
    #endregion

    #region Состояние

    class RoomBookingState
    {
        private Room room;
        private bool isBooked;
        private bool isUnderCleaning;
        private bool isUnderRepair;

        public RoomBookingState(Room room)
        {
            this.room = room;
            isBooked = false;
            isUnderCleaning = false;
            isUnderRepair = false;
        }

        public void BookRoom()
        {
            if (!isBooked && !isUnderCleaning && !isUnderRepair)
            {
                isBooked = true;
                Console.WriteLine("Room {0} has been booked.", room.RoomNumber);
            }
            else
            {
                Console.WriteLine("Room {0} cannot be booked.", room.RoomNumber);
            }
        }

        public void CancelBooking()
        {
            if (isBooked)
            {
                isBooked = false;
                Console.WriteLine("Booking for Room {0} has been canceled.", room.RoomNumber);
            }
            else
            {
                Console.WriteLine("There is no booking for Room {0}.", room.RoomNumber);
            }
        }

        public void SetCleaningState()
        {
            if (!isBooked && !isUnderCleaning && !isUnderRepair)
            {
                isUnderCleaning = true;
                Console.WriteLine("Room {0} is now under cleaning.", room.RoomNumber);
            }
            else
            {
                Console.WriteLine("Cannot set cleaning state for Room {0}.", room.RoomNumber);
            }
        }

        public void SetRepairState()
        {
            if (!isBooked && !isUnderCleaning && !isUnderRepair)
            {
                isUnderRepair = true;
                Console.WriteLine("Room {0} is now under repair.", room.RoomNumber);
            }
            else
            {
                Console.WriteLine("Cannot set repair state for Room {0}.", room.RoomNumber);
            }
        }

        public void ProcessNextState()
        {
            if (isUnderCleaning)
            {
                isUnderCleaning = false;
                Console.WriteLine("Cleaning of Room {0} is complete.", room.RoomNumber);
            }
            else if (isUnderRepair)
            {
                isUnderRepair = false;
                Console.WriteLine("Repair of Room {0} is complete.", room.RoomNumber);
            }
            else
            {
                Console.WriteLine("No further state to process for Room {0}.", room.RoomNumber);
            }
        }
    }

    // Конкретный класс комнаты с добавлением состояния и бронирования
    class RoomWithBooking : Room
    {
        private RoomBookingState bookingState;

        public RoomWithBooking(string roomNumber) : base(roomNumber)
        {
            bookingState = new RoomBookingState(this);
        }

        public void BookRoom()
        {
            bookingState.BookRoom();
        }

        public void CancelBooking()
        {
            bookingState.CancelBooking();
        }

        public void SetCleaningState()
        {
            bookingState.SetCleaningState();
        }

        public void SetRepairState()
        {
            bookingState.SetRepairState();
        }

        public void ProcessNextState()
        {
            bookingState.ProcessNextState();
        }
    }
    #endregion

    class HotelPatterns
    {
        static void Main(string[] args)
        {
            RoomWithBooking room1 = new RoomWithBooking("101");
            RoomWithBooking room2 = new RoomWithBooking("102");

            IExternalHotelManagementSystem externalHotelManagementSystem = new ExternalHotelManagementSystem();
            IHotelManagementSystem hotelManagementSystem = new HotelManagementSystemAdapter(externalHotelManagementSystem);
            hotelManagementSystem.BookRoom("101", DateTime.Now, DateTime.Now.AddDays(3));
            hotelManagementSystem.CancelBooking("101");

            room1.BookRoom();
            room2.SetCleaningState();
            room1.BookRoom();

            room1.ProcessNextState();
            room2.ProcessNextState();

            room1.BookRoom();
            room2.SetRepairState();

            room2.BookRoom();
            room2.ProcessNextState();

            room2.BookRoom();
            room2.CancelBooking();
            room2.CancelBooking();
        }
    }
}