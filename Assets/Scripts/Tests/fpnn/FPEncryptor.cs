using System;

namespace com.fpnn {

    public class FPEncryptor {

        private FPPackage _pkg;
        private bool _cryptoed;

        public FPEncryptor(FPPackage pkg) {

            if (pkg != null) {

                this._pkg = pkg;
            } else {

                this._pkg = new FPPackage();
            }
        }

        public void Clear() {

            this._cryptoed = false;
        }

        public bool IsCrypto() {

            return false;
        }

        public bool GetCryptoed() {

            return this._cryptoed;
        }

        public void SetCryptoed(bool value) {

            this._cryptoed = value;
        }

        public bool IsStreamMode() {

            return false;
        }

        public byte[] DeCode(byte[] bytes) {

            return bytes;
        }

        public byte[] EnCode(byte[] bytes) {

            return bytes;
        }

        public FPData PeekHead(byte[] bytes) {

            if (!this.GetCryptoed()) {

                return this.CommonPeekHead(bytes);
            }

            if (this.IsStreamMode()) {

                return this.StreamPeekHead(bytes);
            }

            return this.CryptoPeekHead(bytes);
        }

        public FPData PeekHead(FPData peek) {

            if (this._cryptoed && peek != null) {

                FPData data = this._pkg.PeekHead(peek.Bytes);

                if (data != null) {

                    data.Bytes = this._pkg.GetByteArrayRange(peek.Bytes, 0, peek.Bytes.Length - 1);
                }

                return data;
            }

            return peek;
        }

        private FPData CommonPeekHead(byte[] bytes) {

            if (bytes != null && bytes.Length == 12) {

                FPData data = this._pkg.PeekHead(bytes);

                if (!this.CheckHead(data)) {

                    return null;
                }

                if (this._pkg.IsOneWay(data)) {

                    data.SetPkgLen(12 + data.GetSS() + data.GetPsize());
                }

                if (this._pkg.IsTwoWay(data)) {

                    data.SetPkgLen(16 + data.GetSS() + data.GetPsize());
                }

                if (this._pkg.IsAnswer(data)) {

                    data.SetPkgLen(16 + data.GetPsize());
                }

                data.Bytes = this._pkg.GetByteArrayRange(bytes, 0, bytes.Length - 1);
                return data;
            }

            return null;
        }

        private FPData StreamPeekHead(byte[] bytes) {

            //TODO
            return null;
        }

        private FPData CryptoPeekHead(byte[] bytes) {

            if (bytes != null && bytes.Length >= 4) {

                FPData data = new FPData();

                data.SetPkgLen((int)BitConverter.ToUInt32(this._pkg.GetByteArrayRange(bytes, 0, 3), 0));
                data.Bytes = this._pkg.GetByteArrayRange(bytes, 4, bytes.Length - 1); 

                if (data.GetPkgLen() > 8 * 1024 * 1024) {

                    return null;
                }

                return data;
            }

            return null;
        }

        private bool CheckHead(FPData data) {

            if (!this._pkg.IsTcp(data) && !this._pkg.IsHttp(data)) {

                return false;
            }

            if (data.GetVersion() < 0 || data.GetVersion() >= FPConfig.FPNN_VERSION.Length) {

                return false;
            }

            if (!this._pkg.CheckVersion(data)) {

                return false;
            }

            if (!this._pkg.IsMsgPack(data) && !this._pkg.IsJson(data)) {

                return false;
            }

            if (!this._pkg.IsOneWay(data) && !this._pkg.IsTwoWay(data) && !this._pkg.IsAnswer(data)) {

                return false;
            }

            return true;
        }
    }
}