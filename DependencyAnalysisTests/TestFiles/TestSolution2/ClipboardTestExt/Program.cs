using Additional;
using ClipboardMonitor.Core.Enums;
using ClipboardMonitor.Core.EventArguments;
using ClipboardMonitor.Core.Factories;
using ClipboardMonitor.Core.Interfaces;

namespace ClipboardTestExt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HelperMethods.LogMessage("Starting Clipboard Monitor Test Application...");
            Console.WriteLine("Creating clipboard listener...");
            var clipboardListener = ClipboardListenerFactory.CreateClipboardListener();

            try
            {
                Console.WriteLine("Starting clipboard listener...");
                clipboardListener.ClipboardChanged += ClipboardListener_ClipboardChanged;
                clipboardListener.Start();
                Console.WriteLine("Clipboard listener started. Press Enter to stop.");
                Console.ReadLine();
            }
            finally
            {
                Console.WriteLine("Stopping clipboard listener...");
                clipboardListener.Stop();
                Console.WriteLine("Clipboard listener stopped.");
            }
        }

        // Event handler for clipboard changed
        private static void ClipboardListener_ClipboardChanged(object? sender, ClipboardChangedEventArgs e)
        {
            if (e.DataType == ClipboardDataType.TEXT)
            {
                // Do something with clipboard text
                Console.WriteLine("Clipboard Text: " + e.ClipboardText);
            }
            if (e.DataType == ClipboardDataType.FILES)
            {
                // Do something with clipboard files (these include file path)
                Console.WriteLine("Clipboard Files: " + string.Join(", ", e.ClipboardFiles ?? Array.Empty<string>()));
            }
            if (e.DataType == ClipboardDataType.CLEARED)
            {
                // Clipboard was cleared
                Console.WriteLine("Clipboard was cleared.");
            }
            if (e.DataType == ClipboardDataType.IMAGE)
            {
                if (e.ClipboardImage != null)
                {
                    // Do something with clipboard image (Bitmap - Windows)
                }
                else if (e.ClipboardImage is IClipboardImage clipboardImage)
                {
                    // Linux clipboard image - can get data (image byte array) or format (e.g. png), or save such as below
                    clipboardImage.Save(Path.Combine("/tmp/ClipboardMonitor/someImage.png"));
                }
            }
            else
            {
                // Handle unknown / null type
            }
        }
    }
}
