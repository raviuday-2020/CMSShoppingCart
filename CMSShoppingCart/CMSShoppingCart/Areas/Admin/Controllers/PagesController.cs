using CMSShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        public static CMSCartEntities db;

        public ActionResult Index()
        {
            List<PageVM> pageList;
            using (db = new CMSCartEntities())
            {
                pageList = db.tblPages.ToArray().OrderBy(e => e.Sorting).Select(page => new PageVM(page)).ToList();
            }
            return View(pageList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            PageVM model = new PageVM();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                using (db = new CMSCartEntities())
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        model.Slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        model.Slug = model.Slug.Replace(" ", "-").ToLower();
                    }

                    if (db.tblPages.Any(x => x.Title == model.Title))
                    {
                        ModelState.AddModelError(model.Title, "Title already exists");
                        return View(model);
                    }
                    if (db.tblPages.Any(e => e.Slug == model.Slug))
                    {
                        ModelState.AddModelError(model.Slug, "Slug already exists");
                        return View(model);
                    }

                    tblPage page = new tblPage();
                    page.Title = model.Title;
                    page.Slug = model.Slug;
                    page.Body = model.Body;
                    page.Sorting = model.Sorting;
                    page.HasSidebar = model.HasSidebar;
                    db.tblPages.Add(page);
                    db.SaveChanges();
                }
                TempData["createpage"] = "You have added a new page.";
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            PageVM model = new PageVM();

            using (db = new CMSCartEntities())
            {
                model = db.tblPages.Where(e => e.Id == id).Select(page => new PageVM(page)).FirstOrDefault();
                if (model == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                using (db = new CMSCartEntities())
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        model.Slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        model.Slug = model.Slug.Replace(" ", "-").ToLower();
                    }

                    if (db.tblPages.Where(e => e.Id != model.Id).Any(x => x.Title == model.Title))
                    {
                        ModelState.AddModelError("", "Title already exists.");
                        return View(model);
                    }
                    if (db.tblPages.Where(e => e.Id != model.Id).Any(e => e.Slug == model.Slug))
                    {
                        ModelState.AddModelError("", "Slug already exists.");
                        return View(model);
                    }

                    tblPage page = db.tblPages.Find(model.Id);
                    page.Title = model.Title;
                    page.Slug = model.Slug;
                    page.Body = model.Body;
                    page.Sorting = model.Sorting;
                    page.HasSidebar = model.HasSidebar;
                    db.SaveChanges();
                }
                TempData["editpage"] = "You have updated a page.";
                return RedirectToAction("Edit");
            }
        }

        public ActionResult Delete(int id)
        {
            using (db = new CMSCartEntities())
            {
                tblPage page = db.tblPages.Find(id);
                db.tblPages.Remove(page);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}