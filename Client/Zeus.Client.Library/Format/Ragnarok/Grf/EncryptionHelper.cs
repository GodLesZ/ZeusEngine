using System;
using System.Text;

namespace Zeus.Client.Library.Format.Ragnarok.Grf {

    public static class EncryptionHelper {

        public static Encoding Korean = Encoding.GetEncoding(949);
        public static Encoding Ansi = Encoding.GetEncoding(1252);


        static readonly byte[] IpTable = {
            58, 50, 42, 34, 26, 18, 10,  2, 60, 52, 44, 36, 28, 20, 12,  4,
            62, 54, 46, 38, 30, 22, 14,  6, 64, 56, 48, 40, 32, 24, 16,  8,
            57, 49, 41, 33, 25, 17,  9,  1, 59, 51, 43, 35, 27, 19, 11,  3,
            61, 53, 45, 37, 29, 21, 13,  5, 63, 55, 47, 39, 31, 23, 15,  7
        };

        static readonly byte[] FpTable = {
            40,  8, 48, 16, 56, 24, 64, 32, 39,  7, 47, 15, 55, 23, 63, 31,
            38,  6, 46, 14, 54, 22, 62, 30, 37,  5, 45, 13, 53, 21, 61, 29,
            36,  4, 44, 12, 52, 20, 60, 28, 35,  3, 43, 11, 51, 19, 59, 27,
            34,  2, 42, 10, 50, 18, 58, 26, 33,  1, 41,  9, 49, 17, 57, 25
        };

        static readonly byte[] TpTable = {
            16, 7, 20, 21, 29, 12, 28, 17,  1, 15, 23, 26,  5, 18, 31, 10,
            2,  8, 24, 14, 32, 27,  3,  9, 19, 13, 30,  6, 22, 11,  4, 25
        };

        static readonly byte[][] STable = {
            new byte[] {
                0xef, 0x03, 0x41, 0xfd, 0xd8, 0x74, 0x1e, 0x47,  0x26, 0xef, 0xfb, 0x22, 0xb3, 0xd8, 0x84, 0x1e,
                0x39, 0xac, 0xa7, 0x60, 0x62, 0xc1, 0xcd, 0xba,  0x5c, 0x96, 0x90, 0x59, 0x05, 0x3b, 0x7a, 0x85,
                0x40, 0xfd, 0x1e, 0xc8, 0xe7, 0x8a, 0x8b, 0x21,  0xda, 0x43, 0x64, 0x9f, 0x2d, 0x14, 0xb1, 0x72,
                0xf5, 0x5b, 0xc8, 0xb6, 0x9c, 0x37, 0x76, 0xec,  0x39, 0xa0, 0xa3, 0x05, 0x52, 0x6e, 0x0f, 0xd9
            }, new byte[] {
                0xa7, 0xdd, 0x0d, 0x78, 0x9e, 0x0b, 0xe3, 0x95,  0x60, 0x36, 0x36, 0x4f, 0xf9, 0x60, 0x5a, 0xa3,
                0x11, 0x24, 0xd2, 0x87, 0xc8, 0x52, 0x75, 0xec,  0xbb, 0xc1, 0x4c, 0xba, 0x24, 0xfe, 0x8f, 0x19,
                0xda, 0x13, 0x66, 0xaf, 0x49, 0xd0, 0x90, 0x06,  0x8c, 0x6a, 0xfb, 0x91, 0x37, 0x8d, 0x0d, 0x78,
                0xbf, 0x49, 0x11, 0xf4, 0x23, 0xe5, 0xce, 0x3b,  0x55, 0xbc, 0xa2, 0x57, 0xe8, 0x22, 0x74, 0xce
            }, new byte[] {
                0x2c, 0xea, 0xc1, 0xbf, 0x4a, 0x24, 0x1f, 0xc2,  0x79, 0x47, 0xa2, 0x7c, 0xb6, 0xd9, 0x68, 0x15,
                0x80, 0x56, 0x5d, 0x01, 0x33, 0xfd, 0xf4, 0xae,  0xde, 0x30, 0x07, 0x9b, 0xe5, 0x83, 0x9b, 0x68,
                0x49, 0xb4, 0x2e, 0x83, 0x1f, 0xc2, 0xb5, 0x7c,  0xa2, 0x19, 0xd8, 0xe5, 0x7c, 0x2f, 0x83, 0xda,
                0xf7, 0x6b, 0x90, 0xfe, 0xc4, 0x01, 0x5a, 0x97,  0x61, 0xa6, 0x3d, 0x40, 0x0b, 0x58, 0xe6, 0x3d
            }, new byte[] {
                0x4d, 0xd1, 0xb2, 0x0f, 0x28, 0xbd, 0xe4, 0x78,  0xf6, 0x4a, 0x0f, 0x93, 0x8b, 0x17, 0xd1, 0xa4,
                0x3a, 0xec, 0xc9, 0x35, 0x93, 0x56, 0x7e, 0xcb,  0x55, 0x20, 0xa0, 0xfe, 0x6c, 0x89, 0x17, 0x62,
                0x17, 0x62, 0x4b, 0xb1, 0xb4, 0xde, 0xd1, 0x87,  0xc9, 0x14, 0x3c, 0x4a, 0x7e, 0xa8, 0xe2, 0x7d,
                0xa0, 0x9f, 0xf6, 0x5c, 0x6a, 0x09, 0x8d, 0xf0,  0x0f, 0xe3, 0x53, 0x25, 0x95, 0x36, 0x28, 0xcb
            }
        };

