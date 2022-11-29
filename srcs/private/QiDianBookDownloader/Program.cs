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

using CommandLine;
using Microsoft.Playwright;
using QiDianBookDownloader;

return await Parser.Default.ParseArguments<CommandLineOptions>(args)
    .MapResult(RunAndReturnExitCode, HandleParseErrorAndReturnExitCode);

static Task<int> HandleParseErrorAndReturnExitCode(IEnumerable<Error> errorsEnumerable)
{
    var errors = errorsEnumerable.ToList();

    if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError
                        || e.Tag == ErrorType.VersionRequestedError))
    {
        return Task.FromResult(0);
    }

    foreach (var err in errors)
    {
        Console.WriteLine(err);
    }

    return Task.FromResult(1);
}

static async Task<int> RunAndReturnExitCode(CommandLineOptions options)
{
    Console.WriteLine("Happy!");

    using var playwright = await Playwright.CreateAsync();
    await using IBrowser browser = await playwright.Chromium.LaunchAsync(
        new BrowserTypeLaunchOptions()
        {
            Headless = true,
            DownloadsPath = options.OutputDirectory,
        });

    var context = await browser.NewContextAsync();

    var bookDownloader = new BookDownloader(
        context, options.BookId ?? throw new InvalidOperationException("BookId is required."));
    await bookDownloader.Login(
        options.UserName ?? throw new InvalidOperationException("UserName is required."),
        options.Password ?? throw new InvalidOperationException("Password is required."));
    await bookDownloader.FetchCatalog();

    return 0;
}
