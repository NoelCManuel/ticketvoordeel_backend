using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<LastMinuteDeal> LastMinuteDeals { get; set; }
        public DbSet<ContactRequest> ContactRequests { get; set; }
        public DbSet<DynamicPage> DynamicPages { get; set; }
        public DbSet<UploadCategory> UploadCategories { get; set; }
        public DbSet<ImageRepository> ImageRepository { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<PaymentHistory> Payments { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Subscription> Subscription { get; set; }
        public DbSet<IncompleteBooking> IncompleteBooking { get; set; }

    }
}
