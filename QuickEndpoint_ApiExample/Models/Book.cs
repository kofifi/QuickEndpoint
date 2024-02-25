using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using QuickEndpoint_ApiExample.Models; // Changed from BooksApi.Models
using System.Collections.Generic;
using System.Linq;


namespace QuickEndpoint_ApiExample.Models
{
    public class Book
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public int Year { get; set; } // No warning for this as it's a non-nullable value type
    }

}
