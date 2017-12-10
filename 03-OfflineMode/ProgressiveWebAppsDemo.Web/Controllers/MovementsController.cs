using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgressiveWebAppsDemo.Web.Data;
using ProgressiveWebAppsDemo.Web.Models;

namespace ProgressiveWebAppsDemo.Web.Controllers
{
    public class MovementsController : Controller
    {
        private readonly ExpensesContext mExpensesContext;

        private const int PageSize = 10;

        public MovementsController(ExpensesContext expensesContext)
        {
            this.mExpensesContext = expensesContext;
        }

        [HttpPost]
        public IActionResult Create(MovementForm form)
        {
            var category = this.mExpensesContext.Categories.Find(form.CategoryId);
            var amount = form.Amount * (form.Type == "expense" ? -1 : 1);
            byte[] picture;
            using (var ms = new MemoryStream())
            {
                form.Picture?.CopyTo(ms);
                picture = ms.ToArray();
            }

            var movement = new Movement(amount, form.Reason, category, picture, form.CreatedOn);
            this.mExpensesContext.Movements.Add(movement);
            this.mExpensesContext.SaveChanges();

            var balance = this.mExpensesContext.Movements.Sum(m => m.Amount);
            return this.Json(new { movement = movement, balance = balance });
        }
    }
}