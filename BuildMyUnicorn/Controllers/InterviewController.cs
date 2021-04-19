﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuildMyUnicorn.Business_Layer;
using Business_Model.Model;
using System.IO;
using Newtonsoft.Json;
using Business_Model.Helper;

namespace BuildMyUnicorn.Controllers
{
    public class InterviewController : WebController
    {
        // GET: Interview
        public ActionResult Index(string InterviewID)
        {
            int State = (int)EntityState.New;
            IEnumerable<Interview> InterviewList = new InterviewManager().GetAllInterview();
            if (InterviewList == null || InterviewList.Count() == 0)
            {
                if (ResponseType.Redirect.ToString() == CheckModuleCourse(State, (int)ModuleSection.MarketResearch_Interview))
                {
                    return RedirectToAction("Index", "ModuleCourse", new
                    {
                        ControllerName = "Interview",
                        ActionName = "Index",
                        ModuleName = Module.MarketResearch.ToString(),
                        SectionName = ModuleSection.MarketResearch_Interview.ToString()
                    });
                }

            }
            if (InterviewID != null)
            {
                ViewBag.obj = new InterviewManager().GetInterview(Guid.Parse(InterviewID));
                if (ViewBag.obj != null)
                    return View("Interview");
                else
                    return PartialView("_BadRequest");
            }
            else
            return View();
        }

        public ActionResult InterviewList()
        {
            return PartialView("_InterviewListPartial", new InterviewManager().GetAllInterview());
        }

        public ActionResult New()
        {
            ViewBag.Video = new Master().GetSectionModuleVideo((int)Module.MarketResearch, (int)ModuleSection.MarketResearch_Interview);
            return View();
        }

        public string Add(Interview Model)
        {
            return new InterviewManager().AddInterview(Model);
        }

        public void AddInterviewData(string InterviewID)
        {
       
            Stream req = Request.InputStream;
            req.Seek(0, SeekOrigin.Begin);
            string jsonData = new StreamReader(req).ReadToEnd();
            dynamic dyn = JsonConvert.DeserializeObject(jsonData);
            List<InterviewData> InterviewDataList = new List<InterviewData>();
            foreach (var item in dyn.data)
            {
                string key = Convert.ToString(item.Name);
                string value = Convert.ToString(item.Value);
                InterviewDataList.Add(new InterviewData() { KeyField = key, KeyValue = value });
 
            }

            new InterviewManager().AddInterviewData(InterviewDataList, Guid.Parse(InterviewID));
            
        }

        public ActionResult GetData(string InterviewID)
        {
            IEnumerable<InterviewData> modelList = new InterviewManager().GetInterviewData(Guid.Parse(InterviewID));
            return View("InterviewAnswers", modelList);
        }

        public string Delete(string InterviewID)
        {
            return new InterviewManager().DeleteInterview(Guid.Parse(InterviewID));
        }
        public string CheckModuleCourse(int State, int SectionValue)
        {
            if (State == 0)
            {
                string getValue = "0";
                string getClientID = string.Empty;
                string LoginUserID = User.Identity.Name.ToString();
                string SectionName = Enum.GetName(typeof(ModuleSection), SectionValue);
                string CookieID = SectionName.ToString() + LoginUserID;
                if (Request.Cookies[CookieID.ToString()] != null)
                {
                    HttpCookie aCookie = Request.Cookies[CookieID.ToString()];
                    getValue = aCookie.Values["Status"];
                }
                else
                {
                    HttpCookie appCookie = new HttpCookie(CookieID.ToString());
                    appCookie.Values["Status"] = "0";
                    appCookie.Values["ClientID"] = User.Identity.Name.ToString();
                    appCookie.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(appCookie);
                }
                ModuleCourse objCourse = new Master().GetSingleModuleCourse((int)Module.MarketResearch, SectionValue);

                if (getValue == "0" && objCourse.ModuleCourseID != Guid.Empty)
                    return ResponseType.Redirect.ToString();
                else return ResponseType.NotRedirect.ToString();

            }
            else
                return ResponseType.NotRedirect.ToString();

        }
    }
}