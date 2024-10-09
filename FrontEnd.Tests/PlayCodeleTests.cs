﻿using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using Codel_Cloud_Native.Web;
using Codel_Cloud_Native.Web.Components.Pages;
using CodeleLogic;
using RichardSzalay.MockHttp;

namespace FrontEnd.Tests;

[TestClass]
public class PlayCodeleTests : BunitTestContext
{
    [TestMethod]
    public void AttemptsStartAtOne()
    {
        var mock = Services.AddMockHttpClient();
        mock.When("CodeleApi").RespondJson(new string[2] { "hello", "world" });
        var cut = RenderComponent<PlayCodele>();

        cut.Find("strong").MarkupMatches("<strong>Attempt #: 1</strong>");
    }

    [TestMethod]
    public void SubmittingWrongGuess()
    {
        var mock = Services.AddMockHttpClient();
        CodeleWords[] answers = new CodeleWords[2] { new CodeleWords("hello"), new CodeleWords("world") };
        mock.When("CodeleApi").RespondJson(answers);
        var cut = RenderComponent<PlayCodele>(parameters => parameters.Add(p => p.answers, answers).Add(p => p.answer, "world"));

        cut.Find("input").Change("hello");

        cut.Find("button").Click();

        cut.Find("strong").MarkupMatches("<strong>Attempt #: 2</strong>");
    }

    [TestMethod]
    public void SubmittingCorrectGuess()
    {
        var mock = Services.AddMockHttpClient();
        CodeleWords[] answers = new CodeleWords[2] { new CodeleWords("hello"), new CodeleWords("world") };
        mock.When("CodeleApi").RespondJson(answers);
        var cut = RenderComponent<PlayCodele>(parameters => parameters.Add(p => p.answers, answers).Add(p => p.answer, "hello"));

        cut.Find("input").Change("hello");

        cut.Find("button").Click();

        cut.Find("h4").MarkupMatches(@"<h4 class=""modal-title"">You Won!</h4>");
    }

    [TestMethod]
    public void GuessIsNotFiveLetters()
    {
        var mock = Services.AddMockHttpClient();
        CodeleWords[] answers = new CodeleWords[2] { new CodeleWords("hello"), new CodeleWords("world") };
        mock.When("CodeleApi").RespondJson(answers);
        var cut = RenderComponent<PlayCodele>(parameters => parameters.Add(p => p.answers, answers).Add(p => p.answer, "hello"));

        cut.Find("input").Change("worlds");

        cut.Find("button").Click();

        var pTags = cut.FindAll("p");
        pTags[2].MarkupMatches(@"<p style=""color: rgb(197, 3, 3);"">Guess must be 5 characters long</p>");
    }
}