using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository
{
    public class ImageRepositoryRepository : RepositoryBase<ImageRepository>, IImageRepositoryRepository
    {
        public ImageRepositoryRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<ImageRepository> GetAllImageRepository()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<ImageRepository> GetImageRepository(Expression<Func<ImageRepository, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public ImageRepository GetImageRepositoryById(int id)
        {
            return FindByCondition(c => c.Id.Equals(id))
                .FirstOrDefault();
        }

        public ImageRepository CreateImageRepository(ImageRepository imageRepository)
        {
            try
            {
                Create(imageRepository);
                return imageRepository;
            }
            catch (Exception ex)
            {
                return imageRepository;
            }
        }

        public bool UpdateImageRepository(ImageRepository imageRepository)
        {
            try
            {
                Update(imageRepository);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteImageRepository(ImageRepository imageRepository)
        {
            try
            {
                Delete(imageRepository);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
