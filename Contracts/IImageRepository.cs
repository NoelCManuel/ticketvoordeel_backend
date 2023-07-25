using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IImageRepositoryRepository
    {
        IEnumerable<ImageRepository> GetAllImageRepository();
        IEnumerable<ImageRepository> GetImageRepository(Expression<Func<ImageRepository, bool>> predicate);
        ImageRepository GetImageRepositoryById(int imageRepositoryId);
        ImageRepository CreateImageRepository(ImageRepository imageRepository);
        bool UpdateImageRepository(ImageRepository imageRepository);
        bool DeleteImageRepository(ImageRepository imageRepository);
    }
}