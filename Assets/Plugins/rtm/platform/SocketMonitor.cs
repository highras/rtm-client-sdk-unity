using System;
using com.fpnn;

namespace com.rtm
{
    public class SocketMonitor : Singleton<SocketMonitor>
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
                    FPClient.AppleMobileDeviceSwitchToBackground(_isBackground);
#endif
                }
            }
            else
            {
                if (_isBackground)
                {
                    _isBackground = false;

#if UNITY_IOS
                    FPClient.AppleMobileDeviceSwitchToBackground(_isBackground);
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

