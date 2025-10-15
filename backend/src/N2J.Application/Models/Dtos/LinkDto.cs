using System;
using System.Collections.Generic;
namespace N2J.Application.Models.Dtos
{
    public class LinkDto
    {
        public string Href { get; set; } = string.Empty;
        public string Rel { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string? LinkDescription { get; set; }

        public LinkDto() { }

        public LinkDto(string href, string rel, string method, string? description = null)
        {
            Href = href;
            Rel = rel;
            Method = method;
            LinkDescription = description;
        }
    }
}
