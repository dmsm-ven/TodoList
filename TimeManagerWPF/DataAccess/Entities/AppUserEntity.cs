using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.WPF.DataAccess.Entities;

public class AppUserEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }  

    public DateTime? SavePasswordTicksState { get; set; }
}
