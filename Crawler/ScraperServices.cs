using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Crawler
{
    public class ScraperServices
    {
        private readonly IWebDriver _driver;
        private readonly string FHS_NEW_URL = "https://www.fahasa.com/sach-trong-nuoc.html?order=created_at&limit=12&p=1";
        private readonly string FHS_BESTSELLER_URL = "https://www.fahasa.com/sach-trong-nuoc.html?order=num_orders&limit=12&p=1";
        private readonly string BOOKBUY_NEW_URL = "https://bookbuy.vn/sach-ban-chay/p1?order=IsNew";
        private readonly string BOOKBUY_BESTSELLER_URL = "https://bookbuy.vn/sach-ban-chay/p1?order=IsMostBuy";

        public ScraperServices()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            _driver = new ChromeDriver(options);
        }

        public async Task<List<BookCrawlDTO>> ScrapeFahasa(bool crawlNew = true)
        {
            string crUrl = crawlNew ? FHS_NEW_URL : FHS_BESTSELLER_URL;
            string type = crawlNew ? "Mới" : "Bán chạy";

            var newBooks = new List<BookCrawlDTO>();
            _driver.Navigate().GoToUrl(crUrl);
            var bookElements = _driver.FindElements(By.CssSelector(".item-inner"));
            foreach (var element in bookElements)
            {
                var title = element.FindElement(By.CssSelector(".product-name-no-ellipsis a"));
                var price = element.FindElement(By.CssSelector(".special-price .price"));
                var url = title.GetAttribute("href");
                BookCrawlDTO book = new BookCrawlDTO()
                {
                    BookName = title.Text,
                    Price = price.Text,
                    BookUrl = url,
                    DateCrawl = DateTime.Now,
                    Website = "Fahasa",
                    BookType = type
                };

                newBooks.Add(book);
                if (newBooks.Count == 10)
                    break;

            }

            return newBooks;
        }
        public async Task<List<BookCrawlDTO>> ScrapeBookbuy(bool crawlNew = true)
        {
            string crUrl = crawlNew ? BOOKBUY_NEW_URL : BOOKBUY_BESTSELLER_URL;
            string type = crawlNew ? "Mới" : "Bán chạy";
            var newBooks = new List<BookCrawlDTO>();
            _driver.Navigate().GoToUrl(crUrl);
            var bookElements = _driver.FindElements(By.CssSelector(".product-item"));
            foreach (var element in bookElements)
            {
                var title = element.FindElement(By.CssSelector(".t-view a"));
                var price = element.FindElement(By.CssSelector(".p-view .real-price"));
                var url = title.GetAttribute("href");

                BookCrawlDTO book = new BookCrawlDTO()
                {
                    BookName = title.Text,
                    Price = price.Text,
                    BookUrl = url,
                    DateCrawl = DateTime.Now,
                    Website = "Bookbuy",
                    BookType = type

                };

                newBooks.Add(book);
                if (newBooks.Count == 10)
                    break;

            }

            return newBooks;
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }

    }
}
