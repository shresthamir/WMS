using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace GTransfer
{
    public static class GParse
    {
        public static int ToInteger(object objInt)   
        {
            int ReturnVal;
            if (objInt == null || objInt == DBNull.Value)
            {
                return 0;
            }
            if (int.TryParse(objInt.ToString(), out ReturnVal))
                return ReturnVal;
            else
                return 0;
        }

        public static float ToFloat(object obj)
        {
            float ReturnVal;
            if (obj == null || obj == DBNull.Value)
            {
                return 0;
            }
            if (float.TryParse(obj.ToString(), out ReturnVal))
                return ReturnVal;
            else
                return 0;
        }

        public static double ToDouble(object objDouble)
        {
            double ReturnVal;
            if (objDouble == null || objDouble == DBNull.Value)
            {
                return 0;
            }
            if (double.TryParse(objDouble.ToString(), out ReturnVal))
                return ReturnVal;
            else
                return 0;
        }

        public static decimal ToDecimal(object objDouble)
        {
            decimal ReturnVal;
            if (objDouble == null || objDouble == DBNull.Value)
            {
                return 0;
            }
            if (decimal.TryParse(objDouble.ToString(), out ReturnVal))
                return ReturnVal;
            else
                return 0;
        }

        public static long ToLong(object objLong)
        {
            long ReturnVal;
            if (objLong == null || objLong == DBNull.Value)
            {
                return 0;
            }
            if (long.TryParse(objLong.ToString(), out ReturnVal))
                return ReturnVal;
            else
                return 0;
        }

        public static short ToShort(object obj)
        {
            short ReturnVal;
            if (obj == null || obj == DBNull.Value)
            {
                return 0;
            }
            if (short.TryParse(obj.ToString(), out ReturnVal))
                return ReturnVal;
            else
                return 0;
        }

        public static char ToChar(object obj)
        {
            char ReturnVal;
            if (obj == null || obj == DBNull.Value)
            {
                return '0';
            }
            if (char.TryParse(obj.ToString(), out ReturnVal))
                return ReturnVal;
            else
                return '0';
        }

        public static bool ToBool(object obj)
        {
            
            if (obj == null || obj == DBNull.Value)
            {
                return false;
            }
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static byte  ToByte(object obj)
        {
            
            if (obj == null || obj == DBNull.Value)
            {
                return 0;
            }
            try
            {
                return Convert.ToByte (obj);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static object GetStringOrDBNull(string str)
        {
            return string.IsNullOrEmpty(str) ? DBNull.Value : (object)str;
        }
        public static object CopyPropertyValues(object source, object Destination)
        {
            if (source == null)
                return null;
            if (source.GetType() != Destination.GetType())
                return null;
            foreach (PropertyInfo pi in source.GetType().GetProperties())
            {
                if (pi.CanWrite)
                    Destination.GetType().GetProperty(pi.Name).SetValue(Destination, pi.GetValue(source, null), null);
            }
            return Destination;
        }

    }
}