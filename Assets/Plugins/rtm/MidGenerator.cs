namespace com.fpnn.rtm
{
    public class MidGenerator
    {
        static private ushort order = 0;
        static private object interLocker = new object();

        static public long Gen()
        {
            long baseId = ClientEngine.GetCurrentMilliseconds() << 16;

            lock (interLocker)
            {
                return baseId + ++order;
            }
        }
    }
}
