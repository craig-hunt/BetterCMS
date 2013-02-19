﻿using System.Collections.Generic;
using System.Linq;

using BetterCms.Core.DataAccess;
using BetterCms.Core.DataContracts;
using BetterCms.Core.DataServices;
using BetterCms.Core.Events;
using BetterCms.Module.Root.Models;

namespace BetterCms.Module.Pages.Services
{
    public class DefaultTagApiService : ITagApiService
    {
        private IRepository repository { get; set; }

        public DefaultTagApiService(IRepository repository)
        {
            this.repository = repository;
        }

        // Methods implementations
        public IList<ITag> GetTags()
        {
            return repository
                .AsQueryable<Tag>()
                .Cast<ITag>()
                .ToList();
        }

        // Implemented events
        public event PageCreatedEventArgs.PageCreatedEventHandler PageCreated;
    }
}