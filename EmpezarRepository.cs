using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Repository;
using enbloc.DbEntity.Classes;

namespace enbloc
{
    public class  EmpezarRepository<T> : Repository<T> where T : class
    { 
        public EmpezarRepository():base(new empezarContext())
        {
        }
    }
}
