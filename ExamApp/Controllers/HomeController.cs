using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExamApp.Models;
using ExamApp.Data;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Web;

namespace ExamApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExamDbContext _context;

        public HomeController(ILogger<HomeController> logger, ExamDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.ExamTitles = GetExamTitles().Select(x => new SelectListItem
            {
                Selected = x.Id == 1,
                Text = x.Title,
                Value = x.Href
            }).ToList();
            return View();
        }

        private List<ExamTitle> GetExamTitles()
        {
            var titleObjects = new List<ExamTitle>();
            if (!_context.ExamTitles.Any())
            {
                var url = "https://www.wired.com";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                var htmlNodes = doc.DocumentNode.SelectNodes("//a/h2").Take(5);

                foreach (var item in htmlNodes)
                {
                    var hrefNode = doc.DocumentNode.SelectNodes(item.XPath.Split("/h2").First()).First();
                    titleObjects.Add(new ExamTitle { Id = titleObjects.Count + 1, Title = HttpUtility.HtmlDecode( item.InnerText), Href = hrefNode.Attributes.First().Value });
                }

                _context.ExamTitles.AddRange(titleObjects);
                _context.SaveChanges();
            }
            else
            {
                titleObjects = _context.ExamTitles.ToList();
            }

            return titleObjects;
        }

        [HttpPost]
        public IActionResult GetExamText(string href)
        {
            var url = "https://www.wired.com/"+href;
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var text = doc.DocumentNode.SelectSingleNode("//article");
            return Json( text.InnerHtml);
        }

        [HttpPost]
        public IActionResult CreateExam(Exam exam)
        {
            exam.CreatedDate = DateTime.Now;
            _context.Exams.Add(exam);
            _context.SaveChanges();
            return RedirectToAction("ExamList");
        }
        [HttpPost]
        public IActionResult GetQuestions(int ExamId)
        {
            return PartialView("QuestionsPartialView",_context.Questions.Where(x=>x.ExamId==ExamId)
                .Select(x=>new QuestionsPartialViewModel {A=x.A,B=x.B,C=x.C,D=x.D,QuestionId=x.Id,Text=x.Text })
                .ToList());
        }

        [HttpPost]
        public IActionResult GetRightAnswers(int ExamId)
        {
            var answers = _context.Questions.Where(x => x.ExamId == ExamId)
                 .Select(x => x.RightAnswer);
            return Json(answers);
        }

        public IActionResult ExamList()
        {
            return View(_context.Exams);
        }
        public IActionResult Delete(int Id)
        {
          var exam =   _context.Exams.First(x => x.Id == Id);
            if (exam!=null)
            {
                _context.Exams.Remove(exam);
                _context.SaveChanges();
                return RedirectToAction("ExamList");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public IActionResult StartExam(int Id)
        {
            return View(_context.Exams.First(x => x.Id == Id));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
