using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGag.Models
{
    public class StatsModel
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public List<StatsModel> Children { get; set; }
    }
}