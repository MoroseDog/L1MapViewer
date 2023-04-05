using System.Collections.Generic;
using System.IO;

namespace L1MapViewer.Converter {
    class L1Til {

        //畫大地圖用的(將.til檔案 拆成更小的單位)
        public static List<byte[]> Parse(byte[] srcData) {
            List<byte[]> result = new List<byte[]>();
            try {
                using (BinaryReader br = new BinaryReader(new MemoryStream(srcData))) {

                    // 取得Block數量. 
                    int nAllBlockCount = br.ReadInt32();

                    int[] nsBlockOffset = new int[nAllBlockCount + 1];
                    for (int i = 0; i <= nAllBlockCount; i++) {
                        nsBlockOffset[i] = br.ReadInt32();// 載入Block的偏移位置.
                    }
                   
                    int nCurPosition = (int)br.BaseStream.Position;

                    // 載入Block的資料.
                    for (int i = 0; i < nAllBlockCount; i++) {
                        int nPosition = nCurPosition + nsBlockOffset[i];
                        br.BaseStream.Seek(nPosition, SeekOrigin.Begin);

                        int nSize = nsBlockOffset[i + 1] - nsBlockOffset[i];
                        if (nSize <= 0) {
                            nSize = srcData.Length - nsBlockOffset[i];
                        }

                        // int type = br.ReadByte();
                        byte[] data = br.ReadBytes(nSize + 1);
                        result.Add(data);
                    }
                }
            } catch {
                // Utils.outputText("L1Til_Parse發生問題的檔案:" + logFileName, "Log.txt");
            }
            return result;
        }
    }
}

