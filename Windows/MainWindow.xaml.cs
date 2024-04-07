using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QRCode.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnMakeQRCode_Click(object sender, RoutedEventArgs e)
        {
            string text = txtSource.Text;
            Bitmap qrCode = new QRCodeEncoder().Encode(text, Encoding.UTF8);

            imageQRCode.Source = ImageSourceFromBitmap(qrCode);

            btnSaveQRCode.IsEnabled = true;
            btnDecodeQRCode.IsEnabled = true;
        }

        private void btnSaveQRCode_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog() { Filter = "PNG|*.png|JPEG|*.jpg|GIF|*.gif|BMP|*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                BitmapSource bitmapSource = imageQRCode.Source as BitmapSource;
                using (FileStream fileStream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(fileStream);
                }
            }
        }

        private void btnLoadQRCode_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
                bitmap.EndInit();

                imageQRCode.Source = bitmap;
                btnSaveQRCode.IsEnabled = true;
                btnDecodeQRCode.IsEnabled = true;
            }
        }

        private void btnDecodeQRCode_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource imageSource = imageQRCode.Source as BitmapSource;
            Bitmap bitmap = ConvertBitmapSourceToBitmap(imageSource);

            QRCodeBitmapImage qrCodeImage = new QRCodeBitmapImage(bitmap);
            string decodeText = new QRCodeDecoder().decode(qrCodeImage, Encoding.UTF8);

            MessageBox.Show(decodeText, "Распознание QR кода", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void txtSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnMakeQRCode.IsEnabled = !string.IsNullOrEmpty(txtSource.Text);
        }

        private ImageSource ImageSourceFromBitmap(Bitmap bitmap)
        {
            IntPtr handle = bitmap.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private Bitmap ConvertBitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(outStream);
                return new Bitmap(outStream);
            }
        }
    }
}