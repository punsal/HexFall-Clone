using UnityEngine;

namespace Utility.System.Data_System.Parsing_System
{
    public class CsvParser : MonoBehaviour
    {
        private static string[,] craftingMatrix;
#pragma warning disable 649
        [SerializeField] private TextAsset csvData;
#pragma warning restore 649

        private void Start()
        {
            ParseCSV();
        }

        private void ParseCSV()
        {
            if (csvData == null) return;

            var lines = csvData.text.Split('\n');

            craftingMatrix = new string[lines.Length, lines.Length];

            for (var i = 0; i < lines.Length; i++)
            {
                var rows = lines[i].Split(',');
                for (var j = 0; j < rows.Length; j++) craftingMatrix[i, j] = rows[j];
            }
        }

        public static string GetCompatible(string item1, string item2)
        {
            var item1Index = 0;
            for (var columnIndex = 0; columnIndex < craftingMatrix.GetLength(0); columnIndex++)
                if (craftingMatrix[columnIndex, 0] == item1)
                    item1Index = columnIndex;

            var item2Index = 0;
            for (var rowIndex = 0; rowIndex < craftingMatrix.GetLength(0); rowIndex++)
                if (craftingMatrix[rowIndex, 0] == item2)
                    item2Index = rowIndex;

            return craftingMatrix[item1Index, item2Index];
        }
    }
}