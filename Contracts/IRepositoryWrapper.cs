using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        ITestRepository Test { get; }
        IProfileRepository Profile { get; }
        ILastMinuteDealsRepository LastMinuteDeals { get; }
        IContactRequestRepository ContactRequests { get; }
        IDynamicPageRepository DynamicPages { get; }
        IUploadCategoryRepository UploadCategories { get; }
        IImageRepositoryRepository ImageRepository { get; }
        IBlogRepository BlogRepository { get; }
        IBannerRepository BannerRepository { get; }
        IBookingRepository BookingRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        IPaymentTransactionRepository PaymentTransactionRepository { get; }
        ISubscriptionRepository Subscription { get; }
        IIncompleteBookingRepository IncompleteBooking { get; }

        void Save();
    }
}
