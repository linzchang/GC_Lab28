using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Lab28.Controllers
{
    public class APIController : Controller
    {
        const string UserAgent = "Mozilla / 5.0(Windows NT 6.1; Win64; x64; rv: 47.0) Gecko / 20100101 Firefox / 47.0";

        public ActionResult ViewRawData()
        {
            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/");
            request.UserAgent = UserAgent;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader data = new StreamReader(response.GetResponseStream());
                ViewBag.RawData = data.ReadToEnd();
            }

            return View();
        }

        public ActionResult GetDeck()
        {
            //Use the API to create a new deck
            HttpWebRequest request = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");
            request.UserAgent = UserAgent;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //Read the info from the API
                StreamReader data = new StreamReader(response.GetResponseStream());
                JObject dataObject = JObject.Parse(data.ReadToEnd());
                //ViewBag.RawData = data.ReadToEnd();
                //Grab the deck_id and save it using temp data so you use the same deck over and over instead of generating a new deck each time
                var deckID = dataObject["deck_id"];

                //store deck id in temp data
                if (!TempData.ContainsKey("DeckID"))
                {
                    TempData.Add("DeckID", deckID);
                }

                //to reestablish TempData as itself
                //before switching ActionResults / Views
                TempData["DeckID"] = TempData["DeckID"];
            }

            return View();
        }

        public ActionResult ViewCards()
        {
            //to access TempData in next ActionResult
            if (TempData["DeckID"] != null)
            {
                string deckID = TempData["DeckID"].ToString();

                //Use the API to create a new deck
                HttpWebRequest request = WebRequest.CreateHttp($"https://deckofcardsapi.com/api/deck/{deckID}/draw/?count=5");
                request.UserAgent = UserAgent;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Read the info from the API
                    StreamReader data = new StreamReader(response.GetResponseStream());
                    JObject dataObject = JObject.Parse(data.ReadToEnd());

                    ViewBag.Cards = dataObject["cards"];
                }
            }

            //to reestablish TempData as itself
            //before switching ActionResults / Views
            TempData["DeckID"] = TempData["DeckID"];

            return View();
        }

        public ActionResult DrawMoreCards()
        {
            TempData["DeckID"] = TempData["DeckID"];

            return RedirectToAction("ViewCards");
        }
    }


}