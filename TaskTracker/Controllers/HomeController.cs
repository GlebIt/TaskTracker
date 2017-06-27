using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    public class HomeController : Controller
    {
        private TasksDBContext context = new TasksDBContext();

        public ActionResult Index()
        {
            return View(GetViewModel());
        }

        public PartialViewResult LoadCheckList()
        {
            return PartialView("_LoadCheckList", context.CheckList);
        }

        private TasksViewModel GetViewModel()
        {
            return new TasksViewModel { Tasks = context.Tasks, CheckList = context.CheckList };
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Task task)
        {
            context.AddNewTask(task);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Task task = context.GetTaskById(id);
            if (task == null)
                return HttpNotFound();

            return View(task);
        }

        [HttpPost]
        public ActionResult Edit(Task task)
        {
            context.EditTask(task);
            return View(task);
        }

        public ActionResult Delete(Task task)
        {
            context.DeleteTask(task);
            return RedirectToAction("Index");
        }

        public ActionResult UpdateState(int id, int state)
        {
            try
            {
                context.UpdateState(id, state);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult AddToCheckList(int id)
        {
            if(context.CheckList.Tasks.ToList().Exists(t=>t.Id==id))
                ModelState.AddModelError("CheckList", "Check list already contains this task.");

            if (ModelState.IsValid)
            {
                context.AddTaskToCheckList(id);
                return PartialView("_LoadCheckList", context.CheckList);
            }

            return PartialView("_LoadCheckList", context.CheckList);
        }

        public ActionResult DeleteFromCheckList(int id)
        {
            context.DeleteTaskFromCheckList(id);
            return PartialView("_LoadCheckList", context.CheckList);
        }
    }
}