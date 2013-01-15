﻿using System.Web.Mvc;

using BetterCms.Module.Navigation.Command.Sitemap.DeleteSitemapNode;
using BetterCms.Module.Navigation.Command.Sitemap.GetSitemap;
using BetterCms.Module.Navigation.Command.Sitemap.SaveSitemapNode;
using BetterCms.Module.Navigation.Content.Resources;
using BetterCms.Module.Navigation.ViewModels.Sitemap;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;

namespace BetterCms.Module.Navigation.Controllers
{
    /// <summary>
    /// Handles sitemap logic.
    /// </summary>
    public class SitemapController : CmsControllerBase
    {
        /// <summary>
        /// Renders sitemap container.
        /// </summary>
        /// <param name="search">Sitemap node search text.</param>
        /// <returns>
        /// Rendered sitemap container.
        /// </returns>
        public ActionResult Index(string search)
        {
            var sitemap = GetCommand<GetSitemapCommand>().ExecuteCommand(search);
            var json = new
                           {
                               Data = new WireJson
                                          {
                                              Success = true,
                                              Data = sitemap
                                          },
                               Html = RenderView("Index", new SearchableSitemapViewModel())
                           };
            return WireJson(true, json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the sitemap node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>JSON result.</returns>
        [HttpPost]
        public ActionResult SaveSitemapNode(SitemapNodeViewModel node)
        {
            if (ModelState.IsValid)
            {
                var response = GetCommand<SaveSitemapNodeCommand>().ExecuteCommand(node);
                if (response != null)
                {
                    if (node.Id.HasDefaultValue())
                    {
//                        Messages.AddSuccess(NavigationGlobalization.CreateSitemapNode_CreatedSuccessfully_Message);
                    }

                    return Json(new WireJson { Success = true, Data = response });
                }
            }

            return Json(new WireJson { Success = false });
        }

        [HttpPost]
        public ActionResult DeleteSitemapNode(SitemapNodeViewModel node)
        {
            bool success = GetCommand<DeleteSitemapNodeCommand>().ExecuteCommand(node);

            if (success)
            {
//                Messages.AddSuccess(NavigationGlobalization.DeleteSitemapNode_DeletedSuccessfully_Message);
            }

            return Json(new WireJson(success));
        }
    }
}