#if UNITY_2017_1_OR_NEWER

using UnityEngine;

namespace com.fpnn
{
    public static partial class ClientEngine
    {
        static partial void PlatformInit()
        {
            ConnectionMonitor.Instance.Init();
            Application.quitting += () => {
                ClientEngine.Close();
            };
        }

        static partial void PlatformUninit()
        {

        }
    }

    public class ConnectionMonitor : Singleton<ConnectionMonitor>
    {
        private bool _isPause;
        private bool _isFocus;
        private bool _isBackground;

        void OnEnable()
        {
            this._isPause = false;
            this._isFocus = true;
            this._isBackground = false;
        }

        public void Init() { }

        private void CheckInBackground()
        {
            if (_isPause && !_isFocus)
            {
                if (_isBackground == false)
                {
                    _isBackground = true;

#if UNITY_IOS
                    ClientEngine.ChangeForbiddenRegisterConnection(_isBackground);
                    ClientEngine.StopAllConnections();
#endif
                }
            }
            else
            {
                if (_isBackground)
                {
                    _isBackground = false;

#if UNITY_IOS
                    ClientEngine.ChangeForbiddenRegisterConnection(_isBackground);
#endif
                }
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            _isPause = pauseStatus;

            CheckInBackground();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            _isFocus = hasFocus;

            CheckInBackground();
        }
    }
}
#endif
