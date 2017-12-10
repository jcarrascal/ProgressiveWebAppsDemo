# Progressive Web Applications Demo
Now we're going to configure Entity Framework Core, add a few models and context, create the initial migration, seed the database and use it in the page.

Lest start by adding the connection string. For simplicity's sake I'll use SQL Server Local DB. In the ProgressiveWebAppsDemo.Web project open the appsettings.json file to add the ConnectionStrings element at the root object.

```json
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ProgressiveWebAppsDemo;Integrated Security=True"
  },
´´´

Will use this connection string on the Startup class. At the end of the ConfigureServices() method we'll add the following code wich makes the DbContext available to the rest of the application:

```csharp
            services.AddDbContext<ExpensesContext>(options =>
            {
                options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
            });
´´´

Now, we have to create the DbContext which is the ExpensesContext class we are referening int the ConfigureServices() method:

```csharp
using System;
using Microsoft.EntityFrameworkCore;

namespace ProgressiveWebAppsDemo.Web.Data
{
    public class ExpensesContext : DbContext
    {
    }
}
´´´

This class should have at least two things:
1. A constructor that receives a DbContextOptions<ExpensesContext> object that is passed to base(). This constructor receives the connection string we just created.
2. One or more properties of type DbSet<SomeClass> where SomeClass is the object to persist. In our case we'll create two properties for the Movement and Category classes.

These classes use a few property attributes to control how they are handled by EF Core and serialized to JSON like: Key, DatabaseGenerated, Required and JsonIgnore.

## Create and seed the database
To create the database we use EF Core Migrations. First have to execute the following commands on the powershell console:
```
dotnet ef migrations add InitialCreate
´´´

This will create a Migrations folder with the initial state of the database. Now, to create the database we execute the following command:
```
dotnet ef database update
´´´

This database is empty, but the application requires that at least one category is available. To seed the database we add a method to the context:
```
public void EnsureSeedData()
{
    this.Database.ExecuteSqlCommand(@"
        SET IDENTITY_INSERT Categories ON;
        MERGE Categories as Target USING (VALUES 
                (1, 'Default'),
                (2, 'Food'),
                (3, 'Entertainment'),
                (4, 'Health'),
                (5, 'Transport')) AS Source (Id, Name)
            ON Target.Id = Source.Id
            WHEN MATCHED THEN
                UPDATE SET Name = Source.Name
            WHEN NOT MATCHED BY Target THEN
                INSERT (Id, Name) VALUES (Source.Id, Source.Name);
        SET IDENTITY_INSERT Categories OFF
    ");
}
´´´

... and execute it from the Startup class at the end of the Configure() method:
```
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ExpensesContext>();
                context.Database.Migrate();
                context.EnsureSeedData();
            }
´´´
Each time we start the application, the migrations will run and the data will be seeded.

## Testing with EF Core
Now, let's start adding functionality by adding some tests. Remove the UnitTest1.cs class for the ProgressiveWebAppsDemo.Tests project and create a Pages folder. Inside that folder create a IndexModelTests.cs file.

The Index page should show all the categories in alphabetical order, so here's a test for that:
```csharp
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
´´´

Here, we are using the In-Memory database as a way to test that the correct result is being delivered. To implement this test, and the other required functionality, move onto the Index.cshtml.cs file and add the following method:

```csharp
        public void OnGet()
        {
            this.Categories = this.mExpensesContext.Categories.OrderBy(c => c.Name).Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToArray();
            this.Movements = this.mExpensesContext.Movements.Include(m => m.Category).OrderByDescending(m => m.CreatedOn).Take(PageSize).ToArray();
            this.Balance = this.mExpensesContext.Movements.Sum(m => m.Amount);
        }
´´´

Finally add the necessary HTML code to Index.cshtml to display the results.

```html
<form method="POST" enctype="multipart/form-data">
    <h1>$ @this.Model.Balance.ToString("N0")</h1>
    <div>
        <label for="movement-amount">Amount:</label>
        <input id="movement-amount" name="amount" type="number" value="0"/>
    </div>
    <div>
        <button type="submit" name="type" value="expense">
            Expense
        </button>
        <button type="submit" name="type" value="deposit">
            Deposit
        </button>
    </div>
    <div>
        <label for="movement-reason">Reason:</label>
        <input id="movement-reason" name="reason" type="text"/>
    </div>
    <div>
        <label for="movement-category">Category:</label>
        <select id="movement-category-id" name="CategoryId" asp-items="Model.Categories" id="movement-category"></select>
    </div>
    <div>
        <label for="movement-category">Picture:</label>
        <input id="movement-picture" name="Picture" type="file" accept="image/*;capture=camera"/>
    </div>
</form>
<table>
    <thead>
        <tr>
            <th>$</th>
            <th>Date</th>
            <th>Category</th>
            <th>Reason</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var movement in this.Model.Movements)
        {
            <tr>
                <td>@movement.Amount.ToString("N0")</td>
                <td>@movement.CreatedOn</td>
                <td>@movement?.Category?.Name</td>
                <td>@movement.Reason</td>
            </tr>
        }
    </tbody>
</table>
´´´
Now, we have to make an Ajax request to store the Expense or the Deposit as required by the user. On site.js we create a jQuery plugin that handles the expenses form submit event. Here's the relevant code:

```javascript
        self._formSubmit = function (e) {
            e.preventDefault();
            let formData = new FormData(this);
            let button = e.originalEvent.explicitOriginalTarget;
            formData.append(button.name, button.value);
            $.ajax({
                url: cfg.url,
                data: formData,
                type: "POST",
                processData: false,
                contentType: false,
                success: self._processResponse(data)
            });

        };
        this.on("submit", self._formSubmit);
´´´

# Persisting the movement
To persist the movement we'll create a MovementsController. Create a Controllers folder and a MovementsController.cs with the following Create() method:
```csharp
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
´´´