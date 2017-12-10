using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProgressiveWebAppsDemo.Web.Data;

namespace ProgressiveWebAppsDemo.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ExpensesContext mExpensesContext;

        public IndexModel(ExpensesContext expensesContext)
        {
            this.mExpensesContext = expensesContext;
        }

        private const int PageSize = 10;

        public Movement[] Movements { get; private set; }

        public decimal Balance { get; private set; }

        public SelectListItem[] Categories { get; private set; }

        public void OnGet()
        {
            this.Categories = this.mExpensesContext.Categories.OrderBy(c => c.Name).Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToArray();
            this.Movements = this.mExpensesContext.Movements.Include(m => m.Category).OrderByDescending(m => m.CreatedOn).Take(PageSize).ToArray();
            this.Balance = this.mExpensesContext.Movements.Sum(m => m.Amount);
        }
    }
}
