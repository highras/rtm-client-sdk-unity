#if UNITY_2017_1_OR_NEWER

using System.Collections;
using UnityEngine;

namespace com.fpnn.rtm
{
    public class StatusMonitor : Singleton<StatusMonitor>
    {
        bool networkReachable = true;

        public void Init() { }

        public void Start()
        {
            StartCoroutine(PerSecondCoroutine());
        }
        
        public void OnDestroy()
        {
            StopAllCoroutines();
        }

        private IEnumerator PerSecondCoroutine()
        {
            yield return new WaitForSeconds(1.0f);

            while (true)
            {
                CheckNetworkChange();

                yield return new WaitForSeconds(1.0f);
            }
        }

        private void CheckNetworkChange()
        {
            bool reachable = !(Application.internetReachability == NetworkReachability.NotReachable);
            if (networkReachable != reachable)
            {
                networkReachable = reachable;
                RTMControlCenter.NetworkReachableChanged(reachable);
            }
        }
    }
}
#endif
