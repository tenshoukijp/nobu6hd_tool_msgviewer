using LS11DotNet; // LS11DotNetライブラリを使用します。Uses the LS11DotNet library.
using System; // System名前空間を使用します。Uses the System namespace.
using System.Collections; // ArrayListを使用するために必要です。Needed for using ArrayList.
using System.Collections.Generic; // List<T>を使用するために必要です。Needed for using List<T>.
using System.Text; // エンコーディング関連の機能を使用するために必要です。Needed for encoding-related functionalities.

namespace N6HDMsgEditor
{
    internal class Program
    {
        /// <summary>
        /// List<byte>をShift-JIS文字列に変換します。Converts a List<byte> to a Shift-JIS string.
        /// </summary>
        /// <param name="byteList">変換するバイトのリスト。The list of bytes to convert.</param>
        /// <returns>Shift-JIS文字列、エラー発生時はnull。A Shift-JIS string, or null if an error occurs.</returns>
        public static string ConvertListByteToSjisString(List<byte> byteList)
        {
            try
            {
                // List<byte>をbyte[]に変換します。Converts List<byte> to byte[].
                byte[] byteArray = byteList.ToArray();

                // Shift-JISエンコーディングを取得します。Gets the Shift-JIS encoding.
                Encoding sjisEncoding = Encoding.GetEncoding("Shift_JIS");

                // バイト配列をShift-JIS文字列に変換します。Converts the byte array to a Shift-JIS string.
                return sjisEncoding.GetString(byteArray);
            }
            catch (Exception ex)
            {
                // その他の例外処理。Handles other unexpected exceptions.
                Console.WriteLine($"Error: An unexpected error occurred. {ex.Message}");
                return null;  // nullを返します。Alternatively, throw the exception.
            }
        }


        static void Main(string[] args)
        {
            // 古い設計のためArrayListを使用しています。List<List<List<byte>>>の方が適切ですが、複雑になる可能性があります。
            // The original design uses ArrayList, which is outdated.  List<List<List<byte>>> would be more appropriate, but it might be overly complex.
            ArrayList pack_files = new ArrayList(); // パックファイルのリスト。A list of pack files.
            // メモリ上に複数ファイル(的オブジェクト)へと分割デコードする。
            // Decode to multiple files in memory.
            LS11DotNet.Ls11.DecodePack("message.n6p", pack_files); // "message.n6p"ファイルをデコードします。Decodes the "message.n6p" file.

            ArrayList pack_phrases = new ArrayList(); // 抽出されたフレーズのリスト。A list of extracted phrases.
            for (int file_ix = 0; file_ix < pack_files.Count; file_ix++) // pack_files内の各ファイルに対して繰り返します。Iterates through each file in pack_files.
            {
                ArrayList file = (ArrayList)pack_files[file_ix]; // ファイルをArrayListとして取得します。Gets the file as an ArrayList.
                Ls11.SplitData(file, pack_phrases); // データをフレーズに分割します。Splits the data into phrases.
            }

            List<Byte> byte_array; // 各フレーズのバイトデータ。Byte data for each phrase.
            foreach (ArrayList phrase in pack_phrases) // 各フレーズに対して繰り返します。Iterates through each phrase.
            {
                byte_array = new List<Byte>(); // バイト配列を初期化します。Initializes the byte array.

                foreach (var p in phrase) // フレーズ内の各要素に対して繰り返します。Iterates through each element in the phrase.
                {
                    byte_array.Add((byte)p); // 要素をバイトとして追加します。Adds the element as a byte.
                }

                string sjis_with_meta = ConvertListByteToSjisString(byte_array); // バイト配列をShift-JIS文字列に変換します。Converts the byte array to a Shift-JIS string.

                byte_array.Clear(); // バイト配列をクリアします。Clears the byte array.
                Console.WriteLine(sjis_with_meta); // 文字列を出力します。Prints the string.
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
