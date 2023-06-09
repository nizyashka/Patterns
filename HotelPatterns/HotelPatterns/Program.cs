using System;
using System.Collections.Specialized;

namespace HotelPatterns
{
    #region Фабричный метод
    // Абстрактный класс отеля
    abstract class Hotel
    {
        public abstract Room CreateRoom();
    }

    // Конкретный класс отеля
    class LuxuryHotel : Hotel
    {
        public override Room CreateRoom()
        {
            return new LuxuryRoom();
        }
    }

    // Конкретный класс отеля
    class BudgetHotel : Hotel
    {
        public override Room CreateRoom()
        {
            return new BudgetRoom();
        }
    }

    // Абстрактный класс комнаты
    abstract class Room
    {
        public abstract void Display();
    }

    // Конкретный класс комнаты
    class LuxuryRoom : Room
    {
        public override void Display()
        {
            Console.WriteLine("Luxury Room");
        }
    }

    // Конкретный класс комнаты
    class BudgetRoom : Room
    {
        public override void Display()
        {
            Console.WriteLine("Budget Room");
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
    // Контекст (класс бронирования)
    class BookingContext
    {
        private BookingState currentState;

        public BookingContext()
        {
            TransitionTo(new DateSelectionState());
        }

        public void TransitionTo(BookingState state)
        {
            currentState = state;
            currentState.SetContext(this);
        }

        public void SelectDate(DateTime date)
        {
            currentState.SelectDate(date);
        }

        public void SelectRoom(string roomNumber)
        {
            currentState.SelectRoom(roomNumber);
        }

        public void ConfirmBooking()
        {
            currentState.ConfirmBooking();
        }
    }

    // Абстрактное состояние
    abstract class BookingState
    {
        protected BookingContext context;

        public void SetContext(BookingContext context)
        {
            this.context = context;
        }

        public abstract void SelectDate(DateTime date);
        public abstract void SelectRoom(string roomNumber);
        public abstract void ConfirmBooking();
    }

    // Конкретное состояние
    class DateSelectionState : BookingState
    {
        public override void SelectDate(DateTime date)
        {
            Console.WriteLine("Selected date: " + date);
            // Переход к следующему состоянию
            context.TransitionTo(new RoomSelectionState());
        }

        public override void SelectRoom(string roomNumber)
        {
            Console.WriteLine("Please select a date first.");
        }

        public override void ConfirmBooking()
        {
            Console.WriteLine("Please select a date and room first.");
        }
    }

    // Конкретное состояние
    class RoomSelectionState : BookingState
    {
        public override void SelectDate(DateTime date)
        {
            Console.WriteLine("Date already selected: " + date);
        }

        public override void SelectRoom(string roomNumber)
        {
            Console.WriteLine("Selected room: " + roomNumber);
            // Переход к следующему состоянию
            context.TransitionTo(new BookingConfirmationState());
        }

        public override void ConfirmBooking()
        {
            Console.WriteLine("Please select a room first.");
        }
    }

    // Конкретное состояние
    class BookingConfirmationState : BookingState
    {
        public override void SelectDate(DateTime date)
        {
            Console.WriteLine("Date already selected: " + date);
        }

        public override void SelectRoom(string roomNumber)
        {
            Console.WriteLine("Room already selected: " + roomNumber);
        }

        public override void ConfirmBooking()
        {
            Console.WriteLine("Booking confirmed.");
            // Другие действия после подтверждения бронирования
        }
    }

    class Room_State
    {
        private RoomState currentState;

        public Room_State()
        {
            TransitionTo(new NotRentedState());
        }

        public void TransitionTo(RoomState state)
        {
            currentState = state;
            currentState.SetRoom(this);
        }

        public void RentRoom()
        {
            currentState.RentRoom();
        }

        public void CheckOutRoom()
        {
            currentState.CheckOutRoom();
        }

        public void CleanRoom()
        {
            currentState.CleanRoom();
        }

        public void RepairRoom()
        {
            currentState.RepairRoom();
        }
    }

    // Абстрактное состояние комнаты
    abstract class RoomState
    {
        protected Room_State room;

        public void SetRoom(Room_State room)
        {
            this.room = room;
        }

        public abstract void RentRoom();
        public abstract void CheckOutRoom();
        public abstract void CleanRoom();
        public abstract void RepairRoom();
    }

    // Конкретное состояние: комната сдана
    class RentedState : RoomState
    {
        public override void RentRoom()
        {
            Console.WriteLine("Room is already rented.");
        }

        public override void CheckOutRoom()
        {
            Console.WriteLine("Checking out the room.");
            // Переход к состоянию "не сдана"
            room.TransitionTo(new NotRentedState());
        }

        public override void CleanRoom()
        {
            Console.WriteLine("Cannot clean a rented room.");
        }

        public override void RepairRoom()
        {
            Console.WriteLine("Cannot repair a rented room.");
        }
    }

    // Конкретное состояние: комната не сдана
    class NotRentedState : RoomState
    {
        public override void RentRoom()
        {
            Console.WriteLine("Renting the room.");
            // Переход к состоянию "сдана"
            room.TransitionTo(new RentedState());
        }

        public override void CheckOutRoom()
        {
            Console.WriteLine("Cannot check out a room that is not rented.");
        }

        public override void CleanRoom()
        {
            Console.WriteLine("Cleaning the room.");
            // Переход к состоянию "уборка"
            room.TransitionTo(new CleaningState());
        }

        public override void RepairRoom()
        {
            Console.WriteLine("Cannot repair a room that is not rented.");
        }
    }

    // Конкретное состояние: уборка комнаты
    class CleaningState : RoomState
    {
        public override void RentRoom()
        {
            Console.WriteLine("Cannot rent a room that is being cleaned.");
        }

        public override void CheckOutRoom()
        {
            Console.WriteLine("Cannot check out a room that is being cleaned.");
        }

        public override void CleanRoom()
        {
            Console.WriteLine("Room is already being cleaned.");
        }

        public override void RepairRoom()
        {
            Console.WriteLine("Cannot repair a room that is being cleaned.");
        }
    }

    // Конкретное состояние: комната в ремонте
    class RepairState : RoomState
    {
        public override void RentRoom()
        {
            Console.WriteLine("Cannot rent a room that is under repair.");
        }

        public override void CheckOutRoom()
        {
            Console.WriteLine("Cannot check out a room that is under repair.");
        }

        public override void CleanRoom()
        {
            Console.WriteLine("Cannot clean a room that is under repair.");
        }

        public override void RepairRoom()
        {
            Console.WriteLine("Room is already under repair.");
        }
    }
    #endregion

    class HotelPatterns
    {
        static void Main(string[] args)
        {
            Hotel luxuryHotel = new LuxuryHotel();
            Room luxuryRoom = luxuryHotel.CreateRoom();
            luxuryRoom.Display();

            Hotel budgetHotel = new BudgetHotel();
            Room budgetRoom = budgetHotel.CreateRoom();
            budgetRoom.Display();

            IExternalHotelManagementSystem externalHotelManagementSystem = new ExternalHotelManagementSystem();
            IHotelManagementSystem hotelManagementSystem = new HotelManagementSystemAdapter(externalHotelManagementSystem);
            hotelManagementSystem.BookRoom("101", DateTime.Now, DateTime.Now.AddDays(3));
            hotelManagementSystem.CancelBooking("101");

            BookingContext bookingContext = new BookingContext();

            bookingContext.SelectDate(DateTime.Now);
            bookingContext.SelectRoom("101");
            bookingContext.ConfirmBooking();

            Room_State room = new Room_State();
            room.RentRoom(); // Комната сдана
            room.CheckOutRoom(); // Комната не сдана
            room.CleanRoom(); // Комната в уборке
            room.RepairRoom(); // Комната в ремонте
            room.CleanRoom(); //Убрать комнату, которая уже убирается
        }
    }
}