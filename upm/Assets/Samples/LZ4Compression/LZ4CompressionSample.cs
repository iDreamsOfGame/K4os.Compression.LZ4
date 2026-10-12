using System.Text;
using K4os.Compression.LZ4.Utilities;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace K4os.Compression.LZ4.Samples
{
    public class LZ4CompressionSample : MonoBehaviour
    {
        [SerializeField]
        private InputField sourceInputField;

        [SerializeField]
        private InputField encodedInputField;

        [SerializeField]
        private InputField targetInputField;

        [SerializeField]
        private Button compressButton;

        private void Awake()
        {
            sourceInputField.text = "Hello World! 你好！Hello World! 你好！";
            compressButton.onClick.AddListener(OnCompressButtonClicked);
        }

        private void OnDestroy()
        {
            compressButton.onClick.RemoveListener(OnCompressButtonClicked);
        }

        private void OnCompressButtonClicked()
        {
            var source = sourceInputField.text;
            var encodedData = LZ4Compression.Compress(source);
            encodedInputField.text = Encoding.UTF8.GetString(encodedData);
            var decodedData = LZ4Compression.Decompress(encodedData, Encoding.UTF8);
            targetInputField.text = decodedData;
        }
    }
}
