﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Common.Interfaces
{
    public interface IGetClone<T>
    {
        T GetClone(int? grade);
    }
}