        static readonly byte[] Mask = {
            0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01
        };

        private static void _roundFunction(byte[] src) {
            var block = new byte[8];

            block[0] = (byte)(((src[7] << 5) | (src[4] >> 3)) & 0x3f);	// ..0 vutsr
            block[1] = (byte)(((src[4] << 1) | (src[5] >> 7)) & 0x3f);	// ..srqpo n
            block[2] = (byte)(((src[4] << 5) | (src[5] >> 3)) & 0x3f);	// ..o nmlkj
            block[3] = (byte)(((src[5] << 1) | (src[6] >> 7)) & 0x3f);	// ..kjihg f
            block[4] = (byte)(((src[5] << 5) | (src[6] >> 3)) & 0x3f);	// ..g fedcb
            block[5] = (byte)(((src[6] << 1) | (src[7] >> 7)) & 0x3f);	// ..cba98 7
            block[6] = (byte)(((src[6] << 5) | (src[7] >> 3)) & 0x3f);	// ..8 76543
            block[7] = (byte)(((src[7] << 1) | (src[4] >> 7)) & 0x3f);	// ..43210 v

            for (int i = 0; i < STable.Length; i++) {
                block[i] = (byte)((STable[i][block[i * 2 + 0]] & 0xf0) | (STable[i][block[i * 2 + 1]] & 0x0f));
            }

            block[4] = 0; block[5] = 0; block[6] = 0; block[7] = 0;

            for (int i = 0; i < TpTable.Length; i++) {
                byte j = (byte)(TpTable[i] - 1);

                if ((block[(j >> 3) + 0] & Mask[j & 7]) != 0)
                    block[(i >> 3) + 4] |= Mask[i & 7];
            }

            src[0] ^= block[4];
            src[1] ^= block[5];
            src[2] ^= block[6];
            src[3] ^= block[7];
        }

        public static void DesDecodeBlock(byte[] src, int i) {
            var block = new byte[8];
            Buffer.BlockCopy(src, i, block, 0, 8);

            _ip(block);
            _roundFunction(block);
            _fp(block);

            Buffer.BlockCopy(block, 0, src, i, 8);
        }

        private static void _fp(byte[] src) {
            var block = new byte[8];

            for (int i = 0; i < FpTable.Length; i++) {
                byte j = (byte)(FpTable[i] - 1);

                if ((src[(j >> 3) & 7] & Mask[j & 7]) != 0)
                    block[(i >> 3) & 7] |= Mask[i & 7];
            }

            Buffer.BlockCopy(block, 0, src, 0, 8);
        }

        private static void _ip(byte[] src) {
            var block = new byte[8];

            for (int i = 0; i < IpTable.Length; i++) {
                byte j = (byte)(IpTable[i] - 1);

                if ((src[(j >> 3) & 7] & Mask[j & 7]) != 0)
                    block[(i >> 3) & 7] |= Mask[i & 7];
            }

            Buffer.BlockCopy(block, 0, src, 0, 8);
        }

        private static void _nibbleSwap(byte[] block) {
            for (int i = 0; i < block.Length; i++) {
                block[i] = (byte)((block[i] >> 4) | (block[i] << 4));
            }
        }

        public static string DecodeFileName(byte[] fileName) {
            for (int i = 0; i < fileName.Length; i += 8) {
                byte[] block = new byte[8];
                Buffer.BlockCopy(fileName, i, block, 0, 8);
                _nibbleSwap(block);
                Buffer.BlockCopy(block, 0, fileName, i, 8);
                DesDecodeBlock(fileName, i);
            }

            // Files are ALWAYS encoded with ANSI encoding
            return Ansi.GetString(fileName);
        }

        public static string EncodeFileName(byte[] fileName) {
            for (int i = 0; i < fileName.Length; i += 8) {
                byte[] block = new byte[8];
                DesDecodeBlock(fileName, i);
                Buffer.BlockCopy(fileName, i, block, 0, 8);
                _nibbleSwap(block);
                Buffer.BlockCopy(block, 0, fileName, i, 8);
            }

            // Files are ALWAYS encoded with ANSI encoding
            return Ansi.GetString(fileName);
        }

