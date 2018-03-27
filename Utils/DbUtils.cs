using System;
using System.Linq;

namespace Utils
{
    public class DbUtils
    {
        public static Guid GetNewGuid()
        {
            var date = DateTime.Now;
            var bytes = BitConverter.GetBytes(date.Ticks).ToList();

            var rnd = new Random(date.Second * date.Millisecond * date.Minute);
            var rFst = rnd.Next();
            var rScnd = rnd.Next();

            bytes.AddRange(BitConverter.GetBytes(rFst));
            bytes.AddRange(BitConverter.GetBytes(rScnd));

            return new Guid(bytes.ToArray());
        }
    }
}
