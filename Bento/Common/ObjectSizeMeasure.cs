using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Emu.Bento.Common
{
    /// <summary>
    /// オブジェクトサイズ計測器
    /// </summary>
    public static class ObjectSizeMeasure
    {
        /// <summary>
        /// 指定したオブジェクトのサイズを返します。
        /// </summary>
        /// <param name="target">サイズ計測対象のオブジェクト</param>
        /// <returns>指定したオブジェクトのサイズ</returns>
        public static long GetSize(object target)
        {
            BinaryFormatter bf = new BinaryFormatter();
            long size = -1;

            using (MemoryStream stream = new MemoryStream())
            {
                bf.Serialize(stream, target);
                size = stream.Length;
            }
            return size;
        }
    }
}
