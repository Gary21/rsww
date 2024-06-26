﻿using MessagePack;
using CatalogRequestService.Filters;

namespace CatalogRequestService.Queries
{
    [MessagePackObject]
    public class TransportGetQuery
    {
        [Key(0)]
        public TransportQueryFilters? filters { get; set; } = null;
        [Key(1)]
        public Sort? sorting { get; set; } = null;
    }
}
