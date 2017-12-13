using System;
using ProgressiveWebAppsDemo.Web.Data;
using ProgressiveWebAppsDemo.Web.Pages;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ProgressiveWebAppsDemo.Tests.Pages
{
    public class IndexModelTests
    {
        [Fact]
        public void OnGet_WhenCalled_ThenLoadsCategoriesInAlphaOrder()
        {
            var options = new DbContextOptionsBuilder<ExpensesContext>()
                .UseInMemoryDatabase(databaseName: "OnGet_WhenCalled_ThenLoadsCategoriesInAlphaOrder")
                .Options;
            using (var expensesContext = new ExpensesContext(options))
            {
                expensesContext.Categories.Add(new Category("Foo"));
                expensesContext.Categories.Add(new Category("Bar"));
                expensesContext.Categories.Add(new Category("Baz"));
                expensesContext.SaveChanges();
                var model = new IndexModel(expensesContext);

                model.OnGet();

                Assert.NotNull(model.Categories);
                Assert.NotEmpty(model.Categories);
                Assert.Equal(3, model.Categories.Length);
                Assert.Equal("Bar", model.Categories[0].Text);
                Assert.Equal("Baz", model.Categories[1].Text);
                Assert.Equal("Foo", model.Categories[2].Text);
            }
        }
        
        [Fact]
        public void OnGet_WhenThereAreNoMovementsAvailable_ThenMovementsAreAnEmptyArrayAndBalanceIsZero()
        {
            var options = new DbContextOptionsBuilder<ExpensesContext>()
                .UseInMemoryDatabase(databaseName: "OnGet_WhenThereAreNoMovementsAvailable_ThenMovementsAreAnEmptyArrayAndBalanceIsZero")
                .Options;
            using (var expensesContext = new ExpensesContext(options))
            {
                var model = new IndexModel(expensesContext);

                model.OnGet();

                Assert.NotNull(model.Movements);
                Assert.Empty(model.Movements);
                Assert.Equal(0, model.Balance);
            }
        }
        
        [Fact]
        public void OnGet_WhenThereAreThreeMovementsAvailable_ThenMovementsAreDescendingByDateAndBalanceIsTheSum()
        {
            var options = new DbContextOptionsBuilder<ExpensesContext>()
                .UseInMemoryDatabase(databaseName: "OnGet_WhenThereAreThreeMovementsAvailable_ThenMovementsAreDescendingByDateAndBalanceIsTheSum")
                .Options;
            using (var expensesContext = new ExpensesContext(options))
            {
                var category = new Category("Default");
                expensesContext.Categories.Add(category);
                expensesContext.Movements.Add(new Movement(-10, "second", category, null, new DateTime(2017, 11, 10)));
                expensesContext.Movements.Add(new Movement(-10, "third", category, null, new DateTime(2017, 11, 11)));
                expensesContext.Movements.Add(new Movement(50, "first", category, null, new DateTime(2017, 11, 9)));
                expensesContext.SaveChanges();
                var model = new IndexModel(expensesContext);

                model.OnGet();

                Assert.NotNull(model.Movements);
                Assert.NotEmpty(model.Movements);
                Assert.Equal(3, model.Movements.Length);
                Assert.Equal("third", model.Movements[0].Reason);
                Assert.Equal("second", model.Movements[1].Reason);
                Assert.Equal("first", model.Movements[2].Reason);
                Assert.True(model.Movements.All(m => m.Category == category));
                Assert.Equal(30, model.Balance);
            }
        }
    }
}
