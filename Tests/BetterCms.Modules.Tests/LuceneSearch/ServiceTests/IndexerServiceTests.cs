﻿using System;
using System.IO;
using System.Reflection;

using Autofac;

using BetterCMS.Module.LuceneSearch.Services.IndexerService;
using BetterCMS.Module.LuceneSearch.Services.WebCrawlerService;

using BetterCms.Core.DataAccess;
using BetterCms.Core.Security;
using BetterCms.Core.Services;
using BetterCms.Module.Search.Models;

using HtmlAgilityPack;

using NUnit.Framework;

namespace BetterCms.Test.Module.LuceneSearch.ServiceTests
{
    [TestFixture]
    public class IndexerServiceTests : TestBase
    {
        const string FullShortText = "Any section of UI that should update dynamically with Knockout can be handled more simply and in a maintainable fashion.";

        const string FullText = "Knockout is a fast, extensible and simple JavaScript library designed to work with HTML document elements "
                + "using a clean underlying view model. It helps to create rich and responsive user interfaces. "
                + "Any section of UI that should update dynamically (e.g., changing depending on the user’s actions "
                + "or when an external data source changes) with Knockout can be handled more simply and in a maintainable fashion.";

        private const string MiddleText = "... elements using a clean underlying view model. It helps to create rich and responsive user interfaces. "
            + "Any section of UI that should update dynamically (e.g., changing depending on the user’s actions or when...";

        private const string StartText = "Knockout is a fast, extensible and simple JavaScript library designed to work with HTML document "
            + "elements using a clean underlying view model. It helps to create rich and responsive user interfaces. Any section of UI that...";

        private const string EndText = "... of UI that should update dynamically (e.g., changing depending on the user’s actions "
            + "or when an external data source changes) with Knockout can be handled more simply and in a maintainable fashion.";

        const string FullTextForOneLetterSearch = "Any section of UI that should update dynamically with Knockout can be handled more simply and in[...]"
            + "Any section of UI that should update dynamically with Knockout can be handled more simply and in[...]"
            + "Any section of UI that should update dynamically with Knockout can be handled more simply and in a maintainable fashion."
            + "Any section of UI that should update dynamically with Knockout can be handled more simply and in[...]"
            + "Any section of UI that should update dynamically with Knockout can be handled more simply and in[...]";

        private const string FullTextForOneLetterSearchResult = "... in[...]Any section of UI that should update dynamically with Knockout can be handled "
            + "more simply and in a maintainable fashion.Any section of UI that should update dynamically with Knockout can be handled...";

        private const string AuthorizedDocumentPath = "BetterCms.Test.Module.Contents.Documents.page.authorized.htm";

        private bool authorizedDocumentAdded;

        [Test]
        public void Should_Return_Correct_Search_Results()
        {
            var document1 = new HtmlDocument();
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<title>Test title</title>"));
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<body><p>Body with search phrase test</p></body>"));
            
            var document2 = new HtmlDocument();
            document2.DocumentNode.AppendChild(HtmlNode.CreateNode("<title>Test title</title>"));
            document2.DocumentNode.AppendChild(HtmlNode.CreateNode("<body><p>Body without search phrase</p></body>"));

            var page1 = new PageData { AbsolutePath = "/test-1", Content = document1, Id = Guid.NewGuid(), IsPublished = true};
            var page2 = new PageData { AbsolutePath = "/test-2", Content = document2, Id = Guid.NewGuid(), IsPublished = true };

            var service = new DefaultIndexerService(Container.Resolve<ICmsConfiguration>(), Container.Resolve<IRepository>(),
                Container.Resolve<ISecurityService>(), Container.Resolve<IAccessControlService>());
            
            service.Open();
            service.AddHtmlDocument(page1);
            service.AddHtmlDocument(page2);
            service.Close();

            var results = service.Search(new SearchRequest("test"));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 1, "Should return one item.");
            Assert.IsTrue(results.Items[0].Link == page1.AbsolutePath);
        }

