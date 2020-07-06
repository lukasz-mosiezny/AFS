using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AFS_Rekrutacja.Models;
using Newtonsoft.Json;

namespace AFS_Rekrutacja.Controllers
{
    public class SentencesController : Controller
    {
        private masterEntities2 db = new masterEntities2();

        // GET: Sentences
        public ActionResult Index()
        {
            var sentences = db.Sentences.Include(s => s.Translations);
            return View(sentences.ToList());
        }

        // GET: Sentences/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sentences sentences = db.Sentences.Find(id);
            if (sentences == null)
            {
                return HttpNotFound();
            }
            return View(sentences);
        }

        // GET: Sentences/Create
        public ActionResult Create()
        {
            ViewBag.TranslationID = new SelectList(db.Translations, "ID", "TranslationName");
            return View();
        }

        // POST: Sentences/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Text,TranslatedText,TranslationID")] Sentences sentences)
        {
            if (ModelState.IsValid)
            {
                // Get Url of translation API. url = Based url + user's text.
                var url = GetTranslationUrl(sentences);
                // Get translation of user's text.
                sentences.TranslatedText = GetTranslation(url);
                // Display translated text or error message to user.
                ViewBag.translated = sentences.TranslatedText ?? "Reached limit of requests :c Try again later :)";

                if(sentences.TranslatedText != null)
                {
                    db.Sentences.Add(sentences);
                    db.SaveChanges();
                }
                // To show only this translated text to user -> stay on page, do not redirect.
                //return RedirectToAction("Index");
            }

            ViewBag.TranslationID = new SelectList(db.Translations, "ID", "TranslationName", sentences.TranslationID);
            return View(sentences);
        }

        // GET: Sentences/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sentences sentences = db.Sentences.Find(id);
            if (sentences == null)
            {
                return HttpNotFound();
            }
            ViewBag.TranslationID = new SelectList(db.Translations, "ID", "TranslationName", sentences.TranslationID);
            return View(sentences);
        }

        // POST: Sentences/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Text,TranslatedText,TranslationID")] Sentences sentences)
        {
            if (ModelState.IsValid)
            {
                var oldTranslated = sentences.TranslatedText;

                // Get Url of translation API. url = Based url + user's text.
                var url = GetTranslationUrl(sentences);
                // Get translation of user's text -> save old if null.
                sentences.TranslatedText = GetTranslation(url) ?? oldTranslated;
                // Display translated text or error message to user.

                if (sentences.TranslatedText != oldTranslated)
                {
                    ViewBag.translated = sentences.TranslatedText;

                    db.Entry(sentences).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.TranslationID = new SelectList(db.Translations, "ID", "TranslationName", sentences.TranslationID);
            return View(sentences);
        }

        // GET: Sentences/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sentences sentences = db.Sentences.Find(id);
            if (sentences == null)
            {
                return HttpNotFound();
            }
            return View(sentences);
        }

        // POST: Sentences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sentences sentences = db.Sentences.Find(id);
            db.Sentences.Remove(sentences);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Creating two functions instead of one. More readable and opened for extensions.
        // Returns Url to API that gives translation.
        public string GetTranslationUrl(Sentences sentences)
        {
            // Get Url of chosen translation from database.
            var translationUrl = db.Translations.SingleOrDefault(x => x.ID == sentences.TranslationID).TranslationUrl;
            // Add user's text to base url.
            translationUrl = translationUrl + "?text=" +sentences.Text;
            return translationUrl;
        }

        // Returns translated text from any Translation via url.
        public string GetTranslation(string url)
        {
            string translated;
            using(WebClient wc = new WebClient())
            {
                try
                {
                    var json = wc.DownloadString(url);
                    dynamic tmp = JsonConvert.DeserializeObject(json);
                    // Fetching important info from json. Translaton APIs got same structure.
                    translated = tmp.contents.translated;
                }
                catch(Exception e)
                {
                    // Probably limit of requests or less probably - diffrent json's structure.
                    translated = null;
                    // Send message about error to user just in case.
                    ViewBag.error = e.Message;
                }
                return translated;
            }
        }
    }
}
