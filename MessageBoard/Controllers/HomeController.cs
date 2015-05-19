using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MessageBoard.Entities;
using MessageBoard.Models;
using MessageBoard.ViewModels;

namespace MessageBoard.Controllers
{
    public class HomeController : Controller
    {
        private static volatile object _locker = new Object();

        private readonly int MESSAGES_QUEUE_SIZE = 5;
        private readonly int MESSAGES_QUEUE_SIZE_PER_USER = 3;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SendMessage(string sortFieldName = "Time", bool sortAscending = true)
        {
            SendMessageViewModel viewModel = new SendMessageViewModel();
            viewModel.SortFieldName = sortFieldName;
            viewModel.SortAscending = sortAscending;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SendMessage(SendMessageViewModel model)
        {
            model.MessageText = HttpUtility.HtmlEncode(Request.Form["TextArea_MessageText"]);

            if(HttpContext.Application["Messages"] == null)
            {
                HttpContext.Application["Messages"] = new List<MessageEntity>();
            }

            List<MessageEntity> messages = (List<MessageEntity>)HttpContext.Application["Messages"];

#if USE_DICTIONARY
            if (HttpContext.Application["MessagesDictionary"] == null)
            {
                HttpContext.Application["MessagesDictionary"] = new Dictionary<string, List<MessageEntity>>();
            }

            Dictionary<string, List<MessageEntity>> messagesDictionary = (Dictionary<string, List<MessageEntity>>)HttpContext.Application["MessagesDictionary"];
#endif

            MessageEntity message = new MessageEntity();
            message.UserId = HttpContext.Session.SessionID;
            message.Timestamp = DateTime.Now;
            message.Text = model.MessageText;
            
            lock(_locker)
            {
                List<MessageEntity> myMessages = messages.Where(i => i.UserId == HttpContext.Session.SessionID).ToList();

                if (myMessages.Count >= MESSAGES_QUEUE_SIZE_PER_USER)
                {
                    // assume that older messages are always in the begining of the list
                    for (int i = 0; i < myMessages.Count - MESSAGES_QUEUE_SIZE_PER_USER + 1; i++)
                    {
                        messages.Remove(myMessages[i]);
                    }
                }

                if (messages.Count >= MESSAGES_QUEUE_SIZE)
                {
                    // assume that older messages are always in the begining of the list
                    messages.RemoveRange(0, messages.Count - MESSAGES_QUEUE_SIZE + 1);
                }

                messages.Add(message);

#if USE_DICTIONARY
                if (messagesDictionary.ContainsKey(HttpContext.Session.SessionID) 
                    && messagesDictionary[HttpContext.Session.SessionID] != null 
                    && messagesDictionary[HttpContext.Session.SessionID].Count >= MESSAGES_QUEUE_SIZE_PER_USER)
                {
                    // assume that older messages are always in the begining of the list
                    messagesDictionary[HttpContext.Session.SessionID].RemoveRange(
                        0, messagesDictionary[HttpContext.Session.SessionID].Count - MESSAGES_QUEUE_SIZE_PER_USER + 1);
                }

                int allMessagesCount = messagesDictionary.Sum(i => i.Value.Count);

                if (allMessagesCount >= MESSAGES_QUEUE_SIZE)
                {
                    List<MessageEntity> allMessages = new List<MessageEntity>();

                    foreach (var item in messagesDictionary)
                    {
                        foreach (var m in item.Value)
                        {
                            allMessages.Add(m);
                        }
                    }

                    allMessages.Sort((i, j) => i.Timestamp.CompareTo(j.Timestamp));

                    for (int i = 0; i < allMessagesCount - MESSAGES_QUEUE_SIZE_PER_USER + 1; i++)
                    {
                        messagesDictionary[allMessages[i].UserId].Remove(allMessages[i]);

                        // most probably this also will work, and more efiiciently
                        //messagesDictionary[allMessages[i].UserId].RemoveAt(0);
                    }
                }

                if (!messagesDictionary.ContainsKey(HttpContext.Session.SessionID) ||
                    messagesDictionary[HttpContext.Session.SessionID] == null)
                {
                    messagesDictionary[HttpContext.Session.SessionID] = new List<MessageEntity>();
                }

                messagesDictionary[HttpContext.Session.SessionID].Add(message);
#endif
            }

            using (var db = new MessagesDbContext())
            {
                MessageEntity messageEntity = new MessageEntity();
                messageEntity.Id = Guid.NewGuid();
                messageEntity.UserId = message.UserId;
                messageEntity.Timestamp = message.Timestamp;
                messageEntity.Text = message.Text;
                
                db.Messages.Add(messageEntity);
                db.SaveChanges();
            }

            model.ResultMessage = "Message sent: " + model.MessageText;
            model.ResultStatus = ActionResultStatus.Success;
            model.SortFieldName = Request.Form["Hidden_SortFieldName"];
            model.SortAscending = bool.Parse(Request.Form["Hidden_SortAscending"]);

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}