namespace MVCDemo.Models.Demo
{
    public class DemoIndexViewModel
    {
        public bool IDSvr6Enabled
        {
            get
            {
#if USE_IDSVR6
                return true;
#else
                return false;
#endif
            }
        }
    }
}
