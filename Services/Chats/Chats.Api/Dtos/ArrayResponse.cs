﻿using System.Collections.Generic;

namespace Chats.Api.Dtos
{
    public class ArrayResponse<T>
    {
        public int Total { get; }

        public IEnumerable<T> Items { get; }

        public ArrayResponse(IEnumerable<T> items, int total)
        {
            Items = items;
            Total = total;
        }
    }
}