        private static void _decodeFileData(byte[] data, bool type, int cycle) {
            int cnt = 0;
            int offset = 0;

            if (cycle < 3) cycle = 3;
            else if (cycle < 5) cycle++;
            else if (cycle < 7) cycle += 9;
            else cycle += 15;

            for (int lop = 0; lop * 8 < data.Length; lop++, offset += 8) {
                if (lop < 20 || (type == false && lop % cycle == 0)) {
                    DesDecodeBlock(data, offset);
                } else {
                    if (cnt == 7 && type == false) {
                        byte[] tmp = new byte[8];
                        Buffer.BlockCopy(data, offset, tmp, 0, 8);
                        cnt = 0;
                        data[offset] = tmp[3];
                        data[offset + 1] = tmp[4];
                        data[offset + 2] = tmp[6];
                        data[offset + 3] = tmp[0];
                        data[offset + 4] = tmp[1];
                        data[offset + 5] = tmp[2];
                        data[offset + 6] = tmp[5];
                        int a = tmp[7];
                        if (a == 0x00) a = 0x2b;
                        else if (a == 0x2b) a = 0x00;
                        else if (a == 0x01) a = 0x68;
                        else if (a == 0x68) a = 0x01;
                        else if (a == 0x48) a = 0x77;
                        else if (a == 0x77) a = 0x48;
                        else if (a == 0x60) a = 0xff;
                        else if (a == 0xff) a = 0x60;
                        else if (a == 0x6c) a = 0x80;
                        else if (a == 0x80) a = 0x6c;
                        else if (a == 0xb9) a = 0xc0;
                        else if (a == 0xc0) a = 0xb9;
                        else if (a == 0xeb) a = 0xfe;
                        else if (a == 0xfe) a = 0xeb;
                        data[offset + 7] = (byte)a;
                    }

                    cnt++;
                }
            }
        }

        public static void DecryptFileData(byte[] data, bool type, int cycle) {
            if (data.Length % 8 != 0) {
                int idealSizeCompressed = ((data.Length / 8) + 1) * 8;

                byte[] dataFixed = new byte[idealSizeCompressed];
                Buffer.BlockCopy(data, 0, dataFixed, 0, data.Length);
                _decodeFileData(dataFixed, type, cycle);
                Buffer.BlockCopy(dataFixed, 0, data, 0, data.Length);
            } else {
                _decodeFileData(data, type, cycle);
            }
        }

        private static void _encodeFileData(byte[] data, bool type, int cycle) {
            int cnt = 0;
            int offset = 0;

            if (cycle < 3) cycle = 3;
            else if (cycle < 5) cycle++;
            else if (cycle < 7) cycle += 9;
            else cycle += 15;

            for (int lop = 0; lop * 8 < data.Length; lop++, offset += 8) {
                if (lop < 20 || (type == false && lop % cycle == 0)) {
                    DesDecodeBlock(data, offset);
                } else {
                    if (cnt == 7 && type == false) {
                        byte[] tmp = new byte[8];
                        Buffer.BlockCopy(data, offset, tmp, 0, 8);
                        cnt = 0;
                        data[offset] = tmp[3];
                        data[offset + 1] = tmp[4];
                        data[offset + 2] = tmp[5];
                        data[offset + 3] = tmp[0];
                        data[offset + 4] = tmp[1];
                        data[offset + 5] = tmp[6];
                        data[offset + 6] = tmp[2];
                        int a = tmp[7];
                        if (a == 0x00) a = 0x2b;
                        else if (a == 0x2b) a = 0x00;
                        else if (a == 0x01) a = 0x68;
                        else if (a == 0x68) a = 0x01;
                        else if (a == 0x48) a = 0x77;
                        else if (a == 0x77) a = 0x48;
                        else if (a == 0x60) a = 0xff;
                        else if (a == 0xff) a = 0x60;
                        else if (a == 0x6c) a = 0x80;
                        else if (a == 0x80) a = 0x6c;
                        else if (a == 0xb9) a = 0xc0;
                        else if (a == 0xc0) a = 0xb9;
                        else if (a == 0xeb) a = 0xfe;
                        else if (a == 0xfe) a = 0xeb;
                        data[offset + 7] = (byte)a;
                    }

                    cnt++;
                }
            }
        }

        public static void EncryptFileData(byte[] data, bool type, int cycle) {
            if (data.Length % 8 != 0) {
                int idealSizeCompressed = ((data.Length / 8) + 1) * 8;

                byte[] dataFixed = new byte[idealSizeCompressed];
                Buffer.BlockCopy(data, 0, dataFixed, 0, data.Length);
                _encodeFileData(dataFixed, type, cycle);
                Buffer.BlockCopy(dataFixed, 0, data, 0, data.Length);
            } else {
                _encodeFileData(data, type, cycle);
            }
        }

	}


}
