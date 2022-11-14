using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SensorsWPF.Logic
{
    public static class Helper
    {

        public static string FirstLetterToUpper(this string input)
        {
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }



    }
}
