using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.Models;

public interface IHasId
{
    public string Id { get; set; }
}
