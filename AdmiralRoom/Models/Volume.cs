using CoreAudioApi;
using System.Diagnostics;

namespace Huoyaoyuan.AdmiralRoom.Models
{
    class Volume : NotificationObject
    {
        private SimpleAudioVolume simpleaudiovolume;

        #region IsMute
        private bool _ismute;
        public bool IsMute
        {
            get { return _ismute; }
            set
            {
                if (_ismute != value)
                {
                    _ismute = value;
                    simpleaudiovolume.Mute = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Value
        private float _value;
        /// <summary>
        /// 音量。范围为0~1
        /// </summary>
        public float Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    simpleaudiovolume.MasterVolume = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        private Volume() { }
        public static Volume GetInstance()
        {
            var volume = new Volume();
            var processid = Process.GetCurrentProcess().Id;
            var device = (new MMDeviceEnumerator()).GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            for (int i = 0; i < device.AudioSessionManager.Sessions.Count; i++)
            {
                var session = device.AudioSessionManager.Sessions[i];
                if (session.ProcessID == processid)
                {
                    volume.simpleaudiovolume = session.SimpleAudioVolume;
                    volume.IsMute = volume.simpleaudiovolume.Mute;
                    volume.Value = volume.simpleaudiovolume.MasterVolume;
                }
            }
            if (volume.simpleaudiovolume == null) return null;
            return volume;
        }
    }
}
