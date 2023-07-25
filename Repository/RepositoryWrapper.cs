using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private ITestRepository _test;
        private IProfileRepository _profile;
        private ILastMinuteDealsRepository _lastMinuteDeal;
        private IContactRequestRepository _contactRequest;
        private IDynamicPageRepository _dynamicPage;
        private IUploadCategoryRepository _uploadCategory;
        private IImageRepositoryRepository _imageRepository;
        private IBlogRepository _blogRepository;
        private IBannerRepository _bannerRepository;
        private IBookingRepository _bookingRepository;
        private IPaymentRepository _paymentRepository;
        private IPaymentTransactionRepository _paymentTransactionRepository;
        private ISubscriptionRepository _subscriptionRepository;
        private IIncompleteBookingRepository _incompleteBookingRepository;

        public ITestRepository Test
        {
            get
            {
                if (_test == null)
                {
                    _test = new TestRepository(_repoContext);
                }
                return _test;
            }
        }

        public IProfileRepository Profile
        {
            get
            {
                if (_profile == null)
                {
                    _profile = new ProfileRepository(_repoContext);
                }
                return _profile;
            }
        }

        public ISubscriptionRepository Subscription
        {
            get
            {
                if (_subscriptionRepository == null)
                {
                    _subscriptionRepository = new SubscriptionRepository(_repoContext);
                }
                return _subscriptionRepository;
            }
        }

        public IIncompleteBookingRepository IncompleteBooking
        {
            get
            {
                if (_incompleteBookingRepository == null)
                {
                    _incompleteBookingRepository = new IncompleteBookingRepository(_repoContext);
                }
                return _incompleteBookingRepository;
            }
        }

        public ILastMinuteDealsRepository LastMinuteDeals
        {
            get
            {
                if (_lastMinuteDeal == null)
                {
                    _lastMinuteDeal = new LastMinuteDealRepository(_repoContext);
                }
                return _lastMinuteDeal;
            }
        }

        public IContactRequestRepository ContactRequests
        {
            get
            {
                if (_contactRequest == null)
                {
                    _contactRequest = new ContactRequestRepository(_repoContext);
                }
                return _contactRequest;
            }
        }

        public IDynamicPageRepository DynamicPages
        {
            get
            {
                if (_dynamicPage == null)
                {
                    _dynamicPage = new DynamicPageRepository(_repoContext);
                }
                return _dynamicPage;
            }
        }

        public IUploadCategoryRepository UploadCategories
        {
            get
            {
                if (_uploadCategory == null)
                {
                    _uploadCategory = new UploadCategoryRepository(_repoContext);
                }
                return _uploadCategory;
            }
        }

        public IImageRepositoryRepository ImageRepository
        {
            get
            {
                if (_imageRepository == null)
                {
                    _imageRepository = new ImageRepositoryRepository(_repoContext);
                }
                return _imageRepository;
            }
        }

        public IBlogRepository BlogRepository
        {
            get
            {
                if (_blogRepository == null)
                {
                    _blogRepository = new BlogRepository(_repoContext);
                }
                return _blogRepository;
            }
        }

        public IBannerRepository BannerRepository
        {
            get
            {
                if (_bannerRepository == null)
                {
                    _bannerRepository = new BannerRepository(_repoContext);
                }
                return _bannerRepository;
            }
        }

        public IBookingRepository BookingRepository
        {
            get
            {
                if (_bookingRepository == null)
                {
                    _bookingRepository = new BookingRepository(_repoContext);
                }
                return _bookingRepository;
            }
        }

        public IPaymentRepository PaymentRepository
        {
            get
            {
                if (_paymentRepository == null)
                {
                    _paymentRepository = new PaymentRepository(_repoContext);
                }
                return _paymentRepository;
            }
        }

        public IPaymentTransactionRepository PaymentTransactionRepository
        {
            get
            {
                if (_paymentTransactionRepository == null)
                {
                    _paymentTransactionRepository = new PaymentTransactionRepository(_repoContext);
                }
                return _paymentTransactionRepository;
            }
        }

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }

        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}
