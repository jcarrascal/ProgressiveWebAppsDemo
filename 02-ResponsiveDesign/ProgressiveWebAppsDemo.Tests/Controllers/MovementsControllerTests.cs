using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgressiveWebAppsDemo.Web.Controllers;
using ProgressiveWebAppsDemo.Web.Data;
using ProgressiveWebAppsDemo.Web.Models;
using Xunit;

namespace ProgressiveWebAppsDemo.Tests.Controllers
{
    public class MovementsControllerTests
    {
        [Fact]
        public void Create_WhenGivenAnExpense_ThenTheBalanceIsReducedAndANewMovementIsCreated()
        {
            var options = new DbContextOptionsBuilder<ExpensesContext>()
                .UseInMemoryDatabase(databaseName: "Create_WhenGivenAnExpense_ThenTheBalanceIsReduced")
                .Options;
            using (var expensesContext = new ExpensesContext(options))
            {
                expensesContext.Categories.Add(new Category("Foo"));
                var controller = new MovementsController(expensesContext);

                var form = new MovementForm
                {
                    Amount = 100,
                    Type = "expense",
                    CategoryId = 1
                };
                var result = controller.Create(form);

                Assert.IsAssignableFrom<JsonResult>(result);
                dynamic json = ((JsonResult)result).Value;
                //Assert.Equal(-100M, (decimal)json.balance);
                //Assert.Equal(-100M, (decimal)json.movement.amount);
                Assert.Equal(1, expensesContext.Movements.ToArray().Length);
                Assert.Equal(-100M, expensesContext.Movements.Sum(m => m.Amount));
            }
        }

        [Fact]
        public void Create_WhenGivenADeposit_ThenTheBalanceIsIncreasedAndANewMovementIsCreated()
        {
            var options = new DbContextOptionsBuilder<ExpensesContext>()
                .UseInMemoryDatabase(databaseName: "Create_WhenGivenADeposit_ThenTheBalanceIsIncreasedAndANewMovementIsCreated")
                .Options;
            using (var expensesContext = new ExpensesContext(options))
            {
                expensesContext.Categories.Add(new Category("Foo"));
                var controller = new MovementsController(expensesContext);

                var form = new MovementForm
                {
                    Amount = 100,
                    Type = "deposit",
                    CategoryId = 1
                };
                var result = controller.Create(form);

                Assert.IsAssignableFrom<JsonResult>(result);
                dynamic json = ((JsonResult)result).Value;
                //Assert.Equal(100M, (decimal)json.balance);
                //Assert.Equal(100M, (decimal)json.movement.amount);
                Assert.Equal(1, expensesContext.Movements.ToArray().Length);
                Assert.Equal(100M, expensesContext.Movements.Sum(m => m.Amount));
            }
        }
    }
}