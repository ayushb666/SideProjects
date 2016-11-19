using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QRCodeDecoder
{
    public partial class Form1 : Form
    {
        private string fileName = "";
        private int n = 21;
        private int[,] dataMatrix = new int[21, 21];
        private int[,] permanentMatrix = new int[21, 21];
        private string maskpattern = "";
        private string errorpattern = "";
        private string encodingType = "";
        private int messageLength;
        string encodedMessage = "";
        private int lengthEachBlock;
        private char[] dictionary = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',' ','$','%','*','+','-','.','/',':'};

        private delegate int Mydelegate(int i, int j);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            qrImage.Visible = false;
            decoder.Enabled = false;
            message.Text = "";
            fillPermanentMatrix();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            fileDialog.FileName = "";
            fileDialog.Title = "Select a QR Image";
            fileDialog.Filter = "All images|*.jpg; *.bmp; *.png";
            fileDialog.ShowDialog();
            if (fileDialog.FileName.ToString() != "")
            {
                fileName = fileDialog.FileName.ToString();
                selectPic.Text = fileName;
                qrImage.Image = Image.FromFile(fileName);
                qrImage.Visible = true;
                decoder.Enabled = true;
                message.Text = "File Uploaded";
            }
        }

        // Currently works for a 21*21 size qr code and Byte Encoded Message.
        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap dataFile = new Bitmap(fileName);
            Bitmap data = getSizeofQR(data: dataFile);
            qrImage.Image = Image.FromHbitmap(data.GetHbitmap());
            getDataMatrix(data: data);
            getUnmaskedInfoBits();
            unmaskDataMatrix();
            getEncodingType();
            switch (encodingType)
            {
                case "0001": // Numeric Encoding
                    getMessageLength(7);
                    int totalBlocks;
                    if ((messageLength % 3) == 0)
                    {
                        messageLength = (messageLength / 3);
                        totalBlocks = messageLength * 10;
                    }else
                    {
                        if ((messageLength % 3) == 1)
                        {
                            totalBlocks = 4;
                        }else
                        {
                            totalBlocks = 7;
                        }
                        messageLength = (messageLength / 3);
                        totalBlocks += (messageLength * 10);
                    }    
                    getEncodedData(totalBlocks,n-8,n-1);
                    message.Text = "Your Message is : " + convertNumericMessage(totalBlocks);
                    break;

                case "0010": // AlphaNumeric Encoding
                    getMessageLength(7);
                    int total;
                    if (messageLength % 2 == 0)
                    {
                        messageLength = (messageLength / 2);
                        total = (messageLength * 11);
                    }else
                    {
                        total = 6;
                        messageLength = (messageLength / 2);
                        total = (messageLength * 11);
                    }
                    encodedMessage += dataMatrix[n - 7, n - 2].ToString();
                    getEncodedData(total-1, n - 8, n - 1);
                    message.Text = "Your Message is : " + convertAlphaNumericMessage(total);
                    break;

                case "0100": // 8 Byte Encoding
                    getMessageLength(6);
                    getEncodedData(messageLength*8,n-7,n-1);
                    message.Text = "Your Message is : " + convertByteMessage();
                    break;

                default:
                    message.Text = "Cannot Decode This Format Data";
                    break;
            }

            decoder.Enabled = false;
        }

        private string convertByteMessage()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < messageLength * 8; i += 8)
            {
                sb.Append(Convert.ToChar(Convert.ToInt32(encodedMessage.Substring(i, 8), 2)));
            }
            return sb.ToString();
        }

        private string convertNumericMessage(int len)
        {
            StringBuilder sb = new StringBuilder();
            int additional = len % 10;
            for (int i = 0; i < len-additional ; i += 10)
            {
                sb.Append(Convert.ToString(Convert.ToInt32(encodedMessage.Substring(i, 10), 2)));
            }
            if (additional != 0)
            {
                sb.Append(Convert.ToString(Convert.ToInt32(encodedMessage.Substring(len - additional, additional), 2)));
            }
            return sb.ToString();
        }

        private string convertAlphaNumericMessage(int len)
        {
            StringBuilder sb = new StringBuilder();
            int additional = len % 11;
            for (int i = 0; i < len - additional; i += 11)
            {
                int num = Convert.ToInt32(encodedMessage.Substring(i, 11), 2), temp = 0;
                char first, second;
                while((num -temp)%45 != 0){ temp++; }
                second = dictionary[temp];
                first = dictionary[(num - temp) / 45];
                sb.Append(first);
                sb.Append(second);
            }
            if (additional != 0)
            {
                sb.Append(dictionary[Convert.ToInt32(encodedMessage.Substring(len - additional, additional), 2)]);
            }
            return sb.ToString();
        }

        private void getMessageLength(int x)
        {
            string length = "";
            for (int i = 3; i <= x; i++)
            {
                length += (dataMatrix[n - i, n - 1].ToString() + dataMatrix[n - i, n - 2].ToString());
            }
            if (encodingType == "0010")
            {
                length = length.Substring(0, 9);
            }
            messageLength = Convert.ToInt32(length, 2);
        }

        private void getEncodedData(int temp,int i,int j)
        {
            bool flag = true; // when true moves up and when false move down
            StringBuilder sb = new StringBuilder();
            while (temp> 0)
            {
                if(permanentMatrix[i,j] == 5 || permanentMatrix[i, j] == 4)
                {
                    while(permanentMatrix[i, j] == 5 || permanentMatrix[i, j] == 4)
                    {
                        if (flag) { i--; }
                        else { i++; }
                    }
                }

                sb.Append(dataMatrix[i, j]);
                if (temp - 1 != 0){sb.Append(dataMatrix[i, j - 1]);}

                if (flag)
                {
                    if ((i - 1 >= 0) && (permanentMatrix[i - 1, j] != 5)){ i--; }
                    else{
                        j -= 2;
                        flag = false;}
                }else
                {
                    if((i+1<=n-1) && (permanentMatrix[i + 1, j] != 5)) { i++; }
                    else{
                        j -= 2;
                        flag = true;}
                }
                temp-= 2;
            }
            encodedMessage += sb.ToString();
        }

        private Bitmap getSizeofQR(Bitmap data)
        {
            Bitmap temp;
            int lengthBlackPart = 0, starti = 0, startj = 0, flag1 = 0, flag2 = 0;
            for (int i = 0; i < data.Height; i++)
            {
                for (int j = 0; j < data.Width; j++)
                {
                    var t = data.GetPixel(i, j);
                    if ((t.B+t.G+t.R)/3 <10 && flag1 ==0)
                    {
                        if (flag2 == 0)
                        {
                            starti = i;
                            startj = j;
                            flag2 = 1;
                        }
                        lengthBlackPart++;
                    }else if (lengthBlackPart > 0)
                    {
                        flag1 = 1;
                        break;
                    }
                }
                if(flag1 == 1) { break; }
            }
            lengthEachBlock = lengthBlackPart / 7;
            temp = new Bitmap(lengthEachBlock * n, lengthEachBlock * n);
            for (int i = 0; i < lengthEachBlock * n; i++)
            {
                for (int j = 0; j < lengthEachBlock * n; j++)
                {
                    temp.SetPixel(i, j, data.GetPixel(i+starti, j+startj));
                }
            }
            return temp;
        }

        private void getEncodingType()
        {
            encodingType  += dataMatrix[n-1,n-1].ToString() + dataMatrix[n-1,n-2].ToString() + dataMatrix[n-2,n-1].ToString() + dataMatrix[n-2,n-2].ToString();
        }

        private void unmaskDataMatrix()
        {
            Mydelegate f;

            switch (maskpattern)
            {
                case "000":
                    f = (i, j) => { return (i + j) % 2; }; 
                    break;

                case "001":
                    f = (i, j) => { return i % 2; };
                    break;

                case "010":
                    f = (i, j) => { return j % 3; };
                    break;

                case "011":
                    f = (i, j) => { return (i + j) % 3; };
                    break;

                case "100":
                    f = (i, j) => { return (i / 2 + j / 3) % 2; };
                    break;

                case "101":
                    f = (i, j) => { return ((i * j) % 2 + (i * j) % 3); };
                    break;

                case "110":
                    f = (i, j) => { return ((i * j) % 3 + (i * j)) % 2; };
                    break;

                case "111":
                    f = (i, j) => { return ((i * j) % 3 + i + j) % 2; };
                    break;

                default:
                    f = (i, j) => { return (i + j) % 2; };
                    break;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (f(i,j) == 0 && (permanentMatrix[i,j] !=5 && permanentMatrix[i, j] != 4))
                    {
                        int t = dataMatrix[i, j];
                        if (dataMatrix[i, j] == 0) { dataMatrix[i, j] = 1; }
                        else if(dataMatrix[i, j] == 1) { dataMatrix[i, j] = 0; }
                        t = dataMatrix[i, j];
                    } 
                }
            }
        }

        private void getUnmaskedInfoBits()
        {
            string maskpatternTemp = dataMatrix[8, 2].ToString() + dataMatrix[8, 3].ToString() + dataMatrix[8, 4].ToString();
            string errorpatternTemp = dataMatrix[8, 0].ToString() + dataMatrix[8, 1].ToString();
            int x = Convert.ToInt32(errorpatternTemp+ maskpatternTemp, 2);
            x = x ^ Convert.ToInt32("10101",2);
            string pattern = Convert.ToString(x, 2);
            if(pattern.Length < 5)
            {
                for (int i = 0; i < 5 - pattern.Length; i++)
                {
                    pattern = '0' + pattern;
                }
            }
            maskpattern += pattern.Substring(2,3);
            errorpattern += pattern.Substring(0, 2);
        }

        private void getDataMatrix(Bitmap data)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var k = data.GetPixel(j*lengthEachBlock+1 ,i*lengthEachBlock+1);
                    if (((k.R + k.B + k.G)/3) < 100) { dataMatrix[i, j] = 1; }
                    else { dataMatrix[i, j] = 0; }
                }
            }
        }

        private void fillPermanentMatrix()
        {   // top left
            for (int i = 0; i <= 8; i++)
            {
                for (int j = 0; j <= 8; j++)
                { permanentMatrix[i, j] = 5; } // 5 represents permanent effect
            }
            // top right
            for (int i = 0; i <= 8; i++)
            {
                for (int j = n - 1; j >= n - 8; j--)
                { permanentMatrix[i, j] = 5; }
            }
            // bottom left
            for (int i = n - 1; i >= n - 8; i--)
            {
                for (int j = 0; j <= 8; j++)
                { permanentMatrix[i, j] = 5; }
            }

            for (int i = 6; i < n - 7; i += 2) { permanentMatrix[6, i] = 4; }
            for (int i = 6; i < n - 7; i += 2) { permanentMatrix[i, 6] = 4; }

        }

        private void fileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e){}

    }
}