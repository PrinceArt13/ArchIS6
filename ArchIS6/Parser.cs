using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Reflection;
//using ArchIS6.Context;
using OpenQA.Selenium.DevTools.V117.Target;
using OpenQA.Selenium.DevTools.V117.Tracing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ArchIS6
{
    internal class Parser
    {
        static void Main(string[] args)
        {
            const string url = "https://www.citilink.ru/catalog/smartfony/";
            ParseURLs(url);
            var db = new PrintsevAis6Context();
            foreach (var t in db.Smartphones)
            {
                Console.WriteLine($"Название: {t.Name}\nКоличество ядер: {t.Cores}\nЦена: {t.Price}\n");
            }
        }

        static void ParseURLs(string url)
        {
            //открытие браузера
            var chromeDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;

            var options = new ChromeOptions();

            // Создание драйвера браузера.
            using IWebDriver driver = new ChromeDriver(chromeDriverService, options);

            // Установка таймаута ожидания для корректного парсинга догружаемых элементов.
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl(url);

            List<string> URLlist = driver.FindElements(By.CssSelector("div.app-catalog-1tp0ino a")).Select(elem => elem.GetAttribute("href") + "properties/").ToList();
            List<Smartphone> smartphones;
            smartphones = ParseEverySmartphone(driver, URLlist);

            using (var dbcon = new PrintsevAis6Context())
            {
                dbcon.AddRange(smartphones);
                dbcon.SaveChanges();
            }
        }

        static List<Smartphone> ParseEverySmartphone(IWebDriver driver, List<string> URLlist)
        {
            Dictionary<string, string> element = new();
            List<Smartphone> smartphones = new();
            string key = "";
            string value = "";

            foreach (string url in URLlist.Take(3))
            {
                Smartphone smartphone = new();
                driver.Navigate().GoToUrl(url);
                //подождать загрузку страницы
                driver.FindElement(By.CssSelector("div.app-catalog-3z12b7 h1"));
                smartphone.Id = Guid.NewGuid();
                smartphone.Name = driver.FindElement(By.CssSelector("h1.e1ubbx7u0.eml1k9j0.app-catalog-tn2wxd.e1gjr6xo0")).Text.Replace("Характеристики", "");
                smartphone.Price = driver.FindElement(By.CssSelector("span.e1j9birj0.e106ikdt0.app-catalog-1f8xctp.e1gjr6xo0")).Text;

                foreach (var elem in driver.FindElements(By.CssSelector("div.app-catalog-xc0ceg.e1ckvoeh5")))
                {
                    try
                    {
                        key = elem.FindElement(By.CssSelector("span.e1ckvoeh1.e106ikdt0.app-catalog-13gqfj6.e1gjr6xo0")).Text.ToLower();
                        value = elem.FindElement(By.CssSelector("span.e1ckvoeh0.e106ikdt0.app-catalog-1uhv1s4.e1gjr6xo0")).Text;
                        element.Add(key, value);
                    }
                    catch
                    {
                        continue;
                    }
                }

                foreach (var record in element)
                {
                    if (record.Key == "количество ядер")
                        smartphone.Cores = record.Value;
                }
                smartphones.Add(smartphone);

            }

            return smartphones;
        }


    }
}