        [Test]
        public void Should_Return_Correct_Snippet_FromMiddleText()
        {
            var document1 = new HtmlDocument();
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<title>Test title</title>"));
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<body>" + ReplaceStringWithNumber(FullText, 3) + "</body>"));

            var page1 = new PageData { AbsolutePath = "/test-3", Content = document1, Id = Guid.NewGuid(), IsPublished = true };

            var service = new DefaultIndexerService(Container.Resolve<ICmsConfiguration>(), Container.Resolve<IRepository>(),
                Container.Resolve<ISecurityService>(), Container.Resolve<IAccessControlService>());

            service.Open();
            service.AddHtmlDocument(page1);
            service.Close();

            var results = service.Search(new SearchRequest("section3"));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 1, "Should return one item.");
            // Should be found the middle of the string, because the key word is in the middle of long text
            Assert.AreEqual(results.Items[0].Snippet, ReplaceStringWithNumber(MiddleText, 3));
        }
        
        [Test]
        public void Should_Return_Correct_Snippet_FromTextStart()
        {
            var document1 = new HtmlDocument();
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<title>Test title</title>"));
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<body>" + ReplaceStringWithNumber(FullText, 4) + "</body>"));

            var page1 = new PageData { AbsolutePath = "/test-4", Content = document1, Id = Guid.NewGuid(), IsPublished = true };

            var service = new DefaultIndexerService(Container.Resolve<ICmsConfiguration>(), Container.Resolve<IRepository>(),
                Container.Resolve<ISecurityService>(), Container.Resolve<IAccessControlService>());

            service.Open();
            service.AddHtmlDocument(page1);
            service.Close();

            var results = service.Search(new SearchRequest("extensible4"));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 1, "Should return one item.");
            // Should be found the start of the string, because the start word is in the start
            Assert.AreEqual(results.Items[0].Snippet, ReplaceStringWithNumber(StartText, 4));
        }
        
        [Test]
        public void Should_Return_Correct_Snippet_FromTextEnd()
        {
            var document1 = new HtmlDocument();
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<title>Test title</title>"));
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<body>" + ReplaceStringWithNumber(FullText, 5) + "</body>"));

            var page1 = new PageData { AbsolutePath = "/test-5", Content = document1, Id = Guid.NewGuid(), IsPublished = true };

            var service = new DefaultIndexerService(Container.Resolve<ICmsConfiguration>(), Container.Resolve<IRepository>(),
                Container.Resolve<ISecurityService>(), Container.Resolve<IAccessControlService>());

            service.Open();
            service.AddHtmlDocument(page1);
            service.Close();

            var results = service.Search(new SearchRequest("maintainable5"));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 1, "Should return one item.");
            // Should be found the end of the string, because the key word is in the end
            Assert.AreEqual(results.Items[0].Snippet, ReplaceStringWithNumber(EndText, 5));
        }
        
        [Test]
        public void Should_Return_Correct_Snippet_Full()
        {
            var document1 = new HtmlDocument();
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<title>Test title</title>"));
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<body>" + ReplaceStringWithNumber(FullShortText, 6) + "</body>"));

            var page1 = new PageData { AbsolutePath = "/test-6", Content = document1, Id = Guid.NewGuid(), IsPublished = true };

            var service = new DefaultIndexerService(Container.Resolve<ICmsConfiguration>(), Container.Resolve<IRepository>(),
                Container.Resolve<ISecurityService>(), Container.Resolve<IAccessControlService>());

            service.Open();
            service.AddHtmlDocument(page1);
            service.Close();

            var results = service.Search(new SearchRequest("dynamically6"));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 1, "Should return one item.");
            // Should be found whole string, because it's too short for crop
            Assert.AreEqual(results.Items[0].Snippet, ReplaceStringWithNumber(FullShortText, 6));
        }
        
        [Test]
        public void Should_Return_Correct_Snippet_FullWord()
        {
            var document1 = new HtmlDocument();
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<title>Test title</title>"));
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<body>" + FullTextForOneLetterSearch + "</body>"));

            var page1 = new PageData { AbsolutePath = "/test-7", Content = document1, Id = Guid.NewGuid(), IsPublished = true };

            var service = new DefaultIndexerService(Container.Resolve<ICmsConfiguration>(), Container.Resolve<IRepository>(),
                Container.Resolve<ISecurityService>(), Container.Resolve<IAccessControlService>());

            service.Open();
            service.AddHtmlDocument(page1);
            service.Close();

            var results = service.Search(new SearchRequest("a"));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 1, "Should return one item.");
            // Should be found separate word "a" excluding "a" in another words
            Assert.AreEqual(results.Items[0].Snippet, FullTextForOneLetterSearchResult);
        }

        [Test]
        public void Should_Delete_DocumentFromindex()
        {
            var document1 = new HtmlDocument();
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<title>Deleted document title</title>"));
            document1.DocumentNode.AppendChild(HtmlNode.CreateNode("<body>text which will be deleted</body>"));

            var page1 = new PageData { AbsolutePath = "/test-delete-1", Content = document1, Id = Guid.NewGuid(), IsPublished = true };
            var page2 = new PageData { AbsolutePath = "/test-delete-2", Content = document1, Id = Guid.NewGuid(), IsPublished = true };
            var page3 = new PageData { AbsolutePath = "/test-delete-3", Content = document1, Id = Guid.NewGuid(), IsPublished = true };

            var service = new DefaultIndexerService(Container.Resolve<ICmsConfiguration>(), Container.Resolve<IRepository>(),
                Container.Resolve<ISecurityService>(), Container.Resolve<IAccessControlService>());

            service.Open();
            service.AddHtmlDocument(page1);
            service.AddHtmlDocument(page2);
            service.AddHtmlDocument(page3);
            service.Close();

            // Search result should return 3 objects
            var results = service.Search(new SearchRequest("deleted"));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 3, "Should return 3 items.");

            // Delete 2 objects
            service.Open();
            service.DeleteDocuments(new [] { page1.Id, page2.Id });
            service.Close();

            // Search result should return 1 object
            results = service.Search(new SearchRequest("deleted"));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 1, "Should return one item.");
            Assert.AreEqual(results.Items[0].Link, page3.AbsolutePath);
        }

        [Test]
        public void Should_Return_Correctly_SavedHtmlDocument()
        {
            var service = new DefaultIndexerService(Container.Resolve<ICmsConfiguration>(), Container.Resolve<IRepository>(),
                    Container.Resolve<ISecurityService>(), Container.Resolve<IAccessControlService>());

            AddAuthorizedDocumentToIndex(service);

            var results = service.Search(new SearchRequest("\"Test page HTML content\""));

            Assert.IsNotNull(results.Items);
            Assert.AreEqual(results.Items.Count, 1, "Should return one item.");
            // Should be found separate word "a" excluding "a" in another words
            Assert.AreEqual(results.Items[0].Snippet, "authorized-html-example Test page HTML content.");
        }

        private void AddAuthorizedDocumentToIndex(DefaultIndexerService service)
        {
            if (!authorizedDocumentAdded)
            {
                string html;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(AuthorizedDocumentPath))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }
                }

                var document = new HtmlDocument();
                document.LoadHtml(html);

                var page = new PageData { AbsolutePath = "/test-authorized-document", Content = document, Id = Guid.NewGuid(), IsPublished = true };

                service.Open();
                service.AddHtmlDocument(page);
                service.Close();

                authorizedDocumentAdded = true;
            }
        }

        private string ReplaceStringWithNumber(string text, int suffix)
        {
            return text
                .Replace("section", string.Concat("section", suffix))
                .Replace(" a ", string.Concat(" a", suffix, " "))
                .Replace("maintainable", string.Concat("maintainable", suffix))
                .Replace("dynamically", string.Concat("dynamically", suffix))
                .Replace("extensible", string.Concat("extensible", suffix));
        }
    }
}
