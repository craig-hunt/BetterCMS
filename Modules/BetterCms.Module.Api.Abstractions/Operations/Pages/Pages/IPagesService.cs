﻿using BetterCms.Module.Api.Operations.Pages.Pages.Search;

namespace BetterCms.Module.Api.Operations.Pages.Pages
{
    public interface IPagesService
    {
        GetPagesResponse Get(GetPagesRequest request);

        ISearchPagesService Search { get; }
    }
}