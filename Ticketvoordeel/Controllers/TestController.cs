using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketvoordeel.Helpers;

namespace Ticketvoordeel.Controllers
{
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class TestController : Controller
    {
        private IRepositoryWrapper _repository;
        private IMapper _mapper;

        public TestController(IRepositoryWrapper repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [Route("/getall")]
        public JsonResult getall()
        {
            try
            {
                return Json(_repository.Test.GetAllTests());
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json("error");
            }
        }

        //[Route("/create")]
        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult create([FromBody]Test test)
        //{
        //    return Json(_testService.SaveTest(test));
        //}

        //[Route("/update")]
        //public JsonResult update(Test test)
        //{
        //    return Json(_testService.SaveTest(test));
        //}

        //[Route("/delete/{id}")]
        //public JsonResult delete(int id)
        //{
        //    Test testToDelete = new Test();
        //    testToDelete = _testService.GetTestById(id).Result;
        //    _testService.DeleteTest(testToDelete);
        //    return Json("success");
        //}

        //[Route("/get/{id}")]
        //public JsonResult get(int id)
        //{
        //    return Json(_testService.GetTestById(id).Result);
        //}

        //[Route("/getcustom")]
        //public JsonResult getcustom()
        //{
        //    return Json(_testService.GetCustom().Result);
        //}

        //[Route("/get")]
        //public JsonResult getpredicate()
        //{
        //    return Json(_testService.Get(c => c.Id > 1).Result);
        //}
    }
}