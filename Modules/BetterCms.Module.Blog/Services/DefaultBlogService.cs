﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BetterCms.Core.DataAccess.DataContext;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc.Helpers;

using NHibernate.Criterion;

namespace BetterCms.Module.Blog.Services
{
    public class DefaultBlogService : IBlogService
    {
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The cms configuration
        /// </summary>
        private readonly ICmsConfiguration configuration;

        public DefaultBlogService(IUnitOfWork unitOfWork, ICmsConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        public string CreateBlogPermalink(string title)
        {
            var url = title.Transliterate();
            url = AddUrlPathSuffixIfNeeded(url);

            return url;
        }

        /// <summary>
        /// Path exists in database.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Url path.</returns>
        private bool PathExistsInDb(string url)
        {
            var exists = unitOfWork.Session
                .QueryOver<Page>()
                .Where(p => !p.IsDeleted && p.PageUrl == url)
                .Select(p => p.Id)
                .RowCount();
            return exists > 0;
        }

        /// <summary>
        /// Adds the URL path suffix if needed.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>Url path.</returns>
        private string AddUrlPathSuffixIfNeeded(string url)
        {
            var urlPattern = configuration.ArticleUrlPattern;
            urlPattern = urlPattern.TrimEnd('/');

            if (urlPattern == "" || urlPattern.IndexOf("{0}", StringComparison.OrdinalIgnoreCase) == -1)
            {
                urlPattern = "/{0}";
            }
            var fullUrl = string.Format(urlPattern + "/", url);

            // Check, if such record exists
            var exists = PathExistsInDb(fullUrl);

            if (exists)
            {
                // Load all titles
                var urlToReplace = string.Format(urlPattern + "-", url);
                var urlToSearch = string.Format("{0}%", urlToReplace);
                Page alias = null;

                var paths = unitOfWork.Session
                    .QueryOver(() => alias)
                    .Where(p => !p.IsDeleted)
                    .Where(Restrictions.InsensitiveLike(Projections.Property(() => alias.PageUrl), urlToSearch))
                    .Select(p => p.PageUrl)
                    .List<string>();

                int maxNr = 0;
                var recheckInDb = false;
                foreach (var path in paths)
                {
                    int pathNr;
                    var intStr = path.Replace(urlToReplace, null).Split('-')[0].Trim('/');

                    if (int.TryParse(intStr, out pathNr))
                    {
                        if (pathNr > maxNr)
                        {
                            maxNr = pathNr;
                        }
                    }
                    else
                    {
                        recheckInDb = true;
                    }
                }

                if (maxNr == int.MaxValue)
                {
                    recheckInDb = true;
                }
                else
                {
                    maxNr++;
                }

                if (string.IsNullOrWhiteSpace(url))
                {
                    fullUrl = "-";
                    recheckInDb = true;
                }
                else
                {
                    fullUrl = string.Format(urlPattern + "-{1}/", url, maxNr);
                }

                if (recheckInDb)
                {
                    url = string.Format(fullUrl.Trim('/'));
                    return AddUrlPathSuffixIfNeeded(url);
                }
            }

            return fullUrl;
        }
    }
}