namespace FinnZan.Utilities
{
    using System;
    using System.Security.Cryptography;

    public class RandomnessGenerator
    {
        private static string LOWER_CHAR = "abcdefghijklmnopqrstuvwxyz";
        private static string UPPER_CHAR = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string NUMERIC_CHAR = "0123456789";
        private static string SYMBOL_CHAR = "!@#$%^&*()-+=_";        

        public static string GetString(int length, bool lower = true, bool upper = true, bool numeric = true, bool symbol = false)
        {
            try
            {
                string charTable = string.Empty;

                if (lower)
                {
                    charTable += LOWER_CHAR;
                }

                if (upper)
                {
                    charTable += UPPER_CHAR;
                }

                if (numeric)
                {
                    charTable += NUMERIC_CHAR;
                }

                if (symbol)
                {
                    charTable += SYMBOL_CHAR;
                }

                if (charTable.Length <= 0)
                {
                    return null;
                }

                string ret = string.Empty;

                RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();
                byte[] rand = new byte[length];
                gen.GetBytes(rand);
                for (int i = 0; i < rand.Length; i++)
                {
                    var r = ((int)rand[i]) % charTable.Length;
                    ret += charTable[r];
                }

                return ret;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool GetInteger(int min, int max, out int ret)
        {
            if (max <= min)
            {
                ret = 0;
                return false;
            }

            try
            {
                RNGCryptoServiceProvider gen = new RNGCryptoServiceProvider();
                byte[] rand = new byte[4];
                gen.GetBytes(rand);
                var v = BitConverter.ToInt32(rand, 0);

                v = (Math.Abs(v)%(max - min +1));

                v += min;
                ret = v;

                return true;
            }
            catch (Exception ex)
            {
                CommonTools.HandleException(ex);
                ret = 0;
                return false;
            }
        }
    }


}
