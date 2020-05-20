using System.Threading;
using UnityEngine;
using com.fpnn;
using com.fpnn.rtm;

public class Main : MonoBehaviour
{
    public interface ITestCase
    {
        void Start(string endpoint, long pid, long uid, string token);
        void Stop();
    }

    private string rtmServerEndpoint = "rtm-nx-front.ilivedata.com:13325";
    private long pid = 1000001;
    private long uid = 99688848;
    private string token = "0AF1E4089148A8D0552DF401BE6404B8";

    Thread testThread;
    ITestCase tester;

    // Start is called before the first frame update
    void Start()
    {
        com.fpnn.common.ErrorRecorder RerrorRecorderecorder = new ErrorRecorder();
        Config config = new Config
        {
            errorRecorder = RerrorRecorderecorder
        };
        ClientEngine.Init(config);

        RTMConfig rtmConfig = new RTMConfig()
        {
            defaultErrorRecorder = RerrorRecorderecorder
        };
        RTMControlCenter.Init(rtmConfig);

        testThread = new Thread(TestMain)
        {
            IsBackground = true
        };
        testThread.Start();
    }

    void TestMain()
    {
        //-- Examples
        // tester = new Chat();
        // tester = new Data();
        // tester = new Files();
        // tester = new Friends();
        // tester = new Groups();
        // tester = new Histories();
        // tester = new Login();
        // tester = new Messages();
        // tester = new Rooms();
        // tester = new RTMSystem();
        tester = new Users();

        tester.Start(rtmServerEndpoint, pid, uid, token);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        tester.Stop();
        Debug.Log("Test App exited.");
    }
}
