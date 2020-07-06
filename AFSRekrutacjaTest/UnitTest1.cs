using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AFS_Rekrutacja.Controllers;
using System.Web.Mvc;

namespace AFS_Rekrutacja.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void ContactViewTest()
        {
            var controller = new HomeController();
            var result = controller.Contact() as ViewResult;
            Assert.AreEqual("Contact", result.ViewName);
        }
        [TestMethod]
        public void AboutViewTest()
        {
            var c = new HomeController();
            var r = c.About() as ViewResult;
            Assert.AreEqual("About", r.ViewName);
        }
    }
    [TestClass]
    public class SentencesControllerTest
    {
        [TestMethod]
        public void EditMethodTest01()
        {
            var controller = new SentencesController();
            var result = controller.Edit(16) as ViewResult;
            var sentence = (Models.Sentences)result.ViewData.Model;
            Assert.AreEqual("My translation", sentence.Text);
        }
        [TestMethod]
        public void EditMethodTest02()
        {
            var controller = new SentencesController();
            var result = controller.Edit(20) as ViewResult;
            var sentence = (Models.Sentences)result.ViewData.Model;
            Assert.AreEqual(2, sentence.TranslationID);
        }
        [TestMethod]
        public void GetTranslationUrlMethodTest01()
        {
            var controller = new SentencesController();
            Models.Sentences testSentence = new Models.Sentences();
            testSentence.ID = 24;
            testSentence.Text = "myTest";
            testSentence.TranslatedText = "";
            testSentence.TranslationID = 1;

            var testUrl = controller.GetTranslationUrl(testSentence);

            Assert.AreEqual("https://api.funtranslations.com/translate/leetspeak.json?text=myTest", testUrl);
        }
        // Checking GetTranslation useless - one text -> multiple possible corect outputs.
    }
}
