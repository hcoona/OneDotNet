// Copyright (c) 2022 Zhang Shuai<zhangshuai.ustc@gmail.com>.
// All rights reserved.
//
// This file is part of OneDotNet.
//
// OneDotNet is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// OneDotNet is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// OneDotNet. If not, see <https://www.gnu.org/licenses/>.

#define LOCAL_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clocks;
using Microsoft.Playwright;

namespace QiDianBookDownloader
{
    internal class BookDownloader
    {
        private readonly IBrowserContext context;
        private readonly BookInformation book;
        private readonly RateLimiter.IRateLimiter rateLimiter;

        private IPage? page;

        public BookDownloader(IBrowserContext context, long bookId)
        {
            this.context = context;
            this.book = new BookInformation
            {
                BookId = bookId,
            };
            this.rateLimiter =
                RateLimiter.RateLimiter.CreateBursty(2, 5, new SystemStopwatchProvider());
        }

        private IPage Page =>
            this.page
            ?? throw new InvalidOperationException("Must login before do any other things.");

        public async Task Login(string username, string password)
        {
            this.page = await this.context.NewPageAsync();

#if !LOCAL_DEBUG
            await this.Page.GotoAsync("https://passport.qidian.com/");

            await this.Page.Locator("#username").FillAsync(username);
            await this.Page.Locator("#password").FillAsync(password);

            await this.Page.RunAndWaitForNavigationAsync(async () =>
            {
                await this.Page.Locator(".btnLogin").ClickAsync();
            });
#endif
        }

        public async Task FetchCatalog()
        {
#if !LOCAL_DEBUG
            await this.Page.GotoAsync($"https://book.qidian.com/info/{this.BookId}/#Catalog");
#else
            await this.Page.GotoAsync(
                "file:///C:/Users/shuaizhang/Downloads/%E5%9C%A8%E7%AC%AC%E5%9B%9B%E5%A4%A9%E7%81%BE%E4%B8%AD%E5%B9%B8%E5%AD%98(%E7%BA%B3%E8%A5%BF%E5%88%A9%E4%BA%9A)%E6%9C%80%E6%96%B0%E7%AB%A0%E8%8A%82%E5%9C%A8%E7%BA%BF%E9%98%85%E8%AF%BB-%E8%B5%B7%E7%82%B9%E4%B8%AD%E6%96%87%E7%BD%91%E5%AE%98%E6%96%B9%E6%AD%A3%E7%89%88.mhtml");  // editorconfig-checker-disable-line
#endif

            var bookInfo = this.Page.Locator("div.book-info");
            this.book.BookName = await bookInfo.Locator("h1 > em").TextContentAsync();
            this.book.BookAuthor = await bookInfo.Locator("a.writer").TextContentAsync();
            Console.WriteLine($"bookName={this.book.BookName}, bookAuthor={this.book.BookAuthor}");

            var parts = this.Page.Locator("div.volume");
            var partsCount = await parts.CountAsync();
            Console.WriteLine($"volumesCount={partsCount}");

            for (int i = 0; i < partsCount; i++)
            {
                var part = parts.Nth(i);
                var partInformation = new BookPartInformation();
                this.book.BookParts.Add(partInformation);

                var firstChildNodeIsAnchor = await part
                    .Locator("h3")
                    .EvaluateAsync<bool>("node => node.children[0].nodeName === 'A'")
                    .ConfigureAwait(false);
                if (firstChildNodeIsAnchor)
                {
                    partInformation.Name = await part
                        .Locator("h3")
                        .EvaluateAsync<string>("node => node.childNodes[2].textContent.trim()")
                        .ConfigureAwait(false);
                }
                else
                {
                    partInformation.Name = await part
                        .Locator("h3")
                        .EvaluateAsync<string>("node => node.childNodes[0].textContent.trim()")
                        .ConfigureAwait(false);
                }

                Console.WriteLine($"partName={partInformation.Name}");

                var sectionHeaders = part.Locator("ul a");
                var sectionHeadersCount = await sectionHeaders.CountAsync();
                Console.WriteLine($"sectionHeadersCount={sectionHeadersCount}");

                for (int j = 0; j < sectionHeadersCount; j++)
                {
                    var sectionHeader = sectionHeaders.Nth(j);
                    var name = await sectionHeader.TextContentAsync();
                    var href = await sectionHeader.GetAttributeAsync("href");
                    Console.WriteLine($"\tcontent={name}, href={href}");

                    partInformation.BookSections.Add(new BookSectionInformation
                    {
                        Name = name,
                        Href = href,
                    });
                }

                Console.WriteLine();
            }
        }

        private record BookSectionInformation
        {
            public string? Name { get; init; } = default!;

            public string? Href { get; init; } = default!;
        }

        private record BookPartInformation
        {
            public string? Name { get; set; }

            public IList<BookSectionInformation> BookSections { get; } =
                new List<BookSectionInformation>();
        }

        private record BookInformation()
        {
            public long BookId { get; init; }

            public string? BookName { get; set; }

            public string? BookAuthor { get; set; }

            public IList<BookPartInformation> BookParts { get; } =
                new List<BookPartInformation>();
        }
    }
}
