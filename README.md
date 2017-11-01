CSPlayStoreScraper
==================

A simple web crawler for scraping android apps data in Play Store. This is a forked project from [GooglePlayStoreCrawler](https://github.com/MarcelloLins/GooglePlayAppsCrawler) project.

I simplified the original code by removing MongoDB dependency and made some improvements and bug fixing. In short, it's a plain and simple web crawler for Google Play Store that can export the result into CSV and JSON file.

You don't have to input any of your Google Account credentials since this Crawler acts like a "Logged Out" user.

# What is this project about ? 

From GooglePlayStoreCrawler's description:

"The main idea of this project is to gather/mine data about apps of the Google Play Store and build a rich database so that developers, android fans and anyone else can use to generate statistics about the current play store situation

There are many questions we have no answer at the moment and we should be able to answer with this database."

# What do I need before I start?

* Know C# and .NET

* Visual Studio 2013 (for compiling the source code).

* Start from "Program.cs", it contains example code to run the crawler.

* To find out how the crawler works, study the "PlayStoreScraper.Crawl" method.

