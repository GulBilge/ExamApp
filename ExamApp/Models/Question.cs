using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamApp.Models
{
    public class Question
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public string Text { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string RightAnswer { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ExamId { get; set; }
    }
}
