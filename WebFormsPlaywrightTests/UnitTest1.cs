using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WebFormsPlaywrightTests
{
    [TestFixture]
    public class ToDoAppE2ETests
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private IBrowserContext _context;

        private readonly string _appUrl = "http://localhost/MyApp/ToDo.aspx";

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 500 // slows down actions so you can visually follow
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = null
            });

            _page = await _context.NewPageAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [SetUp]
        public async Task TestSetup()
        {
            await _page.GotoAsync(_appUrl);
        }

        [Test, Order(1)]
        public async Task AddNewTask_ShouldAppearInList()
        {
            string taskText = "Playwright Add Task " + Guid.NewGuid();
            await _page.FillAsync("#txtNewItem", taskText);
            await _page.ClickAsync("#btnAddItem");

            var lastTask = _page.Locator("ul#list li").Last;
            string lastTaskText = await lastTask.Locator("span.task-text").InnerTextAsync();

            Assert.AreEqual(taskText, lastTaskText);
        }

        [Test, Order(2)]
        public async Task MarkTaskAsDone_ShouldUpdateClass()
        {
            string taskText = "Playwright Done Task " + Guid.NewGuid();
            await _page.FillAsync("#txtNewItem", taskText);
            await _page.ClickAsync("#btnAddItem");

            var lastTask = _page.Locator("ul#list li").Last;
            await lastTask.Locator(".donetab").ClickAsync();

            string classes = await lastTask.Locator("span.task-text").GetAttributeAsync("class");
            StringAssert.Contains("done", classes);
        }

        [Test, Order(3)]
        public async Task EditTask_ShouldUpdateText()
        {
            string initialText = "Task to edit " + Guid.NewGuid();
            string updatedText = "Updated Task " + Guid.NewGuid();

            await _page.FillAsync("#txtNewItem", initialText);
            await _page.ClickAsync("#btnAddItem");

            var lastTask = _page.Locator("ul#list li").Last;
            await lastTask.Locator("span.task-text").DblClickAsync();

            await _page.FillAsync("#txtNewItem", updatedText);
            await _page.ClickAsync("#btnAddItem");

            string text = await lastTask.Locator("span.task-text").InnerTextAsync();
            Assert.AreEqual(updatedText, text);
        }

        [Test, Order(4)]
        public async Task DeleteTask_ShouldRemoveFromList()
        {
            string taskText = "Task to delete " + Guid.NewGuid();
            await _page.FillAsync("#txtNewItem", taskText);
            await _page.ClickAsync("#btnAddItem");

            var lastTask = _page.Locator("ul#list li").Last;

            // Step 1: click cross (enter delete confirmation)
            await lastTask.Locator(".deletetab").ClickAsync();

            // Step 2: click tick (done tab) to confirm deletion
            var confirmDelete = lastTask.Locator(".donetab");
            await confirmDelete.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached,
                Timeout = 5000
            });

            await confirmDelete.ClickAsync();

            // Allow server-side postback to update Repeater
            await _page.WaitForTimeoutAsync(500);

            // Validate the task was removed
            var tasks = await _page.Locator("ul#list li").AllInnerTextsAsync();
            CollectionAssert.DoesNotContain(tasks, taskText);
        }




        [Test, Order(5)]
        public async Task ChangeTaskColor_ShouldUpdateBackground()
        {
            string taskText = "Color Task " + Guid.NewGuid();
            await _page.FillAsync("#txtNewItem", taskText);
            await _page.ClickAsync("#btnAddItem");

            var lastTask = _page.Locator("ul#list li").Last;
            var colorInput = lastTask.Locator(".colortab input[type=color]");
            string newColor = "#00ff00";

            await colorInput.EvaluateAsync("el => el.style.display='block'");
            await colorInput.FillAsync(newColor);
            await colorInput.PressAsync("Enter");

            string bgColor = await lastTask.Locator("span.task-text").EvaluateAsync<string>("el => getComputedStyle(el).backgroundColor");
            StringAssert.Contains("rgb(0, 255, 0)", bgColor);
        }

        
        [Test, Order(6)]
        public async Task AddEmptyTask_ShouldShowError()
        {
            await _page.FillAsync("#txtNewItem", "   ");
            await _page.ClickAsync("#btnAddItem");

            string errorText = await _page.Locator("#lblError").InnerTextAsync();
            Assert.AreEqual("Enter a task before adding.", errorText);
        }

        [Test, Order(7)]
        public async Task DragAndDropTile_ShouldUpdateOrder()
        {
            var tilesBefore = await _page.QuerySelectorAllAsync(".todo-tile");
            Assert.IsTrue(tilesBefore.Count > 1, "At least two tiles are required for drag and drop test.");

            var initialOrder = new List<string>();
            foreach (var tile in tilesBefore)
                initialOrder.Add((await tile.InnerTextAsync()).Trim());

            var tileToDrag = tilesBefore[0];
            var draggerHandle = await tileToDrag.QuerySelectorAsync(".draggertab");
            Assert.IsNotNull(draggerHandle, "Dragger handle not found in tile.");

            string draggedTileText = (await tileToDrag.InnerTextAsync()).Trim();
            var dragBox = await draggerHandle.BoundingBoxAsync();
            var lastTile = tilesBefore[tilesBefore.Count - 1];
            var lastBox = await lastTile.BoundingBoxAsync();

            Assert.IsNotNull(dragBox, "Bounding box for dragger handle not found.");
            Assert.IsNotNull(lastBox, "Bounding box for last tile not found.");
            await _page.Mouse.MoveAsync(dragBox.X + dragBox.Width / 2, dragBox.Y + dragBox.Height / 2);
            await _page.Mouse.DownAsync();
            await _page.Mouse.MoveAsync(lastBox.X + lastBox.Width / 2, lastBox.Y + lastBox.Height - 5, new MouseMoveOptions { Steps = 10 });
            await _page.Mouse.UpAsync();
            var tilesAfter = await _page.QuerySelectorAllAsync(".todo-tile");
            var updatedOrder = new List<string>();
            foreach (var tile in tilesAfter)
                updatedOrder.Add((await tile.InnerTextAsync()).Trim());

            int oldIndex = initialOrder.IndexOf(draggedTileText);
            int newIndex = updatedOrder.IndexOf(draggedTileText);

            Assert.AreNotEqual(oldIndex, newIndex, "Dragged tile order should have changed after drag-and-drop.");
        }




    }
}
