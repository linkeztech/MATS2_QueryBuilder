using MATS2_QueryBuilder.DataBaseWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATS2_QueryBuilder
{
    public class GlobalVar
    {
        /// <summary>
        /// Database Operations variable to access functions from MySQLOperations
        /// @author - Nitu 
        /// </summary>
        //----------For Database ------------//
        public static MySQLOperation DBOpp = new();
        public static Boolean IsDatabaseDebugEnabled = true;
    }
}
