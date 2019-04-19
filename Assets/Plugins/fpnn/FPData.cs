using System;

namespace com.fpnn {

    public class FPData {   

        private byte[] _magic = FPConfig.TCP_MAGIC;

        public byte[] GetMagic() {

            return this._magic;
        }

        public void SetMagic(byte[] value) {

            this._magic = value;
        }


        private int _version = 1;

        public int GetVersion() {

            return this._version;
        }

        public void SetVersion(int value) {

            this._version = value;
        }


        private int _flag = 1;

        public int GetFlag() {

            return this._flag;
        }

        public void SetFlag(int value) {

            this._flag = value;
        }


        private int _mtype = 1;

        public int GetMtype() {

            return this._mtype;
        }

        public void SetMtype(int value) {

            this._mtype = value;
        }


        private int _ss = 0;

        public int GetSS() {

            return this._ss;
        }

        public void SetSS(int value) {

            this._ss = value;
        }


        private string _method = null;

        public string GetMethod() {

            return this._method;
        }

        public void SetMethod(string value) {

            this._method = value;

            if (this._method != null) {

                this.SetSS(System.Text.Encoding.UTF8.GetBytes(this._method).Length);
            }
        }


        private int _seq = 0;

        public int GetSeq() {

            return this._seq;
        }

        public void SetSeq(int value) {

            this._seq = value;
        }


        private byte[] _msgpack_data = null;

        public byte[] MsgpackPayload() {

            return this._msgpack_data;
        }

        public void SetPayload(byte[] value) {

            this._msgpack_data = value;

            if (this._msgpack_data != null) {

                this._psize = this._msgpack_data.Length;
            }
        }


        private string _json_data = null;

        public string JsonPayload() {

            return this._json_data;
        }

        public void SetPayload(string value) {

            this._json_data = value;

            if (this._json_data != null) {

                this._psize = System.Text.Encoding.UTF8.GetBytes(this._json_data).Length;
            }
        }


        private int _psize = 0;

        public int GetPsize() {

            return this._psize;
        }

        public void SetPsize(int value) {

            this._psize = value;
        }


        private int _pkgLen = 0;

        public int GetPkgLen() {

            return this._pkgLen;
        }

        public void SetPkgLen(int value) {

            this._pkgLen = value;
        }

        public byte[] Bytes;
    }
}